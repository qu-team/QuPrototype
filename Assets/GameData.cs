using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[Serializable]
struct PlayerData {
    // number of saved Qu
    int quSaved;
    List<LevelData> levels;
    bool[] cardsUnlocked;
    // level reached so far
    int curLevelUnlocked;
    
    int NUnlockedCards {
        get { 
            return cardsUnlocked.Count(e => e);
        }
    }
}

[Serializable]
struct LevelData {
    long maxScore;
    int quSaved;
}

static class GameData {
    const string SAVE_FILE = "gamedata.sav";

    public static PlayerData data;

    // Tries to load data from the save file and returns whether the data was
    // loaded or not. The loaded data is available in GameData.data.
    public static bool Load() {
        if (!File.Exists(SAVE_FILE)) return false;

        try {
            var dataHdl = new LocalDataHandler(Application.persistentDataPath);
            string json = dataHdl.LoadCompressed(SAVE_FILE);
            data = JsonUtility.FromJson<PlayerData>(json);
        } catch (Exception e) {
            LogHelper.Error(typeof(GameData), "Couldn't load save data from " + SAVE_FILE + ": " + e.StackTrace);
            return false;
        }

        return true;
    }

    // Serializes the player data into SAVE_FILE.
    public static void Save() {
        try {
            var dataHdl = new LocalDataHandler(Application.persistentDataPath);
            dataHdl.SaveCompressed(JsonUtility.ToJson(data), SAVE_FILE);
        } catch (Exception e) {
            LogHelper.Error(typeof(GameData), "Couldn't save data to " + SAVE_FILE + ": " + e.StackTrace);
        }
    }
}
