//PB_Enemy - Controlls all the different types of enemies

using UnityEngine;
using System;


public class PB_Enemy:MonoBehaviour{
	public string _type = "Regular";			// This enemies type, type is used to recognize properties of the enemy
	public float _speed;						// How fast this enemy moves
	public Animator _anim;						// Reference to the animator component
	public bool _stunned;					// Enemy stoppes when stunned, happens to enemies with helmets
	public int _killPoint = 2;						// How many points this enemy is worth killing
	public int _alivePoint = -1;					// How many points is lost if this enemy gets past player
	public float _spawnHeight;					// How high on the screen does this enemy spawn
	
	float _stunTimer;			// Timer used to stop being stunned
	bool _alive = true;		// Is this alive or dead
	Rigidbody2D _rb;			// Reference to rigidbody
	
	 void Start() {
		_rb = this.GetComponent<Rigidbody2D>();			// Get component references
		_anim = this.GetComponent<Animator>();
		_anim.Play("Run");
		transform.position = new Vector2(transform.position.x, transform.position.y + _spawnHeight);			// Set position height
		InvokeRepeating("CheckBounds", 5.0f , .5f);			// Check if enemy is in the game bounds
		PB_GameController.instance._enemiesOnScreen++;	// Increase number of enemies on screen counter
	}
	
	void CheckBounds(){								// Check if enemy is in the game bounds
		if(transform.position.y < -2.5f || transform.position.x > 4 || transform.position.x < -4){
			DestroyEnemy();
		}
	}
	
	void DestroyEnemy(){
		if(_alive)
		PB_GameController.instance.AddPoint(_alivePoint); 	// Remove point to game for destroying an alive enemy
		PB_GameController.instance._enemiesOnScreen--;		// Decrease number of enemies on screen counter
		Destroy(gameObject);								// Remove enemy from game completely
	}
	
	void FixedUpdate() {								
		if(_alive){		
			_rb.velocity = new Vector2(_speed, 0.0f);						// Move enemy towards player if it is alive
		}
	}
	
	void LateUpdate() {								
		if(PB_GameController.instance._mode != "Game"){
			_speed = 4.0f;
			transform.localScale = new Vector2(-1.0f, 1.0f);							// Flip graphics if not in game mode
		}else if(!_stunned){
			_speed = -PB_GameController.instance._gameSpeed*2;		// Move backwards if not in game mode
		}else{
			_speed = 0.0f;												// Stop movement if stunned
		}
	}
	
	public void Damage(string type){
		PB_GameController.instance.AddPoint(_killPoint);			// Add point to game for destroying an enemy
		transform.GetComponent<BoxCollider2D>().enabled = false;		// Disable collider to make enemy fall trough the world
		_alive = false;												// Dead
		_rb.isKinematic = false;									// Disable isKinematic to enable physics
		gameObject.GetComponent<Renderer>().sortingOrder = 100;						// Put sprite in front
		if(type == "Jump"||type == "Dive"){							
			_anim.Play("Hit");													// Plays death animation
			_rb.velocity = new Vector2((float)UnityEngine.Random.Range(1, 3), (float)UnityEngine.Random.Range(2, 10));	// Add some random force to the rigidbody
			_rb.angularVelocity = (float)UnityEngine.Random.Range(250, -250);						// Spin rigidbody randomly
		}else if(type == "Slash"){
			_anim.Play("Hit2");
			_rb.velocity = new Vector2((float)UnityEngine.Random.Range(3, 5), (float)UnityEngine.Random.Range(1, 3));
			_rb.angularVelocity = (float)UnityEngine.Random.Range(250, -250);
		}
	}
}
