using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public delegate void TimeEvent();

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public event TimeEvent onNewDay;

    [SerializeField] float secondsInADay;

    [HideInInspector] public float totalTime;
    public int year;
    public int month;
    public int day;
    public float hour;
    public int minute;

    [HideInInspector] public int prevYear;
    [HideInInspector] public int prevDay;


    public void Awake()
    {
        instance = this;
        totalTime = 360;

        year = Mathf.FloorToInt(totalTime / 360);
        day = Mathf.FloorToInt(totalTime) - 359;
        prevYear = year;
        prevDay = day;
    }


    void Update()
    {
        totalTime += Time.deltaTime / secondsInADay;

        year = Mathf.FloorToInt(totalTime / 360);
        month = Mathf.FloorToInt(totalTime / 30 % 12);
        day = Mathf.FloorToInt(totalTime) - 359;
        hour = Mathf.FloorToInt(totalTime * 24 % 24);
        minute = Mathf.FloorToInt(totalTime * 1440 % 60);

        if (year != prevYear) NewYear();
        if (day != prevDay) NewDay();

        PlayerStats.stats.totalPlaytime += Time.deltaTime;
    }

    private void NewYear()
    {
        prevYear = year;
    }

    public static int YearFromTime(float time)
    {
        return Mathf.FloorToInt(time / 360);
    }

    public static int MonthFromTime(float time)
    {
        return Mathf.FloorToInt(time / 30 % 12);
    }

    public static int DayFromTime(float time)
    {
        return Mathf.FloorToInt(time) - 359;
    }

    public static int HourFromTime(float time)
    {
        return Mathf.FloorToInt(time * 24 % 24);
    }

    public static int MinuteFromTime(float time)
    {
        return Mathf.FloorToInt(time * 1440 % 60);
    }

    private void NewDay()
    {
        prevDay = day;
        EconomyManager.instance.DailyUpdate();
        if(onNewDay != null)
        {
            onNewDay();
        }
    }


    public float GetTotalTime()
    {
        return totalTime;
    }


    public int GetShrimpAge(float birthTime)
    {
        return Mathf.FloorToInt(totalTime - birthTime);
    }


    public float CalculateBirthTimeFromAge(float age)
    {
        return totalTime - age;
    }
}
