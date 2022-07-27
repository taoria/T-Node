using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNodeCore.Attribute;
using TNodeCore.Models;
using TNodeCore.Runtime.Interfaces;
using UnityEngine;

namespace TNodeCore.RuntimeCache{
    public class PropAccessor<T1, T2>:IModelPropertyAccessor{
        public readonly Func<T1, T2> Get;
        public readonly Action<T1, T2> Set;
        public PropAccessor(string propName){
            Type t = typeof(T1);
     
            MethodInfo getter = t.GetMethod("get_" + propName);
            MethodInfo setter = t.GetMethod("set_" + propName);
            Type = getter?.ReturnType??setter?.GetParameters()[0].ParameterType;
            if(getter!=null)
                Get = (Func<T1, T2>)Delegate.CreateDelegate(typeof(Func<T1, T2>), null, getter);
            if(setter!=null)
                Set = (Action<T1, T2>)Delegate.CreateDelegate(typeof(Action<T1, T2>), null, setter);
        }
        public static PropAccessor<T1, T2> Create(string propName){
            return new PropAccessor<T1, T2>(propName);
        }

        public object GetValue(object model){
            return Get((T1)model);
        }

        public void SetValue(object model, object value){
            Set((T1)model,(T2)value);
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
        public delegate object GetValueDelegate(IModel nodeData);
        public delegate void SetValueDelegate(IModel nodeData,object value);



        public readonly Dictionary<Type, Dictionary<string,GetValueDelegate>> CachedDelegatesForGettingValue =
            new ();
        public readonly Dictionary<Type,Dictionary<string,SetValueDelegate>> CachedDelegatesForSettingValue =
            new ();
        public readonly Dictionary<Type,Dictionary<string,IModelPropertyAccessor>> CachedPropertyAccessors =
            new ();
        /// <summary>
        /// TODO: Converters now work globally, but it should be possible to specify a converter for a specific graph.but it will be too nested.so in current implementation, we will use a global converter.
        /// </summary>
        private readonly Dictionary<Type,Dictionary<Type,IPortConverterHelper>> CachedPortConverters =
            new ();
        

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
                Debug.Log("find conversion");
                CacheRuntimePortTypeConversion(type);
            }
            else{
                Debug.Log(type);
            }
        }

        private void CacheRuntimePortTypeConversion(Type type){
            if (type.BaseType != null){
                var genericType = type.BaseType.GetGenericTypeDefinition();
                if (genericType != typeof(PortTypeConversion<,>)){
                    return;
                }
            }
            else{
                return;
            }

            var type1 = type.BaseType.GetGenericArguments()[0];
            var type2 = type.BaseType.GetGenericArguments()[1];
            Debug.Log(type1);
            Debug.Log(type2);
            var specificType = typeof(PortConverterHelper<,>).MakeGenericType(type1, type2);
            var instance = Activator.CreateInstance(specificType, type) as IPortConverterHelper;
            if (instance == null){
                return;
            }
            if (!CachedPortConverters.ContainsKey(type1)){
                CachedPortConverters.Add(type1,new Dictionary<Type,IPortConverterHelper>());
            }
            CachedPortConverters[type1].Add(type2,instance);
        }
        
        public object GetConvertedValue(Type from,Type to,object value){
            if(!CachedPortConverters.ContainsKey(from)){
                throw new ConversionFailedException("No converter found for type "+from);
            }
            if(!CachedPortConverters[from].ContainsKey(to)){
                return value;
            }
            return CachedPortConverters[from][to].Convert(value);
        }
        public List<Type> GetSupportedTypes(Type type){
            if(!CachedPortConverters.ContainsKey(type)){
                return null;
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
        public static IModelPropertyAccessor Create(string propName,Type targetType,Type valueType){
            var makeGenericType = typeof (PropAccessor<,>).MakeGenericType(targetType,valueType);
            var constructor = makeGenericType.GetConstructor(new Type[]{typeof(string)});
            var instance = constructor?.Invoke(new object[]{propName});
            return (IModelPropertyAccessor) instance;
        }
        public void CacheRuntimeNodeData(Type type){
            if (type == null) return;
            if(!CachedDelegatesForGettingValue.ContainsKey(type)){
                CachedDelegatesForGettingValue.Add(type, new Dictionary<string, GetValueDelegate>());
                CachedDelegatesForSettingValue.Add(type,new Dictionary<string, SetValueDelegate>());
                CachedPropertyAccessors.Add(type,new Dictionary<string, IModelPropertyAccessor>());
                
                var properties = type.GetProperties();
                foreach(var property in properties){
                    
                    var propertyAccessor = Create(property.Name,type,property.PropertyType);
                    CachedPropertyAccessors[type].Add(property.Name,propertyAccessor);
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
            }
        }
        private GetValueDelegate GetValueDelegateForField(FieldInfo field){
       
            return field.GetValue;
        }
        private SetValueDelegate SetValueDelegateForField(FieldInfo field){
            
            return field.SetValue;
        }
        
  
    }

    public class ConversionFailedException : Exception{
        public ConversionFailedException(string noConverterFoundForType):base(noConverterFoundForType){
            
        
        }
    }

    public static class RuntimeExtension{
     
        //todo  latter on i will try some way caching reflection more efficiently
        public static T GetValue<T>(this IModel data,string path,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForGettingValue[type??data.GetType()][path];
            return (T) method.Invoke(data);
        }
        public static object GetValue(this  IModel data, string path,Type type=null){
            if(!RuntimeCache.Instance.CachedDelegatesForGettingValue.ContainsKey(type??data.GetType())){
                return null;
            }
            var dic = RuntimeCache.Instance.CachedDelegatesForGettingValue[type ?? data.GetType()];
            var method = dic.ContainsKey(path) ? dic[path] : null;
            return method?.Invoke(data);
        }
    
        public static void SetValue<T>(this IModel data,string path,T value,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[type??data.GetType()][path];
            method.Invoke(data,value);
        }
        public static void SetValue(this IModel data,string path,object value,Type type=null){
            var method = RuntimeCache.Instance.CachedDelegatesForSettingValue[type??data.GetType()][path];
            method.Invoke(data,value);
        }
    }

}