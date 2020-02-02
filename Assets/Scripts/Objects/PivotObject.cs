using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotObject : MonoBehaviour
{
    public BuildableObject body;
    public SpriteRenderer feedbackSprite;
    public Joint2D joint;
    public BuildableObject connectedBody
    {
        get { return _connectedBody; }
        set
        {
            if (_connectedBody == value) { return; }
            if (_connectedBody && _connectedBody.connectedBodies.Contains(body)) { _connectedBody.connectedBodies.Remove(body); }
            _connectedBody = value;
            if (_connectedBody && !_connectedBody.connectedBodies.Contains(body)) { _connectedBody.connectedBodies.Add(body); }
        }
    }
    [SerializeField, HideInInspector] private BuildableObject _connectedBody;

    protected void Awake()
    {
        feedbackSprite = GetComponent<SpriteRenderer>();
    }
}