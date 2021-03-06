﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles all UI updates
/// </summary>
public class UIController : MonoBehaviour {

    //Controller object that keeps track of score, hints, hazard/safety modes, and time
    public Controller m_Controller;

    #region Text Objects

    //Text object for our score
    public Text m_scoreText;

    //Text object the current hint
    public Text m_hintText;

    //Text object for the number of hints
    public Text m_hintCount;

    //Text object for our remaining time
    public Text m_timeText;

    //Text object for our current selection mode.
    public Text m_modeText;

    //Text to display for user tasks
    public Text m_DisplayText;

    //Text that shows how much score was added or subtracted
    public Text m_plusOrMinus;

    #endregion

    #region UI Timers
    //Amount of time that the plus or minus text displayed
    [SerializeField] private float m_plusMinusTimer;

    //Amount of time that the hint text is displayed.
    [SerializeField] private float m_hintTimer;
    #endregion

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start ()
    {
        //on start we initialize the first UI display
        UpdateUI();
    }

    /// <summary>
    /// UpdateUI Updates each relevant UI component to display new information
    /// </summary>
    public void UpdateUI()
    {
        UpdateSelectionMode();
        UpdateHintsText();
        UpdateScoreText();
        UpdateTimeText();
        UpdatePlusOrMinusText();
    }

    /// <summary>
    /// updates the text of our score changing indicator
    /// </summary>
    private void UpdatePlusOrMinusText()
    {
        //checking if the text time has expired
        //if so, reset the text object.
        m_plusMinusTimer -= Time.deltaTime;
        if (m_plusMinusTimer < 0)
            m_plusOrMinus.text = "";
    }

    /// <summary>
    /// Updates m_scoreText UI element to reflect new score.
    /// </summary>
    void UpdateScoreText()
    {
        m_scoreText.text = "Score: " + m_Controller.Score;
    }

    /// <summary>
    /// Updates m_plusOrMinus UI text to display the user's recent score change.
    /// </summary>
    /// <param name="scoreChange">The value by which the score is changing</param>
    public void DisplayPlusOrMinusText(int scoreChange)
    {
        m_plusMinusTimer = 2.0f;

        //increase in score
        if (scoreChange > 0)
        {
            m_plusOrMinus.color = Color.green;
            m_plusOrMinus.text = "+" + scoreChange;
        }
        else //decrease in score
        {
            m_plusOrMinus.color = Color.red;
            m_plusOrMinus.text = "" + scoreChange;
        }
    }

    /// <summary>
    /// Display a hint on the UI
    /// </summary>
    /// <param name="hint">The hint to display</param>
    public void DisplayHint(string hint)
    {
        //how long we want to display the hint for
        m_hintTimer = 10.0f;

        //set the m_hintText.text to the hint
        m_hintText.text = hint;
    }

    /// <summary>
    /// Converts the time format of our remaining ingame time to a more user-readable time.
    /// </summary>
    /// <returns>The format for displaying the time</returns>
    private string ConvertTimeFormat()
    {
        //getting how many seconds & minutes we have
        int seconds = (int)m_Controller.TimeLeft % 60;
        int minutes = (int)m_Controller.TimeLeft / 60;

        //returning the proper format of the time string
        if(seconds < 10)
            return minutes.ToString() + ":0" + seconds.ToString();       
        else
            return minutes.ToString() + ":" + seconds.ToString();
    }

    /// <summary>
    /// Updates the m_timeText UI element to display the proper time.
    /// </summary>
    void UpdateTimeText()
    {
        m_timeText.text = "Time: " + ConvertTimeFormat();
    }

    /// <summary>
    /// Updates the m_modeText UI element to display the proper selection mode.
    /// </summary>
    void UpdateSelectionMode()
    {
        string updatedText = "Selection Mode: ";

        //Hazardmode true is hazard tag; false is safety.
        if (m_Controller.HazardMode)
        {
            updatedText += "Hazard";
        }
        else
        {
            updatedText += "Safety";
        }

        m_modeText.text = updatedText;
    }

    /// <summary>
    /// Updates the m_hintText UI element to display the proper amount of hints remaining.
    /// Updates the current hint displaying on the screen.
    /// </summary>
    void UpdateHintsText()
    {
        //update the count of hints
        m_hintCount.text = "Hints: " + m_Controller.Hints;

        //decrementing the hint timer to stop displaying a hint after the timer reaches 0.
        m_hintTimer -= Time.deltaTime;
        if(m_hintTimer <= 0)
        {
            m_hintText.text = "";
        }
    }

    /// <summary>
    /// Updates m_scoreText UI element to reflect new score.
    /// </summary>
    public void UpdateDisplayText(string task)
    {
        m_DisplayText.text = task;
    }

    /// <summary>
    /// Sets up the keyboard after the ingame round is over.
    /// </summary>
    public void DisableUI()
    {
        //disable the ingame UI
        Canvas UICanvas = m_scoreText.GetComponentInParent<Canvas>();
        UICanvas.enabled = false;

        //setting our MonoBehavior's enabled flag to false stops our UIController from having Update() called.
        enabled = false;
    }
}
