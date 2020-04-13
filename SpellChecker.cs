using System.Text.RegularExpressions;

namespace Rubric_automizer
{
    internal class SpellChecker

    {
        internal bool CheckByMatch(string lastSubtitle)
        {
            bool negative = false;
            Match fullPersonName = Regex.Match(lastSubtitle, @"_[\d]+[\,с]");
            if (fullPersonName.Success)
            {
            }
            return negative;
        }

        internal string CleanupString(string str)
        {
            str = str.Replace("  ", " ");

            return str;
        }
    }
}