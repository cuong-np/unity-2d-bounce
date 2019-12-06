using UnityEngine;
using System;


public class FramesPerSecond:MonoBehaviour
{
	public bool _enabled;
	public TextMesh _textMesh;
	public float updateInterval = 0.5f;
	public TextMesh _down;
	
	float accum = 0.0f; // FPS accumulated over the interval
	int frames = 0; // Frames drawn over the interval
	float timeleft; // Left time for current interval
	
	 void Start()
	{
	    timeleft = updateInterval;  
	    _textMesh = transform.GetComponent<TextMesh>();
	    _textMesh.transform.GetComponent<Renderer>().enabled = false;
	    _down = transform.Find("Down").transform.GetComponent<TextMesh>();
	    _down.transform.GetComponent<Renderer>().enabled = false;
	}
	
	 void Update()
	{
	    if(_enabled){
		    timeleft -= Time.deltaTime;
		    accum += Time.timeScale/Time.deltaTime;
		    ++frames;
		    
		    // Interval ended - update GUI text and start new interval
		    if( timeleft <= 0.0f )
		    {
		        // display two fractional digits (f2 format)
		      	_textMesh.text = "FPS " + (accum/frames).ToString("f2");
		        timeleft = updateInterval;
		        accum = 0.0f;
		        frames = 0;
		        
		    }
	    }
	    _down.text = "" +PB_Controller.instance._b1b2DownCounter;
	    if(!_enabled && PB_Controller.instance._b1b2DownCounter > 5){
	    	
	    	this._enabled = true;
	    	_textMesh.transform.GetComponent<Renderer>().enabled = true;
	    	_down.transform.GetComponent<Renderer>().enabled = true;
	    }
	    
	}
}