using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	public GameObject minePrefab;
	public GameObject godPrefab;
    public GameObject explodePrefab;
    public AudioClip soundDeath;

    [HideInInspector] public MapLine curMapLine;
    [HideInInspector] public float moveSpeed = 1f;

	public enum POWERUPS
	{
		Shield,
		Mine,
	};

    private MapManager _mapManager;
    private GameManager _gameManager;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
	private PlayerShip _playerRef;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();
		_playerRef = GameObject.Find("Player").GetComponent<PlayerShip>();
    }

    void FixedUpdate()
    {
        Move();

		if (transform.position.z < -2)
			Destroy (gameObject);
    }

    void Move()
    {
        Vector3 newPos = curMapLine.GetMidPoint();
        newPos = newPos + new Vector3(0f, 0f, transform.position.z - moveSpeed * Time.deltaTime);

        _rigidbody.MovePosition(newPos);

        Vector3 curDirVec = curMapLine.GetDirectionVector();
        Vector3 newDirVec = new Vector3(-curDirVec.y, curDirVec.x, 0);
        //print (Quaternion.Euler(newDirVec));
        _rigidbody.MoveRotation(Quaternion.LookRotation(new Vector3(0f, 0f, 1f), newDirVec));
    }

    public void TakeDamage(int dmg)
    {
        OnDeath();
    }

    public void OnDeath()
    {
        GameObject newExplosion = Instantiate(explodePrefab, gameObject.transform.position, gameObject.transform.rotation);
        AudioSource explosionSource = newExplosion.GetComponent<AudioSource>();
        explosionSource.clip = soundDeath;
        explosionSource.Play();

        Destroy(gameObject);

		// Shield
		int powerup = (int)(Random.value * 2f);
		if (powerup == 1) {
			_playerRef.SetGod (5);
			GameObject newEffect = Instantiate (godPrefab, _playerRef.transform.position, _playerRef.transform.rotation);
			newEffect.transform.SetParent (_playerRef.transform);
			Destroy (newEffect, 5);
		} else {
			foreach (MapLine ml in _mapManager.mapLines) {
				if (Random.Range (0, 10) >= 7) {
					GameObject shellInstance = Instantiate (minePrefab, ml.GetMidPoint(), Quaternion.Euler(-transform.forward));
					shellInstance.GetComponent<Rigidbody>().velocity = 1f * (transform.forward); 
				}
			}
		}
    }

}
