// PB_Blink - Blinks the "Start Game" text in a retro fashion

using UnityEngine;
using System;


public class PB_Blink:MonoBehaviour{
	public float _blinkSpeed = 1.0f;        // How fast blinking on and off

    bool isStart = false;
	void Start() {
        StartBlink();

    }
	
	void Blink() {
		if(gameObject.activeInHierarchy)
            gameObject.GetComponent<Renderer>().enabled = !gameObject.GetComponent<Renderer>().enabled;		// Simply turns on and off renderer
	}
    public void StopBlink()
    {
        if (!isStart)
            return;
        isStart = false;
        CancelInvoke();
        gameObject.GetComponent<Renderer>().enabled = true;
    }
    public void StartBlink()
    {
        if (isStart)
            return;
        isStart = true;
        InvokeRepeating("Blink", _blinkSpeed, _blinkSpeed);
    }
}