using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private string _mID;
    [SerializeField] private LevelSystem _mLevelSys;
    [SerializeField] private Weapon _mWeapon;

    // Références aux prefabs / choix du joueur (utilisé pour UI & instantiation)
    public List<Weapon> unlockedWeapons = new List<Weapon>();

    // Données persistantes : source de vérité pour la sauvegarde (id, level, xp, unlocked)
    public List<WeaponData> unlockedWeaponsData = new List<WeaponData>();

    private int _mScore;
    public int _mStage;

    public bool IsLoaded { get; set; } = false;
    public int Score
    {
        get => _mScore;
        set => _mScore = value;
    }
    public string ID
    {
        get => _mID;
        set => _mID = value;
    }

    public int Stage
    {
        get => _mStage;
        set => _mStage = value;
    }
    public Weapon Weapon
    {
        get => _mWeapon;
        set => _mWeapon = value;
    }

    public LevelSystem LevelSystemPlayer
    {
        get => _mLevelSys;
        set => _mLevelSys = value;
    }

    public void AddScore(int amount)
    {
        Score += Mathf.Max(0, amount);
    }


    // Synchronise unlockedWeaponsData avec l'état runtime des unlockedWeapons (préfab/instance)
    private void SyncUnlockedWeaponData()
    {
        if (unlockedWeaponsData == null) unlockedWeaponsData = new List<WeaponData>();

        foreach (var prefab in unlockedWeapons)
        {
            if (prefab == null) continue;
            string id = prefab.name;

            var data = unlockedWeaponsData.Find(w => w.weaponId == id);
            if (data == null)
            {
                data = new WeaponData { weaponId = id, level = 1, xp = 0, isUnlocked = true };
                unlockedWeaponsData.Add(data);
            }

            Weapon runtimeInstance = null;

            if (Weapon != null && Weapon.name == id && Weapon.gameObject != null && Weapon.gameObject.scene.IsValid())
                runtimeInstance = Weapon;

            if (runtimeInstance == null)
            {
                var allInstances = GameObject.FindObjectsByType<Weapon>(FindObjectsSortMode.None);
                foreach (var inst in allInstances)
                {
                    if (inst == null) continue;
                    if (inst.name == id && inst.gameObject.scene.IsValid()) { runtimeInstance = inst; break; }
                }
            }

            if (runtimeInstance != null && runtimeInstance.WeaponLevelSystem != null)
            {
                data.level = runtimeInstance.WeaponLevelSystem.CurrentLevel;
                data.xp = runtimeInstance.WeaponLevelSystem.CurrentXp;
                data.isUnlocked = true;
            }
            else
            {
                if (prefab.WeaponLevelSystem != null)
                {
                    data.level = prefab.WeaponLevelSystem.CurrentLevel;
                    data.xp = prefab.WeaponLevelSystem.CurrentXp;
                    data.isUnlocked = prefab.IsUnlocked;
                }
            }
        }
    }

    public void SavePlayer(DataSync datasync)
    {
        SyncUnlockedWeaponData();

        PlayerData playerData = new PlayerData
        {
            id = ID,
            score = Score,
            stage = Stage,
            level = LevelSystemPlayer != null ? LevelSystemPlayer.CurrentLevel : 1,
            xp = LevelSystemPlayer != null ? LevelSystemPlayer.CurrentXp : 0,
            equippedWeaponId = Weapon != null ? Weapon.name : "",
            unlockedWeaponsData = new List<WeaponData>()
        };

        if (unlockedWeaponsData != null && unlockedWeaponsData.Count > 0)
        {
            playerData.unlockedWeaponsData.AddRange(unlockedWeaponsData);
        }

        if (playerData.unlockedWeaponsData.Count == 0)
        {
            string defaultWeaponId = "Ship";
            WeaponData defaultWeaponData = new WeaponData
            {
                weaponId = defaultWeaponId,
                level = 1,
                xp = 0,
                isUnlocked = true
            };
            playerData.unlockedWeaponsData.Add(defaultWeaponData);
        }

        datasync.SavePlayerData(playerData);
    }

    public void LoadPlayer(DataSync datasync, string playerId)
    {
        datasync.LoadPlayerData(playerId, (playerData) =>
        {
            if (playerData == null)
            {
                ID = playerId;
                Score = 0;
                Stage = 0;
                if (LevelSystemPlayer != null) { LevelSystemPlayer.CurrentLevel = 1; LevelSystemPlayer.CurrentXp = 0; }
                unlockedWeapons.Clear();
                unlockedWeaponsData.Clear();

                string defaultWeaponId = "Ship";
                var prefab = GameManager.instance?.allWeapons.Find(w => w.name == defaultWeaponId);
                if (prefab != null) unlockedWeapons.Add(prefab);

                unlockedWeaponsData.Add(new WeaponData { weaponId = defaultWeaponId, level = 1, xp = 0, isUnlocked = true });

                SavePlayer(datasync);
                IsLoaded = true;
                //GameManager.instance?.OnPlayerLoaded();
                return;
            }

            ID = playerData.id;
            Score = playerData.score;
            Stage = playerData.stage;

            if (LevelSystemPlayer != null)
            {
                var setMethod = LevelSystemPlayer.GetType().GetMethod("SetLevelAndXp");
                if (setMethod != null) setMethod.Invoke(LevelSystemPlayer, new object[] { playerData.level, playerData.xp });
                else { LevelSystemPlayer.CurrentLevel = playerData.level; LevelSystemPlayer.CurrentXp = playerData.xp; }
            }

            unlockedWeaponsData = playerData.unlockedWeaponsData ?? new List<WeaponData>();

            unlockedWeapons.Clear();
            foreach (var wData in unlockedWeaponsData)
            {
                Weapon prefab = GameManager.instance?.allWeapons.Find(w => w.name == wData.weaponId);
                if (prefab != null) unlockedWeapons.Add(prefab);
            }

            Weapon equipped = unlockedWeapons.Find(w => w.name == playerData.equippedWeaponId);
            if (equipped != null) Weapon = equipped;

            IsLoaded = true;

            // notifier GameManager que le player est chargé (pour RefreshState / UI)
            //GameManager.instance?.OnPlayerLoaded();
        });
    }

    public void ResetForLogin()
    {
        // 1) Mettre à jour les données persistantes à partir des instances runtime
        SyncUnlockedWeaponData();

        // 2) Créer un snapshot immuable et sauvegarder immédiatement (avant de détruire quoi que ce soit)
        if (DataSync.instance != null)
        {
            var snapshot = new PlayerData
            {
                id = ID ?? string.Empty,
                score = Score,
                stage = Stage,
                level = LevelSystemPlayer != null ? LevelSystemPlayer.CurrentLevel : 1,
                xp = LevelSystemPlayer != null ? LevelSystemPlayer.CurrentXp : 0,
                equippedWeaponId = Weapon != null ? Weapon.name.Replace("(Clone)", "").Trim() : string.Empty,
                unlockedWeaponsData = new List<WeaponData>()
            };

            if (unlockedWeaponsData != null)
                snapshot.unlockedWeaponsData.AddRange(unlockedWeaponsData);
            DataSync.instance.SavePlayerData(snapshot);
        }

        // 3) Réinitialisation de l'état runtime (détruire instance, vider listes runtime)
        if (Weapon != null && Weapon.gameObject != null && Weapon.gameObject.scene.IsValid())
        {
            Destroy(Weapon.gameObject);
            Weapon = null;
        }

        // Reconstruire la liste des prefabs débloqués à partir des données persistantes
        unlockedWeapons.Clear();
        if (unlockedWeaponsData != null)
        {
            foreach (var wd in unlockedWeaponsData)
            {
                if (string.IsNullOrEmpty(wd.weaponId)) continue;
                var prefab = GameManager.instance?.allWeapons.Find(w => w != null && (w.name == wd.weaponId ));
                if (prefab != null) unlockedWeapons.Add(prefab);
            }
        }

        // 4) Nettoyage des données de session
        ID = string.Empty;
        Score = 0;
        Stage = 0;

        if (LevelSystemPlayer != null)
        {
            LevelSystemPlayer.CurrentLevel = 1;
            LevelSystemPlayer.CurrentXp = 0;
        }

        IsLoaded = false;
        GameManager.instance.uiManager.ShowLogin();
    }
}

    [Serializable]
public class PlayerData
{
    public string id;
    public int score;
    public int level;
    public float xp;
    public int stage;
    public string equippedWeaponId;
    public List<WeaponData> unlockedWeaponsData;

    //public string equippedSkillId;
    //public List<SkillData> unlockedSkills;
    //public string equippedUltId;
    //public List<UltData> unlockedUlt;
}