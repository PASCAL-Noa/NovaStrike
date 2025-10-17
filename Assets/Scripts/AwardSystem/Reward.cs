using UnityEngine;

[System.Serializable]
public class Reward
{
    public string rewardName;
    public string description;
    public float value;
    public RewardType rewardType;

    // Utilisé uniquement pour les rewards qui débloquent une arme pour le joueur
    [Tooltip("ID / name du prefab d'arme à débloquer pour le joueur")]
    public string weaponIdToUnlock;

    // Application sur une instance d'arme (comportement existant)
    public void Apply(Weapon ship)
    {
        if (!ship) return;
        var shooting = ship.GetComponent<PlayerShooting>();

        switch (rewardType)
        {
            case RewardType.Health:
                ship.statistics.IncrementLevel(StatisticsType.Health, value);
                break;

            case RewardType.Attack:
                ship.statistics.IncrementLevel(StatisticsType.Attack, value);
                break;

            case RewardType.Speed:
                ship.statistics.IncrementLevel(StatisticsType.Speed, value);
                break;

            case RewardType.FireRate:
                ship.statistics.IncrementLevel(StatisticsType.FireRate, value);
                break;

            case RewardType.UnlockSecondCanon:
                shooting?.EnableSecondCannon(true);
                break;

            case RewardType.UnlockThirdCanon:
                shooting?.EnableThirdCannon(true);
                break;

            // Si un reward lié au joueur arrive ici par erreur, on l'ignore
            case RewardType.PlayerUnlockWeapon:
                Debug.LogWarning("Reward.Apply(Weapon) reçu PlayerUnlockWeapon — utiliser ApplyToPlayer pour les rewards joueur.");
                break;
        }
    }

    // Application ciblée sur le Player (ex : débloquer une arme pour le joueur)
    public void ApplyToPlayer(Player player)
    {
        if (player == null) return;

        switch (rewardType)
        {
            case RewardType.PlayerUnlockWeapon:
                if (string.IsNullOrEmpty(weaponIdToUnlock))
                {
                    Debug.LogWarning("ApplyToPlayer : weaponIdToUnlock vide.");
                    return;
                }

                // cherche le prefab correspondant dans le GameManager
                var prefab = GameManager.instance?.allWeapons.Find(w => w.name == weaponIdToUnlock);
                if (prefab == null)
                {
                    Debug.LogWarning($"ApplyToPlayer : prefab weapon '{weaponIdToUnlock}' introuvable dans GameManager.allWeapons.");
                    return;
                }

                // évite les doublons (compare par name ou WeaponName)
                bool already = player.unlockedWeapons.Exists(w => w != null && (w.name == prefab.name));
                if (!already)
                {
                    player.unlockedWeapons.Add(prefab);
                    Debug.Log($"ApplyToPlayer : arme '{weaponIdToUnlock}' débloquée pour le joueur.");

                    // sauvegarde immédiatement la progression si possible
                    if (DataSync.instance != null)
                        player.SavePlayer(DataSync.instance);
                }
                break;

            default:
                Debug.LogWarning("ApplyToPlayer : rewardType non supporté pour application directe au Player.");
                break;
        }
    }
}

public enum RewardType
{
    Health,
    Attack,
    Speed,
    FireRate,
    UnlockSecondCanon,
    UnlockThirdCanon,

    // nouveau : débloquer une arme (stockée par prefab id) pour le joueur
    PlayerUnlockWeapon
}