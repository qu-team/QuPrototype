public class QuLevel{
	public readonly int Number;
	public readonly string Name;
	public readonly float Aperture;
	public readonly int Blades;
	public readonly float DifficultyExp;
	public readonly int QuToNextLevel;

	public QuLevel(int Number, string Name, float Aperture, 
			int Blades, float DifficultyExp, int QuToNextLevel){
		this.Number = Number;
		this.Name = Name;
		this.Aperture = Aperture;
		this.Blades = Blades;
		this.DifficultyExp = DifficultyExp;
		this.QuToNextLevel = QuToNextLevel;
	}

	public static QuLevel[] Levels = {
		new QuLevel(0,"The Beginning",0.5f,3,1f,10),
	};
}
