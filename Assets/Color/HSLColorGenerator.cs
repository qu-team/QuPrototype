using UnityEngine;

public class HSLColorGenerator {

    public float Difficulty {
        set {
            arcAmplitude = 240f / value;
        }
    }

    [Range(0, 1)]
    public float saturation = 1f;

    float arcAmplitude = 240f;

    struct HSLColor {
        public float h; // range: [0, 360)
        public float s;
        public float l;
        public string ToString() {
            return "(" + h + ", " + s + ", " + l + ")";
        }
        public static implicit operator string(HSLColor c) {
            return c.ToString();
        }
    }

    void Start() {
        saturation = 1f;
    }

    public Color[] Generate(int n) {
        /* We use the following algorithm:
         * - first we generate a random angle [0, 360);
         * - then we select the n colors which are equally spaced along the
         *   arc starting at that angle;
         * - the arc has an amplitude depending on the difficulty coefficient.
         */
        float startAngle = Random.Range(0f, 360f);
        float angleStep = arcAmplitude / (n - 1);
        LogHelper.Debug(this, "arc ampl = " + arcAmplitude + ", startAngle = " + startAngle + ", step = " + angleStep);
        Color[] colors = new Color[n];
        for (int i = 0; i < n; ++i) {
            float h = startAngle + angleStep * i;
            if (h > 360)
                h -= 360;
            colors[i] = HSLToRGB(new HSLColor {
                h = h,
                s = saturation,
                l = 0.5f
            });
        }
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
}
