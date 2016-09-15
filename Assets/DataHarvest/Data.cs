[System.Serializable]
public struct DataBundle {
	public bool answerCorrect;
	public float responseTime;
	public float timeSinceStart;
	public float bladeQuDistance;
	public float bladeQuBorderDistance;
	public DataColor color;
	public DataColor backgroundColor;
	public float borderRadius;
    public uint numberOfBlades;
}

[System.Serializable]
public struct DataColor {
	public float r;
	public float g;
	public float b;
}
