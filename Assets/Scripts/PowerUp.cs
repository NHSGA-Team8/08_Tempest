using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public float movementForce;
    public MapLine thisMapLine;

    private bool _straightMovement; //True if moving in only one lane for level one
    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.MovePosition(transform.position + transform.forward * (Time.deltaTime * movementForce * -1));

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
