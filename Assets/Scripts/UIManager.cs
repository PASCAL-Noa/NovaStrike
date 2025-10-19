using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class UIManager : MonoBehaviour
{
    [Header("Panels Principaux")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameRoot;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject debugPanel;

    [Header("Menus Spécifiques")]
    [SerializeField] private ShipSelectionMenu shipSelectionMenu;

    private bool isDebugOpen;
    private Keyboard keyboard;

    private void Awake() => keyboard = Keyboard.current;
    private void Start() => ShowLogin();

    private void Update()
    {
        if (keyboard == null) return;
        if (keyboard.escapeKey.wasPressedThisFrame) HandleEscapeKey();
        if (keyboard.f1Key.wasPressedThisFrame) ToggleDebugMenu();
    }

    #region Panels Logic

    public void ShowLogin()
    {
        SetActivePanel(loginPanel);
    }

    public void ShowMainMenu()
    {
        SetActivePanel(mainMenuPanel);
        shipSelectionMenu.PopulateShips(GameManager.instance.allWeapons);
        Time.timeScale = 1f;
    }

    public void ShowSettings()
    {
        SetActivePanel(settingsPanel);
    }

    public void ShowGame()
    {
        SetActivePanel(gameRoot);
        gameCanvas.SetActive(true);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        SetActivePanel(gameOverPanel);
        gameCanvas.SetActive(false);
        Time.timeScale = 0f;
    }

    private void SetActivePanel(GameObject target)
    {
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameRoot.SetActive(false);
        gameCanvas.SetActive(false);
        gameOverPanel.SetActive(false);
        debugPanel.SetActive(false);
        
        if (target != null) target.SetActive(true);
    }

    #endregion

    #region Input Logic

    private void HandleEscapeKey()
    {
        if (isDebugOpen)
        {
            HideDebugMenu();
            return;
        }

        if (settingsPanel.activeSelf)
        {
            ShowMainMenu();
        }
        else if (gameRoot.activeSelf)
        {
            return;
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void ToggleDebugMenu()
    {
        if (isDebugOpen) HideDebugMenu();
        else ShowDebugMenu();
    }

    private void ShowDebugMenu()
    {
        isDebugOpen = true;
        debugPanel.SetActive(true);
        
        if (gameRoot.activeSelf) Time.timeScale = 0f;
    }

    private void HideDebugMenu()
    {
        isDebugOpen = false;
        debugPanel.SetActive(false);
        
        if (gameRoot.activeSelf) Time.timeScale = 1f;
    }

    #endregion

    #region Gameplay

    public void PlayGame()
    {
        ShowGame();

        if (GameManager.instance._mPlayer.Weapon != null)
        {
            Weapon prefab = GameManager.instance._mPlayer.Weapon;
            Weapon instance = Instantiate(prefab);
            instance.name = prefab.name;

            var rewardManager = instance.GetComponentInChildren<RewardManager>();
            rewardManager?.RefreshState();

            WeaponData wData = GameManager.instance._mPlayer.unlockedWeaponsData?.Find(w => w.weaponId == prefab.name);

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
            rewardManager?.RefreshState();
        }

        GameManager.instance.stageManager?.StartStages();
        GameManager.instance._mPlayer.SavePlayer(DataSync.instance);
    }

    #endregion
}
