using UnityEngine;

namespace CustomExtensions
{
    public static class ColorExtensions
    {

        public static Color SetAlpha(this Color colour, float alpha)
        {
            colour.a = alpha;
            return colour;
        }

        public static Color ShiftBrightness(this Color colour, float shift)
        {
            HSBColor colourHSB = new HSBColor(colour);
            colourHSB.b += shift;
            return HSBColor.ToColor(colourHSB);
        }
    }

}