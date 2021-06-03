//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines a bishop piece and its attribute, inherits BasePiece

using UnityEngine;
using UnityEngine.UI;

public class Bishop : BasePiece
{
    /// <summary>
    /// Create a piece using parameters passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="gameObj">Parameter not used to instantiate the Bishop class</param>
    public override void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject gameObj) {

        base.Create(teamCol, spriteCol, nPieceManager, null);

        //Setup movement
        movement = new Vector3Int(0, 0, 7);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("bishopIcon");
    }
}
