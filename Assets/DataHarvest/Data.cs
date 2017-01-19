using UnityEngine;
using System.Text;
using System.Collections.Generic;

[System.Serializable]
public struct DataBundle {
    public bool answerCorrect;
    public float responseTime;
    public float timeSinceStart;
    public float bladeQuDistance;
    public float bladeQuBorderDistance;
    public DataColor correctColor;
    public List<DataColor> wrongColors;
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

    public const uint VERSION = 1;

    public static DataBundle Create(Level level, bool succeeded) {
        var data = new DataBundle {
            answerCorrect = succeeded,
            responseTime = Time.time - level.PartialStartTime,
            timeSinceStart = level.timer.TimeSinceStart,
            bladeQuDistance = CalculateDistance(level.shutter.opening, 1f), // FIXME: use Qu's actual radius
            bladeQuBorderDistance = CalculateDistance(level.shutter.opening,
                                                      level.shutter.internalCircleRadius),
            correctColor = new DataColor {
                r = level.qu.Color.r,
                g = level.qu.Color.g,
                b = level.qu.Color.b,
            },
            wrongColors = new List<DataColor>(),
            backgroundColor = new DataColor {
                r = level.shutter.BackgroundColor.r,
                g = level.shutter.BackgroundColor.g,
                b = level.shutter.BackgroundColor.b
            },
            borderRadius = level.shutter.internalCircleRadius,
            numberOfBlades = level.shutter.bladesNumber
        };
        foreach (var color in level.shutter.BladeColors) {
            if (color != level.qu.Color)
                data.wrongColors.Add(new DataColor { r = color.r, g = color.g, b = color.b });
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

    // Converts the shutter's `opening` [0-1] to the distance (in Unity's units) from blades to `radius`
    static float CalculateDistance(float opening, float borderRadius) {
        return opening; // TODO 
    }

    [System.Serializable]
    private class JsonArrayWrapper<T> {
#pragma warning disable 0649
        public uint version;
        public T[] items;
#pragma warning restore
    }
}
