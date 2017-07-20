using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper_New : MonoBehaviour, IShipBase {
	
	[Header("References")]
	public GameObject explodePrefab;
	public Rigidbody projectilePrefab;
	public Transform fireTransform;

	[Header("Variables")]
	public float moveSpeed = 1f;
	public float fireSpeed = 10f;
	public float flipCooldown = 1f;

	[Header("Audio")]
	public AudioClip soundFire;
	public AudioClip soundDeath;

	[HideInInspector] public MapLine curMapLine;

	// Private variables 

	private MapManager _mapManager;
	private GameManager _gameManager;
	private int _curBullets;
	private float _lastFire;
	private Rigidbody _rigidbody;
	private AudioSource _audioSource;
	private MapLine _targetMapLine;
	private MapLine _nextMapLine;
	private PlayerShip _playerScript;
	private float _nextMove;

	// Use this for initialization
	void Start () {
		_curBullets = 0;
		_rigidbody = GetComponent<Rigidbody> ();
		_mapManager = GameObject.Find ("MapManager").GetComponent<MapManager> ();
		_gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		_audioSource = GetComponent<AudioSource> ();
		_playerScript = GameObject.Find ("Player").GetComponent<PlayerShip>();
		_nextMove = Time.fixedTime;
		_targetMapLine = curMapLine;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		// Determine where we aim to go
		_playerScript = GameObject.Find ("Player").GetComponent<PlayerShip>();
		if (_playerScript != null)
			_targetMapLine = _playerScript.curMapLine;

		// Find shortest path and next step (_nextMapLine)
		int rightDist = curMapLine.getShortestDist (curMapLine.leftLine, _targetMapLine);
		int leftDist = curMapLine.getShortestDist (curMapLine.rightLine, _targetMapLine);
		if (leftDist < rightDist) {
			_nextMapLine = curMapLine.leftLine;
		} else if (leftDist >= rightDist && leftDist > 0) {
			_nextMapLine = curMapLine.rightLine;
		} else {
			_nextMapLine = curMapLine;
		}

		Move (_nextMove <= Time.fixedTime);
	}

	void Move(bool flip) {
		Vector3 newPos = curMapLine.GetMidPoint();
		if (flip == true) {
			newPos = _nextMapLine.GetMidPoint ();
		}

		if (transform.position.z > 0) {
			newPos = newPos + new Vector3 (0f, 0f, transform.position.z - moveSpeed * 0.02f);
		} else {
			newPos = new Vector3 (newPos.x, newPos.y, 0);
		}

		_rigidbody.MovePosition (newPos);

		if (flip == true) {
			Vector3 curDirVec = _nextMapLine.GetDirectionVector ();
			Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
			_rigidbody.MoveRotation (Quaternion.LookRotation(new Vector3(0f,0f,1f), newDirVec));

			curMapLine = _nextMapLine;

			if (transform.position.z > 0)
				_nextMove = Time.fixedTime + flipCooldown;
			else
				_nextMove = Time.fixedTime + flipCooldown / 2;
		}
	}

	public void Fire(){
		_audioSource.clip = soundFire;
		_audioSource.Play ();
		Rigidbody shellInstance = Instantiate (projectilePrefab, fireTransform.position, fireTransform.rotation) as Rigidbody;
		//shellInstance.GetComponent<PlayerBullet> ().SetShip (gameObject);
		shellInstance.velocity = -fireSpeed * (fireTransform.forward); 
		shellInstance.GetComponent<FlipperBullet> ().SetShip (gameObject);
	}

	public void TakeDamage(int dmg) { 
		OnDeath ();
	}

	public void OnDeath() {
		_gameManager.FlipperDestroyed();

		GameObject newExplosion = Instantiate (explodePrefab, gameObject.transform.position, gameObject.transform.rotation);
		AudioSource explosionSource = newExplosion.GetComponent<AudioSource> ();
		explosionSource.clip = soundDeath;
		explosionSource.Play ();

		Destroy (gameObject);
	}
}
