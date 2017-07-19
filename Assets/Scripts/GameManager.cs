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
	public GameObject tankerPrefab;
	public GameObject spawnEffect;
	public int totalFlippers;
	public int totalTankers;
	public float flipperSpawnDelay;
	public float tankerSpawnDelay;
	public float roundTotalTime;
	public int currentRound;
	public int nextScene;
	public int totalLives;
	public Camera cam;

	public float speedMulti = 5f;

	public Canvas uiCanvas;
	public Text notification;
	public Image flash;

	public AudioClip ac_portalEnter;
	public AudioClip ac_portalDuring;

	public GameObject flipperShell; //Enemy Projectile

	public enum GAMESTATE {PREGAME, STARTING, PLAYING, ENDING};
	public GAMESTATE curGamestate = GAMESTATE.PREGAME;

	[HideInInspector] public Flipper[] flippers;
	[HideInInspector] public int curLives;

	private float _lastSpawn;
	private int _flipperCount;
	private int _tankerCount;
	private float _startTime;
	private GameObject _playerRef;
	private MapManager _mapManager;
	private AudioSource _audioSource;

	private int _remainingEnemies;

	// Use this for initialization
	void Start () {
		curLives = totalLives;
		flippers = new Flipper[totalFlippers];
		_mapManager = GameObject.Find ("MapManager").GetComponent<MapManager> ();
		_playerRef = GameObject.Find ("Player");
		_audioSource = cam.GetComponent<AudioSource> ();
		StartCoroutine (GameLoop ());
		_flipperCount = 0;
		_tankerCount = 0;
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
			//SceneManager.LoadScene(0);
		}
		else
		{
			// Next level if win
		}
	}

	private IEnumerator RoundStarting() {
		SpawnPlayerShip ();
		SpawnEnemyShips();

		yield return new WaitForSeconds(3);
		_startTime = Time.fixedTime;
	}

	private IEnumerator RoundPlaying() {

		while (curLives >= 0 && !(EnemiesAtEdge() == true || _remainingEnemies <= 0))
			yield return null;
	}

	private IEnumerator RoundEnding() {
		//print ("RoundEnding");
		SetEndMessage();
		if (curLives >= 0) {
			_playerRef.GetComponent<PlayerShip> ().movingForward = true;
			StartCoroutine(FlashScreen (1f, 1f));
		}
		yield return new WaitForSeconds(2);
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

	private void SpawnEnemyShips ()
	{
		_remainingEnemies = totalFlippers + totalTankers; // Add new enemies to this
		StartCoroutine(SpawnFlippers ());
		StartCoroutine(SpawnTankers ());
	}

	private IEnumerator SpawnFlippers ()
	{
		for (int i = 0; i < totalTankers; i++)
		{
			_flipperCount++;
			SpawnFlipper ();
			yield return new WaitForSeconds (flipperSpawnDelay);
		}
	}

	private IEnumerator SpawnTankers ()
	{
		for (int i = 0; i < totalTankers; i++)
		{
			_tankerCount++;
			SpawnTanker();
			yield return new WaitForSeconds (tankerSpawnDelay);
		}
	}

	//Random spawn point
	public int RandomVal()
	{
		return (int)(Random.value * (_mapManager.mapLines.Length - 1));
	}
	//Spawns new flipper enemy on field, associated with map line

	public void SpawnFlipper()
	{
		MapLine newMapLine = _mapManager.mapLines [RandomVal ()];
		float _mapDepth = _mapManager.depth;
		GameObject newShip = Instantiate (flipperPrefab, newMapLine.GetMidPoint() + new Vector3 (0, 0, 1 * _mapDepth), flipperPrefab.transform.rotation);
		newShip.GetComponent<Flipper>().SetMapLine (newMapLine);
		newShip.GetComponent<Flipper>().movementForce = currentRound * speedMulti;
	}

	public void SpawnTanker()
	{
		MapLine newMapLine = _mapManager.mapLines [RandomVal ()];
		float _mapDepth = _mapManager.depth;
		GameObject newShip = Instantiate (tankerPrefab, newMapLine.GetMidPoint() + new Vector3 (0, 0, 1 * _mapDepth), flipperPrefab.transform.rotation);
		newShip.GetComponent<Tanker> ().curMapLine = newMapLine;
		newShip.GetComponent<Tanker>().moveSpeed *= currentRound * speedMulti;
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

	public IEnumerator FlashScreen(float wait, float duration) {
		yield return new WaitForSeconds (wait);
		for (int i = 0; i <= 255; i++) {
			flash.color = new Color (1f, 1f, 1f, ((float)i / 255f));
			yield return new WaitForSeconds (duration / 255);
		}
	}

	public void FlipperDestroyed() {
		EnemyDestroyed ("Flipper");
	}
	public void TankerDestroyed() {
		EnemyDestroyed ("Tanker");
	}

	void EnemyDestroyed(string type) {
		if (type == "Flipper") {
			_remainingEnemies--;
		} else if (type == "Tanker") {
			// Tanker spawns two Flippers, which means a net total of +1 enemies
			_remainingEnemies++; 
		}
	}

}
