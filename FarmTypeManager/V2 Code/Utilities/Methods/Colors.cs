using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.Utilities
{
    /// <summary>Static methods used with colors, e.g. to parse strings into color instances.</summary>
    public static class Colors
    {
        /// <summary>Yields an infinite series of colors, parsed from a list of RGB or RGBA strings.</summary>
        /// <param name="colorStrings">A list of RGB or RGBA strings (e.g. "255 0 0" or "255 0 0 255" for pure red). </param>
        /// <param name="mode">The mode to use when selecting colors.</param>
        /// <param name="timesToSelect">The number of times to select colors, depending on mode.</param>
        /// <returns>An infinite yielded series of colors.</returns>
        /// <remarks>
        /// This returns infinite colors by repeating indefinitely.
        /// The "times to select" argument only affects the behavior of some selection modes.
        /// For example, given mode = Order and timesToSelect = 1, this will repeatedly return the first listed color.
        /// </remarks>
        public static IEnumerable<Color> ParseColors(List<string> colorStrings, SelectionMode mode, int timesToSelect)
        {
            List<Color> colors = [];
            foreach (string colorString in colorStrings)
            {
                if (!TryParseColor(colorString, out Color color, out string error))
                    throw new ArgumentException(error);
                colors.Add(color);
            }

            while (true)
            {
                foreach (Color color in Collections.SelectElementsByMode(colors, mode, timesToSelect))
                    yield return color;
            }
        }

        /// <summary>Tries to parse a text string into a color.</summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="color">The parsed color, or a default color if parsing fails.</param>
        /// <param name="error">A description of why this text could not be parsed. Null if parsing succeeds.</param>
        /// <returns>The color indicated by the parsed text.</returns>
        /// <remarks>
        /// Supported formats are "R G B" (red green blue) and "R G B A" (red green blue alpha). Each value must be from 0 to 255, and separated by spaces.
        /// For example, "0 255 0 127" is pure green with 50% transparency.
        /// </remarks>
        public static bool TryParseColor(string text, out Color color, out string error)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                color = default;
                error = $"The text \"{text}\" can't be converted into a color: It's null or blank.";
                return false;
            }

            string[] split = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (split?.Length < 3 || split.Length > 4)
            {
                color = default;
                error = $"The text \"{text}\" can't be converted into a color: Wrong number of space-separated values. Found {split.Length}, expected 3 or 4 (RGB or RGBA).";
                return false;
            }

            if (!int.TryParse(split[0], out int r) || r < 0 || r > 255)
            {
                color = default;
                error = $"The text \"{text}\" can't be converted into a color: Invalid value. 'R' is \"{split[0]}\"; it should be an integer from 0 to 255.";
                return false;
            }
            if (!int.TryParse(split[1], out int g) || g < 0 || g > 255)
            {
                color = default;
                error = $"The text \"{text}\" can't be converted into a color: Invalid value. 'G' is \"{split[1]}\"; it should be an integer from 0 to 255.";
                return false;
            }
            if (!int.TryParse(split[2], out int b) || b < 0 || b > 255)
            {
                color = default;
                error = $"The text \"{text}\" can't be converted into a color: Invalid value. 'B' is \"{split[2]}\"; it should be an integer from 0 to 255.";
                return false;
            }

            if (split.Length == 3)
            {
                color = new(r, g, b);
                error = null;
                return true;
            }

            if (!int.TryParse(split[3], out int a) || a < 0 || a > 255)
            {
                color = default;
                error = $"The text \"{text}\" can't be converted into a color: Invalid value. 'A' is \"{split[3]}\"; it should be an integer from 0 to 255.";
                return false;
            }

            color = new(r, g, b, a);
            error = null;
            return true;
        }
    }
}
