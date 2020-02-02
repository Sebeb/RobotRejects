using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelObject : BuildableObject
{
    public WheelJoint2D wheelJoint;
    public float breakingForce = Mathf.Infinity;

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        wheelJoint = _otherObject.gameObject.AddComponent<WheelJoint2D>();
        wheelJoint.connectedBody = rb;
        wheelJoint.autoConfigureConnectedAnchor = false;
        wheelJoint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
        wheelJoint.breakForce = breakingForce;
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        if (wheelJoint) { Destroy(wheelJoint); }
    }
}