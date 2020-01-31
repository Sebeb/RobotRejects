using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildableObject : MonoBehaviour
{
    public Vector3 buildPosition;
    public Quaternion buildRotation;

    private Rigidbody2D rigidBody;
    private Collider2D collider;
    
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    public abstract void ConnectToObject(BuildableObject _otherObject);

    public void Pickup()
    {

    }

    public void Drop()
    {

    }
}
