using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RewardManager : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private LevelSystem levelSystem;

    // Une seule de ces deux cibles doit être assignée (inspecteur) : weapon OU player
    [Header("Cible (assigner *exactement* une)")]
    [SerializeField] private Weapon targetWeapon;
    [SerializeField] private Player targetPlayer;

    [Header("Récompenses par niveau (ScriptableObjects)")]
    [SerializeField] private List<RewardSet> levelRewardSets = new();

    private RewardSet currentSet;
    private bool[] unlockedMilestones;

    private void Awake()
    {
        // Vérification simple : exactement une des deux cibles doit être fournie
        bool hasWeapon = targetWeapon != null;
        bool hasPlayer = targetPlayer != null;
        if (hasWeapon == hasPlayer) // soit les deux true, soit les deux false -> erreur
        {
            Debug.LogError("RewardManager : configurez exactement l'une des cibles (targetWeapon OR targetPlayer) dans l'inspecteur.");
            enabled = false;
            return;
        }

        if (levelSystem == null)
        {
            Debug.LogError("RewardManager : levelSystem manquant. Assignez un LevelSystem dans l'inspecteur.");
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
        if (!enabled || levelSystem == null || currentSet == null) return;

        float xpRatio = levelSystem.CurrentXp / (10 + levelSystem.CurrentLevel * 20);
        for (int i = 0; i < currentSet.rewardMilestones.Length; i++)
        {
            if (xpRatio >= currentSet.rewardMilestones[i] && !unlockedMilestones[i])
            {
                UnlockReward(i);
            }
        }

        if (levelSystem.CurrentLevel > levelRewardSets.IndexOf(currentSet) + 1)
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

        int currentLevelIndex = Mathf.Clamp(levelSystem.CurrentLevel - 1, 0, levelRewardSets.Count - 1);
        currentSet = levelRewardSets[currentLevelIndex];
        unlockedMilestones = new bool[currentSet.rewardMilestones.Length];

        //Debug.Log($"[RewardManager] Ensemble de récompenses chargé : {currentSet.setName}");
    }

    private void UnlockReward(int index)
    {
        if (currentSet == null) return;

        unlockedMilestones[index] = true;

        Reward reward = currentSet.rewards[index];

        // Appliquer selon la cible configurée
        if (targetWeapon != null)
        {
            reward.Apply(targetWeapon);
        }
        else if (targetPlayer != null)
        {
            reward.ApplyToPlayer(targetPlayer);
        }

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