using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionObject : BuildableObject
{
    public bool selected = true, recordingInput;
    public string actionKey;
    public bool actionKeyDown;

    void Update()
    {
        if (selected) { CheckRecordInput(); }
        if (actionKey != "") { CheckActionKey(); }
    }

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

    private void CheckRecordInput()
    {
        if (Input.GetKeyDown(KeyCode.Return)) { recordingInput = !recordingInput; return; }

        if (recordingInput && Input.anyKeyDown)
        {
            if (Input.inputString == "") { return; }
            actionKey = Input.inputString;
            recordingInput = false;
        }
    }
}