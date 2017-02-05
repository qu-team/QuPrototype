[System.Serializable]
public struct QuLevel {
    public uint number;
    public string name;
    public DataColor bgColor;
    public float aperture;
    public float innerRadius;
    public uint duration;
    public float quResistance;
    public uint blades;
    public float bladesSpeed;
    public float difficultyExp;
    public uint quToNextLevel;
    public StarsPoints stars;
    public bool hasCutscene;
    public int cutscene;
    // If > 0, this forces the color generator to be HSL with this saturation
    public float saturation;
    // If > 0, this forces the color generator to be HSL with this brightness
    public float brightness;
}

[System.Serializable]
public struct StarsPoints {
    public uint first;
    public uint second;
    public uint third;
}
