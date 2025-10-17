using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSync : MonoBehaviour
{
    public static DataSync instance;
    private DatabaseReference dbRef;
    private FirebaseDatabase firebaseDatabase;
    private const string basePath = "PlayerIDList";

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                firebaseDatabase = FirebaseDatabase.GetInstance(app, "https://serverdbunity-default-rtdb.europe-west1.firebasedatabase.app/");
                dbRef = firebaseDatabase.RootReference;

                Debug.Log("Firebase initialis� avec succ�s !");
            }
            else
            {
                Debug.LogError("Firebase non disponible : " + task.Result);
            }
        });
    }

    public void SavePlayerData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData, true);
        dbRef.Child("joueurs").Child(playerData.id).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log($"sauvegarde joueur {playerData.id} Reussis");
            else
                Debug.LogError($"�chec sauvegarde joueur {playerData.id} : " + task.Exception?.Message);
        });
    }

    public void LoadPlayerData(string playerId, Action<PlayerData> onLoaded)
    {
        dbRef.Child("joueurs").Child(playerId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    PlayerData playerData = JsonUtility.FromJson<PlayerData>(snapshot.GetRawJsonValue());
                    onLoaded?.Invoke(playerData);
                    Debug.Log($"Donn�es du joueur {playerId} charg�es !");
                }
                else
                {
                    Debug.LogWarning($"Aucune sauvegarde trouv�e pour {playerId}.");
                    onLoaded?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"Erreur chargement joueur {playerId} : " + task.Exception?.Message);
                onLoaded?.Invoke(null);
            }
        });
    }
}