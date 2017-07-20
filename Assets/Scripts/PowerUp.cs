using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public GameObject explodePrefab;
    public AudioClip soundDeath;

    [HideInInspector] public MapLine curMapLine;
    [HideInInspector] public float moveSpeed = 1f;

    private MapManager _mapManager;
    private GameManager _gameManager;
    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private GameObject _playerRef;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        Move();

        if (transform.position.z < 3)
            OnDeath();
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
    }

}
