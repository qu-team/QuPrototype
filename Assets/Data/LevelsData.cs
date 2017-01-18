using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

// Mapping of levels.json
[Serializable]
struct QuLevels {
	public List<QuLevel> levels;
}

// Contains data on levels' structure
public sealed class LevelsData {
    public readonly List<QuLevel> levels;

    public LevelsData(string fname) {
        try {
            string json = Resources.Load<TextAsset>(fname.Replace(".json", "")).text;
            levels = JsonUtility.FromJson<QuLevels>(json).levels;
            LogHelper.Ok(this, "Loaded " + levels.Count + " levels from " + fname + ".");
        } catch (Exception ex) {
            LogHelper.Error(this, "Couldn't load levels from " + fname + ": " + ex);
        }
    }

    public QuLevel this[int n] {
        get {
            return levels[n];
        }
    }

    public int Count {
        get {
            return levels.Count;
        }
    }
}
