using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {
	public float damageMulti = 0.1f;
	public float lengthMulti;
	[HideInInspector] public float length;
	[HideInInspector] public Spiker spiker;
	private Vector3 startPos;

	// Use this for initialization
	void Start () {
		startPos = transform.position;
		length = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		//print ("scale:" + lengthMulti);
		transform.localScale = new Vector3 (10f, length * lengthMulti, 10f);
		transform.position = new Vector3 (startPos.x, startPos.y, startPos.z - transform.localScale.z / (length * lengthMulti));
	}

	public void SetNewPos(Vector3 newPos) {
		float distance = Vector3.Distance (newPos, startPos);
		SetLength (distance);
	}

	public void SetLength(float newLen) {
		if (newLen > length) {
			length = newLen;
		}
	}
		
	public void TakeDamage(int dmg) {
		length -= damageMulti * (float)dmg;
		GlobalVariables.score += dmg;
		if (length <= 0)
			OnDeath ();
	}

	public void OnDeath() {
		if (spiker != null) {
			spiker.SpikeDestroyed ();
		}
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<IShipBase> () != null && col.tag != "Enemy") {
			col.gameObject.GetComponent<IShipBase> ().TakeDamage (1);
		}
	}
}
