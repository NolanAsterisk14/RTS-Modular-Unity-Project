using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("This determines if the player takes the first turn")]
    private bool playerFirst = true;
    public bool playerTurn;

    void Start()
    {
        if (playerFirst == true)
        {
            playerTurn = true;
        }
        else
        {
            playerTurn = false;
        }
    }


    public void TurnChange()
    {
        playerTurn = !playerTurn;
        
        if (playerTurn == true)
        {
            Debug.Log("It is the Player's turn!");
        }
        if (playerTurn == false)
        {
            Debug.Log("It is the Enemy's turn!");
        }
    }
}
