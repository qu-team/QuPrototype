using UnityEngine;

public class RGBColorGenerator {

    public static readonly float MAX_RADIUS = 0.5f;
    public static readonly float MAX_PADDING = 0.3f;

    public Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);
    public float radius = MAX_RADIUS;
    public float padding = MAX_PADDING;

    public void RandomizeCenter() {
        var range = 1f - radius * 2;
        center = new Vector3(
            Random.Range(0f, range) + radius,
            Random.Range(0f, range) + radius,
            Random.Range(0f, range) + radius);
    }

    public Color[] Generate(int n) {
        var reds = RandomPoints(n, center.x - radius, center.x + radius);
        var greens = RandomPoints(n, center.y - radius, center.y + radius);
        var blues = RandomPoints(n, center.z - radius, center.z + radius);
        return MixColorComponents(n, reds, greens, blues);
    }

    public Color[] MixColorComponents(int n, float[] reds, float[] greens, float[] blues) {
        var colors = new Color[n];
        var remaining = n;
        for (int i = 0; i < n; i++) {
            var r = ExtractFrom(remaining, reds);
            var g = ExtractFrom(remaining, greens);
            var b = ExtractFrom(remaining, blues);
            colors[i] = new Color(r, g, b);
            remaining--;
        }
        return colors;
    }

    /// <summary>
    /// To generate n random points within a range, but keeping a minimum distance between them, a sequence of springs is simulated.
    /// The sequence contains n + 1 springs, and the points are the junctions between them.
    /// The springs at the sequence sides have an equilibrium length of 0, while the internal springs have an equilibrium length equal to the padding.
    /// The algorithm is based on the equation
    ///
    ///     x[1] / x[2] = k[2] / k[1]
    ///
    /// where xs are the springs extensions and ks the springs constants.
    /// The first offset can be calculated with the equation
    ///
    ///     x[1] = k[1..n] / k[0] * (len - x[1])    where  k[1..n] = 1 / Sum i=1..n (1 / k[i])
    ///
    /// then the following value can be calculated by ignoring the first one, but with new length len - x[1],
    /// repeating the process until the last value x[n] is calculated.
    /// The padding is added at the end of the process.
    /// </summary>
    float[] RandomPoints(int n, float from, float to) {
        var springs = RandomValues(n + 1);
        var offsets = new float[n];
        var length = to - from - (n - 1) * padding;
        for (int i = 0; i < n; i++) {
            offsets[i] = Norm01(SpringSequence(springs, i + 1) / springs[i]) * length;
            length -= offsets[i];
        }
        return Pad(offsets, from);
    }

    float[] RandomValues(int n) {
        var values = new float[n];
        for (int i = 0; i < n; i++) { values[i] = Random.value; }
        return values;
    }

    /// <summary>k[i..n] = 1 / Sum j=i..n (1 / k[j])</summary>
    float SpringSequence(float[] springs, int offset) {
        var inverseSum = 0f;
        for (int i = offset; i < springs.Length; i++) {
            inverseSum += 1f / springs[i];
        }
        return 1f / inverseSum;
    }

    float Norm01(float value) {
        return value / (1f + value);
    }

    float[] Pad(float[] values, float initialPadding) {
        values[0] += initialPadding;
        for (int i = 1; i < values.Length; i++) { values[i] += values[i - 1] + padding; }
        return values;
    }

    /// <summary>Chooses a value at random poristion, then deletes it by shifting the following values</summary>
    float ExtractFrom(int remaining, float[] values) {
        var i = Random.Range(0, remaining);
        var value = values[i];
        ShiftLeftFrom(i, values);
        return value;
    }

    void ShiftLeftFrom(int position, float[] array) {
        var lastPosition = array.Length - 1;
        for (int i = position; i < lastPosition; i++) { array[i] = array[i + 1]; }
    }
}
