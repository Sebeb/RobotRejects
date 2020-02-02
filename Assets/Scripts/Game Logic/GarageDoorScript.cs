using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoorScript : MonoBehaviour
{
    public Vector3 deltaPositionOpen = new Vector3(0.0f, 12.0f, 0.0f);

    public float moveSpeed = 10.0f;

    private Vector3 originalPosition_;

    private Vector3 openPosition_;

    private Vector3 targetPosition_;


    void Awake()
    {
        originalPosition_ = transform.position;
        openPosition_ = originalPosition_ + deltaPositionOpen;
        targetPosition_ = originalPosition_;

        GameManager gameManager = GameManager.instance;
        gameManager.enterBuildMode += OnBuildModeActivated;
        gameManager.enterPlayMode += OnPlayModeActivated;    
    }


    void Update()
    {
        float maxChange = moveSpeed*Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition_, maxChange);
    }


    void OnBuildModeActivated()
    {
        targetPosition_ = originalPosition_;
    }


    void OnPlayModeActivated()
    {
        targetPosition_ = openPosition_;
    }
}
