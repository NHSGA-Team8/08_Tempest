using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplayer : MonoBehaviour {
	public int lifeAmt;
	private GameManager _gameManager;
	private Image _selfImage;
	// Use this for initialization
	void Start () {
		_gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		_selfImage = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (_gameManager.curLives > lifeAmt) {
			_selfImage.enabled = true;
		} else {
			_selfImage.enabled = false;
		}
	}
}
