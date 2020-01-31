using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorObject : ActionObject
{
    public WheelJoint2D wheelJoint;
    public float motorSpeed;

    protected override void OnActionStart()
    {
        wheelJoint.useMotor = true;
    }

    protected override void OnActionEnd()
    {
        wheelJoint.useMotor = false;
    }

    public override void ConnectToObject(BuildableObject _otherObject)
    {
        throw new System.NotImplementedException();
    }
}