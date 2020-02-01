using System;

[Serializable]
public class GameLevelData
{
    // Name of scene in Unity (required to load level correctly)
    public string sceneName;

    // Human-readable level name (to be displayed on screen)
    public string levelName;

    // Level number (to be displayed on screen)
    public int levelNumber = -1;

    // Required score to successfully pass the level
    public int requiredScore = 0;


    // Has the level been unlocked (i.e. by successfully completing the previous level)?
    private bool levelUnlocked_ = false;
    public bool levelUnlocked
    {
        get {return levelUnlocked_;}
    }

    // High-score for completing this level
    private int highScore = 0;

    // Name of player who achieved high-score
    private string highScoreName;

    public void UnlockLevel()
    {
        levelUnlocked_ = true;
    }

}
