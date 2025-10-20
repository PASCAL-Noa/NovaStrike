using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;
using System;

public class LoginMenu : MonoBehaviour
{
    [Header("Login Fields")]
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;

    [Header("Signup Fields")]
    [SerializeField] private TMP_InputField signupEmailInput;
    [SerializeField] private TMP_InputField signupPasswordInput;
    [SerializeField] private TMP_InputField signupPseudoInput;

    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackText;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private UIManager uiManager;

    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
    
    public void LoginUser()
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Veuillez entrer un email et un mot de passe !";
            return;
        }

        feedbackText.text = "Connexion en cours...";

        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    feedbackText.text = "Identifiants invalides ou compte inexistant.";
                    Debug.LogError(task.Exception);
                    return;
                }

                player.LoadPlayerFromFirebase(task.Result.User, success =>
                {
                    if (success)
                    {
                        uiManager.ShowMainMenu();
                        feedbackText.text = "";
                    }
                    else
                    {
                        feedbackText.text = "Erreur lors du chargement du joueur.";
                    }
                });
            });

    }
    
   public void RegisterUser() 
   {
       string email = signupEmailInput.text.Trim(); 
       string password = signupPasswordInput.text.Trim();
       string pseudo = signupPseudoInput.text.Trim();

       if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(pseudo))
       {
           feedbackText.text = "Veuillez remplir tous les champs pour créer un compte.";
           return;
       }

       feedbackText.text = "Création du compte...";
       auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
       {
           if (task.IsFaulted || task.IsCanceled)
           {
               string errorMsg = task.Exception?.Flatten().InnerException?.Message ?? "Erreur inconnue";
               feedbackText.text = errorMsg;
               Debug.LogError("Firebase CreateUser error: " + errorMsg);
               return;
           }

           FirebaseUser newUser = task.Result.User;

           if (newUser != null)
           {
               UserProfile profile = new UserProfile { DisplayName = pseudo };
               newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(profileTask =>
               {
                   if (profileTask.IsFaulted || profileTask.IsCanceled)
                   {
                       string profileError = profileTask.Exception?.Flatten().InnerException?.Message ?? "Erreur pseudo";
                       feedbackText.text = profileError;
                       Debug.LogError("Firebase UpdateUserProfile error: " + profileError);
                       return;
                   }
                  
                   player.LoadPlayerFromFirebase(newUser, success =>
                   {
                       if (success)
                       {
                           player.UserName = pseudo;
                           player.SavePlayer(DataSync.instance);
                           
                           uiManager.ShowMainMenu();
                           feedbackText.text = "";
                           Debug.Log("Compte créé avec succès : " + pseudo);
                       }
                       else
                       { 
                           feedbackText.text = "Erreur lors du chargement du joueur.";
                       }
                   });
               });
           }
       });
   }
   
   private void LoadPlayer(FirebaseUser user)
   {
       if (user == null)
       {
           feedbackText.text = "Erreur : utilisateur non trouvé.";
           return;
       }

       feedbackText.text = "Chargement du joueur...";
       player.LoadPlayerFromFirebase(user, success =>
       {
           if (!success)
           {
               feedbackText.text = "Erreur lors du chargement du joueur.";
               return;
           }
           
           if (string.IsNullOrEmpty(player.UserName) && !string.IsNullOrEmpty(user.DisplayName))
               player.UserName = user.DisplayName;

           signupPseudoInput.text = player.UserName;

           uiManager.ShowMainMenu();
           feedbackText.text = "";
           Debug.Log("Player loaded successfully: " + player.UserName);
       });
   }
}
