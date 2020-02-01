using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotObject : BuildableObject
{
    public Transform transform { get { return feedbackSprite.transform; } }
    public SpriteRenderer feedbackSprite;
    public Joint2D joint;
    public BuildableObject connectedObject;

    protected override void Awake()
    {
        base.Awake();

        feedbackSprite = GetComponent<SpriteRenderer>();
    }

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot) { }

    public override void DisconnectPivot(PivotObject _pivot) { }

    public override void DisconnectAllPivots() { }
}