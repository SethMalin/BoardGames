//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines a king piece and its attribute, inherits BasePiece

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class King : BasePiece {

    /// <summary>
    /// Create a piece using parameter passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="gameObj">Parameter not used to instantiate the King class</param>
    public override void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject gameObj) {

        base.Create(teamCol, spriteCol, nPieceManager, null);

        //Setup movement
        movement = new Vector3Int(1, 1, 1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("kingIcon");
    }

    /// <summary>
    /// Reset the king piece to its original state when a new game is started.
    /// </summary>
    public override void Reset() {
        base.Reset();
        hasMoved = false;
    }

    /// <summary>
    /// Move the king piece to a targeted x and y coordinate.
    /// </summary>
    protected override void Move() {

        //Wanted location
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;

        //Move castle to the right if king is castled to the left
        if (targetCell.boardPosition.x == currentCell.boardPosition.x + 2) {
            currentCell.board.allCells[currentX + 3, currentY].currentPiece.Castling(-2);
        }

        //Move castle to the left if king is castling to the right
        if (targetCell.boardPosition.x == currentCell.boardPosition.x - 2) {
            currentCell.board.allCells[currentX - 4, currentY].currentPiece.Castling(+3);
        }

        targetCell.RemovePiece();           //Remove enemy piece on cell
        currentCell.currentPiece = null;    //Clear current cell

        currentCell = targetCell;           //Set current cell
        currentCell.currentPiece = this;    //Set piece on cell

        transform.position = currentCell.transform.position;    //Move piece
        targetCell = null;                  //Reset target cell

        hasMoved = true;

        audioSource.Play();
    }

    /// <summary>
    /// Check if castling is permissible.
    /// </summary>
    /// <param name="targetX">x coordinate of the target cell</param>
    /// <param name="targetY">y coordinate of the target cell</param>
    /// <param name="targetState">cell state we are checking for or against</param>
    /// <returns>true if the cell's state matches the target state</returns>
    private bool MatchState(int targetX, int targetY, Board.CellState targetState) {

        Board.CellState cellState = Board.CellState.None;
        cellState = currentCell.board.ValidateCell(targetX, targetY, this);

        if (cellState == targetState)
            return true;

        return false;
    }

    /// <summary>
    /// Check for the king's possible moves and determines if castling is possible.
    /// </summary>
    protected override void CheckPathing() {

        base.CheckPathing();

        //Wanted location
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;


        //Check left
        if (MatchState(currentX + 1, currentY, Board.CellState.Free)) {
            if (!hasMoved && MatchState(currentX + 2, currentY, Board.CellState.Free)) {
                if (MatchState(currentX + 3, currentY, Board.CellState.Friendly)) {
                    if (currentCell.board.allCells[currentX + 3, currentY].currentPiece.GetComponent<Image>().sprite.name == "towerIcon") {
                        if (!currentCell.board.allCells[currentX + 3, currentY].currentPiece.hasMoved && !CheckForCheck(33))
                            highlightedCells.Add(currentCell.board.allCells[currentX + 2, currentY]);
                    }
                }
            }
        }
        

        //Check right
        if (MatchState(currentX - 1, currentY, Board.CellState.Free)) {
            if (!hasMoved && MatchState(currentX - 2, currentY, Board.CellState.Free)) {
                if (MatchState(currentX - 3, currentY, Board.CellState.Free) && MatchState(currentX - 4, currentY, Board.CellState.Friendly)) {
                    if (currentCell.board.allCells[currentX - 4, currentY].currentPiece.GetComponent<Image>().sprite.name == "towerIcon") {
                        if (!currentCell.board.allCells[currentX - 4, currentY].currentPiece.hasMoved && !CheckForCheck(33))
                            highlightedCells.Add(currentCell.board.allCells[currentX - 2, currentY]);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Scan cells in direct path with the king to find out whether the king is in check or not.
    /// </summary>
    /// <param name="id">Piece ID</param>
    /// <returns>True if the king is in check</returns>
    public override bool CheckForCheck(int id) {
        piecesInCheck.Clear();

        //Check left/Right/Down/Up
        if (Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, -1, 0, id) ||   //Left
            Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, 1, 0, id)  ||   //Right
            Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, 0, -1, id) ||   //Down
            Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, 0, 1, id))      //Up
            return true;

        //Check diagonals
        if (Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, -1, 1, id) ||   //Up left
            Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, -1, -1, id)||   //Down left
            Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, 1, 1, id)  ||   //Up right
            Scanner(currentCell.boardPosition.x, currentCell.boardPosition.y, 1, -1, id))     //Down right
            return true;

        //Check top half for knights
        if (KnightScanner(currentCell.boardPosition.x - 2, currentCell.boardPosition.y + 1) ||
            KnightScanner(currentCell.boardPosition.x - 1, currentCell.boardPosition.y + 2) ||
            KnightScanner(currentCell.boardPosition.x + 1, currentCell.boardPosition.y + 2) ||
            KnightScanner(currentCell.boardPosition.x + 2, currentCell.boardPosition.y + 1))
            return true;

        //Check bottom half for knights
        if (KnightScanner(currentCell.boardPosition.x - 2, currentCell.boardPosition.y - 1) ||
            KnightScanner(currentCell.boardPosition.x - 1, currentCell.boardPosition.y - 2) ||
            KnightScanner(currentCell.boardPosition.x + 1, currentCell.boardPosition.y - 2) ||
            KnightScanner(currentCell.boardPosition.x + 2, currentCell.boardPosition.y - 1))
            return true;

        return false;
    }

    /// <summary>
    /// Scan cells to find out whether it is free, enemy, friendly, or out of border.
    /// </summary>
    /// <param name="currentX">x coordinate of target cell</param>
    /// <param name="currentY">y coordinate of target cell</param>
    /// <param name="timeX">x coordinate multiplier</param>
    /// <param name="timeY">y coordinate multiplier</param>
    /// <param name="id">Jump over piece with given ID to find out if moving said piece would put the king in check</param>
    /// <returns>true if an enemy is present</returns>
    protected bool Scanner(int currentX, int currentY, int timeX, int timeY, int id) {

        int i = 1;
        bool enemy = false;
        bool going = true;

        //Look for enemies past the piece being moved
        while(going && i < 8) {

            //Check for out of bound cells
            if (MatchState(currentX + (i * timeX), currentY + (i * timeY), Board.CellState.OutOfBound)) {
                enemy = false;
                going = false;
            }

            //Check for allies
            if (MatchState(currentX + (i * timeX), currentY + (i * timeY), Board.CellState.Friendly)) {
                if (pieceManager.gameBoard.allCells[currentX + (i * timeX), currentY + (i * timeY)].currentPiece.keyID == id) {
                    i++;
                    continue;
                }

                enemy = false;
                going = false;
            }

            //Check for enemies
            else if (MatchState(currentX + (i * timeX), currentY + (i * timeY), Board.CellState.Enemy)) {
                if (pieceManager.gameBoard.allCells[currentX + (i * timeX), currentY + (i * timeY)].currentPiece.GetComponent<Image>().sprite.name != "knightIcon") {
                    if (pieceManager.gameBoard.allCells[currentX + (i * timeX), currentY + (i * timeY)].currentPiece.KingCheck()) {
                        piecesInCheck.Add(pieceManager.gameBoard.allCells[currentX + (i * timeX), currentY + (i * timeY)].currentPiece);
                        enemy = true;
                    }
                    going = false;
                }
            }
            i++;
        }
        return enemy;
    }

    /// <summary>
    /// Scan for knights that could put the king in check.
    /// </summary>
    /// <param name="currentX">x coordinate of targeted cell</param>
    /// <param name="currentY">y coordinate of targeted cell</param>
    /// <returns>True if the function finds a knight</returns>
    protected bool KnightScanner(int currentX, int currentY) {

        Board.CellState cellState = Board.CellState.None;
        cellState = currentCell.board.ValidateCell(currentX, currentY, this);

        if (cellState == Board.CellState.Enemy)
            if (pieceManager.gameBoard.allCells[currentX, currentY].currentPiece.GetComponent<Image>().sprite.name == "knightIcon") {
                piecesInCheck.Add(pieceManager.gameBoard.allCells[currentX, currentY].currentPiece);
                return true;
            }

        return false;
    }

    /// <summary>
    /// Take everyone of the king's move and check if moving to that cell would put the king in check.
    /// Remove cell if an enemy was found.
    /// </summary>
    public override void LookAhead() {

        bool danger = false;

        for (int i = 0; i < highlightedCells.Count; i++) {

            //Check left/Right/Down/Up
            if (ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, -1, 0) ||   //Left
                ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, 1, 0) ||   //Right
                ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, 0, -1) ||   //Down
                ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, 0, 1))      //Up
                danger = true;

            //Check diagonals
            if (ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, -1, 1) ||   //Up left
                ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, -1, -1) ||   //Down left
                ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, 1, 1) ||   //Up right
                ScanAhead(highlightedCells[i].boardPosition.x, highlightedCells[i].boardPosition.y, 1, -1))     //Down right
                danger = true;

            //Check top half for knights
            if (KnightScanner(highlightedCells[i].boardPosition.x - 2, highlightedCells[i].boardPosition.y + 1) ||
                KnightScanner(highlightedCells[i].boardPosition.x - 1, highlightedCells[i].boardPosition.y + 2) ||
                KnightScanner(highlightedCells[i].boardPosition.x + 1, highlightedCells[i].boardPosition.y + 2) ||
                KnightScanner(highlightedCells[i].boardPosition.x + 2, highlightedCells[i].boardPosition.y + 1))
                danger = true;

            //Check bottom half for knights
            if (KnightScanner(highlightedCells[i].boardPosition.x - 2, highlightedCells[i].boardPosition.y - 1) ||
                KnightScanner(highlightedCells[i].boardPosition.x - 1, highlightedCells[i].boardPosition.y - 2) ||
                KnightScanner(highlightedCells[i].boardPosition.x + 1, highlightedCells[i].boardPosition.y - 2) ||
                KnightScanner(highlightedCells[i].boardPosition.x + 2, highlightedCells[i].boardPosition.y - 1))
                danger = true;

            //print(highlightedCells[i].boardPosition + ": " + danger);

            if (danger) {
                highlightedCells.RemoveAt(i);
                i--;
                danger = false;
            }
        }
    }

    /// <summary>
    /// Scan the king's possible moves for enemies.
    /// </summary>
    /// <param name="currentX">x coordinate</param>
    /// <param name="currentY">y coordinate</param>
    /// <param name="timeX">x multiplier</param>
    /// <param name="timeY">y multiplier</param>
    /// <returns>True if the function finds an enemy</returns>
    public bool ScanAhead(int currentX, int currentY, int timeX, int timeY) {
        int i = 1;
        bool enemy = false;
        bool going = true;

        //Look fro enemies past the piece being moved
        while (going && i < 8) {

            //Check for out of bound cells
            if (MatchState(currentX + (i * timeX), currentY + (i * timeY), Board.CellState.OutOfBound)) {
                enemy = false;
                going = false;
            }

            //Check for allies
            if (MatchState(currentX + (i * timeX), currentY + (i * timeY), Board.CellState.Friendly)) {
                enemy = false;
                going = false;
            }

            //Check for enemies
            else if (MatchState(currentX + (i * timeX), currentY + (i * timeY), Board.CellState.Enemy)) {
                if (pieceManager.gameBoard.allCells[currentX + (i * timeX), currentY + (i * timeY)].currentPiece.GetComponent<Image>().sprite.name != "knightIcon") {
                    if (pieceManager.gameBoard.allCells[currentX + (i * timeX), currentY + (i * timeY)].currentPiece.CellCheck(currentX, currentY))
                        enemy = true;

                    going = false;
                }
            }
            i++;
        }
        return enemy;
    }
}
