using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    private bool goalReached_;
    private GameLevel gameLevel_;

    /*void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "RobotHead" && goalReached_ == false)
        {
            goalReached_ = true;
            Debug.Log("Reached goal");
            if (gameLevel_)
            {
                gameLevel_.OnLevelComplete();
            }
        } 
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "RobotHead" && goalReached_ == false)
        {
            goalReached_ = true;
            Debug.Log("Reached goal");
            if (gameLevel_)
            {
                gameLevel_.OnLevelComplete();
            }
        }    
    }

    public void RegisterGameLevel(GameLevel gameLevel)
    {
        if (gameLevel)
        {
            gameLevel_ = gameLevel;
        }
        else
        {
            Debug.LogError("Invalid GameLevel reference passed");
        }
    }
}
