public class QuLevel {
	public readonly int number;
	public readonly string name;
	public readonly float aperture;
	public readonly int blades;
	public readonly float difficultyExp;
	public readonly int quToNextLevel;

	public QuLevel(int number, string name, float aperture, 
			int blades, float difficultyExp, int quToNextLevel)
	{
		this.number = number;
		this.name = name;
		this.aperture = aperture;
		this.blades = blades;
		this.difficultyExp = difficultyExp;
		this.quToNextLevel = quToNextLevel;
	}
}
