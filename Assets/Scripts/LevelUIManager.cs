using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour
{
    public Text PlayerOneScoreText;

    public Text CurrentLevelText;

    public RectTransform CurrentLevelTextBackground;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerOneScoreText(int new_text)
    {
        PlayerOneScoreText.text = new_text.ToString();
    }

    public void SetCurrentLevelText(int new_level)
    {
        CurrentLevelText.text = new_level.ToString();

        // adjust the width of the text background to fit 
        Vector2 newSize = CurrentLevelTextBackground.sizeDelta;

        if (new_level < 10)
        {
            newSize.x = 16;
        }
        else if (new_level < 100)
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
