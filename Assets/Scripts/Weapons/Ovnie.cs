using UnityEngine;

[RequireComponent(typeof(Statistics))]
public class Ovnie : Weapon
{
    [Header("References")]
    [SerializeField] private WeaponMovement movement;
    [SerializeField] private PlayerShooting shooting;

    private void Start()
    {
        if (statistics == null)
            statistics = GetComponent<Statistics>();

        statistics.Populate();
        statistics.SetStatistic(StatisticsType.Health, statistics.GetMaximum(StatisticsType.Health));

        if (WeaponLevelSystem == null)
            WeaponLevelSystem = GetComponent<LevelSystem>();
        if (movement == null) movement = GetComponent<WeaponMovement>();
        if (shooting == null) shooting = GetComponent<PlayerShooting>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<Entity>(out var enemy))
        {
            TakeDamage(enemy.statistics.GetStatistic(StatisticsType.Attack));
            Destroy(other.gameObject);
        }
    }

    protected override void OnDeath()
    {
        GameManager.instance._mPlayer.Weapon = GameManager.instance.allWeapons[1];
        Destroy(gameObject);
    }



}
