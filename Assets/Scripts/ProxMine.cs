using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxMine : MonoBehaviour {

	public int hits;
	public GameObject explodeParticle;
	public float targetZ = 15f;
	private GameObject ship;

	void Start() {
	}

	void FixedUpdate() {
		if (transform.position.z > targetZ) {
			OnDeath ();
			GetComponent<Rigidbody> ().velocity = Vector3.zero;
			GetComponent<Rigidbody> ().MovePosition (new Vector3 (transform.position.x, transform.position.y, targetZ));
		}
	}

	public void SetShip(GameObject newShip) {
		ship = newShip;
	}

	public void TakeDamage(int dmg) {
		hits -= dmg;
		if (dmg <= 0)
			OnDeath ();
	}

	void OnDeath() {
		GameObject newExplosion = Instantiate (explodeParticle, gameObject.transform.position, gameObject.transform.rotation);
		Destroy (newExplosion, 1f);
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<IShipBase> () != null && col.tag == "Enemy") {
			col.gameObject.GetComponent<IShipBase> ().TakeDamage (1);
			TakeDamage (1);
		}
		if (col.gameObject.GetComponent<TankerBullet> () != null)
			Destroy (col.gameObject);
		if (col.gameObject.GetComponent<FlipperBullet> () != null)
			Destroy (col.gameObject);
		//Destroy (gameObject);
	}
}
