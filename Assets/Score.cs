public struct Score {

    public uint basePoints;
    public float difficultyMultiplier;
    public float comboMultiplier;

    uint currentDifficulty;
    uint combo;

    public uint Value {
        get {
            return (uint)(basePoints + difficultyMultiplier * currentDifficulty + comboMultiplier * combo);
        }
    }

    public uint Difficulty { get { return currentDifficulty; } }

    public uint Combo { get { return combo; } }

    public void Succeeded() {
        currentDifficulty++;
        combo++;
    }

    public void Failed() {
        currentDifficulty /= 2;
        combo = 0;
    }
}
