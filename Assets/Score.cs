public struct Score {

    public uint basePoints;
    public float difficultyMultiplier;

    uint currentDifficulty;

    public uint Value {
        get {
            return (uint)(basePoints + difficultyMultiplier * currentDifficulty * currentDifficulty);
        }
    }

    public uint Difficulty { get { return currentDifficulty; } }

    public void Succeeded() {
        currentDifficulty++;
    }

    public void Failed() {
        currentDifficulty /= 2;
    }
}
