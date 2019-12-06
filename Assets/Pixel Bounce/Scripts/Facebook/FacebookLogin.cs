using Facebook.Unity;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class FacebookLogin : MonoBehaviour
{
    public static FacebookLogin instance;
    public TextMesh nameText;
    [HideInInspector]
    public string UserID = "";
    [HideInInspector]
    public string UserName = "no name";
    [HideInInspector]
    public bool isAuth = false;
    PB_Blink _blink;
    void Awake()
    {
        if (instance)
            GameObject.Destroy(this);
        else
            instance = this;
        _blink = GetComponent<PB_Blink>();
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }
    void Update()
    {
        if (isAuth)
            _blink.StopBlink();
        else
            _blink.StartBlink();
    }
    void OnApplicationQuit()
    {           // Singleton: Ensure that the instance is destroyed when the game is stopped in the editor.
        instance = null;
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    public void Login()
    {
        if (isAuth)
            return;
        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, AuthCallback);
        nameText.text = "Login...";
    }
    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
            List<string> permissions = new List<string>();

            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log("Token:");
            Debug.Log(aToken.UserId);
            Debug.Log(aToken.TokenString);
            //
            nameText.text = "Authenticating...";
            Firebase.Auth.Credential credential =
                Firebase.Auth.FacebookAuthProvider.GetCredential(aToken.TokenString);
            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                    nameText.text = "SignInWithCredentialAsync was canceled";
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    nameText.text = "SignInWithCredentialAsync encountered an error: " + task.Exception;
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                isAuth = true;
                UserID = newUser.UserId;
                UserName = newUser.DisplayName;
                nameText.text = UserName;
                GetHighScore();
            });
        }
        else
        {
            Debug.Log("User cancelled login");
            nameText.text = "...";
        }

    }

    void GetHighScore()
    {
        if (!isAuth)
        {
            Debug.Log("Not SignInWithCredentialAsync");
            return;
        }
        FireBaseDatabase.instance.GetHighScore(UserID, UpdateHighScore);
        
    }

    void UpdateHighScore(int highScore)
    {
        if (highScore >= PB_GameController.instance._highScore)
        {
            PB_GameController.instance._highScore = highScore;
            PB_GameController.instance.SetHighScoreText();
        }
        else
        {
            FireBaseDatabase.instance.WriteData(UserID, UserName, PB_GameController.instance._highScore);
        }
    }

}
