using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private float stageDuration = 30f;
    [SerializeField] private float spawnMultiplierPerStage = 0.9f;
    [SerializeField] private int maxStage = 10;

    [Header("R�f�rence directe (optionnelle)")]
    [SerializeField] private EnemySpawner enemySpawner;

    public int CurrentStage { get; private set; } = 0;
    public float Elapsed { get; private set; } = 0f;
    public bool IsRunning { get; private set; } = false;

    public event Action<int> OnStageChanged;

    private void Update()
    {
        if (!IsRunning) return;
        Elapsed += Time.deltaTime;
        if (stageDuration <= 0f) return;

        while (Elapsed >= stageDuration)
        {
            Elapsed -= stageDuration;
            IncrementStage();
            if (maxStage > 0 && CurrentStage >= maxStage)
            {
                IsRunning = false;
                break;
            }
        }
    }

    public void StartStages()
    {
        Elapsed = 0f;
        CurrentStage = GameManager.instance._mPlayer.Stage;
        IsRunning = true;
        ApplySpawnMultiplier();

        if (GameManager.instance != null && GameManager.instance._mPlayer != null)
        {
            GameManager.instance._mPlayer.Stage = CurrentStage;

        }
    }

    public void StopStages() => IsRunning = false;

    public void ResetStages()
    {
        IsRunning = false;
        Elapsed = 0f;
        CurrentStage = 0;
        ApplySpawnMultiplier();
        OnStageChanged?.Invoke(CurrentStage);
        if (GameManager.instance != null && GameManager.instance._mPlayer != null)
            GameManager.instance._mPlayer.Stage = CurrentStage;
    }

    public void IncrementStage()
    {
        CurrentStage++;
        ApplySpawnMultiplier();
        OnStageChanged?.Invoke(CurrentStage);

        if (GameManager.instance != null && GameManager.instance._mPlayer != null) { 
            GameManager.instance._mPlayer.Stage = CurrentStage;
            GameManager.instance._mPlayer.LevelSystemPlayer.AddXp(5);
            GameManager.instance._mPlayer.SavePlayer(DataSync.instance);    
        }
    }

    private void ApplySpawnMultiplier()
    {
        float multiplier = Mathf.Pow(spawnMultiplierPerStage, CurrentStage);

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawnMultiplier(multiplier);
            return;
        }

    }

    public void RegisterSpawner(EnemySpawner sp)
    {
        enemySpawner = sp;
    }
}