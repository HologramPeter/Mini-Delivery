using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSummary
{
    int targetCoin;
    int targetDeath;
    int targetItemCount;

    public int MaskCount = 0;
    public int PillCount = 0;
    public int SprayCount = 0;
    public int DeathCount = 0;
    public int CoinEarned = 0;

    public LevelSummary(int targetCoin, int targetDeath, int targetItemCount) {
        this.targetCoin = targetCoin;
        this.targetDeath = targetDeath;
        this.targetItemCount = targetItemCount;
    }

    public int GetStars()
    {
        float coinMult = (float) CoinEarned / targetCoin;
        if (coinMult < 1) return -1;
        
        int stars = 0;
        if (coinMult > 2f) stars += 1; //hardwork bonus
        if (DeathCount <= targetDeath) stars += 1; //survival bonus

        if (Mathf.Max(MaskCount, SprayCount, PillCount) <= targetItemCount) stars += 1; //item bonus

        return stars;
    }

    public (bool, string)[] GetSummary()
    {
        (bool, string)[] summary = new (bool, string)[]{
            (false, "Level "),
            (false, "Hardwork. Earn twice the required amount. "),
            (false, "Surviorship. Die at most once. "),
            (false, "Pandemic Expert. Use each items at most once. ")
        };

        float coinMult = (float)CoinEarned / targetCoin;
        if (coinMult > 1)
        {
            summary[0].Item1 = true;
            if (coinMult > 2f) summary[1].Item1 = true;
            if (DeathCount <= targetDeath) summary[2].Item1 = true;
            if (Mathf.Max(MaskCount, SprayCount, PillCount) <= targetItemCount) summary[3].Item1 = true;
            summary[0].Item2 += "completed!";
        }
        else
        {
            summary[0].Item2 += "failed!";
        }

        summary[1].Item2 += CoinEarned.ToString() + "/"+ (targetCoin*2).ToString();

        summary[2].Item2 += DeathCount.ToString() + "/" + targetDeath.ToString();

        summary[3].Item2 +=
            "Spray:" + SprayCount.ToString() + "/" + targetItemCount.ToString() + " "
            + "Pill:" + PillCount.ToString() + "/" + targetItemCount.ToString() + " "
            + "Mask:" + MaskCount.ToString() + "/" + targetItemCount.ToString();

        return summary;
    }

    public int GameCoins()
    {
        int stars = GetStars();
        if (stars < 0) return 0;
        return 10 + 10*stars;
    }
}
