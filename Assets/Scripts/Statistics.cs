using System.Collections.Generic;
using UnityEngine;

public enum StatisticsType
{
    Health,
    Attack,
    Speed,
    FireRate,
    Size
}

[System.Serializable]
public class Statistic
{
    public StatisticsType Type;
    public float Current;
    public float Maximum;
}


public class Statistics : MonoBehaviour
{

    [SerializeField] private List<Statistic> _statistics = new();
    public  List<Statistic> StatisticsList => _statistics;

    public void Increment(StatisticsType statistics, float value)
    {
        if (value < 0) return;
        Populate();
        if (GetStatistic(statistics) + value > GetMaximum(statistics))
        {
            StatisticsList[(int)statistics].Current = GetMaximum(statistics);
            return;
        }
        StatisticsList[(int)statistics].Current += value;
    }
    
    public void Decrement(StatisticsType statistics, float value)
    {
        if (value < 0) return;
        Populate();
        if (GetStatistic(statistics) - value < 0)
        {
            StatisticsList[(int)statistics].Current = 0;
            return;
        }
        StatisticsList[(int)statistics].Current -= value;
    }
    
    public void IncrementLevel(StatisticsType statistics, float value)
    {
        if (value < 0) return; 
        Populate();
        StatisticsList[(int)statistics].Current += value;
        StatisticsList[(int)statistics].Maximum += value;
    }
    
    public void DecrementLevel(StatisticsType statistics, float value)
    {
        if (value < 0) return;
        Populate();
        if (GetStatistic(statistics) - value < 0)
        {
            StatisticsList[(int)statistics].Current = 0;
            StatisticsList[(int)statistics].Maximum = 0;
            return;
        }
        StatisticsList[(int)statistics].Current -= value;
        StatisticsList[(int)statistics].Maximum -= value;
    }

    public void SetStatistic(StatisticsType statistics, float value)
    {
        Populate();
        StatisticsList[(int)statistics].Current = value;
        StatisticsList[(int)statistics].Maximum = value;
    }
    
    public float GetStatistic(StatisticsType statistics)
    {
        Populate();
        return StatisticsList[(int)statistics].Current;
    }
    
    public float GetMaximum(StatisticsType statistics)
    {
        Populate();
        return StatisticsList[(int)statistics].Maximum;
    }
    
    public float GetStatisticScaled(StatisticsType statistics, int level)
    {
        Populate();
        return StatisticsList[(int)statistics].Current + 3 * level;
    }

    public void Populate()
    {
        if (_statistics.Count == (int)StatisticsType.Size) return;
        _statistics.Clear();
        for (int i = 0; i < (int)StatisticsType.Size; i++)
        {
            _statistics.Add(new Statistic { Type = (StatisticsType)i, Current = 0, Maximum = 0 });
        }
    }
}
