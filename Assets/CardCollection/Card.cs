using UnityEngine;

public class Card{

    public static Card[] Collection  = {
        new Card(1, L10N.Label.CARD_1_TITLE.ToString(),
            L10N.Label.CARD_1_DESCRIPTION.ToString(),
            L10N.Label.CARD_1_TASK.ToString()),
        new Card(2, L10N.Label.CARD_2_TITLE.ToString(),
            L10N.Label.CARD_2_DESCRIPTION.ToString(),
            L10N.Label.CARD_2_TASK.ToString()),
        new Card(3, L10N.Label.CARD_3_TITLE.ToString(),
            L10N.Label.CARD_3_DESCRIPTION.ToString(),
            L10N.Label.CARD_3_TASK.ToString()),
        new Card(4, L10N.Label.CARD_4_TITLE.ToString(),
            L10N.Label.CARD_4_DESCRIPTION.ToString(),
            L10N.Label.CARD_4_TASK.ToString()),
        new Card(5, L10N.Label.CARD_5_TITLE.ToString(),
            L10N.Label.CARD_5_DESCRIPTION.ToString(),
            L10N.Label.CARD_5_TASK.ToString()),
        new Card(6, L10N.Label.CARD_6_TITLE.ToString(),
            L10N.Label.CARD_6_DESCRIPTION.ToString(),
            L10N.Label.CARD_6_TASK.ToString()),
        new Card(7, L10N.Label.CARD_7_TITLE.ToString(),
            L10N.Label.CARD_7_DESCRIPTION.ToString(),
            L10N.Label.CARD_7_TASK.ToString()),
        new Card(8, L10N.Label.CARD_8_TITLE.ToString(),
            L10N.Label.CARD_8_DESCRIPTION.ToString(),
            L10N.Label.CARD_8_TASK.ToString()),
        new Card(9, L10N.Label.CARD_9_TITLE.ToString(),
            L10N.Label.CARD_9_DESCRIPTION.ToString(),
            L10N.Label.CARD_9_TASK.ToString()),
        new Card(10, L10N.Label.CARD_10_TITLE.ToString(),
            L10N.Label.CARD_10_DESCRIPTION.ToString(),
            L10N.Label.CARD_10_TASK.ToString()),
        new Card(11, L10N.Label.CARD_11_TITLE.ToString(),
            L10N.Label.CARD_11_DESCRIPTION.ToString(),
            L10N.Label.CARD_11_TASK.ToString()),
        new Card(12, L10N.Label.CARD_12_TITLE.ToString(),
            L10N.Label.CARD_12_DESCRIPTION.ToString(),
            L10N.Label.CARD_12_TASK.ToString()),
        new Card(13, L10N.Label.CARD_13_TITLE.ToString(),
            L10N.Label.CARD_13_DESCRIPTION.ToString(),
            L10N.Label.CARD_13_TASK.ToString()),
        new Card(14, L10N.Label.CARD_14_TITLE.ToString(),
            L10N.Label.CARD_14_DESCRIPTION.ToString(),
            L10N.Label.CARD_14_TASK.ToString()),
        new Card(15, L10N.Label.CARD_15_TITLE.ToString(),
            L10N.Label.CARD_15_DESCRIPTION.ToString(),
            L10N.Label.CARD_15_TASK.ToString()),
        new Card(16, L10N.Label.CARD_16_TITLE.ToString(),
            L10N.Label.CARD_16_DESCRIPTION.ToString(),
            L10N.Label.CARD_16_TASK.ToString()),
        new Card(17, L10N.Label.CARD_17_TITLE.ToString(),
            L10N.Label.CARD_17_DESCRIPTION.ToString(),
            L10N.Label.CARD_17_TASK.ToString()),
        new Card(18, L10N.Label.CARD_18_TITLE.ToString(),
            L10N.Label.CARD_18_DESCRIPTION.ToString(),
            L10N.Label.CARD_18_TASK.ToString()),

	};

    public readonly string Name;
    public readonly string Description;
    public readonly int CardNumber;
    public readonly string UnlockCondition;

    private Texture cardImage;
    public Texture CardImage {
        get {
            if (cardImage != null) return cardImage;
            return cardImage = Resources.Load<Texture>("Cards/" + CardNumber);
        }
    }

    public Card(int num, string n, string desc, string unlock) {
        Name = n;
        Description = desc;
        CardNumber = num;
        UnlockCondition = unlock;
    }
}
