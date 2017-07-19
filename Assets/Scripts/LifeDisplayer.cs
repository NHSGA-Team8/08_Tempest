using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeDisplayer : MonoBehaviour {
	public int lifeAmt;
	private Image _selfImage;
	// Use this for initialization
	void Start () {
		_selfImage = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalVariables.lives >= lifeAmt) {
			_selfImage.enabled = true;
		} else {
			_selfImage.enabled = false;
		}
	}
}
