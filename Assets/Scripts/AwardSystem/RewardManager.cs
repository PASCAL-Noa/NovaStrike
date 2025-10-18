using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RewardManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private LevelSystem levelSystem;

    [Header("Cible (assignez exactement une)")]
    [SerializeField] private Weapon targetWeapon;
    [SerializeField] private Player targetPlayer;

    [Header("Récompenses par niveau (ScriptableObjects)")]
    [SerializeField] private List<RewardSet> levelRewardSets = new();

    private RewardSet currentSet;
    private bool[] unlockedMilestones;

    private void Awake()
    {
        bool hasWeapon = targetWeapon != null;
        bool hasPlayer = targetPlayer != null;

        if (hasWeapon == hasPlayer)
        {
            Debug.LogError("[RewardManager] Configurez exactement UNE cible (targetWeapon OU targetPlayer).");
            enabled = false;
            return;
        }

        if (levelSystem == null)
        {
            Debug.LogError("[RewardManager] LevelSystem manquant. Assignez-en un dans l'inspecteur.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        UpdateRewardSet();
    }

    private void Update()
    {
        if (!enabled || levelSystem == null || currentSet == null)
            return;
        
        int currentLevelIndex = levelSystem.CurrentLevel - 1;
        if (currentLevelIndex >= levelRewardSets.Count) return;
        
        float xpRatio = levelSystem.CurrentXp / levelSystem.XpToNextLevel;

        for (int i = 0; i < currentSet.rewardMilestones.Length; i++)
        {
            if (xpRatio >= currentSet.rewardMilestones[i] && !unlockedMilestones[i])
            {
                UnlockReward(i);
            }
        }
        
        if (currentLevelIndex != levelRewardSets.IndexOf(currentSet))
        {
            UpdateRewardSet();
        }
    }

    private void UpdateRewardSet()
    {
        if (levelRewardSets == null || levelRewardSets.Count == 0)
        {
            currentSet = null;
            unlockedMilestones = null;
            return;
        }

        int currentLevelIndex = levelSystem.CurrentLevel - 1;
        
        if (currentLevelIndex >= levelRewardSets.Count)
        {
            Debug.LogWarning($"[RewardManager] Aucun RewardSet défini pour le niveau {levelSystem.CurrentLevel}. Les récompenses sont désactivées.");
            currentSet = null;
            unlockedMilestones = null;
            return;
        }

        currentSet = levelRewardSets[currentLevelIndex];
        unlockedMilestones = new bool[currentSet.rewardMilestones.Length];

        Debug.Log($"[RewardManager] Chargement des récompenses pour le niveau {currentSet.setName}");
    }

    private void UnlockReward(int index)
    {
        if (currentSet == null || index < 0 || index >= currentSet.rewards.Count)
            return;

        unlockedMilestones[index] = true;

        Reward reward = currentSet.rewards[index];

        if (targetWeapon != null)
            reward.Apply(targetWeapon);
        else if (targetPlayer != null)
            reward.ApplyToPlayer(targetPlayer);

        Debug.Log($"Récompense débloquée : {reward.rewardName} ({reward.rewardType}) valeur {reward.value}");
    }

    public void RefreshState()
    {
        if (levelSystem == null) return;
        UpdateRewardSet();

        if (currentSet == null) return;

        float xpRatio = levelSystem.CurrentXp / Mathf.Max(1f, levelSystem.XpToNextLevel);
        for (int i = 0; i < currentSet.rewardMilestones.Length; i++)
        {
            if (xpRatio >= currentSet.rewardMilestones[i] && !unlockedMilestones[i])
                UnlockReward(i);
        }
    }

    public void SetLevelSystem(LevelSystem ls)
    {
        levelSystem = ls;
        if (enabled) UpdateRewardSet();
    }
}
