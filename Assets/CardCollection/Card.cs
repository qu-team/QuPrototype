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
