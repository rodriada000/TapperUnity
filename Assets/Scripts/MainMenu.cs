using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool player1Pressed = Input.GetButtonUp("Player1Start");
        bool player2Pressed = Input.GetButtonUp("Player2Start");

        if (player1Pressed)
        {
            GameManager.instance.StartGame(isTwoPlayer: false); 
        }
        else if (player2Pressed)
        {
            GameManager.instance.StartGame(isTwoPlayer: true); 
        }
    }
    
}
