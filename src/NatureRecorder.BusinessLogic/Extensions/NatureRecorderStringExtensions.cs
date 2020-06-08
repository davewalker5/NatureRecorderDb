using System.Text.RegularExpressions;

namespace NatureRecorder.BusinessLogic.Extensions
{
    public static class NatureRecorderStringExtensions
    {
        public static string CleanString(this string input)
        {
            return Regex.Replace(input, @"\t|\n|\r", "").Replace("  ", " ").Trim();
        }
    }
}
