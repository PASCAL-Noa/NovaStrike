using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Références")]
    public Player player;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI xpText;

    public TextMeshProUGUI IdText;
    public TextMeshProUGUI AccLevel;
    public TextMeshProUGUI WeaponLevel;
    public TextMeshProUGUI Stage;

    public Image healthBar;
    public Image attBar;
    public Image fireRateBar;
    public Image speedBar;
    public Image xpBar;

    

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player == null || player.Weapon == null) return;

        // --- HEALTH ---
        float currentHealth = player.Weapon.statistics.GetStatistic(StatisticsType.Health);
        float maxHealth = player.Weapon.statistics.GetMaximum(StatisticsType.Health);
        healthBar.fillAmount = currentHealth / maxHealth;

        if (healthText != null) healthText.text = $"{currentHealth}/{maxHealth}";

        // --- ATTACK ---
        float currentAttack = player.Weapon.statistics.GetStatistic(StatisticsType.Attack);
        float maxAttack = player.Weapon.statistics.GetMaximum(StatisticsType.Attack);
        attBar.fillAmount = currentAttack / maxAttack;
        if (attackText != null) attackText.text = currentAttack.ToString("0");

        // --- FIRE RATE ---
        float currentFireRate = player.Weapon.statistics.GetStatistic(StatisticsType.FireRate);
        float maxFireRate = player.Weapon.statistics.GetMaximum(StatisticsType.FireRate);
        fireRateBar.fillAmount = currentFireRate / maxFireRate;
        if (fireRateText != null) fireRateText.text = currentFireRate.ToString("0.0");
        
        // --- SPEED ---
        float currentSpeed = player.Weapon.statistics.GetStatistic(StatisticsType.Speed);
        float maxSpeed = player.Weapon.statistics.GetMaximum(StatisticsType.Speed);
        speedBar.fillAmount = currentSpeed / maxSpeed;
        if (speedText != null) speedText.text = currentSpeed.ToString("0.0");
        
        // --- XP ---
         float currentXP = player.Weapon.WeaponLevelSystem.CurrentXp;
        // float xpToNext = player.Weapon.WeaponLevelSystem.XpToNextLevel;
        xpBar.fillAmount = player.Weapon.WeaponLevelSystem.XpProgression;
        xpText.text = player.Weapon.WeaponLevelSystem.CurrentXp.ToString("0.0") + " / " + GameManager.instance._mPlayer.Weapon.WeaponLevelSystem.XpToNextLevel.ToString("0.0");

        IdText.text = GameManager.instance._mPlayer.ID;
        AccLevel.text = "Player Level " + GameManager.instance._mPlayer.LevelSystemPlayer.CurrentLevel;
        WeaponLevel.text = "Weapon Level " + GameManager.instance._mPlayer.Weapon.WeaponLevelSystem.CurrentLevel;
        Stage.text = "Stage " + GameManager.instance.stageManager.CurrentStage;
    }
}
