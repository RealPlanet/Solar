namespace Solar.Utility.Strings
{
    public static class StringUtils
    {
        public static string GetNumberInFrontOf(string text) => GetNumberInFrontOf(text, out int _);
        public static string GetNumberInFrontOf(string text, out int result)
        {
            string numberText = string.Empty;
            result = 0;

            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    break;

                numberText += c;
            }

            if (numberText != string.Empty)
                result = int.Parse(numberText);

            return numberText;
        }
    }
}
