using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class RoboManager : MonoBehaviour
{
    public GameObject roboParent;
    [SerializeField, ReadOnly] private GameObject roboClone;

    private void Awake()
    {
        GameManager.instance.enterPlayMode += OnEnterPlayMode;
        GameManager.instance.enterBuildMode += OnEnterBuildMode;
    }

    private void OnEnterPlayMode()
    {
        roboClone = Instantiate(roboParent);
        roboParent.SetActive(false);
    }

    private void OnEnterBuildMode()
    {
        Destroy(roboClone);
        roboParent.SetActive(true);
    }
}