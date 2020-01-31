using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BuildableObject : MonoBehaviour
{
    [HideInInspector] public Vector3 buildPosition;
    [HideInInspector] public Quaternion buildRotation;

    private Rigidbody2D rigidBody;
    private Collider2D collider;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    public abstract void ConnectToObject(BuildableObject _otherObject);
    public abstract void Disconnect();

    public void Pickup()
    {
        Disconnect();
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        rigidBody.freezeRotation = true;
    }

    public void Drop()
    {
        rigidBody.freezeRotation = false;
        print("Dropped!");
        CheckForConnection();
    }

    private void CheckForConnection()
    {
        Collider2D[] hit = Physics2D.OverlapPointAll(transform.position);

        if (hit.Length == 0) { return; }

        Collider2D otherCol = hit.FirstOrDefault(o => o.gameObject != gameObject && o.GetComponent<BuildableObject>() != null);

        if (!otherCol) { return; }

        BuildableObject otherObj = otherCol.gameObject.GetComponent<BuildableObject>();

        if (!otherObj) { return; }

        print("Connecting " + gameObject + " to " + otherObj.gameObject);
        ConnectToObject(otherObj);

    }
}