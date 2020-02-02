using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelObject : BuildableObject
{
    public WheelJoint2D wheelJoint;
    public float breakingForce = Mathf.Infinity, dampening = 0.9f, frquency = 9;

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        wheelJoint = _otherObject.gameObject.AddComponent<WheelJoint2D>();
        wheelJoint.connectedBody = rb;
        wheelJoint.autoConfigureConnectedAnchor = false;
        wheelJoint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
        wheelJoint.breakForce = breakingForce;
        wheelJoint.suspension = new JointSuspension2D() { frequency = frquency, dampingRatio = dampening };
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        if (wheelJoint) { Destroy(wheelJoint); }
    }
}