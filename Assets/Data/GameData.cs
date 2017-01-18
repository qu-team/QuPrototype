using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[Serializable]
public struct PlayerData {
    public List<LevelData> levels;
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
        return cardsUnlocked.Length > idx && cardsUnlocked[idx];
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

[Serializable]
public struct LevelData {
    public long maxScore;
    public uint quSaved;

    public LevelData Overwrite(LevelData data) {
        return new LevelData {
            maxScore = Math.Max(data.maxScore, maxScore),
            quSaved = quSaved + data.quSaved
        };
    }
}

public static class GameData {
    const string SAVE_FILE = "gamedata.json";

    public static PlayerData data;

    // Tries to load data from the save file and returns whether the data was
    // loaded or not. The loaded data is available in GameData.data.
    public static bool Load() {
        string fname = Application.persistentDataPath + Path.DirectorySeparatorChar + SAVE_FILE + ".gz";
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
}
