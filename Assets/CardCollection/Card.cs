using UnityEngine;

public class Card{

	public static Card[] Collection  = {
		new Card(1, L10N.Label.CARD_1_TITLE.ToString(), L10N.Label.CARD_1_DESCRIPTION.ToString(), L10N.Label.CARD_1_TASK.ToString()),
        new Card(2, L10N.Label.CARD_2_TITLE.ToString(), L10N.Label.CARD_2_DESCRIPTION.ToString(), L10N.Label.CARD_2_TASK.ToString())
	};

	public readonly string Name;
	public readonly string Description;
	public readonly int CardNumber;
	public readonly string UnlockCondition;

	private Texture cardImage;
	public Texture CardImage {
		get {
			if (cardImage != null) return cardImage;
			return cardImage = Resources.Load("Cards" +
					System.IO.Path.DirectorySeparatorChar
					+ CardNumber) as Texture;
		}
	}

	public Card(int num, string n, string desc, string unlock) {
		Name = n;
		Description = desc;
		CardNumber = num;
		UnlockCondition = unlock;
	}
}
