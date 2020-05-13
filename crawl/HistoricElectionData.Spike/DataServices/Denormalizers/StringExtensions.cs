using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HistoricElectionData.Spike.DataServices.Denormalizers
{
    public static class StringExtensions
    {
        private static readonly TextInfo TextInfo = new CultureInfo("ro-RO", false).TextInfo;
        private static readonly Regex NonLettersOrDigitsRegex = new Regex("[^a-z0-9]", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static string ToTitleCase(this string input)
        {
            return input == null ? null : TextInfo.ToTitleCase(input.ToLower());
        }

        public static string ToLettersAndDigitsOnly(this string input)
        {
            return NonLettersOrDigitsRegex.Replace(input, string.Empty);
        }

        public static string ToFormWithoutDiacritics(this string input)
        {
            if (input == null)
            {
                return null;
            }

            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var character in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).Trim();
        }
    }
}
