using System.Collections.Generic;
using System.Linq;

internal static class UnlockConditions {
    public delegate bool UnlockCondition();

    public static bool IsUnlockConditionTrue(uint n) {
        if (n >= conditions.Count) {
            LogHelper.Warn("UnlockConditions",
                "Called IsUnlockConditionTrue(" + n + ") but conditions are "
                + conditions.Count + "!");
            return false;
        }
        return conditions[(int)n]();
    }

    static List<UnlockCondition> conditions = new List<UnlockCondition> {
        Cond1,
        Cond2,
        Cond3,
        Cond4,
        Cond5,
        Cond6,
        Cond7,
        Cond8,
        Cond9,
        Cond10,
        Cond11,
        Cond12,
        Cond13,
        Cond14,
        Cond15,
        Cond16,
        Cond17,
        Cond18,
    };

    static int EveryLevel { get { return GameData.data.levels.Count; } }

    static bool Cond1() {
        return LevelComplete(0);
    }

    static bool Cond2() {
        return LevelComplete(4);
    }

    static bool Cond3() {
        return ScoreMoreThan(300) >= 5;
    }

    static bool Cond4() {
        return HaveStars(2) >= EveryLevel;
    }

    static bool Cond5() {
        return QuSaved(80) >= 1;
    }

    static bool Cond6() {
        return HaveStars(3) >= EveryLevel;
    }

    static bool Cond7() {
        return LevelComplete(8);
    }

    static bool Cond8() {
        return HaveStarsTotal(10);
    }

    static bool Cond9() {
        foreach (var lv in GameData.data.levels) {
            if (lv.maxCombo >= 15)
                return true;
        }
        return false;
    }

    static bool Cond10() {
        return GameData.data.levels.Aggregate(0L, (acc, lv) => lv.maxScore + acc) >= 2000;
    }

    static bool Cond11() {
        return LevelComplete(4);
    }

    static bool Cond12() {
        return HaveStars(3) >= 5;
    }

    static bool Cond13() {
        return LevelComplete((uint)(EveryLevel - 1));
    }

    static bool Cond14() {
        return false; // TODO: watch all cutscenes in a row
    }

    static bool Cond15() {
        return QuSaved(40) >= EveryLevel;
    }

    static bool Cond16() {
        return GameManager.Instance.TotalTimePlayed >= 3 * 60 * 60;
    }

    static bool Cond17() {
        return GameData.data.QuSaved >= 100;
    }

    static bool Cond18() {
        return ScoreMoreThan(500) >= 1;
    }

    static bool LevelComplete(uint n) {
        var lvs = GameData.data.levels;
        return lvs.Count > n && lvs[(int)n].quSaved >= GameManager.Instance.Levels[(int)n].quToNextLevel;
    }

    static int ScoreMoreThan(int threshold) {
        int lvsAchieved = 0;
        foreach (var lv in GameData.data.levels) {
            if (lv.maxScore >= threshold)
                ++lvsAchieved;
        }
        return lvsAchieved;
    }

    static int QuSaved(int threshold) {
        int lvsAchieved = 0;
        foreach (var lv in GameData.data.levels) {
            if (lv.quSaved >= threshold)
                ++lvsAchieved;
        }
        return lvsAchieved;
    }

    static int HaveStars(int threshold) {
        int lvsAchieved = 0;
        for (int i = 0; i < GameData.data.levels.Count; ++i) {
            if (NStars(i) >= threshold)
                ++lvsAchieved;
        }
        return lvsAchieved;
    }

    static bool HaveStarsTotal(int threshold) {
        int stars = 0;
        for (int i = 0; i < GameData.data.levels.Count; ++i) {
            stars += NStars(i);
            if (stars >= threshold)
                return true;
        }
        return false;
    }

    public static int NStars(int i) {
            var lv = GameData.data.levels[i];
            var sp = GameManager.Instance.Levels[i].stars;
            return lv.maxScore >= sp.third ? 3 :
                   lv.maxScore >= sp.second ? 2 :
                   lv.maxScore >= sp.first ? 1 :
                   0;
    }
}
