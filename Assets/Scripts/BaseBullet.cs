using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour {

	private GameObject ship;

	void Start() {
		Destroy (gameObject, 2f);
	}

	void Explode() {

	}

	public void SetShip(GameObject newShip) {
		ship = newShip;
	}

	void OnDestroy() {
		ship.GetComponent<PlayerShip>().BulletDestroyed ();
	}
}
