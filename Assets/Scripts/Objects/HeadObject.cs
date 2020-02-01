using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadObject : BuildableObject
{
    public FixedJoint2D joint;

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        joint = _otherObject.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = rb;
        joint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        Destroy(joint);
    }
}