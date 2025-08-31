using System.Reflection;

namespace TournamentScraper
{
    public static class ObjectHelper
    {
        public static IReadOnlyDictionary<string, string?> GetPropertiesAndValues(this object obj)
        {
            var properties = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyList = new Dictionary<string, string?>();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var value = prop.GetValue(obj, null);
                propertyList.Add(name, value?.ToString());
            }

            return propertyList;
        }

        public static void SetValue(this object target, string name, string? value)
        {
            var propertyInfo = target.GetType().GetProperty(name);
            if (propertyInfo != null && value is not null)
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(target, value);
                    return;
                }

                var convertedValue = TryParseValue(propertyInfo.PropertyType, value, out var result)
                    ? result
                    : null;

                if (convertedValue is null)
                    return;

                propertyInfo.SetValue(target, convertedValue);
            }
        }

        public static bool TryParseValue(Type type, string input, out object? result)
        {
            result = default!;
            var method = type.GetMethod(
                name: "TryParse",
                bindingAttr: BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: [typeof(string), type.MakeByRefType()],
                modifiers: null
            );

            var ctor = type.GetConstructor([typeof(string)]);
            if (ctor is not null)
            {
                result = ctor.Invoke([input]);
                return true;
            }

            if (method == null)
                return false;

            object[] parameters = [input, result];
            var success = method.Invoke(null, parameters);
            if (success is not bool b || !b)
                return false;
            result = parameters[1];
            return true;
        }
    }
}
