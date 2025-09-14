using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShrimpStats
{
    public string name;
    public bool sex;  // True = male, False = female
    public float birthTime;  // The time at the point of the shrimp's birth
    public bool bornInStore; // If true, born in the player's store, if false bought from shop
    public float hunger;  // Value from 0-100
    public float illnessLevel;  // Value from 0-100
    public int temperament;  // Value from 0-100
    public int saleSlotIndex;
    public float assignedValue;

    public int salineLevel; // Value from 0-100, starting at 50
    public int immunity; // Value from 0-100
    public int metabolism; // Value from 0-100
    public int filtration; // Value from 0-100
    public int temperaturePreference; // Value from 0-100, starting at 50
    public int ammoniaPreference; // Value from 0 - 100, starting 50
    public int PhPreference; // Value from 1 - 14, starting at 7
    
          
    public int geneticSize;
    public int actualSize;

    public Trait primaryColour;
    public Trait secondaryColour;
    public Trait body;
    public Trait head;
    public Trait eyes;
    public Trait pattern;
    public Trait tail;
    public Trait tailFan;
    public Trait legs;

    public int fightHistory;
    public int breedingHistory;
    public int illnessHistory;
    public int moltHistory;
    public bool canBreed;

    public bool[] illness;
    public float[] symptoms;

    public bool CompareTraits(ShrimpStats other)
    {
        if(primaryColour == other.primaryColour
            && secondaryColour == other.secondaryColour
            && body == other.body
            && head == other.head
            && eyes == other.eyes
            && pattern == other.pattern
            && tail == other.tail
            && tailFan == other.tailFan
            && legs == other.legs)
        {
            return true;
        }
        return false;
    }
}
