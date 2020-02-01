using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorObject : ActionObject
{
    public WheelJoint2D wheelJoint;
    public Rigidbody2D wheelRb;
    public float motorSpeed;

    protected override void OnActionStart()
    {
        if (wheelJoint) { wheelJoint.useMotor = true; }
    }

    protected override void OnActionEnd()
    {
        if (wheelJoint) { wheelJoint.useMotor = false; }
    }

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotPoint _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        wheelJoint = _otherObject.gameObject.AddComponent<WheelJoint2D>();
        wheelJoint.connectedBody = wheelRb;
        wheelJoint.motor = new JointMotor2D() { maxMotorTorque = 10000, motorSpeed = motorSpeed };
        wheelJoint.useMotor = actionKeyDown;
        wheelJoint.autoConfigureConnectedAnchor = false;
        wheelJoint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
    }

    public override void DisconnectPivot(PivotPoint _pivot)
    {
        base.DisconnectPivot(_pivot);
        
        Destroy(wheelJoint);
    }
}