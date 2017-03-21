using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

// Returns colors from a premade pool
public class PremadeColorGenerator : IColorGenerator {

    const string COLORS_FILE = "colors";

    ColorTuples colorPool;
    int idx = 0;

    public float Difficulty { get; set; }

    public void Initialize() {
        colorPool = JsonUtility.FromJson<ColorTuples>(Resources.Load<TextAsset>(COLORS_FILE).text);
        //colorPool.tuples.RemoveAll(tuple => tuple.level != GameManager.Instance.CurrentLevel);
        //colorPool.tuples.Shuffle(new System.Random());
        Debug.Assert(colorPool != null && colorPool.tuples != null && colorPool.tuples.Count > 0);
        LogHelper.Ok(this, "Loaded " + colorPool.tuples.Count + " color tuples from Resources/" + COLORS_FILE);
    }

    public Color[] Generate(int n) {
        var tuple = colorPool.tuples[idx];
        Debug.Log(tuple.colors);
        idx = (idx + 1) % colorPool.tuples.Count;
        return tuple.colors.Select(dc => new Color(dc.r/255f, dc.g/255f, dc.b/255f)).ToArray();
    }
}

#pragma warning disable 0649
[Serializable]
class ColorTuples {
    public List<ColorTuple> tuples;
}

[Serializable]
class ColorTuple {
    public int level;
    public uint n;     // number of colors
    public float de;   // deltaE
    public int colorblindness;
    public byte l;     // luminosity
    public byte ch;    // chroma
    public List<DataColor> colors;
}
#pragma warning restore 0649
