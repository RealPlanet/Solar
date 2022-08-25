using System.Text;

namespace Solar.Utility.Strings
{
    public static class StringUtility
    {
        public static string GetNumberInFrontOf(string text) => GetNumberInFrontOf(text, out int _);

        /// <summary>
        /// Converts a string prefixed with a number into a int. <br/>
        /// For example: Invoking GetNumberInFrontOf(124ab, out int result) will return a string of text "124" and will
        /// assign to result the integer value of 124.<br/><br/>
        /// 
        /// In the event of an empty string (or no digits) an empty string will be returned and result will contain a value of zero
        /// 
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <param name="result">The int parsed from the result</param>
        /// <returns></returns>
        public static string GetNumberInFrontOf(string text, out int result)
        {        
            result = 0;
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    break;

                sb.Append(c);
            }

            string numberText = sb.ToString();
            if (numberText != string.Empty)
                result = int.Parse(numberText);

            return numberText;
        }
    }
}
