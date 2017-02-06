using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

/*
 * This is the mapping of the json found in
 * Application.persistentDataPath/gamedata.json.gz
 */
[Serializable]
public struct PlayerData {
    public List<LevelSaveData> levels;
#pragma warning disable 0649
    public bool[] cardsUnlocked;
#pragma warning restore
    // level reached so far
    public uint curLevelUnlocked;

    public uint NUnlockedCards {
        get {
            return (uint)cardsUnlocked.Count(e => e);
        }
    }

    public bool IsCardUnlocked(int idx) {
        return cardsUnlocked != null && cardsUnlocked.Length > idx && cardsUnlocked[idx];
    }

    public uint QuSaved {
        get {
            uint quSaved = 0;
            foreach (var level in levels)
                quSaved += level.quSaved;
            return quSaved;
        }
    }
}

/*
 * Keeps track of the "map progress" of the player.
 * This information is embedded in PlayerData and serialized
 * in Application.persistentDataPath/gamedata.json.gz
 */
[Serializable]
public struct LevelSaveData {
    public long maxScore;
    public uint quSaved;
    public uint maxCombo;

    public LevelSaveData Overwrite(LevelSaveData data) {
        return new LevelSaveData {
            maxScore = Math.Max(data.maxScore, maxScore),
            quSaved = quSaved + data.quSaved,
            maxCombo = Math.Max(data.maxCombo, maxCombo)
        };
    }
}

/*
 * This class provides a convenient interface to gamedata.json.
 * One typically loads the data with Load(), changes entries in `data`
 * and finally calls Save() to serialize the changes.
 */
public static class GameData {
    const string SAVE_FILE = "gamedata.json";

    public static PlayerData data;

    // Tries to load data from the save file and returns whether the data was
    // loaded or not. The loaded data is available in GameData.data.
    public static bool Load() {
        string fname = Application.persistentDataPath + "/" + SAVE_FILE + ".gz";
        if (!File.Exists(fname))
            return false;

        try {
            var dataHdl = new LocalDataHandler(Application.persistentDataPath);
            string json = dataHdl.LoadCompressed(fname);
            data = JsonUtility.FromJson<PlayerData>(json);
        } catch (Exception e) {
            LogHelper.Error("GameData", "Couldn't load save data from " + fname + ": "
                              + e.ToString() + e.StackTrace);
            return false;
        }
        LogHelper.Ok("GameData", "Loaded save data from " + fname);
        return true;
    }

    // Serializes the player data into SAVE_FILE.
    public static void Save() {
        try {
            var dataHdl = new LocalDataHandler(Application.persistentDataPath);
            dataHdl.SaveCompressed(JsonUtility.ToJson(data), SAVE_FILE);
        } catch (Exception e) {
            LogHelper.Error("GameData", "Couldn't save data to " + SAVE_FILE + ": "
                              + e.ToString() + e.StackTrace);
            return;
        }
        LogHelper.Ok("GameData", "Saved data to " + SAVE_FILE);
    }

    public static void EnsureLevelsAreInitialized() {
        int levels = GameManager.Instance.Levels.Count;
        if (data.levels == null) { data.levels = new List<LevelSaveData>(); }
        for (int i = data.levels.Count; i < levels; ++i) {
            data.levels.Add(new LevelSaveData());
        }
    }
}
