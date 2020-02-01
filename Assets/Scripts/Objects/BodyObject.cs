using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyObject : BuildableObject
{
    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotPoint _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);
    }

    public override void DisconnectPivot(PivotPoint _pivot)
    {
        base.DisconnectPivot(_pivot);
    }

}
