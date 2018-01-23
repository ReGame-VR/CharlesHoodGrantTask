using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskCanvas : MonoBehaviour {

    // The canvas for the task
    [SerializeField]
    private Canvas canvas;

    // The text displaying the target score
    [SerializeField]
    private Text targetScoreText;

    // The text displaying the trial score
    [SerializeField]
    private Text trialScoreText;

    // The canvas for the task
    [SerializeField]
    private Text totalScoreText;

    void Start()
    {
        canvas.enabled = true;
        targetScoreText.text = "Target Score: 0";
        trialScoreText.text = "Trial Score: 0";
        totalScoreText.text = "Total Score: 0";
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
    /// Updates the score displayed on the target score text 
    /// </summary>
    /// <param name="score"></param> given target score
    public void UpdateTargetScoreText(float score)
    {
        targetScoreText.text = "Target Score: " + Mathf.Round(score).ToString();
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

}
