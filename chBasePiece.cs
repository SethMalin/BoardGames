//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the BasePiece Object from which all pieces inherit

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class chBasePiece : EventTrigger {
    //Instantiate variables
    protected chCell originalCell = null;                           //Reference to original cell
    public chCell currentCell = null;                               //Reference to current cell
    protected chCell targetCell = null;                             //Reference to target cell
    protected RectTransform rectTransform = null;                   //Piece's geomatrical location
    protected chPieceManager pieceManager;                          //Reference to a PieceManager
    protected Vector3Int movement = Vector3Int.one;                 //Vector of 3 ints for movement
    protected List<chBasePiece> team = new List<chBasePiece>();     //List of pieces which hold the king in check
    protected AudioSource audioSource;                              //Audio

    public List<chCell> highlightedCells = new List<chCell>();      //List of possible moves
    public List<chCell> possibleMoves = new List<chCell>();         //List of possible moves on check
    public Color color = Color.clear;                               //Set color to clear
    public bool isPromotion = false;                                //Check if current piece is a promotion of a pawn
    public bool promoted = false;
    public bool killed = false;
    public bool moveTwo = false;                                    //For pawns only, true if they moved up two cells
    public bool king = false;
    public int keyID;                                               //ID for database

    #region Creation
    /// <summary>
    /// Create a piece using parameter passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    public virtual void Create(Color teamCol, Color32 spriteCol, chPieceManager nPieceManager) {

        pieceManager = nPieceManager;                   //Set pieceManager to the PieceManager GameObject
        color = teamCol;                                //Set team color to Color.black or Color.white
        GetComponent<Image>().color = spriteCol;        //Set the sprite color
        rectTransform = GetComponent<RectTransform>();  //Set to reference to geometrical location
        audioSource = GetComponent<AudioSource>();      //Set to reference to an audio source
    }

    /// <summary>
    /// Place piece on its cell at the start of the game.
    /// </summary>
    /// <param name="newCell">piece's cell</param>
    public void Place(chCell newCell) {

        //Set up cell variables
        originalCell = newCell;
        currentCell = newCell;
        currentCell.currentPiece = this;

        //Set up position an visibility
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }
    #endregion

    /// <summary>
    /// Reset the game to its original state for new game to begin.
    /// </summary>
    public virtual void Reset() {

        Kill();                 //Destroy pieces
        Place(originalCell);    //Place pieces back to their original point
        king = false;
        moveTwo = false;
        killed = false;

        foreach (Transform child in transform) {
            if (child.name.Equals("Crown")) {
                DestroyImmediate(child.gameObject);
            }
        }

    }

    /// <summary>
    /// Kill a piece.
    /// </summary>
    public virtual void Kill() {

        currentCell.currentPiece = null;    //Clear cell
        gameObject.SetActive(false);        //Remove piece
    }

    #region Movement
    /// <summary>
    /// Draw out possible moves.
    /// </summary>
    /// <param name="dirX">x coordinate of target cell</param>
    /// <param name="dirY">y coordinate of target cell</param>
    /// <param name="move">piece's move</param>
    /// <param name="nextMove">true if piece is at its second move</param>
    public void CreateCellPath(int dirX, int dirY, int move, bool nextMove) {

        //Wanted position
        int currentX = currentCell.boardPosition.x;     //Get current X location
        int currentY = currentCell.boardPosition.y;     //Get current Y location

        //Check cells
        for (int i = 1; i <= Mathf.Abs(move); i++) {
            currentX += dirX;
            currentY += dirY;

            //Get state
            chBoard.CellState cellState = chBoard.CellState.None;
            cellState = currentCell.board.ValidateCell(currentX, currentY, this);

            //If enemy, add to the list
            if (cellState == chBoard.CellState.Enemy) {
                cellState = currentCell.board.ValidateCell(currentX + dirX, currentY + dirY, this);

                if (cellState == chBoard.CellState.Free) {
                    highlightedCells.Add(currentCell.board.allCells[currentX + dirX, currentY + dirY]);
                    possibleMoves.Add(currentCell.board.allCells[currentX + dirX, currentY + dirY]);
                    possibleMoves.Add(currentCell.board.allCells[currentX, currentY]);
                }

                break;
            }

            //If free
            if (cellState != chBoard.CellState.Free)
                break;

            //Add to list
            if (!nextMove)
                highlightedCells.Add(currentCell.board.allCells[currentX, currentY]);
        }
    }

    /// <summary>
    /// Check path for possible moves.
    /// </summary>
    public virtual void CheckPathing(bool nextMove) {

        //Diagonals
        if (movement.z == 1 || king) {
            CreateCellPath(1, 1, 1, nextMove);       //Up right
            CreateCellPath(-1, 1, 1, nextMove);      //Up left
        }

        if (movement.z == -1 || king) {
            CreateCellPath(-1, -1, 1, nextMove);     //Down left
            CreateCellPath(1, -1, 1, nextMove);      //Down right
        }
    }

    /// <summary>
    /// Display all possible piece move.
    /// </summary
    public void ShowCells() {
        foreach (chCell cell in highlightedCells)
            cell.outlineImage.enabled = true;
    }

    /// <summary>
    /// Clear all highlighted cells.
    /// </summary>
    protected void ClearCells() {
        foreach (chCell cell in highlightedCells)
            cell.outlineImage.enabled = false;

        foreach (chCell cell in possibleMoves)
            cell.outlineImage.enabled = false;
    }

    /// <summary>
    /// Remove a piece from its current cell and moves it to its new cell.
    /// </summary>
    protected virtual void Move() {

        currentCell.currentPiece = null;    //Clear current cell

        currentCell = targetCell;           //Set current cell
        currentCell.currentPiece = this;    //Set piece on cell

        transform.position = currentCell.transform.position;    //Move piece
        targetCell = null;                  //Reset target cell

        audioSource.Play();
    }

    /// <summary>
    /// Determine if a piece has been skipped over.
    /// If a piece has been skipped over, kill the piece.
    /// </summary>
    protected void CheckForKill() {

        for (int i = 0; i < possibleMoves.Count; i++) {
            if (possibleMoves[i].boardPosition == currentCell.boardPosition) {
                possibleMoves[i + 1].RemovePiece();
                possibleMoves[i].outlineImage.enabled = false;
                killed = true;
            }

            i++;
        }
        possibleMoves.Clear();
    }

    /// <summary>
    /// Check if a piece can capture additional pieces after first move.
    /// If it can, all pieces are disabled except for the current piece.
    /// Possible paths are displayed.
    /// </summary>
    protected void MoreMoves() {

        CheckPathing(true);

        if (highlightedCells.Count > 0) {

            pieceManager.endTurn.GetComponent<Button>().interactable = true;
            pieceManager.endTurn.GetComponent<Image>().color = Color.white;

            team = color == Color.black ? pieceManager.blackPiece : pieceManager.whitePiece;

            foreach (chBasePiece piece in team)
                if (piece.keyID != keyID)
                    piece.enabled = false;

            moveTwo = true;
            ShowCells();
        }
        else {
            killed = false;
            moveTwo = false;

            foreach (chCell cell in currentCell.board.allCells) {
                cell.outlineImage.enabled = false;
            }


            pieceManager.endTurn.GetComponent<Button>().interactable = false;
            pieceManager.endTurn.GetComponent<Image>().color = Color.clear;
        }
    }

    /// <summary>
    /// Create a gameobject with a crown icon and attaches it to the promoted king.
    /// </summary>
    public void CreateCrown() {

        GameObject newObject = new GameObject();
        newObject.transform.SetParent(transform);

        //Scale and place
        newObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        newObject.transform.localRotation = Quaternion.identity;
        newObject.transform.localPosition = new Vector3(0, 0, 0);
        newObject.gameObject.name = "Crown";

        //Store piece
        newObject.AddComponent<Image>();
        newObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("crownLogo");
    }

    #endregion

    #region Events
    /// <summary>
    /// Called when a drag begins.
    /// </summary>
    /// <param name="eventData">event</param>
    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
        if (!moveTwo) {
            CheckPathing(false);
            ShowCells();                  //Show highlights
        }
    }

    /// <summary>
    /// Call during drag.
    /// Move the piece's sprite around. Reset its position if moved to invalid cell.
    /// </summary>
    /// <param name="eventData">event</param>
    public override void OnDrag(PointerEventData eventData) {

        base.OnDrag(eventData);
        transform.position += (Vector3)eventData.delta;     //Follow mouse

        //Set cell to target cell if mouse is within position otherwise clear target cell
        foreach (chCell cell in highlightedCells) {
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition)) {
                targetCell = cell;
                break;
            }

            targetCell = null;
        }
    }

    /// <summary>
    /// Called when player releases the mouse button.
    /// Clear cells, move piece and switch side.
    /// Reset piece position if move is invalid.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        if (!moveTwo)
            ClearCells();                  //Remove highlights

        //If no valid target cell
        if (!targetCell) {
            transform.position = currentCell.gameObject.transform.position; //Move piece back to original position
            return;
        }

        Move(); //Move cell
        CheckForKill();

        highlightedCells.Clear();
        possibleMoves.Clear();

        //if current piece killed another piece, checj for additional moves
        if (killed)
            MoreMoves();

        //promotes pieces that reached the other side of the board
        if (color == Color.black && !isPromotion && currentCell.boardPosition.y == 7 && !king)
            pieceManager.Promotion(currentCell.boardPosition.x, currentCell.boardPosition.y);

        if (color == Color.white && !isPromotion && currentCell.boardPosition.y == 0 && !king)
            pieceManager.Promotion(currentCell.boardPosition.x, currentCell.boardPosition.y);

        //next turn
        if (!moveTwo)
            pieceManager.SwitchSides(color);
    }
    #endregion
}
