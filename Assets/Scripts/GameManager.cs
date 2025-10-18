using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] public Player _mPlayer;
    [SerializeField] public DataSync dataSync;
    [SerializeField] public List<Weapon> allWeapons;
    [SerializeField] public UIManager uiManager; // assigne dans l'inspecteur
    [SerializeField] public StageManager stageManager; // assigne dans l'inspecteur
    [SerializeField] public AudioManager audioManager;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GameOver()
    {
        // sauvegarde l'�tat joueur
        if (_mPlayer != null && dataSync != null)
            _mPlayer.SavePlayer(dataSync);

        // arr�ter stages
        stageManager?.StopStages();

        // afficher UI GameOver
        if (uiManager != null)
            uiManager.ShowGameOver();
    }

    public void HandleReturnToMainMenu()
    {
        if (_mPlayer != null && dataSync != null)
            _mPlayer.SavePlayer(dataSync);


        if (uiManager != null)
            uiManager.ShowMainMenu();
    }
    public void ReturnToLoginSoft()
    {
        Time.timeScale = 1f;

        if (_mPlayer != null) 
            _mPlayer.ResetForLogin();
    }

    public void DebugIncrementeHp() => _mPlayer.Weapon.statistics.Increment(StatisticsType.Health, 10);
    public void DebugDecrementeHp() => _mPlayer.Weapon.statistics.Decrement(StatisticsType.Health, 10);
    public void DebugIncrementeAtt() => _mPlayer.Weapon.statistics.Increment(StatisticsType.Attack, 10);
    public void DebugDecrementeAtt() => _mPlayer.Weapon.statistics.Decrement(StatisticsType.Attack, 10);
    public void DebugIncrementeFireRate() => _mPlayer.Weapon.statistics.Decrement(StatisticsType.FireRate, 0.05f);
    public void DebugDecrementeFireRate() => _mPlayer.Weapon.statistics.Increment(StatisticsType.FireRate, 0.05f);
    public void DebugIncrementeSpeed() => _mPlayer.Weapon.statistics.Increment(StatisticsType.Speed, 1);
    public void DebugDecrementeSpeed() => _mPlayer.Weapon.statistics.Decrement(StatisticsType.Speed, 1);
    public void DebugAddXP() => _mPlayer.Weapon.WeaponLevelSystem.AddXp(10);
    public void DebugAddStage() => stageManager.IncrementStage();

}
