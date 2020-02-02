using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorObject : ActionObject
{
    public WheelJoint2D wheelJoint;
    public float motorSpeed, breakingForce = Mathf.Infinity, dampening = 0.9f, frquency = 9;

    protected override void OnActionStart()
    {
        if (wheelJoint) { wheelJoint.useMotor = true; }
    }

    protected override void OnActionEnd()
    {
        if (wheelJoint) { wheelJoint.useMotor = false; }
    }

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        wheelJoint = _otherObject.gameObject.AddComponent<WheelJoint2D>();
        wheelJoint.connectedBody = rb;
        wheelJoint.motor = new JointMotor2D() { maxMotorTorque = 10000, motorSpeed = motorSpeed };
        wheelJoint.useMotor = actionKeyDown;
        wheelJoint.autoConfigureConnectedAnchor = false;
        wheelJoint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
        wheelJoint.frquency = frquency;
        wheelJoint.breakForce = breakingForce;
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        if (wheelJoint) { Destroy(wheelJoint); }
    }
}