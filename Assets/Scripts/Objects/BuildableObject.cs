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
    public List<BuildableObject> connectedBodies = new List<BuildableObject>();
    [HideInInspector] public int selectionPriority;
    [HideInInspector] public Vector3 buildPosition;
    [HideInInspector] public Quaternion buildRotation;

    protected bool pickedUp;

    [HideInInspector] public Rigidbody2D rb;
    private Rigidbody2D[] rbs;
    [HideInInspector] public Collider2D col;
    [HideInInspector] public SpriteRenderer spriteR;
    [HideInInspector] public Outline outline;
    private int defaultLayer;
    private float defaultGravity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        defaultLayer = gameObject.layer;
        if (!GameManager.instance.playMode && rb.bodyType != RigidbodyType2D.Kinematic) { gameObject.layer = 8; }
        rbs = GetComponentsInChildren<Rigidbody2D>().Where(rb => rb.bodyType != RigidbodyType2D.Kinematic).ToArray();
        if (GameManager.instance.playMode && rb.bodyType != RigidbodyType2D.Kinematic)
        {
            foreach (Rigidbody2D rb in rbs)
            {

                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            if (outline) { Destroy(outline); }
        }
        col = GetComponent<Collider2D>();
        spriteR = GetComponent<SpriteRenderer>();
        selectionPriority = spriteR.sortingOrder;
        spriteR.sortingOrder -= 10;
        pivots = GetComponentsInChildren<PivotObject>();
        foreach (PivotObject pivot in pivots)
        {
            pivot.body = this;
        }
    }

    protected virtual void Update()
    {
        if (pickedUp)
        {
            CheckPivotsForConnections();
            if (GameManager.mouse.target == this) { RotateObject(); }
        }
    }

    private void TryConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot) { if (_otherObject) { ConnectPivotToObject(_otherObject, _pivot); } }
    public virtual void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        if (_pivot.connectedBody) { DisconnectPivot(_pivot); }
        _pivot.connectedBody = _otherObject;

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
        _pivot.connectedBody = null;
    }

    public void Pickup(bool _alsoChildren = true)
    {
        if (pickedUp) { return; }
        pickedUp = true;
        print("Picked up " + gameObject.name);

        gameObject.layer = defaultLayer;

        foreach (Rigidbody2D rb in rbs)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.angularDrag = 50;
            rb.gravityScale = 0;
        }

        if (_alsoChildren)
        {
            DisconnectAllPivots();
            foreach (BuildableObject connectedObject in GetAllConnectedObjects())
            {
                connectedObject.Pickup(false);
            }
        }
        // rb.freezeRotation = true;
    }

    public void Drop(bool _alsoChildren = true)
    {
        if (!pickedUp) { return; }
        pickedUp = false;
        print("Dropped " + gameObject.name);

        foreach (Rigidbody2D rb in rbs)
        {
            rb.freezeRotation = false;
            rb.angularDrag = 0.1f;
            rb.gravityScale = defaultGravity;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (_alsoChildren)
        {
            for (int i = 0; i < pivots.Length; i++)
            {
                TryConnectPivotToObject(CheckForConnection(pivots[i], gameObject), pivots[i]);
                pivots[i].feedbackSprite.color = Color.clear;
            }
            foreach (BuildableObject connectedObject in GetAllConnectedObjects())
            {
                connectedObject.Drop(false);
            }
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
        if (Input.mouseScrollDelta.y != 0) { rb.angularVelocity = Mathf.Sign(Input.mouseScrollDelta.y) * 400; }
        if (Input.GetAxis("Horizontal") != 0) { rb.angularVelocity = Mathf.Sign(-Input.GetAxis("Horizontal")) * 400; }
    }

    public static BuildableObject CheckForConnection(PivotObject _pivot, GameObject _ignore = null) => CheckForConnection(_pivot.transform.position, _ignore);
    public static BuildableObject CheckForConnection(Vector2 _pos, GameObject _ignore = null, int _mask = ~0)
    {
        Collider2D[] hit = Physics2D.OverlapPointAll(_pos, _mask);

        if (hit.Length == 0) { return null; }

        //Get the front most buildable object (which isn't this one)
        BuildableObject otherObj = hit.Where(h => h.gameObject.GetComponent<BuildableObject>() != null && (_ignore == null || h.gameObject != _ignore && !h.transform.IsChildOf(_ignore.transform)))
            .Select(h => h.gameObject.GetComponent<BuildableObject>())
            .OrderByDescending(bo => bo.selectionPriority)
            .FirstOrDefault();

        // Collider2D otherCol = hit.FirstOrDefault(o => o.gameObject != gameObject && o.GetComponent<BuildableObject>() != null);

        // if (!otherCol) { return; }

        // BuildableObject otherObj = otherCol.gameObject.GetComponent<BuildableObject>();

        return otherObj;

        // print("Connecting " + gameObject + " to " + otherObj.gameObject);
        // ConnectToObject(otherObj);

    }

    protected Outlines? currnetOutline;
    public virtual void SetHighlight(Outlines? _outlineType)
    {
        if (_outlineType == currnetOutline) { return; }
        currnetOutline = _outlineType;

        if (!outline) { outline = gameObject.AddComponent<Outline>(); }

        if (_outlineType == null)
        {
            outline.enabled = false;
            for (int i = 0; i < pivots.Length; i++)
            {
                if (!pivots[i].connectedBody) { continue; }

                pivots[i].connectedBody.SetHighlight(null);
            }
        }
        else
        {
            outline.enabled = true;
            outline.color = (int)_outlineType;

            if (_outlineType == Outlines.Highlighted)
            {
                for (int i = 0; i < pivots.Length; i++)
                {
                    if (!pivots[i].connectedBody) { continue; }

                    pivots[i].connectedBody.SetHighlight(Outlines.Connected);
                }
            }
        }
    }

    protected BuildableObject[] GetAllConnectedObjects()
    {
        List<BuildableObject> BOs = new List<BuildableObject>(new BuildableObject[] { this });

        for (int i = 0; i < BOs.Count; i++)
        {
            BOs.AddRange(BOs[i].pivots.Where(p => p.connectedBody != null && !BOs.Contains(p.connectedBody)).Select(p => p.connectedBody));
            BOs.AddRange(BOs[i].connectedBodies.Where(b => b != null && !BOs.Contains(b)));
        }
        BOs.Remove(this);
        return BOs.ToArray();
    }
}

public enum Outlines
{
    Highlighted,
    Selected,
    Connected
}