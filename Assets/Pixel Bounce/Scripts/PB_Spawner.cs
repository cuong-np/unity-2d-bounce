// PB_Spawner - Instantiates enemies in waves

using UnityEngine;
using System;


public class PB_Spawner:MonoBehaviour{
	
	float _waveDelay = .5f;			// Delay between each wave of enemies (spacing)
    int _currentWave;				// Counts how many waves have been spawned, used to check if cycle has reached its end
	int _waveCounter;				// If a wave is to repeated this counts how many times that has happened
	int _currentEnemy;				// Current (last) index of enemy spawned
	float _enemyDelay = .5f;		// Delay between each enemy (spacing)
	int _aliveEnemies;				// Counter for telling how many enemies are alive in the game
	
	public PB_Enemy[] enemies;					// Array of all available enemies that the wave class can use
	public int _repeatToWave;						// Once all waves have been played restart to this wave (set to -1 and the game will start playing random waves at the end of the cycle)
	public int _startWave;							// What wave should the game start on
	public bool _randomWave;					// Play random waves
	public int _minRandomWave;						// Same as _repeatToWave but only applies when playing randomly
	
	public static PB_Spawner instance;		// PB_Spawner is a singleton. EnemySpawner.instance.DoSomeThing();
	
	public void OnApplicationQuit() {			// Ensure that the instance is destroyed when the game is stopped in the editor.
	    instance = null;
	}
	
	public PB_SpawnerWave[] waves;						// holds all the enemy waves in the game
	
	public void Start() {
		if (instance != null){
	        Destroy (gameObject);
	    }else{
	        instance = this;
	        DontDestroyOnLoad (gameObject);
	    }
		//Start waves
		_currentWave = _startWave;			// Sets the wave to start the game with
		InvokeRepeating("Wave", _waveDelay / Mathf.Abs(PB_GameController.instance._gameSpeed),
            _enemyDelay / Mathf.Abs(PB_GameController.instance._gameSpeed));	// Repeating waves besed on enemydelay and wavedeley
	}
	
	public void Wave() {
		if(PB_GameController.instance._mode == "Game"){
			if(_currentEnemy < waves[_currentWave].enemies.Length){		// Check if current enemy counter is smaller than the wave enemy array lenght (if not then start next wave)
				if(waves[_currentWave].enemies[_currentEnemy] > 0)		// Check if waves enemy count is higher than zero
					Instantiate(enemies[waves[_currentWave].enemies[_currentEnemy]-1].gameObject, transform.position, transform.rotation);	// Add current enemy from wave to scene
				_currentEnemy++;										// Next enemy in wave
				_aliveEnemies++;										// Increase counter for enemies on screen
			}else{														// Next wave
				if(_waveCounter >= waves[_currentWave].repeats){		// Check if wave should not be repeated
					if(_currentWave < waves.Length-1){					// This is not the last wave
						if(_randomWave)									// Should a random wave be played
							_currentWave=UnityEngine.Random.Range(_minRandomWave, waves.Length);	// Random wave
						else
							_currentWave++;								// Next wave
					}else {												// Check if this is the last wave
						if(_randomWave){								// Should a random wave be played
							_currentWave=UnityEngine.Random.Range(_minRandomWave, waves.Length);	// Random wave
						}else{
							if(_repeatToWave > -1)						// Check if random wave should start to play now that the waves has reached end of cycle (_repeatToWave -1)
								_currentWave = _repeatToWave;			// Repeats waves
							else
								_randomWave = true;						// _RepeatToWave is -1 so now the game will start playing waves randomly
							_waveCounter = 0;							// Reset counter to start waves over agan
						}
					}
				}else{					
					_waveCounter ++;									// Next wave
				}
				// Wave is going to be repeated
				_currentEnemy = 0;				// Reset current enemy to start new wave
				Invoke("Wave", 0.0f);
			}
			
		}
        else if(PB_GameController.instance._mode == "GameOver"){		// Stop and restart waves when the game is over
			_currentWave = _startWave;
			_currentEnemy = 0;
			_aliveEnemies = 0;
			_randomWave = false;	
		}
	}
}
