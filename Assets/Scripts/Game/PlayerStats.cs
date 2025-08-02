using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
    public static Stats stats = new Stats();
}

[System.Serializable]
public struct Stats
{
    public int timesGameLoaded;
    public float totalPlaytime;

    public int shelfCount;
    public int tankCount;
    public int smallTankCount;
    public int largeTankCount;

    public float totalMoney;
    public float moneyMade;
    public float moneySpent;

    public float reputationGained;
    public float reputationLost;

    public int shrimpSold;
    public int shrimpSoldThroughOpenTank;
    public int requestsCompleted;

    public int totalShrimp;
    public int shrimpBred;
    public int shrimpBorn;
    public int shrimpBought;
    public int shrimpMoved;
    public int mostShrimpInOneTank;

    public int tanksNamed;
    public int shrimpNamed;

    public int illnessesGained;
    public int illnessesCured;

    public int shrimpDeaths;
    public int shrimpDeathsThroughAge;
    public int shrimpDeathsThroughOverpopulation;
    public int shrimpDeathsThroughHunger; //

    public int timesShrimpShopAppOpened;
    public int timesMailAppOpened;
    public int timesSellingAppOpened;
    public int timesItemShopAppOpened;
    public int timesInventoryAppOpened;
    public int timesSettingsAppOpened;
    public int timesVetOpened;

    public float timeSpentMoving;
    public float timeSpentFocusingTank;
    public float timeSpentFocusingShrimp;

    public bool radioPlaying;
    public int currentSongPlaying;
    public int timesRadioToggled;

    public int timesBoughtFood;
}