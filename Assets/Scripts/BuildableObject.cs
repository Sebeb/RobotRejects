using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct PivotPoint
{
    public Transform transform;
    public SpriteRenderer feedbackSprite;
    public Joint2D joint;
    public bool connected;
}

public abstract class BuildableObject : MonoBehaviour
{
    public PivotPoint[] pivotPoints;
    [HideInInspector] public int selectionPriority;
    [HideInInspector] public Vector3 buildPosition;
    [HideInInspector] public Quaternion buildRotation;

    protected bool pickedUp;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Collider2D col;
    [HideInInspector] public SpriteRenderer spriteR;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteR = GetComponent<SpriteRenderer>();
        selectionPriority = spriteR.sortingOrder;
    }

    protected virtual void Update()
    {
        if (pickedUp) { CheckPivotsForConnections(); }
    }

    private void TryConnectPivotToObject(BuildableObject _otherObject, PivotPoint _pivot) { if (_otherObject) { ConnectPivotToObject(_otherObject, _pivot); } }
    public virtual void ConnectPivotToObject(BuildableObject _otherObject, PivotPoint _pivot)
    {
        if (_pivot.connected) { DisconnectPivot(_pivot); }
        _pivot.connected = true;

        spriteR.sortingOrder = selectionPriority = _otherObject.selectionPriority + 1;
        print("Connecting " + gameObject.name + " to " + _otherObject.gameObject.name);
    }

    public virtual void DisconnectAllPivots()
    {
        for (int i = 0; i < pivotPoints.Length; i++)
        {   
            DisconnectPivot(pivotPoints[i]);
        }
    }
    public virtual void DisconnectPivot(PivotPoint _pivot)
    {
        _pivot.connected = false;
    }

    public void Pickup()
    {
        if (pickedUp) { return; }
        pickedUp = true;

        DisconnectAllPivots();
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.freezeRotation = true;
    }

    public void Drop()
    {
        if (!pickedUp) { return; }
        pickedUp = false;

        rb.freezeRotation = false;
        print("Dropped " + gameObject.name);

        for (int i = 0; i < pivotPoints.Length; i++)
        {
            TryConnectPivotToObject(CheckForConnection(pivotPoints[i]), pivotPoints[i]);
            pivotPoints[i].feedbackSprite.color = Color.clear;
        }

    }

    private void CheckPivotsForConnections()
    {
        for (int i = 0; i < pivotPoints.Length; i++)
        {
            pivotPoints[i].feedbackSprite.color = CheckForConnection(pivotPoints[i]) != null ? Color.green : Color.red;
        }
    }

    private BuildableObject CheckForConnection(PivotPoint _pivot)
    {
        Collider2D[] hit = Physics2D.OverlapPointAll(_pivot.transform.position);

        if (hit.Length == 0) { return null; }

        //Get the front most buildable object (which isn't this one)
        BuildableObject otherObj = hit.Where(h => h.gameObject != gameObject).Select(h => h.gameObject.GetComponent<BuildableObject>()).OrderBy(bo => -bo.selectionPriority).FirstOrDefault();

        // Collider2D otherCol = hit.FirstOrDefault(o => o.gameObject != gameObject && o.GetComponent<BuildableObject>() != null);

        // if (!otherCol) { return; }

        // BuildableObject otherObj = otherCol.gameObject.GetComponent<BuildableObject>();

        return otherObj;

        // print("Connecting " + gameObject + " to " + otherObj.gameObject);
        // ConnectToObject(otherObj);

    }
}