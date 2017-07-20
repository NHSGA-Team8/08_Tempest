using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Ethan Zhu and Rachael H.
 */
public class GameManager : MonoBehaviour {

	[Header("Prefabs")]
	public GameObject playerPrefab;
	public GameObject flipperPrefab;
	public GameObject tankerPrefab;
	public GameObject spikerPrefab;
    public GameObject powerUpPrefab;
	public GameObject spawnEffect;

	[Header("Round Settings")]
	public int totalFlippers;
	public int totalTankers;
	public int totalSpikers;
    public int totalPowerUps;
	public float flipperSpawnDelay;
	public float tankerSpawnDelay;
    public float powerUpSpawnDelay;
	public int currentRound;
	public int nextScene;
	public int totalLives;
	public float speedMulti = 5f;
	public bool test = false;

	[Header("AudioClips")]
	public AudioClip ac_portalEnter;
	public AudioClip ac_portalDuring;
	public AudioClip ac_portal;
	public AudioClip ac_gameover;

	[Header("Other References")]
	public AudioSource musicAS;
	public Camera cam;
	public Canvas uiCanvas;
	public Text notification;
	public Text score;
	public Image flash;

	public enum GAMESTATE {PREGAME, STARTING, PLAYING, ENDING};
	[HideInInspector] public GAMESTATE curGamestate = GAMESTATE.PREGAME;

	private float _lastSpawn;
	private float _startTime;

	private int _flipperCount;
	private int _tankerCount;
	private int _enemiesCount;
	private int _spikerCount;
	private int _totalEnemies;
	private int _remainingEnemies;
	private int _remainingFlipper;
	private int _remainingTanker;
	private int _remainingSpiker;
	private GameObject _playerRef;
	private MapManager _mapManager;
	private AudioSource _audioSource;



	// Use this for initialization
	void Start () {
		_mapManager = GameObject.Find ("MapManager").GetComponent<MapManager> ();
		_playerRef = GameObject.Find ("Player");
		_audioSource = cam.GetComponent<AudioSource> ();

		_flipperCount = 0;
		_tankerCount = 0;
		_spikerCount = 0;
		_enemiesCount = 0;
		_totalEnemies = totalFlippers + totalTankers + totalSpikers;
		_remainingEnemies = _totalEnemies;

		if (currentRound == 1) {
			GlobalVariables.Reset ();
		}

		StartCoroutine (GameLoop ());
	}
	
	// Update is called once per frame
	void Update () {
		UpdateScore ();
	}

	private IEnumerator GameLoop()
	{
		curGamestate = GAMESTATE.STARTING;
		yield return StartCoroutine(RoundStarting());
		curGamestate = GAMESTATE.PLAYING;
		yield return StartCoroutine(RoundPlaying());
		curGamestate = GAMESTATE.ENDING;
		yield return StartCoroutine(RoundEnding());

		DestroyAllEnemies ();
		DestroyAllProjectiles ();

		if (GlobalVariables.lives < 0)
		{
			
			// Back to menu if dead
			SceneManager.LoadScene(0);
		}
		else
		{
			// Next level if win
			SceneManager.LoadScene(nextScene);
		}
	}

	private IEnumerator RoundStarting() {
		SpawnPlayerShip ();
		SpawnEnemyShips ();
        StartCoroutine(SpawnPowerUps());

		yield return new WaitForSeconds(1);
		_startTime = Time.fixedTime;
	}

	private IEnumerator RoundPlaying() {

		//while (!(GlobalVariables.lives < 0 || (EnemiesAtEdge() == true && _enemiesCount >= _totalEnemies) || _remainingEnemies <= 0)) // !(EnemiesAtEdge() == true && _remainingEnemies > 0 || _remainingEnemies > 0) 
		while (!(GlobalVariables.lives < 0 || (EnemiesAtEdge() == true && _enemiesCount >= _totalEnemies) || _remainingEnemies <= 0))
			yield return null;
	}

	private IEnumerator RoundEnding() {
		//print ("RoundEnding");
		SetEndMessage();
		if (GlobalVariables.lives >= 0) {
			_audioSource.clip = ac_portalEnter;
			_audioSource.Play ();
			//StartCoroutine (FlashScreen (4f, 0.1f));
			yield return new WaitForSeconds (1);
			_playerRef.GetComponent<PlayerShip> ().movingForward = true;
		
		} else {
			musicAS.Stop ();
			musicAS.clip = ac_gameover;
			musicAS.Play ();
		}

		while (_playerRef.transform.position.z < _mapManager.depth && GlobalVariables.lives >= 0) // Wait for a game over or clear level
			yield return null;



		if (GlobalVariables.lives >= 0) {
			StartCoroutine (FlashScreen (0f, 0.1f, 1f));
			yield return new WaitForSeconds (1);
		} else { // If the player game overs when warping, we refresh status
			SetEndMessage(); 
			yield return new WaitForSeconds (5);
		}
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
		StartCoroutine(FlashScreen (0f, 0.15f, 0.7f));
		DestroyAllEnemies ();
		DestroyAllProjectiles ();
		GlobalVariables.lives--;
		if (GlobalVariables.lives >= 0)
			StartCoroutine (PlayerDied ());
	}

	public IEnumerator PlayerDied() {
		yield return new WaitForSeconds(2);
		SpawnPlayerShip ();

	}

	private void SpawnEnemyShips ()
	{
		
		StartCoroutine(SpawnFlippers ());
		StartCoroutine(SpawnTankers ());
		SpawnSpikers ();
	}

	private IEnumerator SpawnFlippers ()
	{
		for (int i = 0; i < totalFlippers; i++)
		{
			_flipperCount++;
			_enemiesCount++;
			SpawnFlipper ();
			yield return new WaitForSeconds (flipperSpawnDelay);
		}
	}

	private IEnumerator SpawnTankers ()
	{
		for (int i = 0; i < totalTankers; i++)
		{
			_tankerCount++;
			_enemiesCount++;
			SpawnTanker();
			yield return new WaitForSeconds (tankerSpawnDelay);
		}
	}

	// Spikers instantly appear at start of game, so we do not use Coroutines
	private void SpawnSpikers ()
	{
		for (int i = 0; i < totalSpikers; i++)
		{
			_spikerCount++;
			_enemiesCount++;
			SpawnSpiker();

		}
	}

    private IEnumerator SpawnPowerUps()
    {

        for (int i = 0; i < totalPowerUps; i++)
        {
            SpawnPowerUp();
            yield return new WaitForSeconds(powerUpSpawnDelay);
        }


		yield return null;

    }
		
	//Spawns new flipper enemy on field, associated with map line
	public void SpawnFlipper()
	{
		int index = Random.Range (1, _mapManager.mapLines.Length - 1);
		MapLine newMapLine = _mapManager.mapLines [index];
		float _mapDepth = _mapManager.depth;
		GameObject newShip = Instantiate (flipperPrefab, newMapLine.GetMidPoint() + new Vector3 (0, 0, 1 * _mapDepth), flipperPrefab.transform.rotation);
		if (test == true) {
			newShip.GetComponent<Flipper_New>().curMapLine = newMapLine;
			newShip.GetComponent<Flipper_New>().moveSpeed *= currentRound * speedMulti;
		} else {
			newShip.GetComponent<Flipper>().SetMapLine (newMapLine);
			newShip.GetComponent<Flipper>().movementForce = currentRound * speedMulti;
		}


	}

	public void SpawnTanker()
	{
		int index = Random.Range (1, _mapManager.mapLines.Length - 1);
		MapLine newMapLine = _mapManager.mapLines [index];
		float _mapDepth = _mapManager.depth;
		GameObject newShip = Instantiate (tankerPrefab, newMapLine.GetMidPoint() + new Vector3 (0, 0, 1 * _mapDepth), tankerPrefab.transform.rotation);
		newShip.GetComponent<Tanker> ().curMapLine = newMapLine;
		newShip.GetComponent<Tanker>().moveSpeed *= currentRound * speedMulti;
	}

    public void SpawnPowerUp()
    {
        int index = Random.Range(1, _mapManager.mapLines.Length - 1);
        MapLine newMapLine = _mapManager.mapLines[index];
        float _mapDepth = _mapManager.depth;
		GameObject powerUp = Instantiate(powerUpPrefab, newMapLine.GetMidPoint() + new Vector3(0, 0, 1 * _mapDepth), powerUpPrefab.transform.rotation);
        powerUp.GetComponent<PowerUp>().curMapLine = newMapLine;
        powerUp.GetComponent<PowerUp>().moveSpeed *= currentRound * speedMulti;
    }

	public void SpawnSpiker()
	{
		int index = Random.Range (1, _mapManager.mapLines.Length - 1);
		MapLine newMapLine = _mapManager.mapLines [index];
		float _mapDepth = _mapManager.depth;
		GameObject newShip = Instantiate (spikerPrefab, newMapLine.GetMidPoint() + new Vector3 (0, 0, 1 * _mapDepth), spikerPrefab.transform.rotation);
		newShip.GetComponent<Spiker>().moveSpeed *= currentRound * speedMulti;
	}

	bool EnemiesAtEdge() {
		// Only flippers reach edge
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			if (enemy.GetComponent<Flipper>() != null && enemy.transform.position.z > 0.1f)
				return false;
			else if (test == true && enemy.GetComponent<Flipper_New> () != null && enemy.transform.position.z > 0.1f)
				return false;
		}
		return true;
	}

	void SetEndMessage() {
		string msg = "";

		if (GlobalVariables.lives < 0)
			msg = "Game Over";
		else
			msg = "Round Complete";

		notification.text = msg;
	}

	public IEnumerator FlashScreen(float wait, float duration) {
		yield return new WaitForSeconds (wait);
		/*
		for (int i = 0; i <= 255; i++) {
			flash.color = new Color (1f, 1f, 1f, ((float)i / 255f));
			yield return new WaitForSeconds (duration / 255f);
		}
		*/
		float i = 0f;
		while (i < 1f) {
			i += duration;
			flash.color = new Color (1f, 1f, 1f, i);
			yield return new WaitForSeconds (0.01f);
		}
	}

	public IEnumerator FlashScreen(float wait, float duration, float hold) {
		yield return new WaitForSeconds (wait);
		float i = 0f;
		while (i < 1f) {
			i += duration;
			flash.color = new Color (1f, 1f, 1f, i);
			yield return new WaitForSeconds (0.01f);
		}
		yield return new WaitForSeconds (hold);
		i = 1f;
		while (i > 0f) {
			i -= duration;
			flash.color = new Color (1f, 1f, 1f, i);
			yield return new WaitForSeconds (0.01f);
		}
	}

	public void FlipperDestroyed() {
		EnemyDestroyed ("Flipper");
		GlobalVariables.score += 150;
	}
	public void TankerDestroyed() {
		EnemyDestroyed ("Tanker");
		GlobalVariables.score += 100;
	}
	public void SpikerDestroyed() {
		EnemyDestroyed ("Spiker");
		GlobalVariables.score += 50;
	}
    public void PowerUpDestroyed()
    {
        GlobalVariables.score += 50;
    }

	void EnemyDestroyed(string type) {
		if (type == "Flipper") {
			_remainingEnemies--;
		} else if (type == "Tanker") {
			// Tanker spawns two Flippers, which means a net total of +1 enemies
			_remainingEnemies++; 
		} else if (type == "Spiker") {
			_remainingEnemies--;
		}
	}

	void UpdateScore() {
		score.text = GlobalVariables.score.ToString();
	}

	public void DestroyAllEnemies() {
		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
			// Spikers persist
			if (enemy.GetComponent<Spiker>() != null) continue;
			Destroy (enemy); // Note that Destroy doesn't call OnDeath, meaning that _remainingEnemies won't decrease and score won't increase
			_remainingEnemies--; // If remainingEnemies doesn't decrease, the game will not end
		}
	}

	public void DestroyAllProjectiles() {
		foreach (GameObject proj in GameObject.FindGameObjectsWithTag("Projectile")) {
			// Spikes persist
			if (proj.GetComponent<Spikes>() != null) continue;
			Destroy (proj); // Note that Destroy doesn't call OnDeath, meaning that _remainingEnemies won't decrease and score won't increase
		}
	}
}
