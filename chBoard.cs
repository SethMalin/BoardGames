//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the board class

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chBoard : MonoBehaviour {
    //Initialize variables
    public GameObject cellPrefab;                   //reference cell prefab
    public chCell[,] allCells = new chCell[8, 8];   //create array of cells
    public enum CellState { None, Friendly, Enemy, Free, OutOfBound };

    /// <summary>
    /// Create the board.
    /// </summary>
    public void CreateBoard() {

        //Create cells and position them within the board
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {

                //Create a cell
                GameObject newCell = Instantiate(cellPrefab, transform);                        //instantiate cell

                //Position cell on the board
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((i * 200) + 100, (j * 200) + 100);   //position cell

                //Place cells
                allCells[i, j] = newCell.GetComponent<chCell>();
                allCells[i, j].CreateCell(new Vector2Int(i, j), this);
            }
        }
    }

    /// <summary>
    /// Validate a state and checks if it is free, friendly, enemy, or out of bound.
    /// </summary>
    /// <param name="targetX">x coordinate of target cell</param>
    /// <param name="targetY">y coordinate of target cell</param>
    /// <param name="checkingPiece">piece calling the function</param>
    /// <returns>state of the cell</returns>
    public CellState ValidateCell(int targetX, int targetY, chBasePiece checkingPiece) {

        //Check bounds
        if (targetX < 0 || targetX > 7)
            return CellState.OutOfBound;

        if (targetY < 0 || targetY > 7)
            return CellState.OutOfBound;

        //Get cell
        chCell targetCell = allCells[targetX, targetY];

        //If cell has piece
        if (targetCell.currentPiece != null) {
            //If friendly
            if (checkingPiece.color == targetCell.currentPiece.color)
                return CellState.Friendly;
            //If enemy
            if (checkingPiece.color != targetCell.currentPiece.color)
                return CellState.Enemy;
        }

        return CellState.Free;
    }
}
