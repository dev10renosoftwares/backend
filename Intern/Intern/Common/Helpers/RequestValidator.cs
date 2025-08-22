using Common.Helpers;
using System.Net;

namespace Intern.Common.Helpers
{
    public class RequestValidator
    {

      
            public static void ValidateStrings<T>(T model)
            {
                var properties = typeof(T).GetProperties()
                    .Where(p => p.PropertyType == typeof(string));

                foreach (var prop in properties)
                {
                    var value = prop.GetValue(model) as string;

                    if (string.IsNullOrWhiteSpace(value) || value.Trim().ToLower() == "string")
                    {
                        throw new AppException($"Field '{prop.Name}' cannot be empty or 'string'.", HttpStatusCode.BadRequest);
                    }
                }
            }
        

    }
}
