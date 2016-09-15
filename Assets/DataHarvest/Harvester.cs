using UnityEngine;
using System;
using System.Collections.Generic;

// Harvester is the object acting as an interface to the harvester thread.
// All its methods are synchronous and execute on the main thread, but it
// communicates with the harvester thread via a queue.
public class Harvester {

    private List<DataBundle> storedData;
    private HarvesterDaemon daemon;

    public Harvester(HarvesterDaemon daemon) {
        storedData = new List<DataBundle>();
        this.daemon = daemon;
    }

    // Saves the data from a single tap-to-tap session and stores it into memory.
    public void SaveSingleSessionData(Level level, bool succeeded) {
        storedData.Add(CreateDataBundle(level, succeeded));
    }

    public void SendStoredData() {
        daemon.dataPipe.Enqueue(new List<DataBundle>(storedData));
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
            borderRadius = level.shutter.internalCircleRadius,
            numberOfBlades = level.shutter.bladesNumber
        };
    }

    // Converts the shutter's `opening` [0-1] to the distance (in Unity's units) from blades to `radius`
    private float CalculateDistance(float opening, float borderRadius) {
        return opening; // TODO 
    }
}
