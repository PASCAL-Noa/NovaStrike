using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRewardSet", menuName = "Game/Reward Set")]
public class RewardSet : ScriptableObject
{
    [Header("Informations")]
    public string setName = "Level 1 Rewards";

    [Header("Paliers d'XP pour débloquer chaque récompense (entre 0 et 1)")]
    [Range(0f, 1f)] public float[] rewardMilestones = { 0.2f, 0.5f, 0.7f, 1f };

    [Header("Récompenses associées (dans le même ordre que les paliers)")]
    public List<Reward> rewards = new();
}