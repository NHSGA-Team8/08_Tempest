using UnityEngine;
using System.Collections;

/*
 * Created by Rachael H.
 */
public class FlipperBullet : MonoBehaviour
{
	private GameObject ship;
	// Use this for initialization
	void Start ()
	{
		Destroy (gameObject, 5.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (transform.position.z == 0) {
			Destroy (gameObject);
		}
	}

	public void SetShip(GameObject newShip) {
		ship = newShip;
	}

	void OnDestroy ()
	{
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.GetComponent<PlayerBullet> () != null) {
			Destroy (gameObject);
			//Destroy (other.gameObject);
		}
		if (other.gameObject.GetComponent<PlayerShip> () != null) {
			Destroy (gameObject);
			other.gameObject.GetComponent<PlayerShip> ().TakeDamage (1);
		}
	}
}

