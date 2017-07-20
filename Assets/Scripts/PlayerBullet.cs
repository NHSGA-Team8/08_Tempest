using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {

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
		ship.GetComponent<PlayerShip>().BulletDestroyed ();
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<IShipBase> () != null && col.tag == "Enemy")
			col.gameObject.GetComponent<IShipBase> ().TakeDamage (1);
		if (col.gameObject.GetComponent<TankerBullet> () != null)
			Destroy (col.gameObject);
		if (col.gameObject.GetComponent<Spikes> () != null)
			col.gameObject.GetComponent<Spikes>().TakeDamage (1);
		Destroy (gameObject);
	}

}
