using Newtonsoft.Json;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

        object ExecuteDynamicMethod(MethodInfo info, object def, out bool error, params object[] defaultparameters);

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

        public virtual object ChangeType(object obj, Type toType)
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

        public virtual T ChangeType<T>(object obj)
        {
            return (T)ChangeType(obj, typeof(T));
        }

        public virtual T GetInstance<T>()
        {
            return container.GetInstance<T>();
        }

        public virtual object GetInstance(Type type)
        {
            return container.GetInstance(type);
        }

        public virtual IEnumerable<T> GetAllTypesOf<T>()
        {
            return container.GetAllInstances<T>().ToArray();
        }

        public virtual IEnumerable<Type> GetTypes(Type implementedInterface)
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

                if (Attribute.IsDefined(type, typeof(NoDescpAttribute)))
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
                    if (Attribute.IsDefined(type, typeof(NoDescpAttribute)))
                        continue;

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

        public virtual object ExecuteDynamicMethod(MethodInfo info, object def, out bool error, params object[] defaultparameters)
        {
            try
            {
                error = false;
                if (info == null)
                    return null;

                var pars = info.GetParameters();

                if (pars.Length <= 0)
                    return ExecuteMethod(info, def, out error);

                var args = new object[pars.Length];

                for (var i = 0; i < pars.Length; i++)
                {
                    var par = pars[i];

                    var pt = par.ParameterType;

                    var fit = defaultparameters.FirstOrDefault(t => pt.IsAssignableFrom(t.GetType()));

                    if (fit != null)
                    {
                        args[i] = fit;
                        continue;
                    }

                    var next = GetInstance(pt);
                    if (next != null)
                    {
                        args[i] = next;
                        continue;
                    }

                    args[i] = pt.IsValueType ? Activator.CreateInstance(pt) : null;
                }

                return ExecuteMethod(info, def, out error, args);
            }
            catch (Exception ex)
            {
                error = true;
                return ex;
            }
        }

        public virtual object ExecuteMethod(MethodInfo info, object def, out bool error, params object[] pars)
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

        public virtual T Clone<T>(T item)
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
