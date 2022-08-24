using Solar.Utility.Strings;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SolarX.Utility.Console
{
    /// <summary>
    /// Allows for the definition of text colour within a string of text.<br/>
    /// By default the marked for color definition is the character '^' followed by a number,
    /// this marker can be set to something else when instanciating a object. <br/>
    /// The color number corresponds to the enum value of the System.ConsoleColor,
    /// for example ^15 will set the following text to white color.<br/>
    /// Accepted values (unless enum was updated) are:<br/>
    ///     Black = 0<br/>
    ///     DarkBlue = 1<br/>
    ///     DarkGreen = 2<br/>
    ///     DarkCyan = 3<br/>
    ///     DarkRed = 4<br/>
    ///     DarkMagenta = 5<br/>
    ///     DarkYellow = 6<br/>
    ///     Gray = 7<br/>
    ///     DarkGray = 8<br/>
    ///     Blue = 9<br/>
    ///     Green = 10<br/>
    ///     Cyan = 11<br/>
    ///     Red = 12<br/>
    ///     Magenta = 13<br/>
    ///     Yellow = 14<br/>
    ///     White = 15<br/>
    /// <br/>     
    /// It is important to know that the implementation of this class is lazy, the text is analyzed completely upon the first
    /// color/text pair request, or after invoking the specific bake method.<br/>
    /// </summary>
    public sealed class ColoredText
    {
        public string OriginalText { get; }
        public string ColorMarker { get; }

        private bool m_Baked = false;
        private readonly string[] m_DelimiterArray;
        private ImmutableArray<ConsoleColor> BakedColors;
        private ImmutableArray<string> BakedText;

        public static ColoredText From(string originalText, string colorMarker = "^") => new ColoredText(originalText, colorMarker);

        public bool BakeText()
        {
            bool isValid = false;

            ImmutableArray<ConsoleColor>.Builder builderColors = ImmutableArray.CreateBuilder<ConsoleColor>();
            ImmutableArray<string>.Builder builderText = ImmutableArray.CreateBuilder<string>();

            string[] textLines = OriginalText.Split(m_DelimiterArray, StringSplitOptions.None);
            // Add the first token before processing because the first token is always before the first delimiter
            builderText.Add(textLines[0]);
            // Not used color, as when getting pairs the first one will always get the current console color
            builderColors.Add(ConsoleColor.White);

            // Skip first entry as this one will never have a delimiter before it
            for (int i = 1; i < textLines.Length; i++)
            {
                string line = textLines[i];
                string lineColor = StringUtils.GetNumberInFrontOf(line);
                // Substring without allocating new memory
                ReadOnlySpan<char> actualLineSpan = line.AsSpan(lineColor.Length);
                builderText.Add(actualLineSpan.ToString());
                if (!Enum.TryParse<ConsoleColor>(lineColor, out ConsoleColor color))
                {
                    builderColors.Add(builderColors[builderColors.Count - 1]);
                    continue;
                }

                builderColors.Add(color);
            }

            BakedColors = builderColors.ToImmutableArray();
            BakedText = builderText.ToImmutableArray();

            m_Baked = isValid;
            return isValid;
        }

        private ColoredText(string originalText, string colorMarker, bool doBake = false)
        {
            OriginalText = originalText;
            ColorMarker = colorMarker;
            m_DelimiterArray = new[] { ColorMarker };

            if (doBake)
                BakeText();
        }

        public IEnumerable<(ConsoleColor, string)> NextLine()
        {
            if (!m_Baked)
                BakeText();

            yield return (System.Console.ForegroundColor, BakedText[0]);
            for (int i = 1; i < BakedText.Length; i++)
            {
                yield return (BakedColors[i], BakedText[i]);
            }
        }
    }
}
