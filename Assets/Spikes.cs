using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
	public float damageMulti = 2f;
	[HideInInspector] public float length;
	[HideInInspector] public Spiker spiker;
	private Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		length = 1;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3 (1f, length, 1f);
		transform.position = new Vector3 (startPos.x, startPos.y, startPos.z - length / 2);
	}

	public void SetLength(float newLen) {
		if (newLen > length) {
			length = newLen;
		}
	}
		
	public void TakeDamage(int dmg) {
		length -= damageMulti * (float)dmg;
	}

	public void OnDestroy() {
		if (spiker != null) {
			spiker.SpikeDestroyed ();
		}
	}
}
