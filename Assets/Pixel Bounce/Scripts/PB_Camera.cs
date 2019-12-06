// PB_Camera - Used for shaking camera effects.

using UnityEngine;
using System;


public class PB_Camera:MonoBehaviour{
	float _shake = 0.0f;						// How long to shake
	float _shakeAmount = 0.1f;				// How hard to shake
	
	public static PB_Camera instance;				// PB_Camera is a singleton. PB_Camera.instance.DoSomeThing();
	
	
     void OnApplicationQuit() {						// Ensure that the instance is destroyed when the game is stopped in the editor.
	    instance = null;
	}
	
	 void Awake() {		// Singleton: Destroys itself in case another instance is already in the scene
		if (instance != null){
	        Destroy (gameObject);
	    }else{
	        instance = this;
	        DontDestroyOnLoad (gameObject);
	    }
	}
	
     void ShakeInvoke() {
		if (_shake > 0) {
			transform.localPosition = UnityEngine.Random.insideUnitSphere * _shakeAmount;						// Moves camera randomly
			transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);		// Freezes camera z position
			_shake -= 0.02f;																			// Counts down to end the shake 
		} else {
			_shake = 0.0f;
			CancelInvoke();
			transform.position = new Vector3(0.0f,0.0f,-10.0f);
		}
	}
	
	public void Shake(float shakeTime) {											// Call this function to start shaking camera
		_shake = shakeTime;
		InvokeRepeating("ShakeInvoke", 0.01f, 0.01f);
	}
}