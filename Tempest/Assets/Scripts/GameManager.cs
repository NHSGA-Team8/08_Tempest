using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Ethan Zhu and Rachael H.
 */
public class GameManager : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject flipperPrefab;
	public GameObject spawnEffect;
	public int totalFlippers;
	public float spawnDelay;
	public float roundTotalTime;
	public int currentRound;
	public int nextScene;
	public int totalLives;
	public Camera camera;
	public Image flash;

	public Canvas uiCanvas;
	public Text notification;

	public AudioClip ac_portalEnter;
	public AudioClip ac_portalDuring;

	public GameObject flipperShell; //Enemy Projectile

	public enum GAMESTATE {PREGAME, STARTING, PLAYING, ENDING};
	public GAMESTATE curGamestate = GAMESTATE.PREGAME;

	[HideInInspector] public Flipper[] flippers;
	[HideInInspector] public int curLives;

	private float _lastSpawn;
	private int _flipperCount;
	private float _startTime;
	private GameObject _playerRef;
	private MapManager _mapManager;
	private AudioSource _audioSource;

	// Use this for initialization
	void Start () {
		curLives = totalLives;
		flippers = new Flipper[totalFlippers];
		_mapManager = GameObject.Find ("MapManager").GetComponent<MapManager> ();
		_playerRef = GameObject.Find ("Player");
		_audioSource = camera.GetComponent<AudioSource> ();
		StartCoroutine (GameLoop ());
		_flipperCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator GameLoop()
	{
		curGamestate = GAMESTATE.STARTING;
		yield return StartCoroutine(RoundStarting());
		curGamestate = GAMESTATE.PLAYING;
		yield return StartCoroutine(RoundPlaying());
		curGamestate = GAMESTATE.ENDING;
		yield return StartCoroutine(RoundEnding());

		if (curLives < 0)
		{
			// Back to menu if dead
			SceneManager.LoadScene(0);
		}
		else
		{
			SceneManager.LoadScene(0);
		}
	}

	private IEnumerator RoundStarting() {
		SpawnPlayerShip ();
		StartCoroutine (SpawnEnemyShips());

		yield return new WaitForSeconds(3);
		_startTime = Time.fixedTime;
	}

	private IEnumerator RoundPlaying() {

		while (curLives >= 0 && EnemiesAtEdge() == false && _flipperCount < totalFlippers)
			yield return null;
	}

	private IEnumerator RoundEnding() {
		//print ("RoundEnding");
		SetEndMessage();
		if (curLives >= 0) {
			_playerRef.GetComponent<PlayerShip> ().movingForward = true;
			StartCoroutine (FlashScreen (3,0.1f));
		}
		yield return new WaitForSeconds(5);

	}

	void SpawnPlayerShip() {
		if (_playerRef == null) {
			_playerRef = Instantiate (playerPrefab, _mapManager.mapLines [_mapManager.startMapLineIndex].GetMidPoint (), Quaternion.Euler (0f, 0f, 0f));
		} else {
			_playerRef.transform.position = _mapManager.mapLines [_mapManager.startMapLineIndex].GetMidPoint ();
			_playerRef.SetActive (true);
		}
		GameObject spawnSparkles = Instantiate (spawnEffect, _playerRef.transform);
		Destroy (spawnSparkles, 5f);
	}

	public void OnPlayerDeath()
	{
		StartCoroutine (PlayerDied ());
	}

	public IEnumerator PlayerDied() {
		curLives--;
		yield return new WaitForSeconds(2);
		SpawnPlayerShip ();

	}

	private IEnumerator SpawnEnemyShips ()
	{
		for (int i = 0; i < totalFlippers; i++)
		{
			_flipperCount++;
			CreateNew ();
			yield return new WaitForSeconds (spawnDelay);
		}
	}
	//Random spawn point
	public int RandomVal()
	{
		return (int)(Random.value * (_mapManager.mapLines.Length - 1));
	}
	//Spawns new flipper enemy on field, associated with map line
	public void CreateNew()
	{
		//print ("CreateNew");
		//float _rand1;
		int _rand1;
		bool _straightMovement1;
		MapLine thisMapLine;
		Vector3 _vertex1, _vertex2, _lineCenter;
		float _mapDepth;
		if (currentRound == 1)
		{
			_straightMovement1 = true;
		}
		else
		{
			_straightMovement1 = false;
		}
		_rand1 = RandomVal ();
		thisMapLine = _mapManager.mapLines [_rand1];
		_mapDepth = _mapManager.depth;
		GameObject newFlipper = Instantiate (flipperPrefab, thisMapLine.GetMidPoint() + new Vector3 (0, 0, 1 * _mapDepth), flipperPrefab.transform.rotation);
		newFlipper.GetComponent<Flipper>().SetMapLine (thisMapLine);
	}

	bool EnemiesAtEdge() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			if (enemy.transform.position.z > 0.1f)
				return false;
		}
		return true;
	}

	void SetEndMessage() {
		string msg = "";

		if (curLives < 0)
			msg = "Game Over";
		else
			msg = "Round Complete";

		notification.text = msg;
	}

	private IEnumerator FlashScreen (int duration, float speed)
	{
		for (int i = 0; i < 255; i++)
		{
			flash.color = new Color (255, 255, 255, i);
			yield return new WaitForSeconds (speed);
		}
		yield return new WaitForSeconds (duration);
		for (int i = 255; i >= 0; i--)
		{
			flash.color = new Color (255, 255, 255, i);
			yield return new WaitForSeconds (speed);
		}
	}
}
