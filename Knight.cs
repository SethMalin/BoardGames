//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines a knight piece and its attribute, inherits BasePiece

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knight : BasePiece
{
    /// <summary>
    /// Create a piece using parameter passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="gameObj">Parameter not used to instantiate the Knight class</param>
    public override void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject gameObj) {

        base.Create(teamCol, spriteCol, nPieceManager, null);

        //Setup movement
        GetComponent<Image>().sprite = Resources.Load<Sprite>("knightIcon");
    }

    /// <summary>
    /// Create list of possible moves for the knight.
    /// </summary>
    /// <param name="flipper">multiplier to determine move direction</param>
    private void CreateCellPath(int flipper) {

        //Current location
        int currentX = currentCell.boardPosition.x;
        int currentY = currentCell.boardPosition.y;

        //Left
        MatchesState(currentX - 2, currentY + (1 * flipper));

        //Upper left
        MatchesState(currentX - 1, currentY + (2 * flipper));

        //Upper right
        MatchesState(currentX + 1, currentY + (2 * flipper));

        //Right
        MatchesState(currentX + 2, currentY + (1 * flipper));
    }

    /// <summary>
    /// Check for possible paths.
    /// </summary>
    protected override void CheckPathing() {

        CreateCellPath(1);  //Draw top half
        CreateCellPath(-1); //Draw bottom half
    }

    /// <summary>
    /// Check for moves on enemy.
    /// </summary>
    /// <param name="targetX">x coordinate of target cell</param>
    /// <param name="targetY">y coordinate of target cell</param>
    private void MatchesState(int targetX, int targetY) {

        Board.CellState cellState = Board.CellState.None;
        cellState = currentCell.board.ValidateCell(targetX, targetY, this);

        if (cellState != Board.CellState.Friendly && cellState != Board.CellState.OutOfBound) {
            highlightedCells.Add(currentCell.board.allCells[targetX, targetY]);
        }
    }
}
