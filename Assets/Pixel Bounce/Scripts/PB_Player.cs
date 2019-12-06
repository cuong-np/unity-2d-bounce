// PB_Player - Handles player movement, collisions, particle FX and sounds

using UnityEngine;
using System;
using System.Collections;


public class PB_Player : MonoBehaviour
{
    Rigidbody2D _rb;            // Rigidbody attached to player
    public Animator _anim;                      // Animator attached to player
    bool _jumping;          // The player is jumping
    bool _diving;           // The player is diving downwards after jumping
    bool _stunned;          // Stunned player can not move, happens when diving into the ground
    float _stunTimer;           // Time the player is stunned
    PB_Camera _cam;             // Camera script used to shake the screen
    float _saveXpos;            // Saves the position of the player at start
    bool _alive = true;     // Is the player alive or dead
    bool _canSwing;         // Player can only swing once in the air
    Transform _sprite;

    //Effects
    public ParticleSystem _crashFX;         // Particle effects used by the player
    public ParticleSystem _landFX;
    public ParticleSystem _swingFX;
    public ParticleSystem _jumpFX;
    public ParticleSystem _hitFX;
    public ParticleSystem _helmFX;
    public ParticleSystem _deathFX;
    public ParticleSystem _featherFX;

    //Sounds
    public AudioClip _jumpSound;                // Sounds Used by the player
    public AudioClip _bounceSound;
    public AudioClip _deathSound;
    public AudioClip _bounceHardSound;
    public AudioClip _diveSound;
    public AudioClip _crashSound;
    public AudioClip _landSound;
    public AudioClip _helmSound;
    public AudioClip _swingSound;

    public static PB_Player instance;           // Singleton: PB_Player is a singleton. PB_Player.instance.DoSomeThing();


    void OnApplicationQuit()
    {           // Singleton: Ensure that the instance is destroyed when the game is stopped in the editor.
        instance = null;
    }

    void Awake()
    {                       // Singleton: Destroys itself in case another instance is already in the scene
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Start()
    {
        _sprite = transform.Find("Sprite");     // Reference to the character sprite
        _rb = this.GetComponent<Rigidbody2D>();         // Assign components to variables
        _anim = _sprite.GetComponent<Animator>();
        _cam = PB_Camera.instance;
        _saveXpos = transform.position.x;               // Save start position of player
        Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
        r.gravityScale *= PB_GameController.instance._gameSpeed;
        r.mass *= 1 + (1 - PB_GameController.instance._gameSpeed);
    }

    public void Button1()
    {                                   // Button 1 is called by the PB_Controller
        if (PB_GameController.instance._mode == "Game")
        {       // Jumping
            Jump(10.0f, false, true);
        }
    }

    public void Button2()
    {                                               // Button 2 is called by the PB_Controller
        if (_canSwing && PB_GameController.instance._mode == "Game")
        {   //Swing club
            Slash();
        }
    }


    void Update()
    {
        transform.position = new Vector2(_saveXpos, transform.position.y);                          // fix the x position of player
        if (_alive && PB_GameController.instance._mode == "Game")
        {
            if (_stunTimer > 0)
            {                                       //Count down stun timer if player is stunned
                _stunTimer -= Time.deltaTime;
            }
            else if (_stunned)
            {
                _stunned = false;
                _anim.Play("Run");                                  // Starts running animation when no longer stunned
            }
        }
    }


    public void Slash()
    {   // Player melee attack
        _rb.velocity = new Vector2(0.0f, 5.0f);                     // Add a bit of jump velocity when slashing							
        _canSwing = false;                                  // Not able to slash again until performing a jump
        _anim.Play("Slash");                                // Play Slash animation
        SoundController.instance.Play(_swingSound, .75f, UnityEngine.Random.Range(1.0f, .75f));
        Invoke("SlashToJumpAnim", .165f);                   // Runs a delayed function to end Slash animation
    }

    public void SlashToJumpAnim()
    {   // Delayed function to end Slash animation
        _anim.Play("Jump");
    }

    public void Jump(float _amount, bool bounce, bool dive)
    {           // Jumping and Dive funtion (_amount = how much velocity the jump has)
        if (!_stunned && _alive)
        {                                           // Player can not jump if stunned or dead
            if (!_jumping || !dive)
            {
                if (!bounce)
                {
                    SoundController.instance.Play(_jumpSound, .5f, UnityEngine.Random.Range(1.0f, 1.25f));
                    _jumpFX.transform.position = transform.position;
                    _jumpFX.Play();
                    _jumpFX.Play();
                }
                if (_canSwing)                                              // Checks if player can swing weapon
                    _anim.Play("Jump");
                _rb.velocity = new Vector2(0.0f, _amount);
                _jumping = true;                                                // Can't jump again
                _canSwing = true;                                           // Player is jumping and can swing weapon
            }
            else if (!_diving && transform.position.y > -.25f)
            {               // Player dives if already jumping
                _anim.Play("Dive");
                _rb.velocity = new Vector2(0.0f, -_amount * .75f);
                _diving = true;
                _canSwing = true;
                SoundController.instance.Play(_diveSound, .75f, UnityEngine.Random.Range(1.5f, 1.25f));
            }
        }
    }

    //Checks if player collides with other colliders
    IEnumerator OnCollisionEnter2D(Collision2D col)
    {
        if (PB_GameController.instance._mode == "Game")
        {
            if (col.gameObject.tag == "Ground")
            {               // Collides with colliders in the layer named Ground		
                if (!_diving)
                {
                    _anim.Play("Run");                      // Plays the running animation clip
                    SoundController.instance.Play(_landSound, .1f, UnityEngine.Random.Range(.5f, .75f));
                    _landFX.transform.position = transform.position;
                    _landFX.Play();
                }
                else
                {
                    _stunned = true;                        // Stuns the player if diving into the ground
                    _stunTimer = 1.0f;                          // Player is stunned for 1 second
                    _anim.Play("Stunned");                  // Play stun animation
                    _cam.Shake(.5f);                            // Shake the camera
                    SoundController.instance.Play(_crashSound, .5f, UnityEngine.Random.Range(1.0f, .75f));
                    _crashFX.transform.position = transform.position;   // Set FX position
                    _crashFX.Play();                                    // Play FX
                }
                _canSwing = false;                              // Can only swing in the air
                _jumping = false;                               // Not jumping any more
                _diving = false;                                // Not diving
            }
            else if (col.gameObject.tag == "Enemy")
            {               //Collides with colliders in the layer named Enemy
                PB_Enemy e = col.gameObject.GetComponent<PB_Enemy>();
                if (e._type == "Regular")
                {
                    if (_jumping)
                    {
                        _jumping = false;
                        if (!_diving)
                        {
                            Kill("Jump", e);
                        }
                        else
                        {
                            Kill("Dive", e);
                        }
                        yield return new WaitForSeconds(.01f);
                        _diving = false;
                    }
                    else
                    {
                        Death();
                    }
                }
                else if (e._type == "Flapping")
                {
                    if (this._canSwing)
                    {
                        Death();
                    }
                    else
                    {
                        Kill("Slash", e);
                        Jump(5.0f, true, false);
                        this._canSwing = true;
                    }
                }
                else if (e._type == "Helmet")
                {
                    if (_jumping)
                    {
                        _jumping = false;
                        if (!_diving)
                        {
                            Jump(10.0f, true, true);
                            SoundController.instance.Play(_helmSound, .5f, UnityEngine.Random.Range(.75f, 1.0f));
                        }
                        else
                        {
                            _helmFX.transform.position = e.transform.position;
                            _helmFX.Emit(1);
                            _hitFX.transform.position = e.transform.position + (Vector3)new Vector2(.1f, .25f); ;
                            _hitFX.Play();
                            e._type = "Regular";
                            e._stunned = true;
                            Jump(10.0f, true, true);
                            e._anim.Play("Stunned");
                            _cam.Shake(.25f);
                            SoundController.instance.Play(_helmSound, .5f, UnityEngine.Random.Range(1.25f, 1.5f));
                        }
                        _diving = false;
                    }
                    else
                    {
                        Death();
                    }

                }
                else if (e._type == "Spiked")
                {           // Spike helmet enemies kills player
                    if (_jumping)
                    {
                        _jumping = false;
                        if (!_diving)
                        {
                            Death();
                        }
                        else
                        {
                            _helmFX.transform.position = e.transform.position;
                            _helmFX.Emit(1);
                            _hitFX.transform.position = e.transform.position + (Vector3)new Vector2(.1f, .25f); ;
                            _hitFX.Play();
                            e._type = "Regular";
                            e._stunned = true;
                            Jump(10.0f, true, true);
                            e._anim.Play("Stunned");
                            _cam.Shake(.25f);
                            SoundController.instance.Play(_helmSound, .5f, UnityEngine.Random.Range(1.25f, 1.5f));
                        }
                        _diving = false;
                    }
                    else
                    {
                        Death();
                    }
                }
            }
        }
    }

    public void Kill(string type, PB_Enemy e)
    {           // Kills an enemy in different ways
        if (type == "Jump")
        {
            e.Damage("Jump");
            Jump(10.0f, true, true);
            SoundController.instance.Play(_bounceSound, .5f, UnityEngine.Random.Range(1.0f, 1.25f));

        }
        else if (type == "Dive")
        {
            _cam.Shake(.25f);
            Jump(15.0f, true, true);
            e.Damage("Dive");
            SoundController.instance.Play(_bounceHardSound, .5f, UnityEngine.Random.Range(1.0f, .75f));
            _hitFX.transform.position = e.transform.position + (Vector3)new Vector2(.1f, .25f); ;
            _hitFX.Play();

        }
        else if (type == "Slash")
        {
            e.Damage("Slash");
            SoundController.instance.Play(_bounceSound, .5f, UnityEngine.Random.Range(1.0f, 1.25f));
        }
        _featherFX.transform.position = e.transform.position;
        _featherFX.Emit(10);                        // Feathers splash when enemy dies
    }

    public void Alive()
    {
        _alive = true;
        _sprite.GetComponent<Renderer>().enabled = true;
        _anim.Play("Idle");
        this._jumping = false;
        this._diving = false;
    }

    public void Death()
    {                               // Player dies
        if (_alive)
        {
            _alive = false;
            _deathFX.transform.position = transform.position;
            _deathFX.Emit(1);                       // Plays death FX
            _sprite.GetComponent<Renderer>().enabled = false;       // Disables player renderer so that it is no longer visible
            SoundController.instance.Play(_deathSound, .5f, UnityEngine.Random.Range(1.0f, .75f));
            PB_GameController.instance.SetHighScore();
            PB_GameController.instance.SetMode("GameOver");
        }
    }


}
