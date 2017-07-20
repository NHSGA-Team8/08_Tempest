using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiker : MonoBehaviour, IShipBase {

	public float maxZ = 23f;
	public float minZ = 3f;
	public float moveSpeed = 0.1f;

	public GameObject explodePrefab;
	public GameObject spikePrefab;
	public AudioClip soundDeath;

	[HideInInspector] public bool forward = true;

	private MapManager _mapManager;
	private GameManager _gameManager;
	private float _lastFire;
	private Rigidbody _rigidbody;
	private AudioSource _audioSource;
	private GameObject _playerRef;
	private Spikes spike;

	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody> ();
		_mapManager = GameObject.Find ("MapManager").GetComponent<MapManager> ();
		_gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		_audioSource = GetComponent<AudioSource> ();
		CreateSpike ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void FixedUpdate () {
		Move ();
		float len = _mapManager.depth - transform.position.z;
		//print (len);
		spike.SetLength (len);
		//spike.SetNewPos (transform.position);
	}

	void Move() {
		Vector3 newPos = transform.position;
		if (forward == true) {
			newPos = newPos - new Vector3 (0f, 0f, moveSpeed * Time.deltaTime);
		} else {
			newPos = newPos + new Vector3 (0f, 0f, moveSpeed * Time.deltaTime);
		}

		//print (newPos.z - moveSpeed * Time.deltaTime * 1f);
		_rigidbody.MovePosition (newPos);

		float newZ = transform.position.z;

		if (newZ <= minZ) {
			forward = false;
		} else if (newZ >= maxZ) {
			forward = true;
		}
	}

	// Called to fire a projectile.
	public void Fire() {

	}

	// Called when a projectile damages the ship. Should call OnDeath() if it kills;
	public void TakeDamage(int dmg) {
		OnDeath ();
	}

	// Called when the ship dies. Add points, do game state detection, etc.
	public void OnDeath() {

		// TODO spawn two flippers
		_gameManager.SpikerDestroyed();
		

		GameObject newExplosion = Instantiate (explodePrefab, gameObject.transform.position, gameObject.transform.rotation);
		AudioSource explosionSource = newExplosion.GetComponent<AudioSource> ();
		explosionSource.clip = soundDeath;
		explosionSource.Play ();

		Destroy (gameObject);
	}

	public void SpikeDestroyed() {
		CreateSpike ();
	}

	void CreateSpike() {
		spike = Instantiate (spikePrefab, transform.position, spikePrefab.transform.rotation).GetComponent<Spikes>();
		spike.spiker = this;
	}
		
}
