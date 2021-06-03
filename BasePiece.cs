//CSCI 401 - Board Games Project (2021)
//Dylan Grandjean, Seth Malin, Taline Mkrtschjan, Matthew SanSouci
//
//Defines the BasePiece Object from which all pieces inherit

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class BasePiece : EventTrigger
{
    //Instantiate variables
    protected Cell originalCell = null;                             //Reference to original cell
    protected Cell targetCell = null;                               //Reference to target cell
    protected RectTransform rectTransform = null;                   //Piece's geomatrical location
    protected PieceManager pieceManager;                            //Reference to a PieceManager
    protected Vector3Int movement = Vector3Int.one;                 //Vector of 3 ints for movement
    protected List<Cell> possibleMoves = new List<Cell>();          //List of possible moves on check
    protected List<Cell> highlightedCells = new List<Cell>();       //List of possible moves
    protected AudioSource audioSource;                              //Audio
    protected GameObject promMenu;                                  //Promotion Menu


    public Cell currentCell = null;                                 //Reference to current cell
    public List<BasePiece> piecesInCheck = new List<BasePiece>();   //List of pieces which hold the king in check
    public Color color = Color.clear;                               //Set color to clear
    public bool hasMoved = false;                                   //Tower variable for castling
    public bool isPromotion = false;                                //Check if current piece is a promotion of a pawn
    public bool movedTwo = false;                                   //For pawns only, true if they moved up two cells
    public int keyID;                                               //ID for database

    public bool isFirstMove = true;                                 //Check if it is first turn
    public int steps = 0;                                           //Calculates amount of steps taken for promotion
    public bool enPassantLeft = false;                              //Checks if pawn on left can be taken by EnPassant
    public bool enPassantRight = false;                             //Checks if pawn on right can be taken by EnPassant
    public int turnsAfter = 0;                                      //Value resets EnPassant on this piece

    #region Creation
    /// <summary>
    /// Create a piece using parameters passed by the PieceManager script.
    /// </summary>
    /// <param name="teamCol">Team color</param>
    /// <param name="spriteCol">Sprite color</param>
    /// <param name="nPieceManager">GameObject that holds the PieceManager script</param>
    /// <param name="promMenuUI">Parameter not used to instantiate the BasePiece class</param>
    public virtual void Create(Color teamCol, Color32 spriteCol, PieceManager nPieceManager, GameObject promMenuUI) {

        pieceManager = nPieceManager;                   //Set pieceManager to the PieceManager GameObject
        color = teamCol;                                //Set team color to Color.black or Color.white
        GetComponent<Image>().color = spriteCol;        //Set the sprite color
        rectTransform = GetComponent<RectTransform>();  //Set to reference to geometrical location
        audioSource = GetComponent<AudioSource>();      //Set to reference to an audio source
        promMenu = promMenuUI;                          //Set to reference to the Promotion Menu Canvas
    }

    /// <summary>
    /// Place piece on its cell at the start of the game.
    /// </summary>
    /// <param name="newCell">piece's cell</param>
    public void Place(Cell newCell) {

        //Set up cell variables
        originalCell = newCell;
        currentCell = newCell;
        currentCell.currentPiece = this;

        //Set up position an visibility
        transform.position = newCell.transform.position;
        gameObject.SetActive(true);
    }
    #endregion

    #region King Check
    /// <summary>
    /// Check if the king is within taking distance.
    /// </summary>
    /// <returns>True if the king can be taken</returns>
    public virtual bool KingCheck() {

        //Horizontal
        if (FindKing(1, 0, movement.x) ||     //Right
            FindKing(-1, 0, movement.x) ||    //Left
            FindKing(0, 1, movement.y) ||     //Up
            FindKing(0, -1, movement.y))      //Down
            return true;

        //Diagonals
        if (FindKing(1, 1, movement.z) ||      //Up right
            FindKing(-1, 1, movement.z) ||     //Up left
            FindKing(-1, -1, movement.z) ||    //Down left
            FindKing(1, -1, movement.z))       //Down right
            return true;

        return false;
    }

    /// <summary>
    /// Scan cells to find the king piece.
    /// </summary>
    /// <param name="dirX">x coordinate of target cell</param>
    /// <param name="dirY">y coordinate of target cell</param>
    /// <param name="move">piece's move</param>
    /// <returns>True if king is found</returns>
    public bool FindKing(int dirX, int dirY, int move) {

        //Target position
        int currentX = currentCell.boardPosition.x;     //Get current X location
        int currentY = currentCell.boardPosition.y;     //Get current Y location

        //Check if piece has a direct path to king
        for (int i = 1; i <= move; i++) {
            currentX += dirX;
            currentY += dirY;

            //Get state
            Board.CellState cellState = Board.CellState.None;
            cellState = currentCell.board.ValidateCell(currentX, currentY, this);

            //Find King
            if (cellState == Board.CellState.Enemy)
                if (currentCell.board.allCells[currentX, currentY].currentPiece.GetComponent<Image>().sprite.name == "kingIcon")
                    return true;
        }

        return false;
    }

    /// <summary>
    /// Check if a piece could take the king is it were to move to (currentX, currentY).
    /// </summary>
    /// <param name="currentX">x coordinate of target cell</param>
    /// <param name="currentY">y coordinate of target cell</param>
    /// <returns>true if the piece can take the king</returns>
    public virtual bool CellCheck(int currentX, int currentY) {

        //Horizontal
        if (FindCell(1, 0, movement.x, currentX, currentY) ||     //Right
            FindCell(-1, 0, movement.x, currentX, currentY)||     //Left
            FindCell(0, 1, movement.y, currentX, currentY) ||     //Up
            FindCell(0, -1, movement.y, currentX, currentY))      //Down
            return true;

        //Diagonals
        if (FindCell(1, 1, movement.z, currentX, currentY) ||     //Up right
            FindCell(-1, 1, movement.z, currentX, currentY) ||    //Up left
            FindCell(-1, -1, movement.z, currentX, currentY) ||   //Down left
            FindCell(1, -1, movement.z, currentX, currentY))      //Down right
            return true;

        return false;
    }

    /// <summary>
    /// Compare coordinate to determine if the king can be reached.
    /// </summary>
    /// <param name="dirX">x coordinate of target cell</param>
    /// <param name="dirY">y coordinate of target cell</param>
    /// <param name="move">piece's move</param>
    /// <param name="kingX">king's x coordinate</param>
    /// <param name="kingY">king's y coordinate</param>
    /// <returns></returns>
    protected bool FindCell(int dirX, int dirY, int move, int kingX, int kingY) {

        //Target position
        int currentX = currentCell.boardPosition.x;     //Get current X location
        int currentY = currentCell.boardPosition.y;     //Get current Y location

        //Check if piece would take the king if king moves
        for (int i = 1; i <= move; i++) {
            currentX += dirX;
            currentY += dirY;

            //Get state
            Board.CellState cellState = Board.CellState.None;
            cellState = currentCell.board.ValidateCell(currentX, currentY, this);

            if (cellState == Board.CellState.OutOfBound)
                return false;

            //Find King
            if(currentCell.board.allCells[currentX, currentY].boardPosition == currentCell.board.allCells[kingX, kingY].boardPosition)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the king is in check.
    /// </summary>
    /// <param name="id">piece's keyID</param>
    public void PossibleThreat(int id) {
        if (color == Color.white) {
            if (pieceManager.whiteKing.CheckForCheck(id)) {
                CreateCheckPath(pieceManager.whiteKing, currentCell.boardPosition.x, currentCell.boardPosition.y, false);
                pieceManager.whiteKing.piecesInCheck.Clear();
            }
        }
        else if (color == Color.black) {
            if (pieceManager.blackKing.CheckForCheck(id)) {
                CreateCheckPath(pieceManager.blackKing, currentCell.boardPosition.x, currentCell.boardPosition.y, false);
                pieceManager.blackKing.piecesInCheck.Clear();
            }
        }
    }

    /// <summary>
    /// Virtual function used by king to determine if in check.
    /// </summary>
    /// <param name="id">piece's keyID</param>
    /// <returns>True if the king is in check</returns>
    public virtual bool CheckForCheck(int id) { return false; }

    /// <summary>
    /// Called when the king is in check. Determine if the king can move.
    /// </summary>
    /// <param name="king">king gameobject</param>
    public void KingIsInCheck(BasePiece king) {
        ResetCheckForCheck();   //Reset piecesInCheck, possibleMoves, and highlightedCells lists
        CheckPathing();         //Gets king's possible moves

        //If only one piece has the king in check
        if (king.piecesInCheck.Count == 1) {

            //if piece is the king check if any cell can block or take checking piece
            if (keyID == 13 || keyID == 29) {
                CreateCheckPath(king, king.currentCell.boardPosition.x, king.currentCell.boardPosition.y, true);
                RemoveCheckCells(true);
            }
            else {
                CreateCheckPath(king, king.currentCell.boardPosition.x, king.currentCell.boardPosition.y, false);
                RemoveCheckCells(false);
            }

            LookAhead();
        }
        //if more than one piece hold the king in check, check if king can move at all
        else if (king.piecesInCheck.Count > 1) {
            if (keyID == 13 || keyID == 29) {
                CreateCheckPath(king, king.currentCell.boardPosition.x, king.currentCell.boardPosition.y, true);
                RemoveCheckCells(true);
                LookAhead();
                if (highlightedCells.Count == 0)
                    print("CheckMate!");
            }
        }

        piecesInCheck.Clear();
    }

    /// <summary>
    /// Create a list of all the cells standing between the king and the piece holding it in check.
    /// </summary>
    /// <param name="king">king in check</param>
    /// <param name="currentX">x coordinate of target cell</param>
    /// <param name="currentY">y coordinate of target cell</param>
    /// <param name="check">true if the king is in check</param>
    public void CreateCheckPath(BasePiece king, int currentX, int currentY, bool check) {

        int targetX = 0;
        int targetY = 0;
        int intX = 0;
        int intY = 0;

        foreach (BasePiece piece in king.piecesInCheck) {
            if (piece.GetComponent<Image>().sprite.name != "knightIcon") {
                //Check the two X position
                if (currentX > piece.currentCell.boardPosition.x)
                    intX = 1;
                else if (currentX < piece.currentCell.boardPosition.x)
                    intX = -1;

                //Check the two Y position
                if (currentY > piece.currentCell.boardPosition.y)
                    intY = 1;
                else if (currentY < piece.currentCell.boardPosition.y)
                    intY = -1;

                if (check) {
                    targetX += intX;
                    targetY += intY;
                }

                //Draws path between the king and the piece checking it
                while ((piece.currentCell.boardPosition.x + targetX) != currentX || (piece.currentCell.boardPosition.y + targetY) != currentY) {
                    possibleMoves.Add(piece.currentCell.board.allCells[piece.currentCell.boardPosition.x + targetX, piece.currentCell.boardPosition.y + targetY]);
                    targetX += intX;
                    targetY += intY;
                }

                if (piece.GetComponent<Image>().sprite.name != "pawnIcon") {

                    if ((!pieceManager.whiteKingInCheck && !pieceManager.blackKingInCheck) || (keyID == 13 || keyID == 29)) {

                        targetX = intX;
                        targetY = intY;

                        while (currentX + targetX >= 0 || currentX + targetX <= 7 || currentY + targetY >= 0 || currentY + targetY <= 7) {

                            //Get state
                            Board.CellState cellState = Board.CellState.None;
                            cellState = currentCell.board.ValidateCell(currentX + targetX, currentY + targetY, this);

                            if (cellState == Board.CellState.Free) {
                                possibleMoves.Add(piece.currentCell.board.allCells[currentX + targetX, currentY + targetY]);
                                targetX += intX;
                                targetY += intY;
                            }
                            else { return; }
                        }
                    }

                }
            }
            else {

                //Left and right
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x - 2, piece.currentCell.boardPosition.y + 1);
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x - 2, piece.currentCell.boardPosition.y - 1);
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x + 2, piece.currentCell.boardPosition.y + 1);
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x + 2, piece.currentCell.boardPosition.y - 1);

                //Up and down
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x - 1, piece.currentCell.boardPosition.y + 2);
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x + 1, piece.currentCell.boardPosition.y + 2);
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x - 1, piece.currentCell.boardPosition.y - 2);
                ValidateKnightCheckCell(piece, piece.currentCell.boardPosition.x + 1, piece.currentCell.boardPosition.y - 2);
            }
        }
    }

    /// <summary>
    /// Validate knight cell that could put the king in check. If not out of bound, add to the list of possible moves.
    /// </summary>
    /// <param name="piece">piece gameobject</param>
    /// <param name="currentX">x coordinate of target cell</param>
    /// <param name="currentY">y coordinate of target cell</param>
    private void ValidateKnightCheckCell(BasePiece piece, int currentX, int currentY) {

        //Get state
        Board.CellState cellState = Board.CellState.None;
        cellState = currentCell.board.ValidateCell(currentX, currentY, this);

        if (cellState != Board.CellState.OutOfBound)
            possibleMoves.Add(piece.currentCell.board.allCells[currentX, currentY]);
    }

    /// <summary>
    /// Remove all cells that should not appear if check is a risk.
    /// </summary>
    /// <param name="b">comparison bool</param>
    public void RemoveCheckCells(bool b) {
        bool isSame;

        //if moving the piece causes check, look for possible cells to move to
        if (possibleMoves.Count > 0) {
            for (int i = 0; i < highlightedCells.Count; i++) {

                isSame = false;

                for (int j = 0; j < possibleMoves.Count; j++) {
                    if (highlightedCells[i].boardPosition == possibleMoves[j].boardPosition)
                        isSame = true;
                }

                if (isSame == b) {
                    highlightedCells.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    /// <summary>
    /// Check the entirety of the board to determing if a piece can save the king.
    /// </summary>
    /// <param name="king">king in check</param>
    /// <returns>true if no move is possible</returns>
    public bool GlobalPathCheck(BasePiece king) {

        List<BasePiece> pieces = king.color == Color.white ? pieceManager.whitePiece : pieceManager.blackPiece;
        ResetCheckForCheck();

        //Look through every piece
        foreach (BasePiece piece in pieces) {
            if (piece.gameObject.activeSelf) {
                piece.highlightedCells.Clear();
                piece.CheckPathing();
                if (piece.keyID == 13 || piece.keyID == 29) {
                    piece.CreateCheckPath(king, king.currentCell.boardPosition.x, king.currentCell.boardPosition.y, true);
                    piece.RemoveCheckCells(true);
                }
                else {
                    piece.CreateCheckPath(king, king.currentCell.boardPosition.x, king.currentCell.boardPosition.y, false);
                    piece.RemoveCheckCells(false);
                }

                piece.LookAhead();

                if (piece.highlightedCells.Count != 0)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Reset lists.
    /// </summary>
    private void ResetCheckForCheck() {
        highlightedCells.Clear();
        possibleMoves.Clear();

        pieceManager.whiteKing.piecesInCheck.Clear();
        pieceManager.blackKing.piecesInCheck.Clear();

        pieceManager.whiteKing.CheckForCheck(33);
        pieceManager.blackKing.CheckForCheck(33);
    }
    #endregion

    /// <summary>
    /// Reset the game to its original state for new game to begin.
    /// </summary>
    public virtual void Reset() {

        Kill();                 //Destroy pieces
        Place(originalCell);    //Place pieces back to their original point
        isPromotion = false;
        movedTwo = false;
        hasMoved = false;
        
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
    private void CreateCellPath(int dirX, int dirY, int move) {

        //Wanted position
        int currentX = currentCell.boardPosition.x;     //Get current X location
        int currentY = currentCell.boardPosition.y;     //Get current Y location

        //Check cells
        for(int i = 1; i <= move; i++) {
            currentX += dirX;
            currentY += dirY;

            //Get state
            Board.CellState cellState = Board.CellState.None;
            cellState = currentCell.board.ValidateCell(currentX, currentY, this);

            //If enemy, add to the list
            if (cellState == Board.CellState.Enemy) {
                highlightedCells.Add(currentCell.board.allCells[currentX, currentY]);
                break;
            }

            //If free
            if (cellState != Board.CellState.Free)
                break;

            //Add to list
            highlightedCells.Add(currentCell.board.allCells[currentX, currentY]);
        }
    }

    /// <summary>
    /// Check path for possible moves.
    /// </summary>
    protected virtual void CheckPathing() {

        //Horizontal
        CreateCellPath(0, 1, movement.x);       //Right
        CreateCellPath(0, -1, movement.x);      //Left

        //Vertical
        CreateCellPath(1, 0, movement.y);       //Up
        CreateCellPath(-1, 0, movement.y);      //Down

        //Diagonals
        CreateCellPath(1, 1, movement.z);       //Up right
        CreateCellPath(-1, 1, movement.z);      //Up left
        CreateCellPath(-1, -1, movement.z);     //Down left
        CreateCellPath(1, -1, movement.z);      //Down right
    }

    /// <summary>
    /// Virtual function for the King class.
    /// </summary>
    public virtual void LookAhead() { }

    /// <summary>
    /// Display all possible piece move.
    /// </summary>
    protected void ShowCells() {
        foreach (Cell cell in highlightedCells)
            cell.outlineImage.enabled = true;
    }

    /// <summary>
    /// Clear all highlighted cells.
    /// </summary>
    protected void ClearCells() {
        foreach (Cell cell in highlightedCells)
            cell.outlineImage.enabled = false;

        foreach (Cell cell in possibleMoves)
            cell.outlineImage.enabled = false;

        highlightedCells.Clear();
        possibleMoves.Clear();
    }

    /// <summary>
    /// Remove a piece from its current cell and moves it to its new cell.
    /// </summary>
    protected virtual void Move() {

        targetCell.RemovePiece();           //Remove enemy piece on cell
        currentCell.currentPiece = null;    //Clear current cell

        currentCell = targetCell;           //Set current cell
        currentCell.currentPiece = this;    //Set piece on cell

        transform.position = currentCell.transform.position;    //Move piece
        targetCell = null;                  //Reset target cell

        audioSource.Play();
    }

    /// <summary>
    /// Virtual function for the Tower class.
    /// </summary>
    /// <param name="i"></param>
    public virtual void Castling(int i) {}
#endregion

    #region Events
    /// <summary>
    /// Called when a drag begins.
    /// Check if a king is in check, if not create possible moves.
    /// </summary>
    /// <param name="eventData">event</param>
    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);

        ResetCheckForCheck();

        if (pieceManager.whiteKingInCheck)
            KingIsInCheck(pieceManager.whiteKing);
        else if (pieceManager.blackKingInCheck)
            KingIsInCheck(pieceManager.blackKing);
        else {
            CheckPathing();
            PossibleThreat(keyID);
            LookAhead();
            RemoveCheckCells(false);
        }
        ShowCells();                  //Show highlights
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
        foreach(Cell cell in highlightedCells) {
            if(RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition)) {
                targetCell = cell;
                break;
            }

            targetCell = null;
        }
    }

    /// <summary>
    /// Called when player releases the mouse button.
    /// Clear cells, move pice and switch side.
    /// Reset piece position if move is invalid.
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData) {

        base.OnEndDrag(eventData);
        ClearCells();                  //Remove highlights

        //If no valid target cell
        if (!targetCell) {
            transform.position = currentCell.gameObject.transform.position; //Move piece back to original position
            return;
        }

        Move(); //Move cell
        pieceManager.SwitchSides(color);    //Oponent turn
    }
#endregion
}
