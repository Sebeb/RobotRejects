using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    private bool levelComplete_;
    private float levelTime_;
    private GameLevelData gameLevelData_;

    public BuildableObject[] initialObjectArray;

    public GameObject buildZoneObject;
    public GameObject finishLine;

    public GameObject successTextPrefab;

    private void Awake()
    {
        if (buildZoneObject == null)
        {
            Debug.LogError("Cannot find BuildZone object in this level");
        }

        if (finishLine)
        {
            GoalScript goal = finishLine.GetComponent<GoalScript>();
            if (goal)
            {
                goal.RegisterGameLevel(this);
            }
            else
            {
                Debug.LogError("FinishLine object has no GoalScript attached");
            }
        }
        else
        {
            Debug.LogError("Cannot find FinishLine in this level");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        OnLevelLoaded();
    }

    // Update is called once per frame
    void Update()
    {
        if (levelComplete_)
        {
            if (Input.GetKeyDown("n"))
            {
                GameManager gameManager = GameManager.instance;
                gameManager.LoadNextLevel();
            }
        }
        else
        {
            levelTime_ += Time.deltaTime;
        }
    }

    public void OnLevelLoaded()
    {
        GameManager gameManager = GameManager.instance;
        gameLevelData_ = gameManager.currentLevelData;
        if (gameLevelData_ != null) { Debug.Log("Successfully loaded level " + gameLevelData_.sceneName); }
    }

    public void OnLevelComplete()
    {
        levelComplete_ = true;
        Debug.Log("Completed level in " + levelTime_ + " second(s)");

        if (successTextPrefab)
        {
            Instantiate(successTextPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }

        //GameManager gameManager = GameManager.instance;
        //gameManager.LoadNextLevel();
    }
}