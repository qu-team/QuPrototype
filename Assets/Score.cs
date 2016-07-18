using UnityEngine;

public struct Score {

    public uint basePoints;
    public float difficultyMultiplier;

    uint currentDifficulty;
    float snapshot;

    void Start() {
        snapshot = Time.fixedTime;
    }

    public uint Value {
        get {
            return (uint)((basePoints + difficultyMultiplier * currentDifficulty * currentDifficulty) / (Time.fixedTime - snapshot));
        }
    }

    public uint Difficulty { get { return currentDifficulty; } }

    public void Succeeded() {
        currentDifficulty++;
        snapshot = Time.fixedTime;
    }

    public void Failed() {
        currentDifficulty /= 2;
        snapshot = Time.fixedTime;
    }
}
