using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance_;

    public static GameManager instance
    {
        get
        {
            if (!instance_)
            {

                instance_ = (Instantiate(Resources.Load("GameManager")) as GameObject).GetComponent<GameManager>();
            }
            return instance_;
        }
    }

    // Id of the current level in the array
    private int currentLevelId;

    // Total number of levels in the game
    private int numOfLevels;

    // Data of the current level
    private GameLevelData currentLevelData_;
    public GameLevelData currentLevelData
    {
        get { return currentLevelData_; }
    }

    // Name of MainMenu scene (for when we return after quitting/game-over)
    public string mainMenuSceneName;

    // Array of all GameLevels to be played
    //public string[] gameLevelNames;
    public GameLevelData[] gameLevelData;

    public bool playMode;
    public delegate void GameEvent();
    public GameEvent enterPlayMode, enterBuildMode;

    void Awake()
    {
        instance_ = this;
        DontDestroyOnLoad(this.gameObject);
        numOfLevels = gameLevelData.Length;

        // Automatically unlock first level (if available)
        if (numOfLevels > 0)
        {
            gameLevelData[0].UnlockLevel();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playMode = !playMode;
            if (playMode) { enterPlayMode?.Invoke(); }
            else { enterBuildMode?.Invoke(); }
        }
    }

    public void StartGame()
    {
        currentLevelId = 0;
        LoadNextLevel();
    }

    public void GameOver()
    {

    }

    public void LoadNextLevel()
    {
        if (currentLevelId < numOfLevels)
        {
            playMode = false;
            currentLevelData_ = gameLevelData[currentLevelId++];
            currentLevelData_.UnlockLevel();
            //currentSceneName = gameLevelData[currentLevelId++].sceneName;
            SceneManager.LoadScene(currentLevelData_.sceneName, LoadSceneMode.Single);
            Debug.Log("Loading next scene : " + currentLevelData_.levelName);
        }
        else
        {
            Debug.LogError("No more scenes to be loaded");
            SceneManager.LoadScene(mainMenuSceneName, LoadSceneMode.Single);
        }
    }

}