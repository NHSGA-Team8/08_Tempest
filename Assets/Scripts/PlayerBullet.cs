using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

	private GameObject ship;

	void Start() {
		Destroy (gameObject, 1.5f);
	}

	public void SetShip(GameObject newShip) {
		ship = newShip;
	}

	void OnDestroy() {
		ship.GetComponent<PlayerShip>().BulletDestroyed ();
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<IShipBase> () != null)
			col.gameObject.GetComponent<IShipBase> ().TakeDamage (1);
		Destroy (gameObject);
	}

}
