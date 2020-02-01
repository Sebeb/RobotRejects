using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cakeslice;
using UnityEngine;

// [Serializable]
// public struct PivotObject
// {

// }

public abstract class BuildableObject : MonoBehaviour
{
    public PivotObject[] pivots;
    [HideInInspector] public int selectionPriority;
    [HideInInspector] public Vector3 buildPosition;
    [HideInInspector] public Quaternion buildRotation;

    protected bool pickedUp;

    [HideInInspector] public Rigidbody2D rb;
    private Rigidbody2D[] rbs;
    [HideInInspector] public Collider2D col;
    [HideInInspector] public SpriteRenderer spriteR;
    private Outline outline;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rbs = GetComponentsInChildren<Rigidbody2D>().Where(rb => rb.bodyType != RigidbodyType2D.Kinematic).ToArray();
        if (GameManager.instance.playMode && rb.bodyType != RigidbodyType2D.Kinematic) { rb.bodyType = RigidbodyType2D.Dynamic; }
        col = GetComponent<Collider2D>();
        spriteR = GetComponent<SpriteRenderer>();
        selectionPriority = spriteR.sortingOrder;
        pivots = GetComponentsInChildren<PivotObject>();
    }

    protected virtual void Update()
    {
        if (pickedUp)
        {
            CheckPivotsForConnections();
            RotateObject();
        }
    }

    private void TryConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot) { if (_otherObject) { ConnectPivotToObject(_otherObject, _pivot); } }
    public virtual void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        if (_pivot.connectedObject) { DisconnectPivot(_pivot); }
        _pivot.connectedObject = _otherObject;

        spriteR.sortingOrder = selectionPriority = _otherObject.selectionPriority + 1;
        print("Connecting " + gameObject.name + " to " + _otherObject.gameObject.name);
    }

    public virtual void DisconnectAllPivots()
    {
        for (int i = 0; i < pivots.Length; i++)
        {
            DisconnectPivot(pivots[i]);
        }
    }
    public virtual void DisconnectPivot(PivotObject _pivot)
    {
        _pivot.connectedObject = null;
    }

    public void Pickup()
    {
        if (pickedUp) { return; }
        pickedUp = true;
        print("Picked up " + gameObject.name);

        DisconnectAllPivots();
        foreach (Rigidbody2D rb in rbs)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.angularDrag = 50;
            rb.gravityScale = 0;
        }
        // rb.freezeRotation = true;
    }

    public void Drop()
    {
        if (!pickedUp) { return; }
        pickedUp = false;
        print("Dropped " + gameObject.name);
        foreach (Rigidbody2D rb in rbs)
        {
            rb.freezeRotation = false;
            rb.angularDrag = 0.1f;
            rb.gravityScale = 1;
            rb.bodyType = RigidbodyType2D.Static;
        }

        for (int i = 0; i < pivots.Length; i++)
        {
            TryConnectPivotToObject(CheckForConnection(pivots[i], gameObject), pivots[i]);
            pivots[i].feedbackSprite.color = Color.clear;
        }

    }

    private void CheckPivotsForConnections()
    {
        for (int i = 0; i < pivots.Length; i++)
        {
            pivots[i].feedbackSprite.color = CheckForConnection(pivots[i], gameObject) != null ? Color.green : Color.red;
        }
    }

    private void RotateObject()
    {
        if (Input.mouseScrollDelta.y != 0) { rb.angularVelocity = Mathf.Sign(Input.mouseScrollDelta.y) * 100; }
        if (Input.GetAxis("Horizontal") != 0) { rb.angularVelocity = Mathf.Sign(Input.GetAxis("Horizontal")) * 100; }
    }

    public static BuildableObject CheckForConnection(PivotObject _pivot, GameObject _ignore = null) => CheckForConnection(_pivot.transform.position, _ignore);

    public static BuildableObject CheckForConnection(Vector2 _pos, GameObject _ignore = null)
    {
        Collider2D[] hit = Physics2D.OverlapPointAll(_pos);

        if (hit.Length == 0) { return null; }

        //Get the front most buildable object (which isn't this one)
        BuildableObject otherObj = hit.Where(h => _ignore == null || h.gameObject != _ignore && !h.transform.IsChildOf(_ignore.transform))
            .Select(h => h.gameObject.GetComponent<BuildableObject>())
            .OrderBy(bo => -bo.selectionPriority)
            .FirstOrDefault();

        // Collider2D otherCol = hit.FirstOrDefault(o => o.gameObject != gameObject && o.GetComponent<BuildableObject>() != null);

        // if (!otherCol) { return; }

        // BuildableObject otherObj = otherCol.gameObject.GetComponent<BuildableObject>();

        return otherObj;

        // print("Connecting " + gameObject + " to " + otherObj.gameObject);
        // ConnectToObject(otherObj);

    }

    public void SetHighlight(bool _enabled)
    {
        if (!outline) { outline = gameObject.AddComponent<Outline>(); }

        if (!_enabled)
        {
            outline.enabled = false;
            for (int i = 0; i < pivots.Length; i++)
            {
                if (!pivots[i].connectedObject || !pivots[i].connectedObject.outline) { continue; }

                pivots[i].connectedObject.outline.enabled = false;
            }
        }
        else
        {
            outline.enabled = true;
            outline.color = (int)Outlines.Highlighted;

            for (int i = 0; i < pivots.Length; i++)
            {
                if (!pivots[i].connectedObject) { continue; }

                if (!pivots[i].connectedObject.outline) { pivots[i].connectedObject.outline = pivots[i].connectedObject.gameObject.AddComponent<Outline>(); }

                pivots[i].connectedObject.outline.enabled = true;
                pivots[i].connectedObject.outline.color = (int)Outlines.Connected;
            }
        }
    }
}

public enum Outlines
{
    Highlighted,
    Selected,
    Connected
}