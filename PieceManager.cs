//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the PieceManager class which handles special events and turn switches

using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class PieceManager : MonoBehaviour
{
    //Instantiate variables
    private Color32 whiteTeamColor = new Color32(206, 206, 203, 255);    //Define white team color
    private Color32 blackTeamColor = new Color32(217, 181, 63, 255);     //Define black team color
    private string[] sort = new string[16]                               //Creates an array to sort pieces
    {
        "P", "P", "P", "P", "P", "P", "P", "P",
        "T", "N", "B", "Q", "K", "B", "N", "T"
    };

    public List<BasePiece> whitePiece = null;                      //Creates a list of white pieces
    public List<BasePiece> blackPiece = null;                      //Creates a list of black pieces
    public GameObject prefab;                                      //Holds a prefab
    public GameObject saveData;                                    //Holds an instance of the saveManager object
    public GameObject promMenuUI;                                  //Hold the promotion menu object
    public GameObject checkMenu;                                   //Hold the checkmate menu object
    public GameObject checkText;                                   //Holds the "check" text
    public GameObject whiteTurn;                                   //Holds arrow for white turn
    public GameObject blackTurn;                                   //Holds arrow for black turn
    public BasePiece whiteKing = null;                             //White King
    public BasePiece blackKing = null;                             //Black king
    public bool blackKingInCheck = false;                          //True if the black king is in check
    public bool whiteKingInCheck = false;                          //True if the white king is in check
    public Board gameBoard = null;                                 //Holds instance of board game
    public int[] pawnLoc = new int[2];                             //Holds the pawn's location

    //Creates a dictionary to define each piece
    private Dictionary<string, Type> pieceLibrary = new Dictionary<string, Type>()
    {
        {"P", typeof(Pawn)},
        {"T", typeof(Tower)},
        {"N", typeof(Knight)},
        {"B", typeof(Bishop)},
        {"K", typeof(King)},
        {"Q", typeof(Queen)}
    };

    /// <summary>
    /// Function called at the start of execution. Set the team colors to the given values.
    /// </summary>
    private void Start() {

        whiteTurn.GetComponent<Image>().color = whiteTeamColor;
        blackTurn.GetComponent<Image>().color = blackTeamColor;
    }

    /// <summary>
    /// Create a PieceManager class using parameters passed by the GameManager class. 
    /// Also creates black and white pieces and places them on the board and loads saved data.
    /// </summary>
    /// <param name="board">game board</param>
    /// <param name="promMenu">promotion menu</param>
    public void Create(Board board, GameObject promMenu) {

        promMenuUI = promMenu;
        gameBoard = board;

        //Create the pieces
        whitePiece = CreatePieces(Color.white, whiteTeamColor, board, 1);
        blackPiece = CreatePieces(Color.black, blackTeamColor, board, 17);

        //Placement
        PlacePieces(1, 0, whitePiece, board);
        PlacePieces(6, 7, blackPiece, board);

        //Next turn
        SwitchSides(Color.black);

        saveData.GetComponent<DataScript>().LoadChessData();
    }

    /// <summary>
    /// Create a chess piece, define its type and place it into its team.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="board">game board</param>
    /// <param name="adder">used to define keyID of each piece for database purposes</param>
    /// <returns>a list of piece gameObjects that either makes up the white team or the black team</returns>
    private List<BasePiece> CreatePieces(Color teamCol, Color32 spriteCol, Board board, int adder) {

        List<BasePiece> newPieces = new List<BasePiece>();

        for(int i=0; i < sort.Length; i++) {

            //Create piece
            GameObject newObject = Instantiate(prefab);
            newObject.transform.SetParent(transform);

            //Scale and place
            newObject.transform.localScale = new Vector3(1, 1, 1);
            newObject.transform.localRotation = Quaternion.identity;

            //Setup type based on dictionnary
            string key = sort[i];
            Type pType = pieceLibrary[key];

            //Store piece
            BasePiece nPiece = (BasePiece)newObject.AddComponent(pType);
            newPieces.Add(nPiece);
            nPiece.Create(teamCol, spriteCol, this, promMenuUI);

            if(key == "K") {
                if (teamCol == Color.white)
                    whiteKing = nPiece;
                else
                    blackKing = nPiece;
            }

            nPiece.keyID = i + adder;
        }

        return newPieces;
    }

    /// <summary>
    /// Place each piece on the board based on the defined sort array.
    /// </summary>
    /// <param name="pRow">pawn row</param>
    /// <param name="rRow">royal row</param>
    /// <param name="pieces">black or white team pieces</param>
    /// <param name="board">game board</param>
    private void PlacePieces(int pRow, int rRow, List<BasePiece> pieces, Board board) {

        for(int i=0; i<8; i++) {

            //Place pieces
            pieces[i].Place(board.allCells[i, pRow]);
            pieces[i + 8].Place(board.allCells[i, rRow]);
        }
    }

    /// <summary>
    /// Set all pieces in a team to playable on their turn and places them above the other team for overlap.
    /// </summary>
    /// <param name="allPieces">List of all the pieces in the black or white team</param>
    /// <param name="value">true if team's turn, false if other team's turn</param>
    private void SetInteractive(List<BasePiece> allPieces, bool value) {
        foreach (BasePiece piece in allPieces) {

            if (value)
                piece.gameObject.transform.SetAsLastSibling();

            piece.enabled = value;
        }
    }

    /// <summary>
    /// End a turn and allow the next team to play. Determine whose turn it is and resets EnPassant for all pawn of that color.
    /// Check if the kings are in check and display a "Check" message if true.
    /// Check if a king in check has any possible moves left, if not, call the Checkmate() function.
    /// Set the playing team to interactive and the other team is disabled.
    /// </summary>
    /// <param name="color">team color</param>
    public void SwitchSides(Color color) {

        //Determines whose turn it is
        bool isBlackTurn = color == Color.white ? true : false;

        //Resets EnPassant value for pieces whose turn just finished
        ResetEnPassant(color);

        //Checks if king is in check
        whiteKingInCheck = whiteKing.CheckForCheck(33);
        blackKingInCheck = blackKing.CheckForCheck(33);

        //Dissplay check text if king in check
        if (whiteKingInCheck || blackKingInCheck)
            checkText.SetActive(true);
        else
            checkText.SetActive(false);

        //Checks for checkmate
        if (whiteKingInCheck) {
            if (!whiteKing.GlobalPathCheck(whiteKing))
                CheckMate();
        }
        else if (blackKingInCheck) {
            if (!blackKing.GlobalPathCheck(blackKing))
                CheckMate();
        }

        //Set interactable
        SetInteractive(whitePiece, !isBlackTurn);
        whiteTurn.SetActive(!isBlackTurn);
        SetInteractive(blackPiece, isBlackTurn);
        blackTurn.SetActive(isBlackTurn);
    }

    /// <summary>
    /// Reset the EnPassant value of all pawn of given team color.
    /// </summary>
    /// <param name="color">team color</param>
    private void ResetEnPassant(Color color) {

        //Reset value for pawns of opposite side who moved two past turn
        if (color == Color.white) {
            foreach (BasePiece piece in blackPiece)
                if (piece.movedTwo)
                    piece.movedTwo = false;
        }
        else {
            foreach (BasePiece piece in whitePiece)
                if (piece.movedTwo)
                    piece.movedTwo = false;
        }
    }

    /// <summary>
    /// Promote a pawn that has reached the other side of the board.
    /// </summary>
    /// <param name="pieceName">name of the piece to promote into : knight, bishop, queen, rook</param>
    public void Promotion(string pieceName) {

        //Kill pawn
        int id = gameBoard.allCells[pawnLoc[0], pawnLoc[1]].currentPiece.keyID;
        gameBoard.allCells[pawnLoc[0], pawnLoc[1]].currentPiece.Kill();

        //Create piece
        GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(transform);

        //Scale and place
        newObject.transform.localScale = new Vector3(1, 1, 1);
        newObject.transform.localRotation = Quaternion.identity;

        //Setup type based on dictionnary
        Type pType = pieceLibrary[pieceName];

        //Store piece
        BasePiece nPiece = (BasePiece)newObject.AddComponent(pType);
        if (pawnLoc[1] > 0) {
            whitePiece.Add(nPiece);
            nPiece.Create(Color.white, whiteTeamColor, this, promMenuUI);
        }
        else {
            blackPiece.Add(nPiece);
            nPiece.Create(Color.black, blackTeamColor, this, promMenuUI);
        }

        //Place  new piece
        nPiece.keyID = id;
        nPiece.isPromotion = true;
        nPiece.Place(gameBoard.allCells[pawnLoc[0], pawnLoc[1]]);

        //Unpause and next player
        promMenuUI.SetActive(false);
        SwitchSides(pawnLoc[1] > 0 ? Color.white : Color.black);
    }

    /// <summary>
    /// Declare checkmate, deactivate all pieces and display the checkmate menu.
    /// </summary>
    public void CheckMate() {
        SetInteractive(whitePiece, false);
        SetInteractive(blackPiece, false);
        checkText.SetActive(false);
        checkMenu.SetActive(true);
    }

    /// <summary>
    /// Reset all pieces to their original state. Deletes pieces that were promoted.
    /// Removes the checkmate text and menu and set turn back to white's.
    /// </summary>
    public void ResetPieces() {

        foreach (BasePiece piece in whitePiece)
            if (piece.isPromotion)
                piece.Kill();
            else
                piece.Reset();

        foreach (BasePiece piece in blackPiece)
            if (piece.isPromotion)
                piece.Kill();
            else
                piece.Reset();

        checkText.SetActive(false);
        checkMenu.SetActive(false);
        SwitchSides(Color.black);
    }
}
