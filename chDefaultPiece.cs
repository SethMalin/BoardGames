//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the default piece and its attribute, inherits chBasePiece

using UnityEngine;
using UnityEngine.UI;

public class chDefaultPiece : chBasePiece 
{
    /// <summary>
    /// Create a piece using parameter passed by the chPieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    public override void Create(Color teamCol, Color32 spriteCol, chPieceManager nPieceManager) {

        base.Create(teamCol, spriteCol, nPieceManager);

        //Setup movement
        movement = color == Color.black ? new Vector3Int(0, 0, 1) : new Vector3Int(0, 0, -1);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("defaultPiece");
    }
}