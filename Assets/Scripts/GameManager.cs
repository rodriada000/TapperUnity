using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScoreKey
{
    EmptyMug,
    Tip,
    Customer,
    HardCustomer,
    HarderCustomer,
    LevelFinish
}

public enum GameMode
{
    SinglePlayer,
    TwoPlayer
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public LevelManager levelManager;

    public LevelUIManager levelUIManager;

    private Dictionary<ScoreKey, int> _scoreTable;

    public Dictionary<ScoreKey, int> ScoreTable
    {
        get
        {
            if (_scoreTable == null)
            {
                _scoreTable = new Dictionary<ScoreKey, int>();
                _scoreTable.Add(ScoreKey.EmptyMug, 100);
                _scoreTable.Add(ScoreKey.Tip, 1500);
                _scoreTable.Add(ScoreKey.Customer, 50);
                _scoreTable.Add(ScoreKey.HardCustomer, 75);
                _scoreTable.Add(ScoreKey.HarderCustomer, 100);
                _scoreTable.Add(ScoreKey.LevelFinish, 1000);
            }

            return _scoreTable;
        }
    }

    public GameMode SelectedGameMode;

    public int CurrentPlayer = 1;

    public int PlayerOneScore;
    public int PlayerOneCurrentLevel;
    public int PlayerOneLives;


    public int PlayerTwoScore;
    public int PlayerTwoCurrentLevel;
    public int PlayerTwoLives;

    // Awake is called before the first frame update
    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        } 
        else if (instance != this) 
        {
            Destroy(gameObject);
        }

        levelManager = GetComponent<LevelManager>();
        levelUIManager = GetComponent<LevelUIManager>();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(bool isTwoPlayer)
    {
        if (isTwoPlayer)
        {
            SelectedGameMode = GameMode.TwoPlayer;
        }
        else
        {
            SelectedGameMode = GameMode.SinglePlayer;
        }

        CurrentPlayer = 1;

        PlayerOneScore = 0;
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);


        PlayerTwoScore = 0;
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);

        PlayerOneCurrentLevel = 1;
        PlayerTwoCurrentLevel = 1;
        levelUIManager.SetCurrentLevelText(PlayerOneCurrentLevel);

        PlayerOneLives = 3;
        PlayerTwoLives = 3;

        levelManager.CurrentLevel = levelManager.AllLevels[0];
    }

    internal void AddToCurrentPlayerScore(ScoreKey toAdd)
    {
        if (CurrentPlayer == 1)
        {
            AddToPlayerOneScore(toAdd);
        }
        else
        {
            AddToPlayerTwoScore(toAdd);
        }
    }

    internal void AddToPlayerOneScore(ScoreKey toAdd)
    {
        PlayerOneScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
    }

    internal void AddToPlayerTwoScore(ScoreKey toAdd)
    {
        PlayerTwoScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerTwoScoreText(PlayerTwoScore);
    }

    
    internal void PlayerLost()
    {
        // Lose a life
        if (CurrentPlayer == 1)
        {
            PlayerOneLives--;
            Debug.Log(string.Format("Player 1 Lives: {0}", PlayerOneLives));
        }
        else
        {
            PlayerTwoLives--;
            Debug.Log(string.Format("Player 2 Lives: {0}", PlayerTwoLives));
        }



        // switch players if two player
        if (SelectedGameMode == GameMode.TwoPlayer)
        {
            SwitchPlayers();
        }

        levelManager.PlayerMissedCustomer = false;
        levelManager.PlayerMissedEmptyMug = false;
        levelManager.PlayerThrewExtraMug = false;

        RestartLevelScene();
    }

    private void SwitchPlayers()
    {
        if (CurrentPlayer == 1)
        {
            CurrentPlayer = 2;
            levelManager.CurrentLevel = levelManager.AllLevels[PlayerTwoCurrentLevel - 1];
        }
        else
        {
            CurrentPlayer = 1;
            levelManager.CurrentLevel = levelManager.AllLevels[PlayerOneCurrentLevel - 1];
        }
    }

    private void RestartLevelScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
