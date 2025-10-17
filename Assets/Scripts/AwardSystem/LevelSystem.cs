using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float currentXp = 0f;

    public int CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }

    public float CurrentXp
    {
        get => currentXp;
        set => currentXp = value;
    }

    public float XpToNextLevel => 80f + CurrentLevel * 20f;
    public float XpProgression => Mathf.Clamp01(currentXp / XpToNextLevel);


    public void AddXp(float amount)
    {
        currentXp += amount;
        if  (currentXp >= XpToNextLevel) LevelUp();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentXp = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            GameManager.instance._mPlayer.Weapon.WeaponLevelSystem.AddXp(5);
        }

        if (Input.GetKeyDown(KeyCode.L))
            GameManager.instance._mPlayer.LevelSystemPlayer.AddXp(5);
    }
}


