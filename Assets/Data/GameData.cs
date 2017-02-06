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
    // This data is guaranteed to have the following properties:
    // 1. data.levels.Count >= GameManager.Instance.Levels.Count (i.e. if a level exists, we have a save entry for it)
    // 2. data.cardsUnlocked.Length >= Card.Collection.Length (i.e. if there are N cards, we know whether a card j is
    //                                                        unlocked or not for 0 <= j < N.)
    // 3. data.curLevelUnlocked < GameManager.Instance.Levels.Count (i.e. curlevelunlocked is a valid level index)
    public static bool Load() {
        string fname = Application.persistentDataPath + "/" + SAVE_FILE + ".gz";
        if (!File.Exists(fname)) {
            // Create a new PlayerData
            InitPlayerData();
            return false;
        }

        try {
            var dataHdl = new LocalDataHandler(Application.persistentDataPath);
            string json = dataHdl.LoadCompressed(fname);
            LogHelper.Debug("GameData", "json = " + json);
            data = JsonUtility.FromJson<PlayerData>(json);
            Validate();
        } catch (Exception e) {
            LogHelper.Error("GameData", "Couldn't load save data from " + fname + ": "
                              + e.ToString() + e.StackTrace);
            return false;
        }

        LogHelper.Ok("GameData", "Loaded save data from " + fname);
        return true;
    }

    // Serializes the player data into SAVE_FILE. This method does NOT validate the data before serializing it.
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

    static void InitPlayerData() {
        data = new PlayerData {
            levels = new List<LevelSaveData>(),
            cardsUnlocked = new bool[Card.Collection.Length]
        };
        int levels = GameManager.Instance.Levels.Count;
        for (int i = data.levels.Count; i < levels; ++i) {
            data.levels.Add(new LevelSaveData());
        }
    }

    static void Validate() {
        // Ensure level save data are not less than actual levels
        int diff = data.levels.Count - GameManager.Instance.Levels.Count;
        if (diff > 0) {
            LogHelper.Warn("GameData", "saved level data have less entries than the correct number of levels: repairing");
            for (int i = 0; i < diff; ++i)
                data.levels.Add(new LevelSaveData());
        }
        // Ensure cards data are not less than actual cards
        diff = data.cardsUnlocked.Length - Card.Collection.Length;
        if (diff > 0) {
            LogHelper.Warn("GameData", "unlocked cards data have less entries than the correct number of levels: repairing");
            bool[] tmpCardsUnlocked = new bool[Card.Collection.Length];
            Array.Copy(data.cardsUnlocked, tmpCardsUnlocked, data.cardsUnlocked.Length);
            data.cardsUnlocked = tmpCardsUnlocked;
        }
        // Ensure cur level unlocked is not greater than maximum level index
        data.curLevelUnlocked = (uint)Math.Min(data.curLevelUnlocked, GameManager.Instance.Levels.Count - 1);
    }
}
