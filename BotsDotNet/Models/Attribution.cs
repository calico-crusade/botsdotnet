using System.Linq;

namespace BotsDotNet
{
    using Utilities;

    public class Attribution
    {
        public object Original { get; private set; }

        public object this[string propname] => GetProperty(propname);
        
        public string[] GetProperties()
        {
            return Original.GetType().GetProperties().Select(t => t.Name).ToArray();
        }

        public object GetProperty(string propname)
        {
            foreach(var prop in Original.GetType().GetProperties())
            {
                if (prop.Name != propname)
                    continue;

                return prop.GetValue(Original);
            }

            return null;
        }

        public T Get<T>(string propname)
        {
            var prop = GetProperty(propname);
            if (prop == null)
                return default(T);

            if (prop is T)
                return (T)prop;

            try
            {
                return (T)prop.ChangeType(typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public Attribution(object original)
        {
            Original = original;
        }
    }
}
