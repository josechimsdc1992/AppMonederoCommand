using System.ComponentModel;

namespace AppMonederoCommand.Utils
{
    public static class EmunExtensions
    {
        public static T GetEnumValueFromDescription<T>(string description)
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.ToLower() == description.ToLower())
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name.ToLower() == description.ToLower())
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            return default(T);
        }
    }
}

