using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ElectionResults.Core.Models;
using Newtonsoft.Json;

namespace ElectionResults.Core.Services
{
    public static class Extensions
    {
        public static string ConvertEnumToString(this Enum type)
        {
            return type
                       .GetType()
                       .GetMember(type.ToString())
                       .FirstOrDefault()
                       ?.GetCustomAttribute<DescriptionAttribute>()
                       ?.Description ?? type.ToString();
        }

        public static ElectionsConfig ParseConfig(this string configJson)
        {
            try
            {
                return JsonConvert.DeserializeObject<ElectionsConfig>(configJson);
            }
            catch (Exception e)
            {
                return new ElectionsConfig();
            }
        }
    }
}
