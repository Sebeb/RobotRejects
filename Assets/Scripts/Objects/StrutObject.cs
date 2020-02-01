using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrutObject : BuildableObject
{
    public DistanceJoint2D joint;
    public float breakForce;
    public Rigidbody2D targetA, targetB;

    protected override void Update()
    {
        base.Update();

        if (targetA && targetB) //Move and scale spring between anchors
        {
            Vector2 pointA = targetA.transform.TransformPoint(joint.anchor);
            Vector2 pointB = targetB.transform.TransformPoint(joint.connectedAnchor);
            Vector2 a2b = pointB - pointA;
            transform.position = pointA + (a2b * 0.5f);
            transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.right, a2b);
        }
    }

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        rb.bodyType = RigidbodyType2D.Static;

        if (targetA == null)
        {
            targetA = _otherObject.rb;
            joint = _otherObject.gameObject.AddComponent<DistanceJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
            joint.enableCollision = true;
            joint.breakForce = breakForce;
        }
        else
        {
            targetB = _otherObject.rb;
            joint.connectedBody = targetB;
            joint.connectedAnchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
            print(_otherObject.transform.InverseTransformPoint(_pivot.transform.position));
            joint.autoConfigureDistance = false;
        }
    }

    public override void DisconnectAllPivots()
    {
        base.DisconnectAllPivots();

        targetA = targetB = null;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        Destroy(joint);
    }
}