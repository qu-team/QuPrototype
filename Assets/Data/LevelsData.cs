using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public sealed class LevelsData {
    public readonly List<QuLevel> levels;

    public LevelsData(string fname) {
        try {
            string json = File.ReadAllText(fname);
            levels = JsonUtility.FromJson<List<QuLevel>>(json);
        } catch (Exception ex) {
            LogHelper.Error(this, "Couldn't load levels from " + fname + ": " + ex);
        }
    }

    public QuLevel this[int n] {
        get {
            return levels[n];
        }
    }
}
