//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the PieceManager class which handles special events and turn switches

using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class chPieceManager : MonoBehaviour 
{
    //Instantiate variables
    private Color32 whiteTeamColor = new Color32(240, 240, 240, 255);    //Define white team color
    private Color32 blackTeamColor = new Color32(55, 55, 55, 255);     //Define black team color

    public List<chBasePiece> whitePiece = null;                      //Creates a list of white pieces
    public List<chBasePiece> blackPiece = null;                      //Creates a list of black pieces
    public GameObject prefab;                                      //Holds a prefab
    public GameObject saveData;                                    //Holds an instance of the saveManager object
    public GameObject gameOverMenu;                                   //Hold the game over menu object
    public GameObject whiteTurn;                                   //Holds arrow for white turn
    public GameObject blackTurn;                                   //Holds arrow for black turn
    public Button endTurn;
    public chBoard gameBoard = null;                                 //Holds instance of board game
    public bool isBlackTurn;
    public int[] pawnLoc = new int[2];

    /// <summary>
    /// Create a chPieceManager class using parameters passed by the chGameManager class. 
    /// Create() also creates black and white pieces and place them on the board and loads saved data.
    /// </summary>
    /// <param name="board">game board</param>
    public void Create(chBoard board) {

        gameBoard = board;

        //Create the pieces
        whitePiece = CreatePieces(Color.white, whiteTeamColor, board, 1);
        blackPiece = CreatePieces(Color.black, blackTeamColor, board, 13);

        //Placement
        PlacePieces(7, 6, 5, whitePiece, board, false);
        PlacePieces(1, 0, 2, blackPiece, board, true);

        //Next turn
        SwitchSides(Color.white);

        saveData.GetComponent<DataScript>().LoadCheckersData();
    }

    /// <summary>
    /// Create a checkers piece and place it into its team.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="board">game board</param>
    /// <param name="adder">used to define keyID of each piece for database purposes</param>
    /// <returns>A list of piece gameObjects that either makes up the white team or the black team</returns>
    private List<chBasePiece> CreatePieces(Color teamCol, Color32 spriteCol, chBoard board, int adder) {

        List<chBasePiece> newPieces = new List<chBasePiece>();

        for (int i = 0; i < 12; i++) {

            //Create piece
            GameObject newObject = Instantiate(prefab);
            newObject.transform.SetParent(transform);

            //Scale and place
            newObject.transform.localScale = new Vector3(1, 1, 1);
            newObject.transform.localRotation = Quaternion.identity;

            //Store piece
            chBasePiece nPiece = newObject.AddComponent<chDefaultPiece>();
            newPieces.Add(nPiece);
            nPiece.Create(teamCol, spriteCol, this);

            nPiece.keyID = i + adder;
        }

        return newPieces;
    }

    /// <summary>
    /// Place each piece on the board in a checker pattern.
    /// </summary>
    /// <param name="pRow">first row</param>
    /// <param name="qRow">second row</param>
    /// <param name="rRow">third row</param>
    /// <param name="pieces">black or white team pieces</param>
    /// <param name="board">game board</param>
    /// <param name="black">true if pieces are black</param>
    private void PlacePieces(int pRow, int qRow, int rRow, List<chBasePiece> pieces, chBoard board, bool black) {

        for (int i = 0; i < 4; i++) {

            //Place pieces
            pieces[i].Place(board.allCells[(i * 2) + 1, pRow]);
            pieces[i + 4].Place(board.allCells[i * 2, qRow]);

            if (black)
                pieces[i + 8].Place(board.allCells[i * 2, rRow]);
            else
                pieces[i + 8].Place(board.allCells[(i * 2) + 1, rRow]);
        }
    }

    /// <summary>
    /// Set all pieces in a team to playable on their turn and places them above the other team for overlap.
    /// </summary>
    /// <param name="allPieces">List of all the pieces in the black or white team</param>
    /// <param name="value">true if team's turn, false is other teams turn</param>
    private void SetInteractive(List<chBasePiece> allPieces, bool value) {
        foreach (chBasePiece piece in allPieces) {
            piece.killed = false;
            piece.moveTwo = false;

            if (value) 
                piece.gameObject.transform.SetAsLastSibling();

            piece.enabled = value;
        }
    }

    /// <summary>
    /// End a turn and allow the next team to play. 
    /// Determine whose turn it is and resets boolean values for the other team.
    /// Check if one of the two players has won.
    /// Set the playing team to interactive and the other team is disabled.
    /// </summary>
    /// <param name="color">team color</param>
    public void SwitchSides(Color color) {

        endTurn.GetComponent<Button>().interactable = false;
        endTurn.GetComponent<Image>().color = Color.clear;

        //Determines whose turn it is
        isBlackTurn = color == Color.white ? true : false;

        List<chBasePiece> team = isBlackTurn ? blackPiece : whitePiece;

        foreach (chBasePiece piece in team) {
            piece.killed = false;
            piece.moveTwo = false;
            piece.highlightedCells.Clear();
        }

        foreach (chCell cell in gameBoard.allCells) {
            cell.outlineImage.enabled = false;
        }

        CheckForWin(isBlackTurn);

        //Set interactable
        SetInteractive(whitePiece, !isBlackTurn);
        whiteTurn.SetActive(!isBlackTurn);
        SetInteractive(blackPiece, isBlackTurn);
        blackTurn.SetActive(isBlackTurn);
    }

    /// <summary>
    /// Call when the "End Turn" button is clicked.
    /// End a turn and allow the next team to play. 
    /// Determine whose turn it is and resets boolean values for the other team.
    /// Check if one of the two players has won.
    /// Set the playing team to interactive and the other team is disabled.
    /// </summary>
    public void SwitchSides() {

        endTurn.GetComponent<Button>().interactable = false;
        endTurn.GetComponent<Image>().color = Color.clear;

        if (whiteTurn.activeSelf)
            isBlackTurn = true;
        else
            isBlackTurn = false;

        List<chBasePiece> team = isBlackTurn ? blackPiece : whitePiece;

        foreach (chBasePiece piece in team) {
            piece.killed = false;
            piece.moveTwo = false;
            piece.highlightedCells.Clear();
        }

        foreach (chCell cell in gameBoard.allCells) {
            cell.outlineImage.enabled = false;
        }

        CheckForWin(isBlackTurn);

        //Set interactable
        SetInteractive(whitePiece, !isBlackTurn);
        whiteTurn.SetActive(!isBlackTurn);
        SetInteractive(blackPiece, isBlackTurn);
        blackTurn.SetActive(isBlackTurn);
    }

    /// <summary>
    /// Check for winning conditions.
    /// Determine if all pieces of a team have been killed.
    /// If some pieces remain, determine if any move is possible.
    /// </summary>
    /// <param name="turnCol">color of playing team</param>
    protected void CheckForWin(bool turnCol) {

        List<chBasePiece> pieces = turnCol ? blackPiece : whitePiece;
        bool allKilled = true;
        bool allBlocked = true;

        foreach (chBasePiece piece in pieces) {
            if (piece.gameObject.activeSelf) {
                allKilled = false;

                piece.CheckPathing(false);

                if (piece.highlightedCells.Count != 0) {
                    allBlocked = false;
                    piece.highlightedCells.Clear();
                    break;
                }
            }
        }

        if (allKilled || allBlocked)
            GameOver();
    }

    /// <summary>
    /// Promot piece to king.
    /// </summary>
    /// <param name="currentX">current x coordinate</param>
    /// <param name="currentY">current y coordinate</param>
    public void Promotion(int currentX, int currentY) {

        gameBoard.allCells[currentX, currentY].currentPiece.king = true;
        gameBoard.allCells[currentX, currentY].currentPiece.CreateCrown();
    }

    /// <summary>
    /// End the game if a player has won.
    /// </summary>
    public void GameOver() {
        SetInteractive(whitePiece, false);
        SetInteractive(blackPiece, false);
        gameOverMenu.SetActive(true);
    }

    /// <summary>
    /// Reset all pieces to their original state.
    /// Deactivate the win menu.
    /// Reset turn for new game.
    /// </summary>
    public void ResetPieces() {

        foreach (chBasePiece piece in whitePiece) {
                piece.Reset();
        }

        foreach (chBasePiece piece in blackPiece) {
                piece.Reset();
        }

        gameOverMenu.SetActive(false);
        SwitchSides(Color.white);
    }
}
