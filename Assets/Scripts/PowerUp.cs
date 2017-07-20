using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public float movementForce;
    public MapLine thisMapLine;
    public GameObject explodePrefab;
    public int levelNum;

    private MapManager _mapManager; //How do I use the same _mapManager as that of the player ship if it's private?
    private GameManager _gameManager;
    private bool _straightMovement; //True if moving in only one lane for level one
    private AudioSource _audioSource;
    private bool _reloaded = true;
    private int _isCW = 0; //isClockWise: 1 = CW
    private int _currPlayerNum;

    private Rigidbody rb;

    public AudioClip soundDeath;

    // Use this for initialization
    void Start () {
        _reloaded = true;
        rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        levelNum = _gameManager.currentRound;

        _straightMovement = false;

        Vector3 curDirVec = thisMapLine.GetDirectionVector();
        Vector3 newDirVec = new Vector3(-curDirVec.y, curDirVec.x, 0);
        rb.MoveRotation(Quaternion.LookRotation(new Vector3(0f, 0f, 1f), newDirVec));
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (rb.position.z <= 0) //In case the player ship is flying in after respawning?
        {
            Vector3 _newPos;
            MapLine _newMapLine, _nextMapLine;
            //transform.position = new Vector3 (transform.position.x, transform.position.y, 0);
            rb.MovePosition(new Vector3(transform.position.x, transform.position.y, 0));
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
            if (GameObject.Find("Player") != null)
            {
                _currPlayerNum = GameObject.Find("Player").GetComponent<PlayerShip>().curMapLine.GetLineNum();
                if (_isCW == 0)
                {
                    int _beCW = _currPlayerNum - thisMapLine.GetLineNum();
                    int _beCCW = _mapManager.mapLines.Length - _currPlayerNum + thisMapLine.GetLineNum();
                    if (_beCW > _beCCW)
                    {
                        _isCW = 1;
                    }
                    else if (_beCW < _beCCW)
                    {
                        _isCW = -1;
                    }
                    else
                    { //Equal distance from player
                        if (Random.value > 0.5)
                        {
                            _isCW = 1;
                        }
                        else
                        {
                            _isCW = -1;
                        }
                    }
                }

                if (thisMapLine == GameObject.Find("Player").GetComponent<PlayerShip>().curMapLine)
                {
                    _nextMapLine = thisMapLine;
                }
                else if (_isCW == 1)
                {
                    _nextMapLine = thisMapLine.leftLine;
                }
                else
                {
                    _nextMapLine = thisMapLine.rightLine;
                }
                _newPos = _nextMapLine.GetMidPoint();
                rb.MovePosition(new Vector3(_newPos.x, _newPos.y, 0));
                Vector3 curDirVec = _nextMapLine.GetDirectionVector();
                Vector3 newDirVec = new Vector3(-curDirVec.y, curDirVec.x, 0);
                //print (Quaternion.Euler(newDirVec));
                rb.MoveRotation(Quaternion.LookRotation(new Vector3(0f, 0f, 1f), newDirVec));
            }
        }
        else if (_straightMovement)
        {
            //Only move in Z direction, aka depth
            //rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
            //rb.AddForce (-1 * movementForce * transform.forward * Time.deltaTime);

            rb.MovePosition(transform.position + transform.forward * (Time.deltaTime * movementForce * -1));
        }
        else //Switching lanes
        {
            Vector3 _newPos;
            MapLine _newMapLine, _nextMapLine = thisMapLine.leftLine;
            Vector3 _newPosZ = transform.position + transform.forward * (Time.deltaTime * movementForce * -1);
            //Move forward by one or a few pixels
            thisMapLine.UpdateMovement(transform.position, Time.deltaTime * 1 * movementForce * 0.2f, out _newPos, out _newMapLine);
            rb.MovePosition(new Vector3(_newPos.x, _newPos.y, _newPosZ.z));
            //While moving to next section of map
            Vector3 curDirVec = _nextMapLine.GetDirectionVector();
            Vector3 newDirVec = new Vector3(-curDirVec.y, curDirVec.x, transform.position.z);
            rb.MoveRotation(Quaternion.LookRotation(new Vector3(0f, 0f, 1f), newDirVec));
        }
    }

    public void TakeDamage(int dmg)
    {
        OnDeath();
    }

    public void OnDeath()
    {
        GameObject powerUpExplosion = Instantiate(explodePrefab, gameObject.transform.position, gameObject.transform.rotation);
        AudioSource explosionSource = powerUpExplosion.GetComponent<AudioSource>();
        explosionSource.clip = soundDeath;
        explosionSource.Play();
        _gameManager.FlipperDestroyed();
        Destroy(gameObject);
        //gameObject.SetActive (false); // Disable enemy
    }

    public bool GetStraightMovement()
    {
        return _straightMovement;
    }
    public void SetStraightMovement(bool isStraight)
    {
        _straightMovement = isStraight;
    }

    public void SetMapLine(MapLine newML)
    {
        thisMapLine = newML;
    }
}
