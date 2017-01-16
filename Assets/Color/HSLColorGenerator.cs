using UnityEngine;

public class HSLColorGenerator : MonoBehaviour {

    [Range(0, 360)]
    public float arcAmplitude = 360f;

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
        arcAmplitude = 360f;
    }

    public Color[] Generate(int n) {
        /* We use the following algorithm:
         * - first we generate a random angle [0, 360);
         * - then we select the n colors which are equally spaced along the
         *   arc centered in said angle.
         * - the arc has an amplitude depending on the difficulty coefficient.
         */
        float center = Random.Range(0, 360);
	print("arc ampl = " + arcAmplitude);
        float startAngle = center - arcAmplitude * 0.5f;
        float angleStep = arcAmplitude / n;
        Color[] colors = new Color[n];
        for (int i = 0; i < n; ++i) {
            colors[i] = HSLToRGB(new HSLColor {
                h = startAngle + angleStep * i,
                s = 1f,
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
	print("x = " + x + ", c = " + c + ", hp = " + hp);
	print("HSL = " + color + ", RGB = " + (new Color(col1.r + m, col1.g + m, col1.b + m)));
        return new Color(col1.r + m, col1.g + m, col1.b + m);
    }
}
