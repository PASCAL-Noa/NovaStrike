using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject SettingsGamePanel;
    [SerializeField] private GameObject GameCanvas;
    [SerializeField] private GameObject Game;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private ShipSelectionMenu shipSelectionMenu;
    [SerializeField] private GameObject debugPanel;
    private bool isSettingsOpen = false;


    public void ShowDebug()
    {
        debugPanel.SetActive(true);
        SettingsGamePanel.SetActive(false);
    }

    public void HideDebug()
    {
        debugPanel.SetActive(false);
        SettingsGamePanel.SetActive(true);
        //ShowSettingsGame();
    }
    
    public void ShowLogin()
    {
        print("Show Login Panel");
        loginPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        SettingsGamePanel.SetActive(false);
        Game.SetActive(false);

    }

    public void ShowMainMenu()
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        Game.SetActive(false);
        SettingsPanel.SetActive(false);
        GameOver.SetActive(false);
        shipSelectionMenu.PopulateShips(GameManager.instance.allWeapons);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        GameOver.SetActive(true);
        GameCanvas.SetActive(false);
        Game.SetActive(false);
        SettingsPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        loginPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        loginPanel.SetActive(false);
        SettingsPanel.SetActive(true);
        Game.SetActive(false);
        mainMenuPanel.SetActive(false);
    }

    public void ShowSettingsInGame()
    {
        SettingsGamePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void HideSettingsGame()
    {
        SettingsGamePanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    private IEnumerator ShowSettingsGame()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isSettingsOpen = !isSettingsOpen;

                if (isSettingsOpen)
                    ShowSettingsInGame();
                else
                    HideSettingsGame();
            }
            yield return null;
        }
    }

    public void PlayGame()
    {
        GameOver.SetActive(false);
        Game.SetActive(true);
        mainMenuPanel.SetActive(false);
        SettingsPanel.SetActive(false);


        if (GameManager.instance._mPlayer.Weapon != null)
        {
            Weapon prefab = GameManager.instance._mPlayer.Weapon;
            Weapon instance = Instantiate(prefab);

            instance.name = prefab.name;

            var weaponReward = instance.GetComponentInChildren<RewardManager>();
            if (weaponReward != null) weaponReward.RefreshState();

            WeaponData wData = null;
            if (GameManager.instance._mPlayer.unlockedWeaponsData != null)
                wData = GameManager.instance._mPlayer.unlockedWeaponsData.Find(w => w.weaponId == prefab.name);

            if (wData != null && instance.WeaponLevelSystem != null)
            {
                var lvlSys = instance.WeaponLevelSystem;
                var setMethod = lvlSys.GetType().GetMethod("SetLevelAndXp");
                if (setMethod != null)
                    setMethod.Invoke(lvlSys, new object[] { wData.level, wData.xp });
                else
                {
                    lvlSys.CurrentLevel = wData.level;
                    lvlSys.CurrentXp = wData.xp;
                }
            }

            instance.IsUnlocked = true;

            GameManager.instance._mPlayer.Weapon = instance;

            if (weaponReward != null)
                weaponReward.RefreshState();
        }

        // d�marrer le timer de stages
        GameManager.instance.stageManager?.StartStages();

        // sauvegarde / UI etc.
        GameManager.instance._mPlayer.SavePlayer(DataSync.instance);
        StartCoroutine(ShowSettingsGame());
        Time.timeScale = 1f;
    }
}