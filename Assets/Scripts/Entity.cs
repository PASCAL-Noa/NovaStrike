using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public float CurrentHp => statistics.GetStatistic(StatisticsType.Health);
    public float MaxHp => statistics.GetMaximum(StatisticsType.Health);
    public event Action<Entity> OnEntityDeath; 

    [Header("Statistics")]
    [SerializeField]
    public Statistics statistics;

    protected virtual void Awake()
    {
        if (statistics == null)
            statistics = GetComponent<Statistics>();
        
        statistics.Populate();
        statistics.SetStatistic(StatisticsType.Health, statistics.GetMaximum(StatisticsType.Health));
    }
    public virtual void Die()
    {
        OnEntityDeath?.Invoke(this);
        OnDeath();
    }

    public virtual void TakeDamage(float amount)
    {
        statistics.Decrement(StatisticsType.Health, amount);
        if (statistics.GetStatistic(StatisticsType.Health) <= 0)
            Die();
    }


    protected abstract void OnDeath();
}