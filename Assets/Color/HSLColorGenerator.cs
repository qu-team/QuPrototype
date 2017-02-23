using UnityEngine;

public class HSLColorGenerator : IColorGenerator {

    const float MIN_COLOR_SEP = 7;

    [Range(0, 1)]
    public float saturation = 1f;
    [Range(0, 1)]
    public float lightness = 0.5f;

    float arcAmplitude = 240f;

    struct HSLColor {
        public float h; // range: [0, 360)
        public float s;
        public float l;
        public override string ToString() {
            return "(" + h + ", " + s + ", " + l + ")";
        }
        public static implicit operator string(HSLColor c) {
            return c.ToString();
        }
    }

    public float Difficulty {
        set {
            arcAmplitude = 240f / value;
        }
    }

    public Color[] Generate(int n) {
        /* We use the following algorithm:
         * - first we generate a random angle [0, 360);
         * - then we select the n colors which are equally spaced along the
         *   arc starting at that angle;
         * - the arc has an amplitude depending on the difficulty coefficient.
         */
        float startAngle = Random.Range(0f, 360f);
        float angleStep = Mathf.Max(MIN_COLOR_SEP, arcAmplitude / (n - 1));
        LogHelper.Debug(this, "arc ampl = " + arcAmplitude + ", startAngle = " + startAngle + ", step = " + angleStep);
        HSLColor[] hslColors = new HSLColor[n];
        for (int i = 0; i < n; ++i) {
            float h = startAngle + angleStep * i;
            if (h > 360)
                h -= 360;
            hslColors[i] = new HSLColor {
                h = h,
                s = saturation,
                l = lightness
            };
        }

        hslColors = Adjust(hslColors);

        Color[] colors = new Color[n];
        for (int i = 0; i < n; ++i)
            colors[i] = HSLToRGB(hslColors[i]);

        return colors;
    }

    Color HSLToRGB(HSLColor color) {
        // Ref: https://en.wikipedia.org/wiki/HSL_and_HSV#Converting_to_RGB
        float c = (1 - Mathf.Abs(2 * color.l - 1)) * color.s;
        float hp = color.h / 60;
        float x = c * (1 - Mathf.Abs(hp % 2 - 1));
        Color col1 = new Color(
            // Red
            0 <= hp && hp <= 1 ? c :
                       hp <= 2 ? x :
                       hp <= 3 ? 0 :
                       hp <= 4 ? 0 :
                       hp <= 5 ? x :
                       hp <  6 ? c :
                                 0,
            // Green
            0 <= hp && hp <= 1 ? x :
                       hp <= 2 ? c :
                       hp <= 3 ? c :
                       hp <= 4 ? x :
                       hp <= 5 ? 0 :
                       hp <= 6 ? 0 :
                                 0,
            // Blue
            0 <= hp && hp <= 1 ? 0 :
                       hp <= 2 ? 0 :
                       hp <= 3 ? x :
                       hp <= 4 ? c :
                       hp <= 5 ? c :
                       hp <= 6 ? x :
                                 0
            );
        float m = color.l - 0.5f * c;

        LogHelper.Debug(this, "x = " + x + ", c = " + c + ", hp = " + hp);
        LogHelper.Debug(this, "HSL = " + color + ", RGB = " + (new Color(col1.r + m, col1.g + m, col1.b + m)));

        return new Color(col1.r + m, col1.g + m, col1.b + m);
    }

    readonly ColorRange[] indistinguishableColors = new ColorRange[] {
        new ColorRange(100, 130), // greens
        new ColorRange(350, 360), // reds
        new ColorRange(235, 250), // blues
    };

    // Try to avoid having 2 or more colors which are perceptually indistinguishable.
    // Algorithm goes as follows:
    // 1. find if a range of "indistinguishableColors" contains at least 2 colors
    // 2. if so, count exactly how many colors it contains
    // 3. break the cycle at that range, then offset all colors such that only 1 color
    //    remains in the range
    HSLColor[] Adjust(HSLColor[] colors) {
        int n = colors.Length;
        for (int i = 0; i < n; ++i)
            LogHelper.Debug(this, "colors[" + i + "] = " + colors[i]);
        HSLColor[] newcolors = new HSLColor[n];
        System.Array.Copy(colors, newcolors, n);
        bool foundInRange = false;
        int colorsInRange = 0;
        foreach (var range in indistinguishableColors) {
            colorsInRange = 0;
            for (int i = 0; i < n; ++i) {
                if (range.min <= colors[i].h && colors[i].h <= range.max) {
                    if (++colorsInRange > 1)
                        foundInRange = true;
                }
            }
            if (foundInRange)
                break;
        }
        if (foundInRange) {
            // Offset colors of (step * (colorsInRange - 1))
            float offset = arcAmplitude / (n - 1) * (colorsInRange - 1);
            LogHelper.Debug(this, "adjusting with offset " + offset);
            for (int i = 0; i < n; ++i) {
                newcolors[i].h += offset;
                if (newcolors[i].h > 360)
                    newcolors[i].h -= 360;
                LogHelper.Debug(this, "newcolors[" + i + "] = " + newcolors[i]);
            }
        }
        return newcolors;
    }
}

struct ColorRange {
    public readonly float min;
    public readonly float max;
    public ColorRange(float min, float max) {
        this.min = min;
        this.max = max;
        Debug.Assert(min <= max, "ColorRange: min must be <= max!");
    }
}
