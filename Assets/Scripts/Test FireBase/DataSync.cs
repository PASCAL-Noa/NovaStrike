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
                firebaseDatabase = FirebaseDatabase.GetInstance(app, "https://novastrike-ae947-default-rtdb.europe-west1.firebasedatabase.app/"); 
                dbRef = firebaseDatabase.RootReference;

                Debug.Log("Firebase initialisé avec succès !");
            }
            else
            {
                Debug.LogError("Firebase non disponible : " + task.Result);
            }
        });
    }


    public void SavePlayerData(PlayerData playerData, Action onSaved = null)
    {
        if (dbRef == null)
        {
            Debug.LogWarning("Firebase pas encore prêt, réessayer plus tard !");
            return;
        }

        string json = JsonUtility.ToJson(playerData, true);
        dbRef.Child("joueurs").Child(playerData.id).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Sauvegarde joueur {playerData.id} réussie !");
                    onSaved?.Invoke();
                }
                else
                    Debug.LogError($"Échec sauvegarde joueur {playerData.id} : {task.Exception?.Message}");
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
                    Debug.Log($"Donn�es du joueur {playerId} chargées !");
                }
                else
                {
                    Debug.LogWarning($"Aucune sauvegarde trouvée pour {playerId}.");
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