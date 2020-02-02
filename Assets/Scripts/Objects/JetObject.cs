using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetObject : ActionObject
{
    public FixedJoint2D joint;
    public float initialForce, continuousForce;

    public ParticleSystem boostingPS;
    private ParticleSystem.EmissionModule boostingEmission;

    protected void Start()
    {
        boostingEmission = boostingPS.emission;
        boostingEmission.enabled = false;
        if (GameManager.instance.playMode)
        {
            foreach (BuildableObject bo in GetAllConnectedObjects())
            {
                ActionObject ao = bo as ActionObject;
                if (ao) { ao.enableInput = true; }
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (actionKeyDown) { rb.AddForce(transform.right * continuousForce); }
    }

    protected override void OnActionStart()
    {
        boostingEmission.enabled = true;
        rb.AddForce(transform.right * initialForce, ForceMode2D.Impulse);
    }

    protected override void OnActionEnd() { boostingEmission.enabled = false; }

    public override void ConnectPivotToObject(BuildableObject _otherObject, PivotObject _pivot)
    {
        base.ConnectPivotToObject(_otherObject, _pivot);

        joint = _otherObject.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = rb;
        joint.anchor = _otherObject.transform.InverseTransformPoint(_pivot.transform.position);
        joint.autoConfigureConnectedAnchor = false;
    }

    public override void DisconnectPivot(PivotObject _pivot)
    {
        base.DisconnectPivot(_pivot);

        Destroy(joint);
    }

}