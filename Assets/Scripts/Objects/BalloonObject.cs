using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonObject : BuildableObject
{
    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        _pivot.joint = _otherObject.gameObject.AddComponent<WheelJoint2D>();
        WheelJoint2D wheelJoint = _pivot.joint as WheelJoint2D;
        wheelJoint.connectedBody = rb;
        wheelJoint.autoConfigureConnectedAnchor = false;
        wheelJoint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
        wheelJoint.connectedAnchor = transform.InverseTransformPoint(_pivot.transform.position);
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);
        
        Destroy(_pivot.joint);
    }
}