//PB_GameController - Keeps track of game mode, score and character unlocks

using UnityEngine;
using System;


public class PB_GameController : MonoBehaviour
{

    public int _score = 0;                                  // How many enemies have been squashed
    public GameObject _scoreText;                       // UI reference to the score text
    public GameObject _scoreTextShadow;             // UI reference to the score text shadow
    public GameObject _scoreContainer;                  // Game object containing score UI

    public int _highScore = 0;
    public GameObject _highScoreText;
    public GameObject _highScoreTextShadow;
    public GameObject _highScoreContainer;

    public float _gameSpeed = 1.5f;                     // Game speed multiplier

    public GameObject _titleContainer;                  // Contains title screen game objects
    public GameObject _title;
    public GameObject _startText;

    public GameObject _newRecord;                       // Contains new records game objects
    public bool _newRecordAchieved;

    public string _mode = "StartScreen";                // Current game mode

    public int _enemiesOnScreen;                        // How many enemies are on screen, counted++ by enemy script

    public AudioClip _newRecordSound;                   // Sound to play if player gets a new record
    public AudioClip _startSound;

    public int _unlockLevel;                            // Used to check how many character has been unlocked
    public RuntimeAnimatorController[] _playerSprites;  // Holds all the characters

    public PB_Player _player;                           // Reference to the player
    public int _currentPlayer;                          // Current player graphics

    public static PB_GameController instance;               // PB_GameController is a singleton. PB_GameController.instance.DoSomeThing();


    void OnApplicationQuit()
    {                       // Ensure that the instance is destroyed when the game is stopped in the editor.
        instance = null;
    }

    void Awake()
    {       // Singleton: Destroys itself in case another instance is already in the scene
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        SoundController.instance.Play(_startSound, .75f, 1.0f);
        _title.SetActive(true);                                 // Enable start screen
        _titleContainer.SetActive(true);                        // Enable title screen
        _startText.SetActive(true);                             // Enable start text
       // _highScore = PlayerPrefs.GetInt("HighScore");           // Load save data: Highscore
        SetHighScoreText();                                     // Set UI text for highscore
        SetScore();                                             // Set UI text
        _player = PB_Player.instance;                           // Reference to player singleton
        CheckUnlocks();                                         // Check what characters are unlocked based on score
    }

    public void Button1()
    {                                       // Button 1 is called by the PB_Controller
        if (_mode == "StartScreen")
        {                               // Start game if in start screen
            SetMode("Game");
        }
        else if (_mode == "NewRecord")
        {                           // Go to start screen if in new record screen
            SetMode("StartScreen");
        }
    }

    public void Button2()
    {                                       // Button 2 is called by the PB_Controller
        if (_mode == "StartScreen")
        {
            _currentPlayer++;                                   // Change character if in start screen
            if (_currentPlayer >= _playerSprites.Length || _currentPlayer > _unlockLevel)   // Check if character cycle has reached its end
                _currentPlayer = 0;                                                             // Restart character cycle
            _player._anim.runtimeAnimatorController = _playerSprites[_currentPlayer];       // Change animation controller to the current character index
        }
    }

    public void Unlock(int l)
    {                       // Set character level and automatiacally change to newest character
        _unlockLevel = l;
        _currentPlayer = l - 1;
        Button2();
    }

    public void CheckUnlocks()
    {                                   // Check what characters are unlocked based on score
                                        //Unlocks bonus characters
        if (this._highScore >= 40 && _unlockLevel < 4)
        {
            Unlock(4);
        }
        else if (this._highScore >= 30 && _unlockLevel < 3)
        {
            Unlock(3);
        }
        else if (this._highScore >= 20 && _unlockLevel < 2)
        {
            Unlock(2);
        }
        else if (this._highScore >= 10 && _unlockLevel < 1)
        {
            Unlock(1);
        }
    }

    //Set the different modes in the game
    public void SetMode(string m)
    {                       // Change the games mode
                            //Start Screen Mode
        if (m == "StartScreen" && _mode != "StartScreen")
        {           // Start screen mode
            PB_Controller.instance.DisableController(1.0f);
            _mode = m;
            _newRecord.SetActive(false);
            SoundController.instance.Play(_startSound, .75f, 1.0f);
            _scoreContainer.SetActive(true);
            _highScoreContainer.SetActive(true);
            _title.GetComponent<Animator>().Play("In");             // Show title
            _startText.SetActive(true);
            PB_Player.instance.Alive();
            CheckUnlocks();                                         // Check what characters are unlocked based on score
                                                                    //Game Play Mode	
        }
        else if (m == "Game" && _mode != "Game")
        {                   // Game mode
            _title.GetComponent<Animator>().Play("Out");                // Hide title
            _startText.SetActive(false);
            PB_Player.instance.GetComponent<BoxCollider2D>().enabled = true;        // Enable player collider
            PB_Player.instance.GetComponent<Rigidbody2D>().isKinematic = false;
            _score = 0;                                             // Reset score for new game
            SetScore();
            _mode = m;
        }
        else if (m == "GameOver" && _mode != "GameOver")
        {           // Player dies
            PB_Controller.instance.DisableController(1.5f);
            _mode = m;
            Invoke("DelayedEndGame", 1.5f);                         // After player dies, wait 1.5 seconds to change to a new mode

        }
        else if (m == "NewRecord" && _mode != "NewRecord")
        {           // When player score is higher than high score the new record screen pops up
            PB_Controller.instance.DisableController(2.0f);
            _mode = m;
            SoundController.instance.Play(_newRecordSound, .75f, 1.0f);
            _newRecord.GetComponent<PB_NewRecord>().NewRecord();
            _newRecord.SetActive(true);
            _scoreContainer.SetActive(false);
            _highScoreContainer.SetActive(false);
            InitializeAdsScript.instance.ShowAds();
        }
    }

    public void DelayedEndGame()
    {
        if (_newRecordAchieved)
        {
            _newRecordAchieved = false;
            SetMode("NewRecord");
        }
        else
        {
            SetMode("StartScreen");
        }
    }

    public void SetScore()
    {           //Set score graphics to text meshes
        if (_score > 9999)          // Clamp scores to 9999
            _score = 9999;
        if (_highScore > 9999)
            _highScore = 9999;
        SetScoreText();                 // Update UI score text      
    }
    public void SetHighScore()
    {
        if (_score > _highScore)
        {
            _newRecordAchieved = true;  // New record achieved (Used to decide what end game screen to use)
            _highScore = _score;        // Change High score	
            SetHighScoreText();         // Update UI high score text
            if (FacebookLogin.instance.isAuth)
            {
                string userID = FacebookLogin.instance.UserID;
                string userName = FacebookLogin.instance.UserName;
                FireBaseDatabase.instance.WriteData(userID, userName, _highScore);
            }
        }
    }
    public void SetScoreText()
    {
        _scoreText.GetComponent<TextMesh>().text = _scoreTextShadow.GetComponent<TextMesh>().text = _score.ToString("0000");
    }

    public void SetHighScoreText()
    {
        _highScoreText.GetComponent<TextMesh>().text = _highScoreTextShadow.GetComponent<TextMesh>().text = _highScore.ToString("0000");
    }

    public void AddPoint(int point)
    {       // Add points to score
        if (_mode == "Game")
        {
            _score += point;                    // Increase score counter
            if (_score < 0) _score = 0;     // Make sure score doesn't go negative
            SetScore();                     //Set score graphics to text meshes
        }
    }
}
