﻿using System;
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
    private bool recording { get { return actionTarget != null && actionTarget.recordingInput; } }

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

        if (Input.GetMouseButtonDown(0)) { Pickup(); }
        else if (Input.GetMouseButtonUp(0)) { Drop(); }
        else if (Input.GetMouseButtonDown(1)) { RecordKey(); }
        else { CheckTarget(); }
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);

        if (!hit)
        {

            target = null;
            return;
        }

        target = BuildableObject.CheckForConnection(transform.position);
    }

    private void RecordKey()
    {
        if (!actionTarget) { return; }

        actionTarget.recordingInput = true;
        tooltipTMP.text = "?";
    }

    private void Pickup()
    {
        if (joint) { Drop(); }

        if (!target) { return; }

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
            Destroy(joint);
            target.Drop();
            target = null;
        }
    }
}