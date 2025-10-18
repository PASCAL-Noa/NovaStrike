using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Statistics), typeof(LevelSystem))]
public class Weapon : Entity
{
    protected Statistics stats;
    [SerializeField] private LevelSystem _mWeaponLevelSystem ;
    [SerializeField] private bool is_unlocked;
    [SerializeField] protected AudioClip deathSfx; 
    
    public LevelSystem WeaponLevelSystem
    {
        get { return _mWeaponLevelSystem; }
        set { _mWeaponLevelSystem = value; }
    }

    public bool IsUnlocked
    {
        get { return is_unlocked; }
        set { is_unlocked = value; }
    }
    
    protected override void OnDeath()
    {
        if (deathSfx != null)
            AudioManager.Instance.PlaySFX(deathSfx);
        
        GameManager.instance._mPlayer.Weapon = GameManager.instance.allWeapons[0];
        GameManager.instance?.GameOver();
        Destroy(gameObject);
    }
}

[Serializable]
public class WeaponData
{
    public string weaponId;
    public int level;
    public float xp;
    public bool isUnlocked;
}
