using UnityEngine;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public struct DataBundle {
    public int levelNum;
    public bool answerCorrect;
    public float responseTime;
    public float timeSinceStart;
    public float bladeQuDistance;
    public float bladeQuBorderDistance;
    public List<DataColor> colors;
    public int correctColor;
    public int guessedColor;
    public DataColor backgroundColor;
    public float borderRadius;
    public uint numberOfBlades;
}

[System.Serializable]
public struct DataColor {
    public float r;
    public float g;
    public float b;
    public static implicit operator Color(DataColor d) {
        return new Color(d.r, d.g, d.b);
    }
}

public static class Data {

    public const uint VERSION = 2;

    public static DataBundle Create(Level level, Color guessedColor) {
        var data = new DataBundle {
            levelNum = GameManager.Instance.CurrentLevel,
            answerCorrect = guessedColor == level.qu.Color,
            responseTime = Time.time - level.PartialStartTime,
            timeSinceStart = level.timer.TimeSinceStart,
            bladeQuDistance = BladesAbsoluteDistanceFromQu(level),
            bladeQuBorderDistance = BladesAbsoluteDistanceFromBorder(level),
            colors = new List<DataColor>(),
            correctColor = -1,
            guessedColor = -1,
            backgroundColor = new DataColor {
                r = level.shutter.BackgroundColor.r,
                g = level.shutter.BackgroundColor.g,
                b = level.shutter.BackgroundColor.b
            },
            borderRadius = level.shutter.internalCircleRadius,
            numberOfBlades = level.shutter.bladesNumber
        };
        for (int i = 0; i < level.shutter.BladeColors.Count; ++i) {
            var color = level.shutter.BladeColors[i];
            data.colors.Add(new DataColor {
                    r = color.r,
                    g = color.g,
                    b = color.b
            });
            if (color == level.qu.Color) {
                UnityEngine.Debug.Assert(data.correctColor == -1, "More than 1 correct color???");
                data.correctColor = i;
            }
            if (color == guessedColor) {
                UnityEngine.Debug.Assert(data.guessedColor == -1, "More than 1 guessed color???");
                data.guessedColor = i;
            }
         }
        return data;
    }

    // Serialize a IEnumerable of DataBundles into a string, adding version information.
    public static string Serialize(IEnumerable<DataBundle> dataList) {
        StringBuilder jsonBuilder = new StringBuilder("{\"version\":" + VERSION + ",\"items\":[");
        foreach (DataBundle data in dataList) {
            jsonBuilder.Append(UnityEngine.JsonUtility.ToJson(data));
            jsonBuilder.Append(",");
        }

        if (jsonBuilder.Length == 1) return null; // should never happen

        // remove trailing comma
        jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
        jsonBuilder.Append("]}");

        return jsonBuilder.ToString();
    }

    public static List<DataBundle> Deserialize(string data) {
        try {
            var bundles = UnityEngine.JsonUtility.FromJson<JsonArrayWrapper<DataBundle>>(data);
            if (bundles.version != VERSION)
                return null;
            return new List<DataBundle>(bundles.items);
        } catch (System.ArgumentException) {
            LogHelper.Warn("Data", "failed to parse JSON: " + data);
            return null;
        }
    }

    // Converts the shutter's `opening` [0-1] to the distance (in Unity's units) from blades to qU
    static float BladesAbsoluteDistanceFromQu(Level level) {
        var absoluteOpening = level.shutter.opening * level.shutter.relativeSize;
        return absoluteOpening - level.qu.transform.localScale.x;
    }

    // Converts the shutter's `opening` [0-1] to the distance (in Unity's units) from blades to `radius`
    static float BladesAbsoluteDistanceFromBorder(Level level) {
        var absoluteOpening = level.shutter.opening * level.shutter.relativeSize;
        return absoluteOpening - level.shutter.internalCircleRadius;
    }

    [System.Serializable]
    private class JsonArrayWrapper<T> {
#pragma warning disable 0649
        public uint version;
        public T[] items;
#pragma warning restore
    }
}
