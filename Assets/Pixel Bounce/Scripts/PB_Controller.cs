// PB_Controller - Controls the games buttons 

using UnityEngine;
using System;


public class PB_Controller : MonoBehaviour
{
    public GameObject _rankButton;
    public GameObject _fbButton;
    public GameObject _leftButton;                  // Reference to the scene left button 
    public GameObject _rightButton;             // Reference to the scene right button
    public bool _b1Down;
    public bool _b2Down;
    public bool _b1b2Down;
    public float _b1b2DownCounter;
    bool _enabled = true;
    public Sprite _leftGraphics;                    // Reference to the prefab left button graphics
    public Sprite _rightGraphics;                   // Reference to the prefab right button graphics
    public Sprite _leftGraphicsDown;                // Reference to the prefab left button graphics when button is pressed
    public Sprite _rightGraphicsDown;               // Reference to the prefab right button graphics when button is pressed

    public GameObject[] _controlledGameObjects;
    public static PB_Controller instance;       // PB_Controller is a singleton. PB_Controller.instance.DoSomeThing();
    void OnApplicationQuit()
    {           // Ensure that the instance is destroyed when the game is stopped in the editor.
        instance = null;
    }

    void Start()
    {                       // Destroys this in case another instance is present
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

    public void DisableController(float seconds)
    {           // Disables controller for x amount of time
        if (_enabled)
        {
            _enabled = false;
            Invoke("DisableControllerInvoke", seconds);
        }
    }

    public void DisableControllerInvoke()
    {
        _enabled = true;
    }

    public void Update()
    {
        // Check if player is using mouse or keyboard
#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
        if (Input.GetButtonDown("Jump"))
        {
            LoginFacebook();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Button1();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Button2();
        }
        if (Input.GetButtonDown("Fire3"))
        {
            RankBtnDown();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Button1Up();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            Button2Up();
        }
        if (Input.GetButtonUp("Fire3"))
        {
            RankBtnUp();
        }



#endif
        // Check if player is touching the buttons
        foreach (Touch touch in Input.touches)
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == _leftButton)
                {                       //User touches a button
                    Button2();
                }
                else if (hit.collider.gameObject == _rightButton)
                {               //User touches a button
                    Button1();
                }
                else if (hit.collider.gameObject == _fbButton)
                {
                    LoginFacebook();
                }
                else if (hit.collider.gameObject == _rankButton)
                {
                    RankBtnDown();
                }
            }
            if (touch.phase == TouchPhase.Ended && Physics.Raycast(ray, out hit))
            {       //When user stops touching screen but not pressing button (button just resets, nothing happens)
                if (hit.collider.gameObject == _leftButton)
                    Button2Up();
                if (hit.collider.gameObject == _rightButton)
                    Button1Up();
                if (hit.collider.gameObject == _rankButton)
                    RankBtnUp();
            }
        }
        // Check if player is touching both the buttons (used to bring up the Frame Per Seconds debug numbers) (Can be removed)
        if (this._b1Down && this._b2Down)
        {
            this._b1b2Down = true;
            _b1b2DownCounter += Time.deltaTime;
        }
        else
        {
            this._b1b2Down = false;
            _b1b2DownCounter = 0.0f;
        }
    }
    void HandleMouse()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);

        if (Input.GetMouseButtonDown(0) && hit.collider)
        {
            if (hit.collider.gameObject == _leftButton)
            {                       //User touches a button
                Button2();
            }
            else if (hit.collider.gameObject == _rightButton)
            {               //User touches a button
                Button1();
            }
            else if (hit.collider.gameObject == _fbButton)
            {
                LoginFacebook();
            }
        }
        if (Input.GetMouseButtonUp(0) && hit.collider)
        {
            if (hit.collider.gameObject == _leftButton)
                Button2Up();
            if (hit.collider.gameObject == _rightButton)
                Button1Up();
        }
    }
    void Button1Up()
    {
        _b1Down = false;
        _rightButton.GetComponent<SpriteRenderer>().sprite = _rightGraphics;            // Change graphics of button when button is released
    }

    void Button2Up()
    {
        _b2Down = false;
        _leftButton.GetComponent<SpriteRenderer>().sprite = _leftGraphics;          // Change graphics of button when button is released
    }

    void Button1()
    {
        _b1Down = true;
        _rightButton.GetComponent<SpriteRenderer>().sprite = _rightGraphicsDown;        // Change graphics of button when button is pressed
        if (_enabled)
        {
            for (int i = 0; i < _controlledGameObjects.Length; i++)
            {
                _controlledGameObjects[i].SendMessage("Button1", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    void Button2()
    {
        _b2Down = true;
        _leftButton.GetComponent<SpriteRenderer>().sprite = _leftGraphicsDown;  // Change graphics of button when button is pressed	
        for (int i = 0; i < _controlledGameObjects.Length; i++)
        {
            _controlledGameObjects[i].SendMessage("Button2", SendMessageOptions.DontRequireReceiver);
        }
    }

    void RankBtnUp()
    {
        SpriteRenderer r = _rankButton.GetComponent<SpriteRenderer>();
        r.color = new Color(1, 1, 1, 1);
        GoToLeaderBoard();
    }

    void RankBtnDown()
    {
        SpriteRenderer r = _rankButton.GetComponent<SpriteRenderer>();
        r.color = new Color(1, 1, 1, 0.5f);
    }

    void LoginFacebook()
    {
        FacebookLogin.instance.Login();
    }

    void GoToLeaderBoard()
    {
        LeaderBoard.instance.OnClickRankButton();
    }
}