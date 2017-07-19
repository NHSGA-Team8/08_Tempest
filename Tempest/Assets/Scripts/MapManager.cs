using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Ethan Zhu and Rachael H.
 */
public class MapManager : MonoBehaviour {

	// Vertices of a map, from left to right
	// The Z axis does not matter; all ships / enemies have a set value of that
	public Vector3[] mapVertices; 

	// Whether the map loops on the two edges.
	// This should be true for complete shapes (like a O shape) and false for incomplete shapes (like a U shape)
	public bool isLoop;

	// MapLines are generated at Start(), and they represent a line in the map.
	[HideInInspector] public MapLine[] mapLines;

	public int startMapLineIndex;

	public float depth;

	// Use this for initialization
	void Start () {
		mapLines = new MapLine[mapVertices.Length - 1];
		for (int i = 0; i < mapLines.Length; i++) {
			mapLines [i] = new MapLine (mapVertices[i], mapVertices[i+1]);
			mapLines [i].SetLineNum (i);
		}

		for (int i = 0; i < mapLines.Length; i++) {
			if (i>0) 
				mapLines [i].SetLeftMapLine (mapLines [i - 1]);
			if (i<mapLines.Length - 1)
				mapLines [i].SetRightMapLine (mapLines [i + 1]);
		}

		if (isLoop == true) {
			mapLines [0].SetLeftMapLine (mapLines [mapLines.Length - 1]);
			mapLines [mapLines.Length - 1].SetRightMapLine (mapLines [0]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < mapLines.Length - 1; i++) {
			Debug.DrawLine (mapVertices [i], mapVertices [i + 1]);
		}
	}

	// return the normal vector of the line and the pos
	public Quaternion GetVerticeNormal(Vector3 pos){

		Quaternion finalQuat = Quaternion.Euler(0f,0f,0f);

		// TODO Determine which edge the pos is on

		// TODO If not on edge, return null

		// TODO Calculate the normal on the line

		// Return the Quaternion
		return finalQuat;

	}
		
}
