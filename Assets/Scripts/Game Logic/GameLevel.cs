using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    private bool levelComplete_;

    private float levelTime_;

    private GameLevelData gameLevelData_;

    public GameObject successTextPrefab;

    public float cameraSpeed = 50.0f;

    public float cameraSizeMin = 1.0f;
    public float cameraSizeMax = 20.0f;

    private Vector3 cameraTargetPosition_;

    private GameObject mainCameraObject_;

    private Camera mainCamera_;

    private float cameraTargetSize_;

    private float origCameraSize_;

    private Vector3 origCameraPosition_;

    private float lastMouseScrollDelta_;

    private GameObject robotHead_ { get { return HeadObject.activeHead.gameObject; } }

    private GameObject buildZone_;
    
    private GameObject finishLine_;


    private void Awake()
    {
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
                origCameraSize_ = mainCamera_.fieldOfView;
                origCameraPosition_ = mainCameraObject_.transform.position;
                cameraTargetSize_ = origCameraSize_;
                cameraTargetPosition_ = mainCamera_.transform.position;
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

        finishLine_ = GameObject.FindWithTag("Finish");
        if (finishLine_)
        {
            GoalScript goal = finishLine_.GetComponent<GoalScript>();
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

        lastMouseScrollDelta_ = Input.mouseScrollDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager gameManager = GameManager.instance;

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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gameManager.LoadNextLevel();
            }

            if (mainCamera_)
            {
                // If in play mode, then camera follows robot head
                if (gameManager.playMode)
                {
                    if (robotHead_)
                    {
                        cameraTargetPosition_ = new Vector3(robotHead_.transform.position.x, robotHead_.transform.position.y, mainCameraObject_.transform.position.z);
                        cameraTargetSize_ = Mathf.Clamp(cameraTargetSize_ - Input.mouseScrollDelta.y, cameraSizeMin, cameraSizeMax);
                    }
                }
                // Otherwise allow player to move (unless they're dragging an object)
                else if (!GameManager.mouse.objectPickedUp)
                {
                    Vector3 oldPosition = mainCameraObject_.transform.position;
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        cameraTargetPosition_ = new Vector3(cameraTargetPosition_.x - cameraSpeed * Time.deltaTime, cameraTargetPosition_.y, cameraTargetPosition_.z);
                    }

                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        cameraTargetPosition_ = new Vector3(cameraTargetPosition_.x + cameraSpeed * Time.deltaTime, cameraTargetPosition_.y, cameraTargetPosition_.z);
                    }

                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        cameraTargetPosition_ = new Vector3(cameraTargetPosition_.x, cameraTargetPosition_.y - cameraSpeed * Time.deltaTime, cameraTargetPosition_.z);
                    }

                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        cameraTargetPosition_ = new Vector3(cameraTargetPosition_.x, cameraTargetPosition_.y + cameraSpeed * Time.deltaTime, cameraTargetPosition_.z);
                    }
                    if (Input.GetMouseButton(2))
                    {
                        cameraTargetPosition_ += new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
                    }


                    //cameraTargetSize_ *= (Input.mouseScrollDelta.y - lastMouseScrollDelta_);
                    cameraTargetSize_ = Mathf.Clamp(cameraTargetSize_ - Input.mouseScrollDelta.y, cameraSizeMin, cameraSizeMax);

                }

            }

        }

        lastMouseScrollDelta_ = Input.mouseScrollDelta.y;

        float maxChange = cameraSpeed*Time.deltaTime;
        mainCamera_.fieldOfView = Mathf.MoveTowards(mainCamera_.fieldOfView, cameraTargetSize_, maxChange);
        mainCameraObject_.transform.position = Vector3.MoveTowards(mainCameraObject_.transform.position, cameraTargetPosition_, maxChange);
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
        cameraTargetPosition_ = finishLine_.transform.position;
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
            cameraTargetSize_ = origCameraSize_;
            cameraTargetPosition_ = origCameraPosition_;
        }
    }

    public void OnPlayModeActivated()
    {
        cameraTargetSize_ = origCameraSize_;
    }
}