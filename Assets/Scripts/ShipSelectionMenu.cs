using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShipSelectionMenu : MonoBehaviour
{
    [SerializeField] private Transform shipsContainer; // Parent des boutons
    [SerializeField] private GameObject shipButtonPrefab; // Prefab bouton vaisseau

    public void PopulateShips(List<Weapon> allWeapons)
        {
        if (shipsContainer != null)
        {
            for (int i = shipsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(shipsContainer.GetChild(i).gameObject);
            }
        }
        int offsetY = 0;
        foreach (Weapon weapon in allWeapons)
        {
            GameObject btnObj = Instantiate(shipButtonPrefab, shipsContainer);

            Vector3 position = btnObj.transform.position;
            position.y -= offsetY;
            btnObj.transform.position = position;
            Button btn = btnObj.GetComponent<Button>();
            string showName;

            if (GameManager.instance._mPlayer.unlockedWeapons.Find(w => w.name == weapon.name))
            {
                showName = weapon.name;
                if (GameManager.instance._mPlayer.Weapon == weapon)
                {
                    btn.Select();
                }
                btn.onClick.AddListener(() =>
                {
                    GameManager.instance._mPlayer.Weapon = weapon;
                    print(GameManager.instance._mPlayer.Weapon.name);
                });
            }
            else
            {
                btn.interactable = false;
                showName = "Not Unlocked Yet";
            }

            btn.GetComponentInChildren<TMP_Text>().text = showName;
            offsetY += 180;
        }
    }
}