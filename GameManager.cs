//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the GameManageer class which creates board at the start of the game

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Board board;
    public PieceManager pieceManager;
    public GameObject promMenuUI;

    /// <summary>
    /// Call function when execution starts.
    /// Create game board and piece manager.
    /// </summary>
    void Start() {
        board.CreateBoard();
        pieceManager.Create(board, promMenuUI);
    }
}
