using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

	private GameObject ship;

	void Start() {
		Destroy (gameObject, 2f);
	}

	public void SetShip(GameObject newShip) {
		ship = newShip;
	}

	void OnDestroy() {
		ship.GetComponent<PlayerShip>().BulletDestroyed ();
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.GetComponent<Flipper> ())
			collision.gameObject.GetComponent<Flipper> ().TakeDamage (1);
		Destroy (gameObject);
	}

}
