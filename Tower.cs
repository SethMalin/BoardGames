//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines a rook piece and its attribute, inherits BasePiece

using UnityEngine;
using UnityEngine.UI;

public class Tower : BasePiece
{
    /// <summary>
    /// Create a piece using parameter passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="gameObj">Parameter not used to instantiate the Tower class</param>
    public override void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject gameObj) {

        base.Create(teamCol, spriteCol, nPieceManager, null);

        //Setup movement
        movement = new Vector3Int(7, 7, 0);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("towerIcon");
    }

    /// <summary>
    /// Move the rook to its new position if the king castled.
    /// </summary>
    /// <param name="i">cell adder to determine which cell the castle is moving to</param>
    public override void Castling(int i) {

        currentCell.currentPiece = null;    //Clear current cell

        currentCell = currentCell.board.allCells[currentCell.boardPosition.x + i, currentCell.boardPosition.y];           //Set current cell
        currentCell.currentPiece = this;    //Set piece on cell

        transform.position = currentCell.transform.position;    //Move piece
    }

    /// <summary>
    /// Call the Move() function from BasePiece and change hasMoved to true.
    /// </summary>
    protected override void Move() {
        base.Move();
        hasMoved = true;
    }

    /// <summary>
    /// Reset the rook piece to its original state when a new game is started.
    /// </summary>
    public override void Reset() {
        base.Reset();
        hasMoved = false;
    }
}
