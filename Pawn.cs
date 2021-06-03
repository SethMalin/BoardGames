//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines a pawn piece and its attribute, inherits BasePiece

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : BasePiece
{
    /// <summary>
    /// Create a piece using parameter passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="gameObj">Parameter not used to instantiate the Pawn class</param>
    public override void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject gameObj) {

        base.Create(teamCol, spriteCol, nPieceManager, gameObj);

        isFirstMove = true;
        steps = 0;

        //Setup movement
        movement = color == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("pawnIcon");
    }

    /// <summary>
    /// Check if the pawn is within taking distance of the king.
    /// </summary>
    /// <returns>true if the pawn can take the king</returns>
    public override bool KingCheck() {

        if (movement.z == 1)
            if (FindKing(1, 1, 1) ||     //Up right
                FindKing(-1, 1, 1))      //Up left
                return true;

        if(movement.z == -1)
            if(FindKing(-1, -1, 1) ||     //Down left
                FindKing(1, -1, 1))       //Down right
                return true;

        return false;
    }

    /// <summary>
    /// Check if pawn can take the king if king were to move.
    /// </summary>
    /// <param name="currentX">current x coordinate</param>
    /// <param name="currentY">current y coordinate</param>
    /// <returns>true if the pawn is a risk</returns>
    public override bool CellCheck(int currentX, int currentY) {

        if (movement.z == 1)
            if (FindCell(1, 1, 1, currentX, currentY) ||     //Up right
                FindCell(-1, 1, 1, currentX, currentY))      //Up left
                return true;

        if (movement.z == -1)
            if (FindCell(-1, -1, 1, currentX, currentY) ||     //Down left
                FindCell(1, -1, 1, currentX, currentY))       //Down right
                return true;

        return false;
    }

    /// <summary>
    /// Override the move function to allow for pawn to move two cells on first turn and take enemies diagonally.
    /// </summary>
    protected override void Move() {

        //Target location
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;

        //If the pawn moves up two cells, add 2 steps and set movedTwo to true
        if(targetCell.boardPosition.y == currentY + 2 || targetCell.boardPosition.y == currentY - 2) {
            steps += 2;
            movedTwo = true;
        }
        else {
            steps++;
        }

        //Checks if pawn can take a pawn to the left by EnPassant, if true, take the piece
        if(enPassantLeft)
            if(targetCell.boardPosition.x == currentX - 1 && targetCell.boardPosition.y == currentY + movement.z) {
                currentCell.board.allCells[currentX - 1, currentY].currentPiece.Kill();
            }

        //Checks if pawn can take a pawn to the right by EnPassant, if true, take the piece
        if (enPassantRight)
            if (targetCell.boardPosition.x == currentX + 1 && targetCell.boardPosition.y == currentY + movement.z) {
                currentCell.board.allCells[currentX + 1, currentY].currentPiece.Kill();
            }

        targetCell.RemovePiece();                               //Remove enemy piece on cell
        currentCell.currentPiece = null;                        //Clear current cell

        currentCell = targetCell;                               //Set current cell
        currentCell.currentPiece = this;                        //Set piece on cell

        transform.position = currentCell.transform.position;    //Move piece
        targetCell = null;                                      //Reset target cell

        isFirstMove = false;                                    //Set isFirstMove to false

        //If pawn reaches the other side, open promotion menu
        if (steps == 6) {
            Time.timeScale = 0f;
            promMenu.SetActive(true);
            pieceManager.pawnLoc = new int[2] { currentCell.boardPosition.x, currentCell.boardPosition.y };
        }

        //Set EnPassant values to false after moving
        enPassantLeft = false;
        enPassantRight = false;

        //Play piece sound
        audioSource.Play();
    }

    /// <summary>
    /// Check for diagonal moves on enemies.
    /// </summary>
    /// <param name="targetX">x coordinate of target cell</param>
    /// <param name="targetY">y coordinate of target cell</param>
    /// <param name="targetState">state of the cell</param>
    /// <param name="add">determine if the cell should be added to highlighted cells</param>
    /// <returns></returns>
    private bool MatchesState(int targetX, int targetY, Board.CellState targetState, bool add) {

        Board.CellState cellState = Board.CellState.None;
        cellState = currentCell.board.ValidateCell(targetX, targetY, this);

        if(cellState == targetState) {
            if(add)
                highlightedCells.Add(currentCell.board.allCells[targetX, targetY]);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check pathing.
    /// </summary>
    protected override void CheckPathing() {

        Board.CellState cellState = Board.CellState.None;
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;
        cellState = currentCell.board.ValidateCell(currentX, currentY, this);
        highlightedCells.Clear();
        possibleMoves.Clear();

        //Top left
        MatchesState(currentX - movement.z, currentY + movement.z, Board.CellState.Enemy, true);

        //Forward
        if(MatchesState(currentX, currentY +movement.y, Board.CellState.Free, true)) {

            //If cell is free and first move, check next cell
            if (isFirstMove)
                MatchesState(currentX, currentY + (movement.y * 2), Board.CellState.Free, true);
        }

        //Top right
        MatchesState(currentX + movement.z, currentY + movement.z, Board.CellState.Enemy, true);

        //Left en passant
        if(MatchesState(currentX - 1, currentY, Board.CellState.Enemy, false)) {
            if (currentCell.board.allCells[currentX - 1, currentY].currentPiece.GetComponent<Image>().sprite.name == "pawnIcon")
                if (currentCell.board.allCells[currentX - 1, currentY].currentPiece.movedTwo) {
                    highlightedCells.Add(currentCell.board.allCells[currentX - 1, currentY + movement.z]);
                    enPassantLeft = true;
                }
        }

        //Right en passant
        if (MatchesState(currentX + 1, currentY, Board.CellState.Enemy, false)) {
            if (currentCell.board.allCells[currentX + 1, currentY].currentPiece.GetComponent<Image>().sprite.name == "pawnIcon")
                if (currentCell.board.allCells[currentX + 1, currentY].currentPiece.movedTwo) {
                    highlightedCells.Add(currentCell.board.allCells[currentX + 1, currentY + movement.z]);
                    enPassantRight = true;
                }
        }
    }

    /// <summary>
    /// Override Reset() and reset all values to original state.
    /// </summary>
    public override void Reset() {
        base.Reset();
        isFirstMove = true;
        enPassantLeft = false;
        enPassantRight = false;
        steps = 0;
        turnsAfter = 0;
        movedTwo = false;
    }
}
