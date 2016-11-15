class Card{

	public static Card[] Collection  = {
		//FIXME, will need translation in various languages
		new Card(1,"Light","Colors are a result of blabla light", "Complete the campaign"),
		new Card(2,"Animals","Dogs see everything grey, ugly life", "Get three stars in level one")	
	};

	public readonly string Name;
	public readonly string Description;
	public readonly int CardNumber;
	public readonly string UnlockCondition;

	public Card(int num, string n, string desc, string unlock){
		Name = n;
		Description = desc;
		CardNumber = num;
		UnlockCondition = unlock;
	}
}
