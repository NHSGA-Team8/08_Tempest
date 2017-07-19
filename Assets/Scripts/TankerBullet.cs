using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankerBullet : MonoBehaviour {

	public GameObject explodeParticle;
	private GameObject ship;

	void Start() {
		Destroy (gameObject, 1.5f);
	}

	public void SetShip(GameObject newShip) {
		ship = newShip;
	}

	void OnDestroy() {
		GameObject newExplosion = Instantiate (explodeParticle, gameObject.transform.position, gameObject.transform.rotation);
		Destroy (newExplosion, 1f);
		//ship.GetComponent<PlayerShip>().BulletDestroyed ();
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<IShipBase> () != null && col.gameObject != ship) {
			col.gameObject.GetComponent<IShipBase> ().TakeDamage (1);
			Destroy (gameObject);
		}
		if (col.gameObject.GetComponent<PlayerBullet> () != null) {
			Destroy (col.gameObject);
			Destroy (gameObject);
		}
		
	}
}
