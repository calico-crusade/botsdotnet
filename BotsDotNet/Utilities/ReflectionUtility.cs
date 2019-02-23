using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BotsDotNet.Utilities
{
    using Handling;

    public interface IReflectionUtility
    {
        IEnumerable<Type> GetTypes(Type implementedInterface);

        object ChangeType(object obj, Type toType);

        T ChangeType<T>(object obj);

        IEnumerable<T> GetAllTypesOf<T>();

        T GetInstance<T>();

        object GetInstance(Type type);

        object ExecuteMethod(MethodInfo info, object def, out bool error, params object[] pars);

        T Clone<T>(T item);
    }

    public class ReflectionUtility : IReflectionUtility
    {
        public static Encoding Encoder = Encoding.UTF8;

        private Container container;

        public ReflectionUtility(Container container)
        {
            this.container = container;
        }

        public object ChangeType(object obj, Type toType)
        {
            if (obj == null)
                return null;

            var fromType = obj.GetType();

            var to = Nullable.GetUnderlyingType(toType) ?? toType;
            var from = Nullable.GetUnderlyingType(fromType) ?? fromType;

            if (to == from)
                return obj;

            if (to.IsEnum)
            {
                return Enum.ToObject(to, Convert.ChangeType(obj, to.GetEnumUnderlyingType()));
            }

            if (from == typeof(byte[]))
            {
                obj = Encoder.GetString((byte[])obj);

                if (to == typeof(string))
                    return obj;
            }

            if (to == typeof(byte[]) && from == typeof(string))
            {
                return Encoder.GetBytes((string)obj);
            }

            return Convert.ChangeType(obj, to);
        }

        public T ChangeType<T>(object obj)
        {
            return (T)ChangeType(obj, typeof(T));
        }

        public T GetInstance<T>()
        {
            return container.GetInstance<T>();
        }
        
        public object GetInstance(Type type)
        {
            return container.GetInstance(type);
        }

        public IEnumerable<T> GetAllTypesOf<T>()
        {
            return container.GetAllInstances<T>().ToArray();
        }

        public IEnumerable<Type> GetTypes(Type implementedInterface)
        {
            var assembly = Assembly.GetEntryAssembly();
            var alreadyLoaded = new List<string>
            {
                assembly.FullName
            };

            foreach (var type in assembly.DefinedTypes)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (type.ImplementedInterfaces.Contains(implementedInterface))
                    yield return type;
            }

            var assems = assembly.GetReferencedAssemblies()
                .Select(t => t.FullName)
                .Except(alreadyLoaded)
                .ToArray();
            foreach (var ass in assems)
            {
                foreach (var type in GetTypes(implementedInterface, ass, alreadyLoaded))
                {
                    yield return type;
                }
            }
        }

        private IEnumerable<Type> GetTypes(Type implementedInterface, string assembly, List<string> alreadyLoaded)
        {
            if (alreadyLoaded.Contains(assembly))
                yield break;

            alreadyLoaded.Add(assembly);
            var asml = Assembly.Load(assembly);
            foreach (var type in asml.DefinedTypes)
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                if (type.ImplementedInterfaces.Contains(implementedInterface))
                    yield return type;
            }

            var assems = asml.GetReferencedAssemblies()
                .Select(t => t.FullName)
                .Except(alreadyLoaded)
                .ToArray();
            foreach (var ass in assems)
            {
                foreach (var type in GetTypes(implementedInterface, ass, alreadyLoaded))
                {
                    yield return type;
                }
            }

        }

        public object ExecuteMethod(MethodInfo info, object def, out bool error, params object[] pars)
        {
            try
            {
                error = false;

                return info.Invoke(def, pars);
            }
            catch (Exception ex)
            {
                error = true;
                return ex;
            }
        }

        public T Clone<T>(T item)
        {
            var ser = JsonConvert.SerializeObject(item);
            return JsonConvert.DeserializeObject<T>(ser);
        }

        public static MapHandler DependencyInjection()
        {
            return new MapHandler()
                .AllOf<IComparitorProfile>()
                .AllOf<IPlugin>()
                .AllOf<IRestriction>()
                .Use<IReflectionUtility, ReflectionUtility>((c, i) => new ReflectionUtility(c));
        }

        public static T Create<T>()
        {
            return DependencyInjection().Build<T>();
        }
    }
}
