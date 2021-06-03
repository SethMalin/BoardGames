//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the cell class

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chCell : MonoBehaviour {
    //Initialize variables
    public Image outlineImage;                          //reference to image
    public Vector2Int boardPosition = Vector2Int.zero;  //cell location on the board
    public chBoard board = null;                          //Instance of board object
    public chBasePiece currentPiece = null;               //Piece standing on cell
    public RectTransform rectTransform = null;          //reference to cell's transform

    /// <summary>
    /// Create a cell and sets its position.
    /// </summary>
    /// <param name="newBoardPosition">cell position on the board</param>
    /// <param name="newBoard"> game board</param>
    public void CreateCell(Vector2Int newBoardPosition, chBoard newBoard) {
        boardPosition = newBoardPosition;
        board = newBoard;
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Remove piece from the cell.
    /// </summary>
    public void RemovePiece() {

        if (currentPiece != null)
            currentPiece.Kill();
    }
}
