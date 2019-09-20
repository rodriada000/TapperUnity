using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    public Text PlayerOneScoreText;

    public Text PlayerTwoScoreText;

    public Text CurrentLevelText;

    public RectTransform CurrentLevelTextBackground;


    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled.
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GetTextComponents();
        SetPlayerOneScoreText(GameManager.instance.PlayerOneScore);
        SetPlayerTwoScoreText(GameManager.instance.PlayerTwoScore);

        if (GameManager.instance.CurrentPlayer == 1)
        {
            SetCurrentLevelText(GameManager.instance.PlayerOneCurrentLevel);
        }
        else
        {
            SetCurrentLevelText(GameManager.instance.PlayerTwoCurrentLevel);
        }
    }

    void Awake()
    {
    }

    public void GetTextComponents()
    {
        GameObject textObj = GameObject.Find("Player1Score_Text");

        if (textObj != null)
        {
            PlayerOneScoreText = textObj.GetComponent<Text>();
        }

        GameObject player2TextObj = GameObject.Find("Player2Score_Text");

        if (player2TextObj != null)
        {
            PlayerTwoScoreText = player2TextObj.GetComponent<Text>();
        }

        GameObject lvlTextObj = GameObject.Find("Level_Text");

        if (lvlTextObj != null)
        {
            CurrentLevelText = lvlTextObj.GetComponent<Text>();
        }

        GameObject lvlPanelObj = GameObject.Find("LevelText_Panel");

        if (lvlPanelObj != null)
        {
            CurrentLevelTextBackground = lvlPanelObj.GetComponent<RectTransform>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerOneScoreText(int newScore)
    {
        if (PlayerOneScoreText != null)
        {
            PlayerOneScoreText.text = newScore.ToString();
        }
    }

    public void SetPlayerTwoScoreText(int newScore)
    {
        if (PlayerTwoScoreText != null)
        {
            PlayerTwoScoreText.text = newScore.ToString();
        }
    }

    public void SetCurrentLevelText(int newLevel)
    {
        if (CurrentLevelText == null || CurrentLevelTextBackground == null)
        {
            return;
        }

        CurrentLevelText.text = newLevel.ToString();

        // adjust the width of the text background to fit text width
        Vector2 newSize = CurrentLevelTextBackground.sizeDelta;

        if (newLevel < 10)
        {
            newSize.x = 16;
        }
        else if (newLevel < 100)
        {
            newSize.x = 32;
        }
        else
        {
            newSize.x = 48;
        }

        CurrentLevelTextBackground.sizeDelta = newSize;
    }
}
