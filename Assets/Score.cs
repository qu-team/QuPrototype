using UnityEngine;

public struct Score {

    public uint basePoints;
    public float difficultyMultiplier;

    int currentDifficulty;
    float snapshot;

    void Start() {
        snapshot = Time.fixedTime;
        currentDifficulty = 1;
    }

    public int Value {
        get {
            return (int)((basePoints + difficultyMultiplier * currentDifficulty * currentDifficulty) / (Time.fixedTime - snapshot));
        }
    }

    public int Difficulty { get { return currentDifficulty; } }

    public void Succeeded() {
        currentDifficulty++;
        snapshot = Time.fixedTime;
    }

    public void Failed() {
        currentDifficulty = Mathf.Max(currentDifficulty / 2, 1);
        snapshot = Time.fixedTime;
    }
}
