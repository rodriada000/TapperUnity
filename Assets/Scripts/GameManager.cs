using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScoreKey
{
    EmptyMug,
    Tip,
    Customer,
    HardCustomer,
    HarderCustomer,
    LevelFinish
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

    public int PlayerOneScore;

    public int PlayerOneCurrentLevel;

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

        PlayerOneScore = 0;
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);

        PlayerOneCurrentLevel = 1;
        levelUIManager.SetCurrentLevelText(PlayerOneCurrentLevel);

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void AddToPlayerOneScore(ScoreKey toAdd)
    {
        PlayerOneScore += ScoreTable[toAdd];
        levelUIManager.SetPlayerOneScoreText(PlayerOneScore);
    }
}
