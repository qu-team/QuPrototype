using UnityEngine;
using System;
using System.Collections.Generic;

public class Harvester {

    private List<DataBundle> storedData;

    public Harvester() {
        storedData = new List<DataBundle>();
    }

    // Saves the data from a single tap-to-tap session and stores it into memory.
	public void SaveSingleSessionData(Level level, bool succeeded) {
		storedData.Add(CreateDataBundle(level, succeeded));
    }

    public void SendStoredData() {
        // TODO: check network and do asynchronously
        SaveLocally(storedData);
        storedData.Clear();
    }

    private DataBundle CreateDataBundle(Level level, bool succeeded) {
        return new DataBundle {
            answerCorrect = succeeded,
            responseTime = Time.time - level.PartialStartTime,
            timeSinceStart = level.timer.TimeSinceStart,
            bladeQuDistance = CalculateDistance(level.shutter.opening, 1f), // FIXME: use Qu's actual radius
            bladeQuBorderDistance = CalculateDistance(level.shutter.opening,
                                                      level.shutter.internalCircleRadius),
            color = new DataColor {
                r = level.qu.Color.r,
                g = level.qu.Color.g,
                b = level.qu.Color.b,
            },
            backgroundColor = new DataColor {
                r = level.shutter.BackgroundColor.r,
                g = level.shutter.BackgroundColor.g,
                b = level.shutter.BackgroundColor.b
            },
            borderRadius = level.shutter.internalCircleRadius
        };
	}

    private void SaveLocally(IEnumerable<DataBundle> dataList) {
        string fname = GenerateFileName();
        Debug.Log("in file " + fname);
        foreach (DataBundle data in dataList) {
            string jsonData = JsonUtility.ToJson(data);
            Debug.Log(jsonData);
        }
    }

    // Converts the shutter's `opening` [0-1] to the distance (in Unity's units) from blades to `radius`
    private float CalculateDistance(float opening, float borderRadius) {
        return opening; // TODO 
    }

    private string GenerateFileName() {
        var now = DateTime.Now;
        return string.Format("{0}-{1}-{2}T{3}:{4}:{5}_{6}", 
            now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
    }
}
