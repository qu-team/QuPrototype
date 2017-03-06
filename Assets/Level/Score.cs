using UnityEngine;

public struct Score {

    public uint basePoints;
    public float difficultyMultiplier;

    int currentDifficulty;
    int combo;
    float snapshot;

    void Start() {
        snapshot = Time.fixedTime;
        currentDifficulty = 1;
        combo = 0;
    }

    public int Value {
        get {
            var difficultyScore = difficultyMultiplier * currentDifficulty;
            var delay = Time.fixedTime - snapshot;
            var score = (basePoints + difficultyScore) / (1f + delay);
            return (int)Mathf.Max(5, score);
        }
    }

    public int Difficulty { get { return currentDifficulty; } }

    public int Combo { get { return combo; } }

    public void Succeeded() {
        currentDifficulty++;
        combo++;
        snapshot = Time.fixedTime;
    }

    public void Failed() {
        currentDifficulty = Mathf.Max(currentDifficulty - 2, 1);
        combo = 0;
        snapshot = Time.fixedTime;
    }
}
