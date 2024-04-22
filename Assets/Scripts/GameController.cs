
using System.Collections;
using UnityEngine;

//
// Blockade 1976 v2021.02.24
//
// 2021.02.15
//

public class GameController : MonoBehaviour
{
    public static GameController gameController;

    public Transform gameOverText;

    public const int GRID_WIDTH = 29;
    public const int GRID_HEIGHT = 25;

    private const int WINNING_SCORE = 6;

    private int player1Score;
    private int player2Score;
    public int highScore;

    public bool crashed;
    public bool canPlay;
    public bool gameOver;



    private void Awake()
    {
        gameController = this;
    }


    void Start()
    {
        Startup();
    }


    void Update()
    {
        GameLoop();
    }


    private void Startup()
    {
        gameOver = true;
        crashed = false;
        canPlay = false;

        player1Score = 0;
        player2Score = 0;
        highScore = 0;

        ScoreController.scoreController.InitialiseScores();

        gameOverText.gameObject.SetActive(true);
    }


    private void InitialiseSnakeBodyParts()
    {
        // deactivate ammo game objects;
        for (int i = 0; i < Player1ObjectPooler.player1ObjectPooler.pooledObject.Count; i++)
        {
            Player1ObjectPooler.player1ObjectPooler.pooledObject[i].SetActive(false);
        }

        for (int i = 0; i < Player2ObjectPooler.player2ObjectPooler.pooledObject.Count; i++)
        {
            Player2ObjectPooler.player2ObjectPooler.pooledObject[i].SetActive(false);
        }
    }


    private void Initialise()
    {
        player1Score = 0;
        player2Score = 0;

        ScoreController.scoreController.InitialiseScores();

        InitialiseRestart();
    }


    private void InitialiseRestart()
    {
        InitialiseSnakeBodyParts();

        Player1Controller.player1Controller.Initialise();
        Player2Controller.player2Controller.Initialise();

        gameOverText.gameObject.SetActive(false);

        StartCoroutine(StartDelay());
    }


    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);

        canPlay = true;
        crashed = false;
        gameOver = false;
    }


    private void GameLoop()
    {
        if (gameOver)
        {
            UpdateHighScore();

            KeyboardController();
        }
    }


    private void KeyboardController()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartOnePlayer();
        }
    }


    private void StartOnePlayer()
    {
        Initialise();
    }


    public void UpdatePlayer1Score(int points)
    {
        player1Score += points;

        ScoreController.scoreController.UpdateScoreDisplay(player1Score, ScoreController.PLAYER_1);

        if (player1Score == WINNING_SCORE)
        {
            GameOver();
        }

        else
        {
            StartCoroutine(RestartDelay());
        }
    }


    public void UpdatePlayer2Score(int points)
    {
        player2Score += points;

        ScoreController.scoreController.UpdateScoreDisplay(player2Score, ScoreController.PLAYER_2);

        if (player2Score == WINNING_SCORE)
        {
            GameOver();
        }

        else
        {
            StartCoroutine(RestartDelay());
        }
    }


    private IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(5f);

        InitialiseRestart();
    }


    private void GameOver()
    {
        gameOver = true;

        canPlay = false;

        gameOverText.gameObject.SetActive(true);
    }


    private void UpdateHighScore()
    {
        if (player1Score > highScore)
        {
            highScore = player1Score;
        }

        ScoreController.scoreController.UpdateScoreDisplay(highScore, ScoreController.HIGH_SCORE);
    }


} // end of class
