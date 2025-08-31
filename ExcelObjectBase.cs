using System.Reflection;

namespace TournamentScraper
{
    public abstract class ExcelObjectBase<T> : IExcelObject<T>
        where T : notnull
    {
        public abstract T Id { get; }

        public virtual void SetProperties(IReadOnlyDictionary<string, ExcelProperty> properties)
        {
            foreach (var (key, excelProperty) in properties)
            {
                var propertyInfo = GetType().GetProperty(excelProperty.PropertyName);
                if (propertyInfo != null && string.IsNullOrEmpty(excelProperty.Value) is false)
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        propertyInfo.SetValue(this, excelProperty.Value);
                        continue;
                    }

                    var convertedValue = ObjectHelper.TryParseValue(
                        propertyInfo.PropertyType,
                        excelProperty.Value ?? string.Empty,
                        out var result
                    )
                        ? result
                        : null;

                    if (convertedValue is null)
                        return;

                    propertyInfo.SetValue(this, convertedValue);
                }
            }
        }

        public virtual IReadOnlyDictionary<string, ExcelProperty> GetExcelProperties()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertyList = new Dictionary<string, ExcelProperty>();

            foreach (var prop in properties)
            {
                var name = prop.Name;
                var value = prop.GetValue(this, null);
                var excelAttribute = prop.GetCustomAttribute<ExcelAttribute>();
                if (excelAttribute is null)
                    continue;
                propertyList.Add(
                    name,
                    new ExcelProperty
                    {
                        Header = excelAttribute.Header,
                        Order = excelAttribute.Order,
                        PropertyName = name,
                        PropertyType = prop.PropertyType,
                        Value = value?.ToString() ?? string.Empty
                    }
                );
            }

            return propertyList;
        }
    }
}
