using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class Player : MonoBehaviour
{
    [SerializeField] private LevelSystem _mLevelSys;
    [SerializeField] private Weapon _mWeapon;

    public List<Weapon> unlockedWeapons = new List<Weapon>();
    public List<WeaponData> unlockedWeaponsData = new List<WeaponData>();

    public Weapon Weapon { get => _mWeapon; set => _mWeapon = value; }
    public LevelSystem LevelSystemPlayer { get => _mLevelSys; set => _mLevelSys = value; }

    public bool IsLoaded { get; private set; } = false;
    public string FirebaseUserId { get; private set; } = "";

    public string UserName { get; set; } = "Player";
    public int Score { get; private set; } = 0;
    public int Stage { get; set; } = 0;

    public void AddScore(int amount) => Score += Mathf.Max(0, amount);
    public void SetStage(int stage) => Stage = stage;
    
    public void LoadPlayerFromFirebase(FirebaseUser user, Action<bool> callback)
    {
        if (user == null) { callback?.Invoke(false); return; }

        FirebaseUserId = user.UserId;

        DataSync.instance.LoadPlayerData(FirebaseUserId, playerData =>
        {
            if (playerData == null)
                InitializeNewPlayer(FirebaseUserId);
            else
                ApplyPlayerData(playerData);
            
            if (string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(user.DisplayName))
                UserName = user.DisplayName;

            IsLoaded = true;
            callback?.Invoke(true);
        });
    }


    private void InitializeNewPlayer(string userId)
    {
        FirebaseUserId = userId;
        Score = 0;
        Stage = 0;
        if (LevelSystemPlayer != null) { LevelSystemPlayer.CurrentLevel = 1; LevelSystemPlayer.CurrentXp = 0; }

        unlockedWeapons.Clear();
        unlockedWeaponsData.Clear();

        string defaultWeaponId = "Ship";
        var prefab = GameManager.instance?.allWeapons.Find(w => w.name == defaultWeaponId);
        if (prefab != null) unlockedWeapons.Add(prefab);

        unlockedWeaponsData.Add(new WeaponData { weaponId = defaultWeaponId, level = 1, xp = 0, isUnlocked = true });

        SavePlayer(DataSync.instance);
    }

    private void ApplyPlayerData(PlayerData data)
    {
        if (data == null) return;

        FirebaseUserId = data.id;
        UserName = string.IsNullOrEmpty(data.userName) ? "Player" : data.userName;
        Score = data.score;
        Stage = data.stage;
        if (LevelSystemPlayer != null)
        {
            LevelSystemPlayer.CurrentLevel = data.level;
            LevelSystemPlayer.CurrentXp = data.xp;
        }

        unlockedWeaponsData = data.unlockedWeaponsData ?? new List<WeaponData>();
        unlockedWeapons.Clear();

        foreach (var wData in unlockedWeaponsData)
        {
            Weapon prefab = GameManager.instance?.allWeapons.Find(w => w.name == wData.weaponId);
            if (prefab != null) unlockedWeapons.Add(prefab);
        }

        Weapon equipped = unlockedWeapons.Find(w => w.name == data.equippedWeaponId);
        if (equipped != null) Weapon = equipped;
    }

    public void SavePlayer(DataSync dataSync)
    {
        if (dataSync == null) return;

        PlayerData data = new PlayerData
        {
            id = FirebaseUserId,
            userName = UserName,
            score = Score,
            stage = Stage,
            level = LevelSystemPlayer?.CurrentLevel ?? 1,
            xp = LevelSystemPlayer?.CurrentXp ?? 0,
            unlockedWeaponsData = new List<WeaponData>(unlockedWeaponsData),
            equippedWeaponId = Weapon?.name ?? ""
        };

        dataSync.SavePlayerData(data);
    }
    
    public void ResetForLogin()
    {
        IsLoaded = false;
        FirebaseUserId = "";
        Score = 0;
        Stage = 0;

        if (LevelSystemPlayer != null)
        {
            LevelSystemPlayer.CurrentLevel = 1;
            LevelSystemPlayer.CurrentXp = 0;
        }

        Weapon = null;
        unlockedWeapons.Clear();
        unlockedWeaponsData.Clear();

        GameManager.instance.uiManager.ShowLogin();
    }

    public void Logout()
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
            FirebaseAuth.DefaultInstance.SignOut();

        ResetForLogin();
    }
}

[Serializable]
public class PlayerData
{ 
    public string id;
    public string userName;
    public int score;
    public int stage;
    public int level;
    public float xp;
    public string equippedWeaponId;
    public List<WeaponData> unlockedWeaponsData;
}
