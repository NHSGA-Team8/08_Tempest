using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public float distance;
	public GameObject playerRef;

	private Camera _cam;

	// Use this for initialization
	void Start () {
		_cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transform.position.x, transform.position.y, playerRef.transform.position.z + distance);
	}
}
