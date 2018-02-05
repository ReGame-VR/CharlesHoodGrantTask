using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskCanvas : MonoBehaviour {

    // The canvas for the task
    [SerializeField]
    private Canvas canvas;

    // The text displaying the trial score
    [SerializeField]
    private Text trialScoreText;

    // The text displaying the total (cumulative) score
    [SerializeField]
    private Text totalScoreText;

    // The text that displays when the game is over
    [SerializeField]
    private Text gameOverText;

    void Start()
    {
        canvas.enabled = true;
        trialScoreText.text = "Trial Score: 0";
        totalScoreText.text = "Total Score: 0";
        gameOverText.text = "";
    }

    /// <summary>
    /// Turns on the canvas
    /// </summary>
    public void TurnOnCanvas()
    {
        canvas.enabled = true;
    }

    /// <summary>
    /// Turns off the canvas
    /// </summary>
    public void TurnOffCanvas()
    {
        canvas.enabled = false;
    }

    /// <summary>
    /// Updates the score displayed on the trial score text 
    /// </summary>
    /// <param name="score"></param> given trial score
    public void UpdateTrialScoreText(float score)
    {
        trialScoreText.text = "Trial Score: " + Mathf.Round(score).ToString();
    }

    /// <summary>
    /// Updates the score displayed on the total score text 
    /// </summary>
    /// <param name="score"></param> given total score
    public void UpdateTotalScoreText(float score)
    {
        totalScoreText.text = "Total Score: " + Mathf.Round(score).ToString();
    }

    /// <summary>
    /// Enables the game over text to be displayed when the game is over
    /// </summary>
    public void EnableGameOverText()
    {
        gameOverText.text = "Game Over! Press Space to Restart";
    }

}
