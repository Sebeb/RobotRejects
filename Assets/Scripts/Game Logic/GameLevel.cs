using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    private bool levelComplete_;

    private float levelTime_;

    private GameLevelData gameLevelData_;

    public BuildableObject[] initialObjectArray;


    public GameObject finishLine;

    public GameObject successTextPrefab;

    public float cameraSpeed = 10.0f;

    private Vector3 cameraOffset_;

    private GameObject mainCameraObject_;
    private Camera mainCamera_;

    private float cameraSize_;

    private float origCameraSize_;

    private GameObject robotHead_;

    private GameObject buildZone_;


    private void Awake()
    {
        robotHead_ = GameObject.FindWithTag("RobotHead");
        if (robotHead_ == null)
        {
            Debug.LogError("Cannot find RobotHead object");
        }

        buildZone_ = GameObject.FindWithTag("BuildZone");
        if (buildZone_ == null)
        {
            Debug.LogError("Cannot find BuildZone object");
        }

        mainCameraObject_ = GameObject.FindWithTag("MainCamera");
        if (mainCameraObject_)
        {
            mainCamera_ = mainCameraObject_.GetComponent<Camera>();
            if (mainCamera_)
            {
                origCameraSize_ = mainCamera_.orthographicSize;
                cameraSize_ = origCameraSize_;
            }
            else
            {
                Debug.LogError("Cannot find camera component");
            }
        }
        else
        {
            Debug.LogError("Cannot find main camera object in scene");
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

        GameManager gameManager = GameManager.instance;
        gameManager.enterBuildMode += OnBuildModeActivated;
        gameManager.enterPlayMode += OnPlayModeActivated;
    }


    // Update is called once per frame
    void Update()
    {
        GameManager gameManager = GameManager.instance;

        Debug.Log("RobotHead : " + robotHead_.transform.position);

        if (levelComplete_)
        {
            if (Input.GetKeyDown("n"))
            {
                gameManager.LoadNextLevel();
            }
        }
        else
        {
            levelTime_ += Time.deltaTime;

            if (mainCamera_)
            {

                // If in play mode, then camera follows robot head
                if (gameManager.playMode)
                {
                    if (robotHead_)
                    {
                        mainCameraObject_.transform.position = 
                            new Vector3(robotHead_.transform.position.x, robotHead_.transform.position.y, mainCameraObject_.transform.position.z);
                    }
                }
                // Otherwise allow player to
                else
                {
                    Vector3 oldPosition = mainCameraObject_.transform.position;

                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        Vector3 newPosition = new Vector3(oldPosition.x - cameraSpeed*Time.deltaTime, oldPosition.y, oldPosition.z);
                        mainCameraObject_.transform.position = newPosition;
                    }

                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        Vector3 newPosition = new Vector3(oldPosition.x + cameraSpeed*Time.deltaTime, oldPosition.y, oldPosition.z);
                        mainCameraObject_.transform.position = newPosition;
                    }

                }

                // [TODO] Mouse wheel for zooming in and out


            }

        }
    }


    public void OnLevelLoaded()
    {
        GameManager gameManager = GameManager.instance;
        gameLevelData_ = gameManager.currentLevelData;
        if (gameLevelData_ != null)
        {
            Debug.Log("Successfully loaded level " + gameLevelData_.sceneName);
        }
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


    public void OnBuildModeActivated()
    {
        if (levelComplete_ == false)
        {
            cameraSize_ = origCameraSize_;
            mainCamera_.orthographicSize = cameraSize_;
            mainCameraObject_.transform.position = 
                new Vector3(buildZone_.transform.position.x, buildZone_.transform.position.y, mainCameraObject_.transform.position.z);
        }
    }


    public void OnPlayModeActivated()
    {
        cameraSize_ = origCameraSize_;
        mainCamera_.orthographicSize = cameraSize_;
    }
}
