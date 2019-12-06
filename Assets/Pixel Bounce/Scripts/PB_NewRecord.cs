// PB_NewRecord - Sets highscore text in the new record screen

using UnityEngine;
using System;


public class PB_NewRecord:MonoBehaviour{
	public GameObject _highScoreText;
	public GameObject _highScoreTextShadow;
	
	public void Start() {
		gameObject.SetActive(false);
	}
	
	public void NewRecord() {
		gameObject.SetActive(true);
		_highScoreText.GetComponent<TextMesh>().text = _highScoreTextShadow.GetComponent<TextMesh>().text = PB_GameController.instance._highScore.ToString("0000");
	}
}
