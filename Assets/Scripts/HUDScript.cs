using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDScript : MonoBehaviour
{
    public static HUDScript singleton;
    public Text p1ScoreTxt;
    public Text p2ScoreTxt;
    public Image background;
    public Text winnerTxt;
    public Text restartTxt;
    public Image restartBg;

    public int p1Score;
    public int p2Score;
    bool refreshHUD;
    int winner; //P1WIN: winner = 1, P2WIN: winner = 2, DRAW: winner = 3;
    bool gameOver;
    Vector3 beginPos;

    void Awake()
    {
        singleton = this;
    }

	// Use this for initialization
	void Start ()
    {
        p1Score = 0;
        p2Score = 0;
        beginPos = background.transform.localPosition;
        refreshHUD = true;
        gameOver = false;
        winner = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (winner > 0 && Input.GetButtonDown("Start")) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

	    if(refreshHUD)
        {
            UpdateHUD();
            DetermineWinner();
        }

        if(gameOver)
        {
            winnerTxt.enabled = true;
            background.enabled = true;
            restartBg.enabled = true;
            restartTxt.enabled = true;
            DisplayWinner();
            background.transform.localPosition = ((Vector3.up * 1.3f) * Mathf.Sin(Time.time * 2));

        }
        else
        {
            winnerTxt.enabled = false;
            background.enabled = false;
            restartTxt.enabled = false;
            restartBg.enabled = false;
        }
	}

    public void UpdateScore(int score, int playerID)
    {
        if(playerID == 1)
        {
            p2Score = score;

            if(p1Score > 15)
            {
                p1Score = 15;
            }
        }
        else if(playerID == 2)
        {
            p1Score = score;

            if (p2Score > 15)
            {
                p2Score = 15;
            }
        }

        refreshHUD = true;
    }

    void UpdateHUD()
    {
        if (p1Score > 15)
        {
            p1Score = 15;
        }

        if (p2Score > 15)
        {
            p2Score = 15;
        }

        p1ScoreTxt.text = "" + p1Score;
        p2ScoreTxt.text = "" + p2Score;
        refreshHUD = false;
    }

    void DetermineWinner()
    {
        if(p1Score == 15 && p2Score < 15)
        {
            winner = 1;
            gameOver = true;
        }

        if(p2Score == 15 && p1Score < 15)
        {
            winner = 2;
            gameOver = true;
        }

        if(p2Score == 15 && p1Score == 15)
        {
            winner = 3;
            gameOver = true;
        }
    }

    void DisplayWinner()
    {
        switch(winner)
        {
            case 0:
                winnerTxt.text = "";
                break;
            case 1:
                winnerTxt.text = "Player 1 wins";
                break;
            case 2:
                winnerTxt.text = "Player 2 wins";
                break;
            case 3:
                winnerTxt.text = "Draw";
                break;
        }
    }

    public void ResetHUD()
    {
        //incase we don't reload the scene
        p1Score = 0;
        p2Score = 0;
        beginPos = background.transform.localPosition;
        refreshHUD = true;
        gameOver = false;
    }
}
