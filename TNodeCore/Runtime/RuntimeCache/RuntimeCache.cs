﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Interfaces;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Runtime.RuntimeCache{
    public class PortAccessor<T1, T2>:IModelPortAccessor{
        public readonly Func<T1, T2> Get;
        public readonly Action<T1, T2> Set;
        private readonly Action<T1> _resetFunc;
        private readonly T2 _defaultValue;
        public PortAccessor(string name,bool property){
            if (property){
                Type t = typeof(T1);
     
                MethodInfo getter = t.GetMethod("get_" + name);
                MethodInfo setter = t.GetMethod("set_" + name);
                Type = getter?.ReturnType??setter?.GetParameters()[0].ParameterType;
                
                if(getter!=null)
                    Get = (Func<T1, T2>)Delegate.CreateDelegate(typeof(Func<T1, T2>), null, getter);
                if(setter!=null)
                    Set = (Action<T1, T2>)Delegate.CreateDelegate(typeof(Action<T1, T2>), null, setter);
                if (Set != null){
                    var dummy = Activator.CreateInstance<T1>();
        
                    if (Get != null)
                        _defaultValue = Get(dummy);
           
                    _resetFunc += (obj) => {
                        Set(obj, _defaultValue);
                    };
                }
            }
            else{
                Type t = typeof(T1);
                MethodInfo method = t.GetMethod(name);
                if (method == null){
                    throw new Exception("Method not found for name " + name);
                }
                Type = method.MethodPortType();
                Get = (Func<T1, T2>)Delegate.CreateDelegate(typeof(Func<T1, T2>), null, method);
            }

        }



        public object GetValue(object model){
            return Get((T1)model);
        }

        public void SetValue(object model, object value){
            Set((T1)model,(T2)value);
        }

        public void Reset(object model){
            //Get 
            _resetFunc?.Invoke((T1)model);
        }

        public Type Type{ get; set; }
    }



    

    internal class PortConverterHelper<T1,T2> : IPortConverterHelper{
        private readonly PortTypeConversion<T1, T2> _converter;
        public PortConverterHelper(Type type){
            _converter = Activator.CreateInstance(type) as PortTypeConversion<T1, T2>;
        }
        public object Convert(object value){
            return _converter.Convert((T1)value);
        }

 
    }
    //Store a t1 to t2 conversion but use two way converter's convert back method

    internal class PortConverterHelperReverse<T1, T2> : IPortConverterHelper{
        private readonly TwoWayPortTypeConversion<T2, T1> _converter;
        public object Convert(object value){
            return _converter.ConvertBack((T1)value);
        }
        public PortConverterHelperReverse(Type type){
            _converter = Activator.CreateInstance(type) as TwoWayPortTypeConversion<T2, T1>;
        }
    }
    internal interface IPortConverterHelper{
        public object Convert(object value);
   
    }

    public class PropertyNotFoundException : Exception{
        public PropertyNotFoundException(string path):base("Property not found :"+path){
            
        }
    }
    public class RuntimeCache{
        //Singleton instance for the runtime cache
        private static RuntimeCache _instance;
        public static RuntimeCache Instance{
            get{ return _instance ??= new RuntimeCache(); }
        }
        //delegate return a value from a nodedata
        public delegate object GetValueDelegate(Model nodeData);
        public delegate void CachedInputFunction(Model nodeData,object value);
        public delegate object CachedOutputFunction(Model nodeData);
        public delegate void SetValueDelegate(Model nodeData,object value);



        public readonly Dictionary<Type, Dictionary<string,GetValueDelegate>> CachedDelegatesForGettingValue =
            new Dictionary<Type, Dictionary<string,GetValueDelegate>>();
        public readonly Dictionary<Type,Dictionary<string,SetValueDelegate>> CachedDelegatesForSettingValue =
            new Dictionary<Type,Dictionary<string,SetValueDelegate>>();
        public readonly Dictionary<Type,Dictionary<string,IModelPortAccessor>> CachedPortAccessors =
            new Dictionary<Type,Dictionary<string,IModelPortAccessor>> ();
        
        /// <summary>
        /// TODO: Converters now work globally, but it should be possible to specify a converter for a specific graph.but it will be too nested.so in current implementation, we will use a global converter.
        /// </summary>
        private readonly Dictionary<Type,Dictionary<Type,IPortConverterHelper>> CachedPortConverters =
            new Dictionary<Type,Dictionary<Type,IPortConverterHelper>> ();
        

        private readonly Dictionary<Type, Type> _graphBlackboardDictionary = new Dictionary<Type, Type>();

        private static readonly string[] ExcludedAssemblies = new string[]{"Microsoft", "UnityEngine","UnityEditor","mscorlib","System"};

        public RuntimeCache(){
            //Get types in all assemblies except the excluded ones
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(
                assembly => !ExcludedAssemblies.Contains(assembly.GetName().Name)
            ).SelectMany(assembly => assembly.GetTypes());
            //iterate types to check if they have  GraphUsageAttribute
            foreach (var type in types){
                var attributes = type.GetCustomAttributes();
                foreach (var attribute in attributes){
                    if (attribute is GraphUsageAttribute){
                        //if the type has GraphUsageAttribute, add it to the cache
                        AddTypeToCache(type,attribute as GraphUsageAttribute);
                    }

                    if (attribute is InternalUsageAttribute usageAttribute){
                        AddTypeToCache(type,usageAttribute);
                    }
                }
            }
        }
        private void AddTypeToCache(Type type,System.Attribute attribute){
            //Check if the type is a blackboard data type
            if(typeof(BlackboardData).IsAssignableFrom(type)){
                //if it is, add it to the cache
                AddBlackboardDataTypeToCache(type,(GraphUsageAttribute)attribute);
                CacheRuntimeBlackboard(type);
            }
            //Check if the type is a node data type
            if(typeof(NodeData).IsAssignableFrom(type)){
                //if it is, add it to the cache
                CacheRuntimeNodeData(type);
            }
            //Check if the type is implementing IPortTypeConversion<T1,T2>
            if(type.BaseType is{IsGenericType: true} && type.BaseType.GetGenericTypeDefinition()==typeof(PortTypeConversion<,>)){
                //if it is, add it to the cache
                CacheRuntimePortTypeConversion(type);
               
            }
        }
        
        private void CacheRuntimePortTypeConversion(Type type){
   
        
            if (type.BaseType == null) return;
            if (type.BaseType != null){
         
                var genericType = type.BaseType.GetGenericTypeDefinition();
                if (genericType != typeof(PortTypeConversion<,>) && genericType != typeof(TwoWayPortTypeConversion<,>)){
                 
                    return;
                }
            }
         
            //Forward direction
            var type1 = type.BaseType.GetGenericArguments()[0];
            var type2 = type.BaseType.GetGenericArguments()[1];
            var specificType = typeof(PortConverterHelper<,>).MakeGenericType(type1, type2);
            var instance = Activator.CreateInstance(specificType, type) as IPortConverterHelper;
            if (instance == null){
                return;
            }
            if (!CachedPortConverters.ContainsKey(type1)){
                CachedPortConverters.Add(type1,new Dictionary<Type,IPortConverterHelper>());
            }
            CachedPortConverters[type1].Add(type2,instance);
            
            //Reverse direction
            if(type.BaseType.GetGenericTypeDefinition()==typeof(TwoWayPortTypeConversion<,>)){
                var specificTypeReverse = typeof(PortConverterHelperReverse<,>).MakeGenericType(type2, type1);
                var instanceReverse = Activator.CreateInstance(specificTypeReverse, type) as IPortConverterHelper;
                if (instanceReverse == null){
                    return;
                }
                if (!CachedPortConverters.ContainsKey(type2)){
                    CachedPortConverters.Add(type2,new Dictionary<Type,IPortConverterHelper>());
                }
                CachedPortConverters[type2].Add(type1,instanceReverse);
            }
        }
        private readonly Dictionary<Tuple<Type,Type>,bool> _possibleImplicitConversions = new Dictionary<Tuple<Type,Type>,bool> ();

        public bool HasImplicitConversion(Type baseType, Type targetType){
            var tuple = new Tuple<Type, Type>(baseType, targetType);
            if (_possibleImplicitConversions.ContainsKey(tuple)){
                return _possibleImplicitConversions[tuple];
            }
            var res =baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi => {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == baseType;
                });
            return _possibleImplicitConversions[tuple] = res;
        }
        

        private void CachingImplicitConversion(Type baseType, Type targetType){
            if (!HasImplicitConversion(baseType, targetType)) return;
            if (CachedPortConverters.ContainsKey(baseType)&&CachedPortConverters[baseType].ContainsKey(targetType)) return;
            //Create Implicit Conversion Helper that caches the implicit cast function
            var typeConverter = Activator.CreateInstance(typeof(ImplicitConversionHelper<,>).MakeGenericType(baseType, targetType)) as IPortConverterHelper;
            
            if (!CachedPortConverters.ContainsKey(baseType)){
                CachedPortConverters.Add(baseType,new Dictionary<Type,IPortConverterHelper>());
            }
     
            CachedPortConverters[baseType].Add(targetType,typeConverter);
          
        }
        
        public object GetConvertedValue(Type from,Type to,object value){

            if(!CachedPortConverters.ContainsKey(from)){
                //Find the cached port failed ,check if there is an implicit conversion
                //This inner cache method would only run once,so add a guard to prevent it run again,even though the function itself has a guard statement.
                if(HasImplicitConversion(from,to)){
                    CachingImplicitConversion(from,to);
                    return CachedPortConverters[from][to].Convert(value);
                }
                return value;
            }
            if(!CachedPortConverters[from].ContainsKey(to)){
                //Just like above, this function should be checked in here too
                if(HasImplicitConversion(from,to)){
                    CachingImplicitConversion(from,to);
                    return CachedPortConverters[from][to].Convert(value);
                }
                return value;
            }
            return CachedPortConverters[from][to].Convert(value);
        }

        private bool GetImplcitConvertedValue(Type from, Type to){
            throw new NotImplementedException();
        }

        public List<Type> GetSupportedTypes(Type type){
          
            if(!CachedPortConverters.ContainsKey(type)){
                return new List<Type>();
            }
     
            return CachedPortConverters[type].Keys.ToList();
        }


        private void AddBlackboardDataTypeToCache(Type type,GraphUsageAttribute attribute){
            var graphData = attribute.GraphDataType;
            _graphBlackboardDictionary.Add(graphData,type);

        }

        public Type GetAppropriateBlackboardDataType(GraphData graphData){
            var type = graphData.GetType();
            if(_graphBlackboardDictionary.ContainsKey(type)){
                return _graphBlackboardDictionary[type];
            }
            return null;
        }
        public BlackboardData GetBlackboardData(GraphData graphData){
            var type = GetAppropriateBlackboardDataType(graphData);
            if(type != null){
                return (BlackboardData) Activator.CreateInstance(type);
            }
            return null;
        }
        public void CacheRuntimeBlackboard(Type type){
            if (type == null) return;
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                CachedDelegatesForSettingValue.Add(type,new Dictionary<string, SetValueDelegate>());
                var fields = type.GetFields();
                foreach(var field in fields){
                    var getValueDelegate = GetValueDelegateForField(field);
                    CachedDelegatesForGettingValue[type].Add(field.Name,getValueDelegate);
                    
                    var setValueDelegate = SetValueDelegateForField(field);
                    CachedDelegatesForSettingValue[type].Add(field.Name,setValueDelegate);
                }
            }
        }
        //TODO: CACHE IT AS FUNCTION
        private static IModelPortAccessor CreatePortCacheForProperty(string propName,Type targetType,Type valueType){
            var makeGenericType = typeof (PortAccessor<,>).MakeGenericType(targetType,valueType);
            var constructor = makeGenericType.GetConstructor(new Type[]{typeof(string),typeof(bool)});
            var instance = constructor?.Invoke(new object[]{propName,true});
            return (IModelPortAccessor) instance;
        }
        private static IModelPortAccessor CreatePortCacheForMethod(string methodName,Type targetType,Type valueType){
            var makeGenericType = typeof (PortAccessor<,>).MakeGenericType(targetType,valueType);
            var constructor = makeGenericType.GetConstructor(new Type[]{typeof(string),typeof(bool)});
            var instance = constructor?.Invoke(new object[]{methodName,false});
            return (IModelPortAccessor) instance;
        }
        public void CacheRuntimeNodeData(Type type){
            if (type == null) return;
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                CachedDelegatesForSettingValue.Add(type,new Dictionary<string, SetValueDelegate>());
                CachedPortAccessors.Add(type,new Dictionary<string, IModelPortAccessor>());
                var properties = type.GetProperties();
                foreach(var property in properties){
                    var portAttribute =  property.GetCustomAttribute<PortAttribute>();
                    if (portAttribute == null) continue;
                    var propertyAccessor = CreatePortCacheForProperty(property.Name,type,property.PropertyType);
                    CachedPortAccessors[type].Add(property.Name,propertyAccessor);
                }
                //register the fields
                var fields = type.GetFields();
                foreach(var field in fields){
                
                    var getValueDelegate = GetValueDelegateForField(field);
                    CachedDelegatesForGettingValue[type].Add(field.Name,getValueDelegate);
                    if (field.IsPublic){
                        var setValueDelegate = SetValueDelegateForField(field);
                        CachedDelegatesForSettingValue[type].Add(field.Name,setValueDelegate);
                    }
    
                }
                var methods = type.GetMethods();
                foreach(var method in methods){
                    
                    //Check if the method has an [Port] attribute
                    var portAttribute = method.GetCustomAttribute<PortAttribute>();
                    if(portAttribute != null){
                        var propertyAccessor = CreatePortCacheForMethod(method.Name,type,method.ReturnType==typeof(void)?method.GetParameters()[0].ParameterType:method.ReturnType);
                        CachedPortAccessors[type].Add(method.Name,propertyAccessor);
                    }
       
                }
            }
        }

        private SetValueDelegate SetValueDelegateForMethod(MethodInfo method){
            //Check if the method has not an return type
            if(method.ReturnType == typeof(void)){
                //check if the method has only one parameter
                if(method.GetParameters().Length == 1){
                    //Cache the method
                    var methodDelegate = (SetValueDelegate) Delegate.CreateDelegate(typeof(SetValueDelegate),method);
                    return methodDelegate;
                }
            }

            return null;
        }

        private GetValueDelegate GetValueDelegateForField(FieldInfo field){
       
            return field.GetValue;
        }
        private SetValueDelegate SetValueDelegateForField(FieldInfo field){
            
            return field.SetValue;
        }
        
  
    }




    public class ImplicitConversionHelper<T1,T2> : IPortConverterHelper{
        public Func<T1, T2> ConvertFunc;
        public ImplicitConversionHelper(){
            //Caching the implicit method that converts t1 to t2
            var method = typeof(T2).GetMethod("op_Implicit", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(T1) }, null);
            if (method == null){
                //Search it in T1
                Debug.Log($"{typeof(T1)}");
                method = typeof(T1).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.ReturnType==typeof(T2) && x.Name=="op_Implicit");
            }
            //Create the delegate
            if (method != null) 
                ConvertFunc = (Func<T1, T2>) Delegate.CreateDelegate(typeof(Func<T1, T2>), method);
            if (ConvertFunc == null){
                Debug.Log($"{method==null}");
               
            }
        }

        public object Convert(object value){
      
            return ConvertFunc((T1) value);
        }
    }

    public class ConversionFailedException : Exception{
        public ConversionFailedException(string noConverterFoundForType):base(noConverterFoundForType){
            
        
        }
    }

    public static class RuntimeExtension{
     
        //todo  latter on i will try some way caching reflection more efficiently
        public static T GetValue<T>(this Model data,string path,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[type??data.GetType()][path];
            return (T) method.Invoke(data);
        }
        public static object GetValue(this  Model data, string path,Type type=null){
            if(!RuntimeCache.Instance.CachedDelegatesForGettingValue.ContainsKey(type??data.GetType())){
                return null;
            }
            var dic = RuntimeCache.Instance.CachedDelegatesForGettingValue[type ?? data.GetType()];
            var method = dic.ContainsKey(path) ? dic[path] : null;
            return method?.Invoke(data);
        }
        public static object GetListValue(this Model data,string path,int index,Type type=null){
            if(!RuntimeCache.Instance.CachedDelegatesForGettingValue.ContainsKey(type??data.GetType())){
                return null;
            }
            var dic = RuntimeCache.Instance.CachedDelegatesForGettingValue[type ?? data.GetType()];
            var method = dic.ContainsKey(path) ? dic[path] : null;
            if(method == null){
                return null;
            }
            var list = method.Invoke(data) as IList;
            if(list == null){
                return null;
            }
            return list[index];
        }
    
        public static void SetValue<T>(this Model data,string path,T value,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[type??data.GetType()][path];
            method.Invoke(data,value);
        }
        public static void SetValue(this Model data,string path,object value,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[type??data.GetType()][path];
            method.Invoke(data,value);
        }

        public static Type MethodPortType(this MethodInfo info){
            if (info.ReturnType == typeof(void)){
                return info.GetParameters()[0].ParameterType;
            }
            else{
                return info.ReturnType;
            }
        }

        public static Type MemberPortType(this MemberInfo memberInfo){
            if (memberInfo is FieldInfo){
                throw new Exception("FieldInfo is not supported as Port");
            }

            if (memberInfo is MethodInfo methodInfo){
                return MethodPortType(methodInfo);
            }

            if (memberInfo is PropertyInfo propertyInfo){
                return propertyInfo.PropertyType;
            }

            return memberInfo.DeclaringType;
        }

        public static object GetPortValue(this MemberInfo memberInfo,Model data){
            if (memberInfo is FieldInfo){
                throw new Exception("FieldInfo is not supported as Port");
            }

            if (memberInfo is MethodInfo methodInfo){
                return methodInfo.Invoke(data, null);
            }

            if (memberInfo is PropertyInfo propertyInfo){
                return propertyInfo.GetValue(data);
            }

            return null;
        }
    }

}