using System;
using UnityEngine;

[RequireComponent(typeof(Statistics), typeof(LevelSystem))]
public class Weapon : Entity
{
    protected Statistics stats;
    [SerializeField] private LevelSystem _mWeaponLevelSystem ;
    [SerializeField] private bool is_unlocked;
    
  
    public event Action<Weapon> OnWeaponDeath;


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
        
        GameManager.instance.uiManager.ShowGameOver();

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
