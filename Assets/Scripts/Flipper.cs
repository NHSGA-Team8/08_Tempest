using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Created by Rachael H. on 16 July 2017
 */
public class Flipper : MonoBehaviour, IShipBase
{
	//Public
	public float movementForce;
	public float shellSpeed;
	public float reloadTime;
	public int levelNum;
	public GameObject flipperShell;
	//public Rigidbody flipperShell;
	public GameObject player;
	public GameObject flipperEnemy;
	public float respawnTime;
	public MapLine thisMapLine;
	public string inputAxis = "Horizontal";
	public GameObject explodePrefab;
	public float switchTime;

	//Private
	private float _currentHealth;
	private bool _straightMovement; //True if moving in only one lane for level one
	private bool _reloaded;
	private MapManager _mapManager; //How do I use the same _mapManager as that of the player ship if it's private?
	private GameManager _gameManager;
	private int _rand; //private float _rand;
	private Vector3 _vertex1;
	private Vector3 _vertex2;
	private Vector3 _lineCenter;
	private float _mapDepth;
	private float _inputValue;
	private int _isCW = 0; //isClockWise: 1 = CW
	private int _currPlayerNum;
	private AudioSource _audioSource;
	private Quaternion _desiredRotation;

	private bool reachedEnd = false;
	private bool hasFinishedMoving = true;
	private bool _finishedSwitch = true;

	Rigidbody rb;

	//Audio
	public AudioClip soundFire;
	public AudioClip soundDeath;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		_reloaded = true;
		//respawnTime = 0.2f;
		//_rand = Random.value * _mapManager.mapVertices.Length;
		//_rand = RandomVal ();
		//print(Console.WriteLine(MapManager.mapVertices[1]));
		_audioSource = GetComponent<AudioSource> ();
		_mapManager = GameObject.Find("MapManager").GetComponent<MapManager> ();
		_gameManager = GameObject.Find("GameManager").GetComponent<GameManager> ();
		levelNum = _gameManager.currentRound;
		if (levelNum == 1)
		{
			_straightMovement = true;
		}
		else
		{
			_straightMovement = false;
		}
		//_straightMovement = true;
		Vector3 curDirVec = thisMapLine.GetDirectionVector ();
		Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
		transform.rotation = Quaternion.LookRotation(new Vector3(0f,0f,1f), newDirVec);
		//transform.Rotate(newDirVec);
		StartCoroutine (FirePeriodically ());
		/*
		if (Random.value > 0.5)
			_isCW = 1;
		else
			_isCW = -1;
		*/
	}

	// Update is called once per frame
	//Fi
	void Update ()
	{
		if (transform.position.z <= 0)
		{
			rb.constraints = RigidbodyConstraints.FreezePositionZ;
			reachedEnd = true;
		}

		if (reachedEnd && hasFinishedMoving) //In case the player ship is flying in after respawning?
		{
			StartCoroutine (RotateAroundEdge ());
		}
		else if (_straightMovement)
		{
			//Only move in Z direction, aka depth
			//rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
			//rb.AddForce (-1 * movementForce * transform.forward * Time.deltaTime);

			//rb.MovePosition (transform.position + transform.forward * (Time.deltaTime * movementForce * -1));
			transform.position = transform.position + transform.forward * (Time.deltaTime * movementForce * -1);
		}
		else //Switching lanes
		{
			/*
			Vector3 _newPos;
			MapLine _newMapLine;
			MapLine _nextMapLine = thisMapLine.leftLine;
			Vector3 _newPosZ = transform.position + transform.forward * (Time.deltaTime * movementForce * -1);
			//Move forward by one or a few pixels
			thisMapLine.UpdateMovement (transform.position, Time.deltaTime * 1 * movementForce * 0.2f, out _newPos, out _newMapLine);
			//transform.position = new Vector3(_newPos.x, _newPos.y, _newPosZ.z);
			transform.position = new Vector3(_nextMapLine.GetMidPoint().x, _nextMapLine.GetMidPoint ().y, _newPosZ.z);
			//While moving to next section of map
			Vector3 curDirVec = _nextMapLine.GetDirectionVector ();
			Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, transform.position.z);
			transform.rotation = Quaternion.LookRotation(new Vector3(0f,0f,1f), newDirVec);
			if (_nextMapLine != null) {
				thisMapLine = _nextMapLine;
			}
			*/
			if (!reachedEnd) {
				Vector3 _newPosZ = transform.position + transform.forward * (Time.deltaTime * movementForce * -1);
				transform.position = new Vector3 (transform.position.x, transform.position.y, _newPosZ.z);
				if (_finishedSwitch) {
					//StartCoroutine (SwitchLanes (transform.position.z));
					//StartCoroutine (SwitchLanes (_newPosZ.z));
					StartCoroutine (SwitchLanes ());
				}
			}
		}
	}

	private IEnumerator RotateAroundEdge ()
	{
		hasFinishedMoving = false;
		yield return new WaitForSeconds (switchTime);
		Vector3 _newPos;
		MapLine _newMapLine, _nextMapLine;
		//transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
		//rb.MovePosition (new Vector3 (transform.position.x, transform.position.y, 0));
		//rb.constraints = RigidbodyConstraints.FreezePositionZ;
		if (GameObject.Find ("Player") != null) {
			_currPlayerNum = GameObject.Find ("Player").GetComponent<PlayerShip> ().curMapLine.GetLineNum ();
			if (_isCW == 0) {
				int _beCW = _currPlayerNum - thisMapLine.GetLineNum ();
				int _beCCW = _mapManager.mapLines.Length - _currPlayerNum + thisMapLine.GetLineNum ();
				if (_beCW > _beCCW) {
					_isCW = 1;
				} else if (_beCW < _beCCW) {
					_isCW = -1;
				} else { //Equal distance from player
					if (Random.value > 0.5) {
						_isCW = 1;
					} else {
						_isCW = -1;
					}
				}
			}
			//Move (_isCW);
			/*
				thisMapLine.UpdateMovement (transform.position, Time.deltaTime * _isCW * movementForce * 0.2f, out _newPos, out _newMapLine);
				if (_newMapLine != null) {
					thisMapLine = _newMapLine;
				}
				*/
			if (thisMapLine == GameObject.Find ("Player").GetComponent<PlayerShip> ().curMapLine) {
				_nextMapLine = thisMapLine;
			} else if (_isCW == 1) {
				_nextMapLine = thisMapLine.leftLine;
				Debug.Log ("Left");
			} else {
				_nextMapLine = thisMapLine.rightLine;
				Debug.Log ("Right");
			}
			if (_nextMapLine != null) {
				thisMapLine = _nextMapLine;
			}
			_newPos = _nextMapLine.GetMidPoint();
			Debug.Log ("Current Position: "+transform.position);
			Debug.Log ("New Position: "+_newPos);
			transform.position = new Vector3 (_newPos.x, _newPos.y, 0);
			//rb.MovePosition (new Vector3 (_newPos.x, _newPos.y, 0));
			Vector3 curDirVec = _nextMapLine.GetDirectionVector ();
			Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
			//print (Quaternion.Euler(newDirVec));
			transform.rotation = Quaternion.LookRotation (new Vector3 (0f, 0f, 1f), newDirVec);
			//rb.MoveRotation (Quaternion.LookRotation (new Vector3 (0f, 0f, 1f), newDirVec));
			hasFinishedMoving = true;
		}
	}
	private IEnumerator SwitchLanes ()
	//private IEnumerator SwitchLanes (float z)
	{
		_finishedSwitch = false;
		yield return new WaitForSeconds (switchTime);
		Vector3 _newPos;
		MapLine _newMapLine, _nextMapLine;
		_nextMapLine = thisMapLine.leftLine;
		if (_nextMapLine != null) {
			thisMapLine = _nextMapLine;
		}
		_newPos = _nextMapLine.GetMidPoint();
		//transform.position = new Vector3 (_newPos.x, _newPos.y, z);
		transform.position = new Vector3 (_newPos.x, _newPos.y, transform.position.z);
		Vector3 curDirVec = _nextMapLine.GetDirectionVector ();
		Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
		transform.rotation = Quaternion.LookRotation (new Vector3 (0f, 0f, 1f), newDirVec);
		_finishedSwitch = true;
	}
	void Move(int _dir){
		/*
		Vector3 newPos;
		MapLine newMapLine;
			thisMapLine.UpdateMovement (transform.position, Time.deltaTime * dir * movementForce, out newPos, out newMapLine);
			//rb.MovePosition (newPos);
			rb.MovePosition (new Vector3(newPos.x, newPos.y, 0));
			if (newMapLine != null)
			{
				thisMapLine = newMapLine;
			}
			*/
		bool legacyMovement = true;
		/*
		if (legacyMovement == true) {
			Vector3 newPos;
			MapLine newMapLine;
			Quaternion newQuat;

			thisMapLine.UpdateMovement (transform.position, Time.deltaTime * _dir * movementForce, out newPos, out newMapLine);

			if (movingForward == true) {
				newPos += new Vector3 (0f, 0f, transform.position.z + moveSpeed * 0.02f);
			}

			_rigidbody.MovePosition (newPos);

			if (newMapLine != null) {
				curMapLine = newMapLine;
			}
		} else {

			Vector3 newPos = _nextMapLine.GetMidPoint();
			if (movingForward == true) {
				newPos = newPos + new Vector3 (0f, 0f, transform.position.z + moveSpeed * 0.02f);
			}

			_rigidbody.MovePosition (newPos);

			Vector3 curDirVec = _nextMapLine.GetDirectionVector ();
			Vector3 newDirVec = new Vector3 (-curDirVec.y, curDirVec.x, 0);
			//print (Quaternion.Euler(newDirVec));
			_rigidbody.MoveRotation (Quaternion.LookRotation(new Vector3(0f,0f,1f), newDirVec));

			curMapLine = _nextMapLine;
		}
		*/
	}
	// Called to fire a projectile.
	public void Fire()
	{
		//if (GameObject.Find("PlayerShip") != null)
			//GameObject newFlipperShell = Instantiate (flipperShell, transform.position, rb.rotation);
		GameObject newFlipperShell = Instantiate (flipperShell, transform.position, transform.rotation);
			//Rigidbody newFlipperShell = Instantiate (flipperShell, transform.position, transform.rotation) as Rigidbody;
			//newFlipperShell.GetComponent<Rigidbody> ().AddForce (shellSpeed * transform.forward * Time.deltaTime);
			//newFlipperShell.GetComponent<Rigidbody> ().MovePosition (newFlipperShell.transform.position + shellSpeed * transform.forward * Time.deltaTime);
			//newFlipperShell.velocity = 20f * transform.forward; //Directly changing velocity
			newFlipperShell.GetComponent<Rigidbody> ().velocity = -1 * 10f * transform.forward; //Directly changing velocity
	}

	private IEnumerator FirePeriodically()
	{
		yield return new WaitForSeconds (reloadTime);
		if (GameObject.Find("PlayerShip") != null)
			Fire ();
	}

	// Called when a projectile damages the ship. Should call OnDeath() if it kills;
	public void TakeDamage(int dmg)
	{
		OnDeath ();
	}

	// Called when the ship dies. Add points, do game state detection, etc.
	public void OnDeath()
	{
		GameObject newExplosion = Instantiate (explodePrefab, gameObject.transform.position, gameObject.transform.rotation);
		AudioSource explosionSource = newExplosion.GetComponent<AudioSource> ();
		explosionSource.clip = soundDeath;
		explosionSource.Play ();
		_gameManager.FlipperDestroyed ();
		Destroy (gameObject);
		//gameObject.SetActive (false); // Disable enemy
	}

	//void OnCollisionEnter(Collision collider) {
	void OnCollisionEnter(Collision other)
	{
		/*
		if (collider.gameObject.GetComponent<PlayerShip> ()) {
			collider.gameObject.GetComponent<PlayerShip> ().TakeDamage (1);
			OnDeath ();
		}
		*/
		if (other.gameObject.GetComponent<PlayerShip> ()) {
			other.gameObject.GetComponent<PlayerShip> ().TakeDamage (1);
			OnDeath ();
		}
		//if (other.GetType() == typeof(PlayerShip))
		/*
		if (other.gameObject.tag == "Player") 
		{
			other.gameObject.GetComponent<PlayerShip> ().TakeDamage (1);
			OnDeath ();
		}
		*/
	}

	public bool GetStraightMovement()
	{
		return _straightMovement;
	}
	public void SetStraightMovement(bool isStraight)
	{
		_straightMovement = isStraight;
	}

	public void SetMapLine(MapLine newML) {
		thisMapLine = newML;
	}
}
