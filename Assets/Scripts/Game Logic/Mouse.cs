using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    public LayerMask layerMask;
    public TextMeshPro tooltipTMP;
    public SpriteRenderer tooltipSR;

    public BuildableObject target
    {
        set
        {
            if (target == value) { return; }

            if (target) { _target.SetHighlight(false); }
            _target = value;
            if (target)
            {
                target.SetHighlight(true);
            }
            actionTarget = target as ActionObject;
        }
        get { return _target; }
    }

    public Color[] recordingColors;
    public float recordingColorSpeed;
    private BuildableObject _target;

    private TargetJoint2D joint;
    private Rigidbody2D rb2d;
    private ActionObject actionTarget;
    private bool recording { get { return actionTarget != null && actionTarget.recordingInput && !GameManager.instance.playMode; } }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (GameManager.instance.playMode) { return; }
        if (recording)
        {
            int colorId = (int)(Time.time * recordingColorSpeed) % recordingColors.Length;
            tooltipSR.color = recordingColors[colorId];
            return;
        }

        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
        if (joint) { joint.target = transform.position; }

        if (Input.GetMouseButton(0) && !joint) { Pickup(); }
        else if (!Input.GetMouseButton(0) && joint) { Drop(); }
        else if (Input.GetMouseButtonDown(1)) { RecordKey(); }
        else if (!joint) { CheckTarget(); }
        UpdateTooltip();
    }

    private void UpdateTooltip()
    {
        if (actionTarget) { tooltipTMP.text = actionTarget.actionKey == "" || recording ? "?" : actionTarget.actionKey; }
        tooltipTMP.color = tooltipTMP.color.SetA(Mathf.MoveTowards(tooltipTMP.color.a, actionTarget != null ? 1 : 0, Time.deltaTime * 2.0f));

        tooltipSR.color = Color.white.SetA(tooltipTMP.color.a);
    }

    private void CheckTarget()
    {
        target = BuildableObject.CheckForConnection(transform.position, null, layerMask);
    }

    private void RecordKey()
    {
        if (!actionTarget) { return; }

        actionTarget.recordingInput = true;
        tooltipTMP.text = "?";
    }

    private void Pickup()
    {
        if (!target) { return; }

        if (joint != null) { Drop(); }

        target.Pickup();

        joint = target.gameObject.AddComponent<TargetJoint2D>();
        joint.connectedBody = rb2d;
        // joint.distance = 0.05f;
        joint.autoConfigureTarget = false;
        joint.anchor = target.transform.InverseTransformPoint(transform.position);
    }

    private void Drop()
    {
        if (target)
        {
            target.Drop();
            target = null;
        }
        if (joint) { Destroy(joint); }
    }
}