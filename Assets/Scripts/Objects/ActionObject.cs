using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionObject : BuildableObject
{
    public bool recordingKey, enableInput;
    public string actionKey;
    public bool actionKeyDown;

    protected override void Awake()
    {
        base.Awake();

        GameManager.instance.enterBuildMode += DisableActionKey;
    }

    protected override void Update()
    {
        base.Update();

        if (recordingKey) { CheckRecordKey(); }
        if (enableInput && actionKey != "" && GameManager.instance.playMode) { CheckActionKey(); }
    }

    private void DisableActionKey() => actionKeyDown = false;
    private void CheckActionKey()
    {
        if (Input.GetKey(actionKey))
        {
            if (!actionKeyDown)
            {
                OnActionStart();
                actionKeyDown = true;
            }
        }
        else
        {
            if (actionKeyDown)
            {
                OnActionEnd();
                actionKeyDown = false;
            }
        }
    }

    protected abstract void OnActionStart();
    protected abstract void OnActionEnd();

    private void CheckRecordKey()
    {
        if (recordingKey)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                actionKey = "";
                recordingKey = false;
            }
            else if (Input.anyKeyDown)
            {
                if (Input.inputString == "" || Input.inputString == "space") { return; }
                actionKey = Input.inputString;
                recordingKey = false;
            }
        }
    }
}