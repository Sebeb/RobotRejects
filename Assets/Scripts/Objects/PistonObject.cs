using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistonObject : ActionObject
{
    public SpringJoint2D joint;
    public Rigidbody2D targetA, targetB;

    public float extensionDistance;
    private float retractedDistance;

    public SpriteRenderer foregroundSpriteR;

    protected override void Update()
    {
        base.Update();

        if (targetA && targetB) //Move and scale spring between anchors
        {
            Vector2 pointA = targetA.transform.TransformPoint(joint.anchor);
            Vector2 pointB = targetB.transform.TransformPoint(joint.connectedAnchor);
            Vector2 a2b = pointB - pointA;
            transform.position = pointA + (a2b * 0.5f);
            transform.localScale = transform.localScale.SetY(a2b.magnitude / retractedDistance);
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.down, a2b);
            foregroundSpriteR.sortingOrder = spriteR.sortingOrder + 1;
            foregroundSpriteR.transform.localScale = Vector3.one.SetY(1 / transform.localScale.y);
        }
    }

    protected override void OnActionStart()
    {
        if (joint) { joint.distance = retractedDistance + extensionDistance; }
    }

    protected override void OnActionEnd()
    {
        if (joint) { joint.distance = retractedDistance; }
    }

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        rb.bodyType = RigidbodyType2D.Static;

        if (targetA == null)
        {
            targetA = _otherObject.rb;
            joint = _otherObject.gameObject.AddComponent<SpringJoint2D>();
            joint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
            joint.enableCollision = true;
            joint.autoConfigureDistance = joint.autoConfigureConnectedAnchor = false;
        }
        else
        {
            targetB = _otherObject.rb;
            joint.connectedBody = targetB;
            joint.connectedAnchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
            joint.distance = retractedDistance = Vector2.Distance(targetA.transform.TransformPoint(joint.anchor), _pivot.transform.position);
        }
    }

    public override void DisconnectAllPivots()
    {
        base.DisconnectAllPivots();

        transform.localScale = Vector3.one;
        targetA = targetB = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        Destroy(joint);
    }
}