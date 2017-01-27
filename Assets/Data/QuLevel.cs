[System.Serializable]
public struct QuLevel {
	public uint number;
	public string name;
	public DataColor bgColor;
	public float aperture;
	public float innerRadius;
	public uint duration;
	public uint blades;
	public float bladesSpeed;
	public float difficultyExp;
	public uint quToNextLevel;
    public StarsPoints stars;
}

[System.Serializable]
public struct StarsPoints {
    public uint first;
    public uint second;
    public uint third;
}
