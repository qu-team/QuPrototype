using UnityEngine;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Conversions;

public class LABColorGenerator {

    public float Difficulty {
        set {
            totalSpacing = 200f / Mathf.Pow(value, 0.5f);
        }
    }

    [Range(0, 100)]
    public float lightness = 50f;

    /** Initial sum of spacings between picked colors */
    float totalSpacing = 200f;

    void Start() {
        lightness = 50f;
    }

    public Color[] Generate(int n) {
        /*
         * LAB color space has 3 variables:
         * L from 0 to 100
         * A from -128 to 128
         * B from -128 to 128
         * We keep L fixed and pick either a random A or B. Then we choose 3 equally-spaced B's (or A's) starting from
         * a random value. The spacing between B's (or A's) is inversely proportional to the difficulty.
         */
        bool pickA = Random.Range(0, 1f) < 0.5f;
        float a = Random.Range(-128, 128);
        float firstValue = Random.Range(-128, 128 - totalSpacing);
        float step = totalSpacing / (n - 1);
        LogHelper.Debug(this, "totalSpacing = " + totalSpacing + ", firstValue = " + firstValue + ", step = " + step);
        Color[] colors = new Color[n];
        for (int i = 0; i < n; ++i) {
            float b = firstValue + step * i;
            Debug.Assert(-128 <= b && b <= 128, "b should be within -128 and 128 but is " + b + "!");
            colors[i] = new Lab {
                    L = lightness,
                    A = pickA ? a : b,
                    B = pickA ? b : a
            }.To<Rgb>();
            LogHelper.Debug(this, "values = " + lightness + ", " + (pickA ? a : b) + ", " + (pickA ? b : a));
            LogHelper.Debug(this, "color["+i+"] = "+colors[i]);
        }
        return colors;
    }
}
