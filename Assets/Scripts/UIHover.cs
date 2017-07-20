using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHover : MonoBehaviour {

	public float minY;
	public float maxY;

	public float value;

	private bool moveAdd = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (moveAdd == true) {
			transform.position = new Vector3 (transform.position.x, transform.position.y + value, transform.position.z);
		} else {
			transform.position = new Vector3 (transform.position.x, transform.position.y - value, transform.position.z);
		}

		if (transform.position.y >= maxY)
			moveAdd = false;
		else if (transform.position.y <= minY)
			moveAdd = true;
	}
}
