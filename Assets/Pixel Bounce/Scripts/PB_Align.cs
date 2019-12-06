// PB_Align - Used to align UI elements left and right on the screen

using UnityEngine;
using System;


public class PB_Align:MonoBehaviour{
	public bool _alignLeft;
	public bool _alignRight;
	
	public void Start() {
		Invoke("Align", 0.0f);
	}
	
	public void Align() {
		if(_alignLeft){
			transform.position = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3 (0.0f,0.0f,0.0f)).x, transform.position.y);				// Uses camera to figure out where left is on the screen
		}else if(_alignRight){
			transform.position = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3 ((float)Screen.width,0.0f,0.0f)).x,transform.position.y);	// Uses camera to figure out where right is on the screen
		}
		Invoke("Align", 1.0f);
	}
}