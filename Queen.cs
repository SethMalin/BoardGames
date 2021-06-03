//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines a queen piece and its attribute, inherits BasePiece

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Queen : BasePiece
{
    /// <summary>
    /// Create a piece using parameter passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="gameObj">Parameter not used to instantiate the Queen class</param>
    public override void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject gameObj) {

        base.Create(teamCol, spriteCol, nPieceManager, null);

        //Setup movement
        movement = new Vector3Int(7, 7, 7);
        GetComponent<Image>().sprite = Resources.Load<Sprite>("queenIcon");
    }
}
