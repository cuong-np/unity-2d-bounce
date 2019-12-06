using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine;
using Firebase.Extensions;
using System.Collections.Generic;

public struct User
{
    public string name;
    public int score;
    public void Init(string _name ="",int _score =0)
    {
        name = _name;
        score = _score;
    }
}
public class FireBaseDatabase : MonoBehaviour
{
     
    public static FireBaseDatabase instance;
    [SerializeField]
    private string URL = "https://bounce2dunitygame.firebaseio.com/";
    DatabaseReference mDatabase;
    void Awake()
    {
        if (instance)
            GameObject.Destroy(this);
        else
            instance = this;
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(URL);
        mDatabase = FirebaseDatabase.DefaultInstance.RootReference;

    }
    void Start()
    {
        // Set this before calling into the realtime database.
    }
    void OnApplicationQuit()
    {           // Singleton: Ensure that the instance is destroyed when the game is stopped in the editor.
        instance = null;
    }
    void ReadInfo()
    {
        Debug.Log("ReadInfo");
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child("E7xjHlAgCNh5Tkv9goASFBPQjXB3")
      .GetValueAsync().ContinueWith(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              // Do something with snapshot...
              Debug.Log("ok");
              Debug.Log(snapshot.ToString());
              string s = snapshot.Child("score").Value.ToString();
              int score = 0;
              int.TryParse(s, out score);
              Debug.Log(score);
          }
      });
    }
    public void GetHighScore(string userId, System.Action<int> callbackFunction)
    {
        int score = 0;
        FirebaseDatabase.DefaultInstance
      .GetReference("users").Child(userId)
      .GetValueAsync().ContinueWithOnMainThread(task =>
      {
          if (task.IsFaulted)
          {
              // Handle the error...
              Debug.LogError("GetScore error");
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              // Do something with snapshot...
              string s = snapshot.Child("score").Value.ToString();
              int.TryParse(s, out score);
              callbackFunction(score);
          }
      });
    }
    public void WriteData(string userId, string userName, int highScore)
    {
        mDatabase.Child("users").Child(userId).Child("name").SetValueAsync(userName);
        mDatabase.Child("users").Child(userId).Child("score").SetValueAsync(highScore);
    }

    public void GetTopScore(int top , System.Action<List<User>> callbackFunction)
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users").OrderByChild("score").LimitToLast(top)
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("GetTopScore error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
                List<User> list = new List<User>();
                foreach (var child in snapshot.Children)
                {
                    string name = child.Child("name").Value.ToString();
                    int score = 0;
                    int.TryParse(child.Child("score").Value.ToString(), out score);
                    User _user = new User();
                    _user.Init(name, score);
                    list.Add(_user);
                }

                callbackFunction(list);
            }
        });
    }
}
