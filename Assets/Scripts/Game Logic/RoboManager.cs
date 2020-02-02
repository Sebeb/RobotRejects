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
        roboParent = gameObject.AddChild("Parts");
    }

    private void Start()
    {
        //Gather the robots;
        foreach (BuildableObject bo in GameObject.FindObjectsOfType<BuildableObject>())
        {
            if (bo.transform.parent == null)
            {
                bo.transform.parent = roboParent.transform;
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Reset"))
        {
            if (go.transform.parent == null)
            {
                go.transform.parent = roboParent.transform;
            }
        }
    }

    private void OnDisable()
    {
        GameManager.instance.enterPlayMode -= OnEnterPlayMode;
        GameManager.instance.enterBuildMode -= OnEnterBuildMode;
    }

    private void OnEnterPlayMode()
    {
        roboClone = Instantiate(roboParent, roboParent.transform.position, roboParent.transform.rotation);
        roboParent.SetActive(false);
    }

    private void OnEnterBuildMode()
    {
        Destroy(roboClone);
        roboParent.SetActive(true);
    }
}