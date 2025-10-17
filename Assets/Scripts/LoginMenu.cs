using UnityEngine;
using TMPro;
using System.Threading;
using System.Collections;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerIdInput;
    [SerializeField] private TMP_Text confirmButtonText;
    [SerializeField] private GameManager gameManager;
    public UIManager uiManager;
    public IEnumerator WaitForPlayerLoad()
    {
        confirmButtonText.text = "Chargement...";

        while (!gameManager._mPlayer.IsLoaded)
        {
            yield return null; 
        }

        print("Chargement terminé !");

        uiManager.ShowMainMenu();
    }
    public void OnConfirmClicked()
    {
        string playerId = playerIdInput.text.Trim();
        if (!string.IsNullOrEmpty(playerId))
        {
            gameManager._mPlayer.LoadPlayer(gameManager.dataSync, playerId);
            StartCoroutine(WaitForPlayerLoad());
        }
        else
        {
            confirmButtonText.text = "Veuillez entrer un ID !";
        }
    }
}