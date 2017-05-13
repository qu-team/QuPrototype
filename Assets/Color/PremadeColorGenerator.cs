using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

// Returns colors from a premade pool
public class PremadeColorGenerator : IColorGenerator {

    const string COLORS_FILE = "colors";

    // The list of color tuple lists. The outer list is indexed by difficulty,
    // whereas the inner lists are random shuffled and all their elements have
    // the same DE.
    IList<IList<ColorTuple>> colorGroups;
    // This list keeps track of the index we're extracting tuples at, group-wise.
    IList<int> groupIdx;
    // The currently used DE group, related to Difficulty.
    int usedGroup;

    public float Difficulty {
        set {
            usedGroup = colorGroups == null ? 0 : (int)Mathf.Min(value / 4, colorGroups.Count - 1);
            LogHelper.Debug(this, "colorGroup[" + usedGroup + "] has " + colorGroups[usedGroup].Count + " tuples.");
            while (usedGroup < colorGroups.Count - 1 && colorGroups[usedGroup].Count == 0) {
                ++usedGroup;
                LogHelper.Debug(this, "colorGroup[" + usedGroup + "] has " + colorGroups[usedGroup].Count + " tuples.");
            }
        }
    }

    public void Initialize() {
        var colorPool = JsonUtility.FromJson<ColorTuples>(Resources.Load<TextAsset>(COLORS_FILE).text);
        Debug.Assert(colorPool != null && colorPool.tuples != null && colorPool.tuples.Count > 0);
        LogHelper.Ok(this, "Loaded " + colorPool.tuples.Count + " color tuples from Resources/" + COLORS_FILE);
        // Keep only tuples for this level and sort them by descending DE
        colorPool.tuples.RemoveAll(tuple => (tuple.level - 1) != GameManager.Instance.CurrentLevel);
        LogHelper.Info(this, "Kept " + colorPool.tuples.Count + " color tuples for level " + (GameManager.Instance.CurrentLevel + 1));
        colorPool.tuples.OrderByDescending(tuple => tuple.de);
        // Divide colors in a list [[shuffled tuples with DE1], [shuffled tuples with DE2], ...]
        ShuffleBySameDE(colorPool);
        Difficulty = 0;
    }

    public Color[] Generate(int n) {
        // Take group of tuples with index `usedGroup`. This is a list of shuffled tuples
        // with the same DE. Then, extract entry with index `groupIdx[usedGroup]` and update
        // said index. As we shuffled the tuples with the same DE, this corresponds to extracting
        // random non-repeated entries based on the current difficulty.
        LogHelper.Debug(this, "colorGroup[" + usedGroup + "] has " + colorGroups[usedGroup].Count + " tuples.");
        for (int i = 0; i < colorGroups[usedGroup].Count; ++i)
            LogHelper.Debug(this, colorGroups[usedGroup][i].colors.ToArray().ToString());
        LogHelper.Debug(this, "groupIdx[" + usedGroup + "] = " + groupIdx[usedGroup]);
        var tuple = colorGroups[usedGroup][groupIdx[usedGroup]];
        groupIdx[usedGroup] = (groupIdx[usedGroup] + 1) % colorGroups[usedGroup].Count;
        LogHelper.Debug(this, "DE: " + tuple.de);
        // Finally, convert the tuples from the serialization format to Unity colors.
        LogHelper.Debug(this, "Group: " + usedGroup + ", count: " + tuple.colors.Count);
        return tuple.colors.Select(dc => new Color(dc.r/255f, dc.g/255f, dc.b/255f)).ToArray();
    }

    void ShuffleBySameDE(ColorTuples colorPool) {
        colorGroups = DivideByDE(colorPool);
        groupIdx = new List<int>(colorGroups.Count);
        var rand = new System.Random();
        foreach (var group in colorGroups) {
            group.Shuffle(rand);
            groupIdx.Add(0);
        }
    }

    IList<IList<ColorTuple>> DivideByDE(ColorTuples colorPool) {
        var list = new List<IList<ColorTuple>>();
        int idx = 0;
        float de = 0f;
        List<ColorTuple> partial = null;
        do {
            var tup = colorPool.tuples[idx];
            Debug.Assert(tup.colors.Count > 0);
            float nde = tup.de;
            if (nde != de) {
                if (partial != null)
                    list.Add(partial);
                partial = new List<ColorTuple>();
            }
            partial.Add(tup);
            de = nde;
            ++idx;
        } while (idx < colorPool.tuples.Count);

        return list;
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
