using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class DataScript : MonoBehaviour
{
    public PieceManager pieceManager;
    public GameObject chPieceManager;
    public GameObject triviaManager;
    public GameObject categoryManager;

    /// <summary>
    /// Get the name of a database table based on passed parameter.
    /// </summary>
    /// <param name="num">passed int used to determine table</param>
    /// <returns>name of the database</returns>
    private string GetTableName(int num) {

        switch (num) {
            case 1: return "CHESS_PIECES_1";
            case 2: return "CHESS_PIECES_2";
            case 3: return "CHESS_PIECES_3";
            case 4: return "TRIVIA_1";
            case 5: return "TRIVIA_2";
            case 6: return "TRIVIA_3";
            case 7: return "CHECKERS_PIECES_1";
            case 8: return "CHECKERS_PIECES_2";
            case 9: return "CHECKERS_PIECES_3";
            default: return " ";
        }
    }

    #region CHESS GAME
    /// <summary>
    /// Load chess game data from database.
    /// </summary>
    public void LoadChessData() {

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";
        int num = 0;
        int loadState = 0;

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Check if file was loaded from a save file
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT ID, Load_State FROM GAME_DATA;";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();
        while (reader.Read()) {
            int id = reader.GetInt32(0);
            loadState = reader.GetInt32(1);

            if (loadState == 2) {
                num = id;
                break;
            }
        }

        if(loadState == 2) {
            //Send commands to database and read from it
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "SELECT " +
                       "Current_xCell, " +
                       "Current_yCell, " +
                       "Promotion, " +
                       "Promotion_Type, " +
                       "Castling, " +
                       "En_Passant, " +
                       "Captured, " +
                       "First_Move, " +
                       "Steps, " +
                       "Turns_After, " +
                       "Turn_Color " +
                       "FROM CHESS_PIECES_" + num + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();

            string turnCol = "";
            int i = 0;
            int j = 0;
            int subI = 0;

            while (reader.Read()) {
                if (subI < 16) {
                    pieceManager.whitePiece[i].currentCell.currentPiece = null;
                    pieceManager.whitePiece[i].currentCell = pieceManager.whitePiece[i].currentCell.board.allCells[reader.GetInt32(0), reader.GetInt32(1)];
                    pieceManager.whitePiece[i].currentCell.currentPiece = pieceManager.whitePiece[i];
                    pieceManager.whitePiece[i].transform.position = pieceManager.whitePiece[i].currentCell.transform.position;

                    pieceManager.whitePiece[i].isPromotion = reader.GetBoolean(2);
                    string promType = reader.GetString(3);
                    if (pieceManager.whitePiece[i].isPromotion) {
                        pieceManager.pawnLoc = new int[2] { pieceManager.whitePiece[i].currentCell.boardPosition.x, pieceManager.whitePiece[i].currentCell.boardPosition.y };
                        pieceManager.Promotion(promType);
                    }
                    pieceManager.whitePiece[i].hasMoved = reader.GetBoolean(4);
                    pieceManager.whitePiece[i].movedTwo = reader.GetBoolean(5);
                    pieceManager.whitePiece[i].gameObject.SetActive(!reader.GetBoolean(6));
                    pieceManager.whitePiece[i].isFirstMove = reader.GetBoolean(7);
                    pieceManager.whitePiece[i].steps = reader.GetInt32(8);
                    pieceManager.whitePiece[i].turnsAfter = reader.GetInt32(9);
                    turnCol = reader.GetString(10);

                    pieceManager.whitePiece[i].currentCell.currentPiece = pieceManager.whitePiece[i];
                    pieceManager.whitePiece[i].transform.position = pieceManager.whitePiece[i].currentCell.gameObject.transform.position;

                    i++;
                }


                if(subI >= 16)
                 {
                    pieceManager.blackPiece[j].currentCell.currentPiece = null;
                    pieceManager.blackPiece[j].currentCell = pieceManager.blackPiece[j].currentCell.board.allCells[reader.GetInt32(0), reader.GetInt32(1)];
                    pieceManager.blackPiece[j].currentCell.currentPiece = pieceManager.blackPiece[j];
                    pieceManager.blackPiece[j].transform.position = pieceManager.blackPiece[j].currentCell.transform.position;

                    pieceManager.blackPiece[j].isPromotion = reader.GetBoolean(2);
                    string promType = reader.GetString(3);
                    if (pieceManager.blackPiece[j].isPromotion) {
                        pieceManager.pawnLoc = new int[2] { pieceManager.blackPiece[j].currentCell.boardPosition.x, pieceManager.blackPiece[j].currentCell.boardPosition.y };
                        pieceManager.Promotion(promType);
                    }
                    pieceManager.blackPiece[j].hasMoved = reader.GetBoolean(4);
                    pieceManager.blackPiece[j].movedTwo = reader.GetBoolean(5);
                    pieceManager.blackPiece[j].gameObject.SetActive(!reader.GetBoolean(6));
                    pieceManager.blackPiece[j].isFirstMove = reader.GetBoolean(7);
                    pieceManager.blackPiece[j].steps = reader.GetInt32(8);
                    pieceManager.blackPiece[j].turnsAfter = reader.GetInt32(9);

                    pieceManager.blackPiece[j].currentCell.currentPiece = pieceManager.blackPiece[j];
                    pieceManager.blackPiece[j].transform.position = pieceManager.blackPiece[j].currentCell.transform.position;

                    j++;
                }

                subI++;
            }

            if (turnCol.Equals("black"))
                pieceManager.SwitchSides(Color.black);
            else
                pieceManager.SwitchSides(Color.white);

            //Change value back to 1
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE GAME_DATA SET Load_State = 1 WHERE ID = " + num + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    /// <summary>
    /// Save game state to database.
    /// </summary>
    /// <param name="num">table number</param>
    public void SaveChessData(int num) {

        string table = GetTableName(num);

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Save turn
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE " + table + " SET Turn_Color = " + (pieceManager.blackTurn.activeSelf ? "'white';" : "'black';");
        
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Save white pieces state
        for (int i = 0; i < 16; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Current_xCell = " + pieceManager.whitePiece[i].currentCell.boardPosition.x + ", " +
                       "Current_yCell = " + pieceManager.whitePiece[i].currentCell.boardPosition.y + ", " +
                       "Promotion = " + pieceManager.whitePiece[i].isPromotion + ", " +
                       "Promotion_Type = '" + pieceManager.whitePiece[i].GetComponent<Image>().sprite.name + "', " +
                       "Castling = " + pieceManager.whitePiece[i].hasMoved + ", " +
                       "En_Passant = " + pieceManager.whitePiece[i].movedTwo + ", " +
                       "Captured = " + !pieceManager.whitePiece[i].gameObject.activeSelf + ", " +
                       "First_Move = " + pieceManager.whitePiece[i].isFirstMove + ", " +
                       "Steps = " + pieceManager.whitePiece[i].steps + ", " +
                       "Turns_After = " + pieceManager.whitePiece[i].turnsAfter + " " +
                       "WHERE PieceID = " + (i+1) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        //Save black pieces state
        for (int i = 0; i < 16; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Current_xCell = " + pieceManager.blackPiece[i].currentCell.boardPosition.x + ", " +
                       "Current_yCell = " + pieceManager.blackPiece[i].currentCell.boardPosition.y + ", " +
                       "Promotion = " + pieceManager.blackPiece[i].isPromotion + ", " +
                       "Promotion_Type = '" + pieceManager.blackPiece[i].GetComponent<Image>().sprite.name + "', " +
                       "Castling = " + pieceManager.blackPiece[i].hasMoved + ", " +
                       "En_Passant = " + pieceManager.blackPiece[i].movedTwo + ", " +
                       "Captured = " + !pieceManager.blackPiece[i].gameObject.activeSelf + ", " +
                       "First_Move = " + pieceManager.blackPiece[i].isFirstMove + ", " +
                       "Steps = " + pieceManager.blackPiece[i].steps + ", " +
                       "Turns_After = " + pieceManager.blackPiece[i].turnsAfter + " " +
                       "WHERE PieceID = " + (i + 17) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        //Change value to 1
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE GAME_DATA SET Load_State = 1 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    /// <summary>
    /// Clear database.
    /// </summary>
    /// <param name="num">table number</param>
    public void ClearChessData(int num) {

        string table = GetTableName(num);

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Current_xCell = NULL, " +
                       "Current_yCell = NULL, " +
                       "Promotion = NULL, " +
                       "Promotion_Type = NULL, " +
                       "Castling = NULL, " +
                       "En_Passant = NULL, " +
                       "Captured = 0, " +
                       "First_Move = NULL, " +
                       "Steps = NULL, " +
                       "Turns_After = NULL, " +
                       "Turn_Color = NULL;";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Update data to database
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE GAME_DATA SET Load_State = 0 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }
    #endregion

    #region TRIVIA GAME
    /// <summary>
    /// Load trivia game data to database.
    /// </summary>
    /// <returns>true if connection was successful</returns>
    public bool LoadTriviaData() {
        EventScript eventScript = triviaManager.GetComponent<EventScript>();
        CategoryScript catScript = categoryManager.GetComponent<CategoryScript>();

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";
        int num = 0;
        int loadState = 0;

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Check if file was loaded from a save file
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT ID, Load_State FROM GAME_DATA;";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();
        while (reader.Read()) {
            int id = reader.GetInt32(0);
            loadState = reader.GetInt32(1);

            if (loadState == 2) {
                num = (id - 3);
                break;
            }
        }

        if (loadState == 2) {
            //Send commands to database and read from it
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "SELECT " +
                       "Question, " +
                       "Answer_1_Correct, " +
                       "Answer_2, " +
                       "Answer_3, " +
                       "Answer_4, " +
                       "Array_Index " +
                       "FROM TRIVIA_" + num + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();

            int i = 0;

            while (reader.Read()) {
                if (i > 104) {
                    catScript.vgMixed[i-105][0] = reader.GetString(0);              //Load question
                    catScript.vgMixed[i-105][1] = reader.GetValue(1).ToString();    //Answer 1
                    catScript.vgMixed[i-105][2] = reader.GetValue(2).ToString();    //Answer 2
                    catScript.vgMixed[i-105][3] = reader.GetValue(3).ToString();    //Answer 3
                    catScript.vgMixed[i-105][4] = reader.GetValue(4).ToString();    //Answer 4
                    eventScript.vgIndex = reader.GetInt32(5);                   //Get array index
                }
                else if (i > 69) {
                    catScript.ricMixed[i-70][0] = reader.GetString(0);             //Load question
                    catScript.ricMixed[i-70][1] = reader.GetValue(1).ToString();   //Answer 1
                    catScript.ricMixed[i-70][2] = reader.GetValue(2).ToString();   //Answer 2
                    catScript.ricMixed[i-70][3] = reader.GetValue(3).ToString();   //Answer 3
                    catScript.ricMixed[i-70][4] = reader.GetValue(4).ToString();   //Answer 4
                    eventScript.ricIndex = reader.GetInt32(5);                  //Get array index
                }
                else if (i > 34) {
                    catScript.langMixed[i-35][0] = reader.GetString(0);            //Load question
                    catScript.langMixed[i-35][1] = reader.GetValue(1).ToString();  //Answer 1
                    catScript.langMixed[i-35][2] = reader.GetValue(2).ToString();  //Answer 2
                    catScript.langMixed[i-35][3] = reader.GetValue(3).ToString();  //Answer 3
                    catScript.langMixed[i-35][4] = reader.GetValue(4).ToString();  //Answer 4
                    eventScript.langIndex = reader.GetInt32(5);                 //Get array index
                }
                else {
                    catScript.histMixed[i][0] = reader.GetString(0);            //Load question
                    catScript.histMixed[i][1] = reader.GetValue(1).ToString();  //Answer 1
                    catScript.histMixed[i][2] = reader.GetValue(2).ToString();  //Answer 2
                    catScript.histMixed[i][3] = reader.GetValue(3).ToString();  //Answer 3
                    catScript.histMixed[i][4] = reader.GetValue(4).ToString();  //Answer 4
                    eventScript.histIndex = reader.GetInt32(5);                 //Get array index
                }
                i++;
            }

            //Send commands to database and read from it
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "SELECT " +
                       "Points, " +
                       "Cheese, " +
                       "Steps, " +
                       "Before, " +
                       "Right, " +
                       "Current_Category, " +
                       "Current_Question, " +
                       "Card_1, " +
                       "Card_2, " +
                       "Card_3, " +
                       "Card_4 " +
                       "FROM TRIVIA_" + num + " " +
                       "WHERE QuestionID BETWEEN 1 and 4;";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();

            i = 0;

            while (reader.Read()) {
                
                eventScript.points[i] = reader.GetInt32(0);         //Get points
                eventScript.cheeseWon[i] = reader.GetBoolean(1);    //Get cheeses
                
                if (i == 0) {
                    eventScript.steps = reader.GetInt32(2);        //Get steps
                    eventScript.before = reader.GetInt32(3);       //Get before
                    eventScript.right = reader.GetInt32(4);        //Get right
                    eventScript.currentCat = reader.GetInt32(5);   //Get the current category
                    eventScript.questionButton.transform.Find("questionTxt").GetComponent<Text>().text = reader.GetValue(6).ToString();
                    eventScript.card[0].transform.Find("cardTxt").GetComponent<Text>().text = reader.GetValue(7).ToString();     //Load answers back on the cards
                    eventScript.card[1].transform.Find("cardTxt").GetComponent<Text>().text = reader.GetValue(8).ToString();     //Load answers back on the cards
                    eventScript.card[2].transform.Find("cardTxt").GetComponent<Text>().text = reader.GetValue(9).ToString();     //Load answers back on the cards
                    eventScript.card[3].transform.Find("cardTxt").GetComponent<Text>().text = reader.GetValue(10).ToString();     //Load answers back on the cards
                }
                i++;
            }

            //Change value back to 1
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE GAME_DATA SET Load_State = 1 WHERE ID = " + (num + 3) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();

            //Close connection
            reader.Close();
            reader = null;
            dbCmd.Dispose();
            dbCmd = null;
            dbConn.Close();
            dbConn = null;

            return true;
        }

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
        return false;
    }

    /// <summary>
    /// Save game state to database.
    /// </summary>
    /// <param name="num">table number</param>
    public void SaveTriviaData(int num) {

        EventScript eventScript = triviaManager.GetComponent<EventScript>();
        CategoryScript catScript = categoryManager.GetComponent<CategoryScript>();
        string table = GetTableName(num);

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "UPDATE " + table + " " +
                          "SET " +
                          "Steps = " + eventScript.steps + ", " +
                          "Before = " + eventScript.before + ", " +
                          "Right = " + eventScript.right + ", " +
                          "Current_Category = " + eventScript.currentCat + ", " +
                          "Current_Question = '" + eventScript.questionButton.transform.Find("questionTxt").GetComponent<Text>().text.Replace("'", "''") + "', " +
                          "Card_1 = '" + eventScript.card[0].transform.Find("cardTxt").GetComponent<Text>().text.Replace("'", "''") + "', " +
                          "Card_2 = '" + eventScript.card[1].transform.Find("cardTxt").GetComponent<Text>().text.Replace("'", "''") + "', " +
                          "Card_3 = '" + eventScript.card[2].transform.Find("cardTxt").GetComponent<Text>().text.Replace("'", "''") + "', " +
                          "Card_4 = '" + eventScript.card[3].transform.Find("cardTxt").GetComponent<Text>().text.Replace("'", "''") + "' " +
                          "WHERE QuestionID = " + 1 + ";";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Save mixed arrays
        for (int i = 0; i < 35; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Question = '" + catScript.histMixed[i][0].Replace("'", "''") + "', " +
                       "Answer_1_Correct = '" + catScript.histMixed[i][1].Replace("'", "''") + "', " +
                       "Answer_2 = '" + catScript.histMixed[i][2].Replace("'", "''") + "', " +
                       "Answer_3 = '" + catScript.histMixed[i][3].Replace("'", "''") + "', " +
                       "Answer_4 = '" + catScript.histMixed[i][4].Replace("'", "''") + "', " +
                       "Array_Index = " + eventScript.histIndex + " " +
                       "WHERE QuestionID = " + (i + 1) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        for (int i = 0; i < 35; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Question = '" + catScript.langMixed[i][0].Replace("'", "''") + "', " +
                       "Answer_1_Correct = '" + catScript.langMixed[i][1].Replace("'", "''") + "', " +
                       "Answer_2 = '" + catScript.langMixed[i][2].Replace("'", "''") + "', " +
                       "Answer_3 = '" + catScript.langMixed[i][3].Replace("'", "''") + "', " +
                       "Answer_4 = '" + catScript.langMixed[i][4].Replace("'", "''") + "', " +
                       "Array_Index = " + eventScript.langIndex + " " +
                       "WHERE QuestionID = " + (i + 36) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        for (int i = 0; i < 35; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Question = '" + catScript.ricMixed[i][0].Replace("'", "''") + "', " +
                       "Answer_1_Correct = '" + catScript.ricMixed[i][1].Replace("'", "''") + "', " +
                       "Answer_2 = '" + catScript.ricMixed[i][2].Replace("'", "''") + "', " +
                       "Answer_3 = '" + catScript.ricMixed[i][3].Replace("'", "''") + "', " +
                       "Answer_4 = '" + catScript.ricMixed[i][4].Replace("'", "''") + "', " +
                       "Array_Index = " + eventScript.ricIndex + " " +
                       "WHERE QuestionID = " + (i + 71) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        for (int i = 0; i < 35; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Question = '" + catScript.vgMixed[i][0].Replace("'", "''") + "', " +
                       "Answer_1_Correct = '" + catScript.vgMixed[i][1].Replace("'", "''") + "', " +
                       "Answer_2 = '" + catScript.vgMixed[i][2].Replace("'", "''") + "', " +
                       "Answer_3 = '" + catScript.vgMixed[i][3].Replace("'", "''") + "', " +
                       "Answer_4 = '" + catScript.vgMixed[i][4].Replace("'", "''") + "', " +
                       "Array_Index = " + eventScript.vgIndex + " " +
                       "WHERE QuestionID = " + (i + 106) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        for (int i = 0; i < 4; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Points = '" + eventScript.points[i] + "', " +
                       "Cheese = " + eventScript.cheeseWon[i] + " " +
                       "WHERE QuestionID = " + (i + 1) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        //Change value to 1
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE GAME_DATA SET Load_State = 1 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    /// <summary>
    /// Clear database.
    /// </summary>
    /// <param name="num">table number</param>
    public void ClearTriviaData(int num) {

        string table = GetTableName(num);

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Question = NULL, " +
                       "Answer_1_Correct = NULL, " +
                       "Answer_2 = NULL, " +
                       "Answer_3 = NULL, " +
                       "Answer_4 = NULL, " +
                       "Array_Index = NULL, " +
                       "Points = NULL, " +
                       "Cheese = NULL, " +
                       "Steps = NULL, " +
                       "Before = NULL, " +
                       "Right = NULL, " +
                       "Current_Category = NULL, " +
                       "Card_1 = NULL, " +
                       "Card_2 = NULL, " +
                       "Card_3 = NULL, " +
                       "Card_4 = NULL;";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Update data to database
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE GAME_DATA SET Load_State = 0 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }
    #endregion

    #region CHECKERS GAME
    /// <summary>
    /// Load checkers game data to database.
    /// </summary>
    public void LoadCheckersData() {

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";
        int num = 0;
        int loadState = 0;

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Check if file was loaded from a save file
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT ID, Load_State FROM GAME_DATA;";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();
        while (reader.Read()) {
            int id = reader.GetInt32(0);
            loadState = reader.GetInt32(1);

            if (loadState == 2) {
                num = (id - 6);
                break;
            }
        }

        if (loadState == 2) {
            //Send commands to database and read from it
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "SELECT " +
                       "Current_xCell, " +
                       "Current_yCell, " +
                       "Promotion, " +
                       "Captured, " +
                       "Is_Enabled, " +
                       "Move_Two , " +
                       "Cell_1_X, " +
                       "Cell_1_Y, " +
                       "Cell_2_X, " +
                       "Cell_2_Y, " +
                       "Cell_3_X, " +
                       "Cell_3_Y, " +
                       "Pos_1_X, " +
                       "Pos_1_Y, " +
                       "Pos_2_X, " +
                       "Pos_2_Y, " +
                       "Pos_3_X, " +
                       "Pos_3_Y, " +
                       "Turn_Color " +
                       "FROM CHECKERS_PIECES_" + num + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();

            chPieceManager script = chPieceManager.GetComponent<chPieceManager>();

            int i = 0;
            int j = 0;
            int subI = 0;

            while (reader.Read()) {

                script.isBlackTurn = reader.GetBoolean(18);

                if (subI == 0) {
                    if (script.isBlackTurn)
                        script.SwitchSides(Color.white);
                    else
                        script.SwitchSides(Color.black);

                    foreach (chCell cell in script.gameBoard.allCells) {
                        cell.currentPiece = null;
                    }
                }

                if (subI < 12) {
                    script.whitePiece[i].currentCell = script.whitePiece[i].currentCell.board.allCells[reader.GetInt32(0), reader.GetInt32(1)];

                    if (!reader.GetBoolean(3))
                        script.whitePiece[i].currentCell.currentPiece = script.whitePiece[i];

                    script.whitePiece[i].transform.position = script.whitePiece[i].currentCell.transform.position;

                    script.whitePiece[i].king = reader.GetBoolean(2);
                    if (script.whitePiece[i].king)
                        script.Promotion(reader.GetInt32(0), reader.GetInt32(1));

                    script.whitePiece[i].enabled = reader.GetBoolean(4);

                    script.whitePiece[i].moveTwo = reader.GetBoolean(5);
                    if (script.whitePiece[i].moveTwo) {

                        script.endTurn.GetComponent<Button>().interactable = true;
                        script.endTurn.GetComponent<Image>().color = Color.white;

                        script.whitePiece[i].highlightedCells.Add(script.gameBoard.allCells[reader.GetInt32(6), reader.GetInt32(7)]);
                        script.whitePiece[i].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(6), reader.GetInt32(7)]);
                        script.whitePiece[i].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(12), reader.GetInt32(13)]);

                        if (reader.GetInt32(8) != -1) {
                            script.whitePiece[i].highlightedCells.Add(script.gameBoard.allCells[reader.GetInt32(8), reader.GetInt32(9)]);
                            script.whitePiece[i].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(8), reader.GetInt32(9)]);
                            script.whitePiece[i].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(14), reader.GetInt32(15)]);
                        }

                        if (reader.GetInt32(10) != -1) {
                            script.whitePiece[i].highlightedCells.Add(script.gameBoard.allCells[reader.GetInt32(10), reader.GetInt32(11)]);
                            script.whitePiece[i].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(10), reader.GetInt32(11)]);
                            script.whitePiece[i].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(16), reader.GetInt32(17)]);
                        }

                        script.whitePiece[i].ShowCells();
                    }

                    if (!reader.GetBoolean(3))
                        script.whitePiece[i].currentCell.currentPiece = script.whitePiece[i];

                    script.whitePiece[i].transform.position = script.whitePiece[i].currentCell.gameObject.transform.position;

                    if (reader.GetBoolean(3)) {
                        script.whitePiece[i].gameObject.SetActive(false);
                    }

                    i++;
                }

                if (subI >= 12) {
                    script.blackPiece[j].currentCell = script.blackPiece[j].currentCell.board.allCells[reader.GetInt32(0), reader.GetInt32(1)];

                    if (!reader.GetBoolean(3))
                        script.blackPiece[j].currentCell.currentPiece = script.blackPiece[j];

                    script.blackPiece[j].transform.position = script.blackPiece[j].currentCell.transform.position;

                    script.blackPiece[j].king = reader.GetBoolean(2);
                    if (script.blackPiece[j].king)
                        script.Promotion(reader.GetInt32(0), reader.GetInt32(1));

                    script.blackPiece[j].enabled = reader.GetBoolean(4);

                    script.blackPiece[j].moveTwo = reader.GetBoolean(5);
                    if (script.blackPiece[j].moveTwo) {

                        script.endTurn.GetComponent<Button>().interactable = true;
                        script.endTurn.GetComponent<Image>().color = Color.white;

                        script.blackPiece[j].highlightedCells.Add(script.gameBoard.allCells[reader.GetInt32(6), reader.GetInt32(7)]);
                        script.blackPiece[j].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(6), reader.GetInt32(7)]);
                        script.blackPiece[j].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(12), reader.GetInt32(13)]);

                        if (reader.GetInt32(8) != -1) {
                            script.blackPiece[j].highlightedCells.Add(script.gameBoard.allCells[reader.GetInt32(8), reader.GetInt32(9)]);
                            script.blackPiece[j].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(8), reader.GetInt32(9)]);
                            script.blackPiece[j].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(14), reader.GetInt32(15)]);
                        }

                        if (reader.GetInt32(10) != -1) {
                            script.blackPiece[j].highlightedCells.Add(script.gameBoard.allCells[reader.GetInt32(10), reader.GetInt32(11)]);
                            script.blackPiece[j].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(10), reader.GetInt32(11)]);
                            script.blackPiece[j].possibleMoves.Add(script.gameBoard.allCells[reader.GetInt32(16), reader.GetInt32(17)]);
                        }

                        script.blackPiece[j].ShowCells();
                    }

                    if (!reader.GetBoolean(3))
                        script.blackPiece[j].currentCell.currentPiece = script.blackPiece[j];
                    script.blackPiece[j].transform.position = script.blackPiece[j].currentCell.gameObject.transform.position;

                    if (reader.GetBoolean(3)) {
                        script.blackPiece[j].gameObject.SetActive(false);
                    }

                    j++;
                }

                subI++;
            }

            //Change value back to 1
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE GAME_DATA SET Load_State = 1 WHERE ID = " + (num + 6) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    /// <summary>
    /// Save game state to database.
    /// </summary>
    /// <param name="num">table number</param>
    public void SaveCheckersData(int num) {

        chPieceManager script = chPieceManager.GetComponent<chPieceManager>();

        string table = GetTableName(num);

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Save turn
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE " + table + " SET Turn_Color = " + (script.isBlackTurn);

        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Save white pieces state
        for (int i = 0; i < 12; i++) {
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Current_xCell = " + script.whitePiece[i].currentCell.boardPosition.x + ", " +
                       "Current_yCell = " + script.whitePiece[i].currentCell.boardPosition.y + ", " +
                       "Promotion = " + script.whitePiece[i].king + ", " +
                       "Captured = " + !script.whitePiece[i].gameObject.activeSelf + ", " +
                       "Is_Enabled = " + script.whitePiece[i].isActiveAndEnabled + ", " +
                       "Move_Two = " + script.whitePiece[i].moveTwo + ", " +
                       "Cell_1_X = " + (script.whitePiece[i].highlightedCells.Count > 0 ? script.whitePiece[i].highlightedCells[0].boardPosition.x : -1) + ", " +
                       "Cell_1_Y = " + (script.whitePiece[i].highlightedCells.Count > 0 ? script.whitePiece[i].highlightedCells[0].boardPosition.y : -1) + ", " +
                       "Cell_2_X = " + (script.whitePiece[i].highlightedCells.Count > 1 ? script.whitePiece[i].highlightedCells[1].boardPosition.x : -1) + ", " +
                       "Cell_2_Y = " + (script.whitePiece[i].highlightedCells.Count > 1 ? script.whitePiece[i].highlightedCells[1].boardPosition.y : -1) + ", " +
                       "Cell_3_X = " + (script.whitePiece[i].highlightedCells.Count > 2 ? script.whitePiece[i].highlightedCells[2].boardPosition.x : -1) + ", " +
                       "Cell_3_Y = " + (script.whitePiece[i].highlightedCells.Count > 2 ? script.whitePiece[i].highlightedCells[2].boardPosition.y : -1) + ", " +
                       "Pos_1_X = " + (script.whitePiece[i].possibleMoves.Count > 0 ? script.whitePiece[i].possibleMoves[1].boardPosition.x : -1) + ", " +
                       "Pos_1_Y = " + (script.whitePiece[i].possibleMoves.Count > 0 ? script.whitePiece[i].possibleMoves[1].boardPosition.y : -1) + ", " +
                       "Pos_2_X = " + (script.whitePiece[i].possibleMoves.Count > 2 ? script.whitePiece[i].possibleMoves[3].boardPosition.x : -1) + ", " +
                       "Pos_2_Y = " + (script.whitePiece[i].possibleMoves.Count > 2 ? script.whitePiece[i].possibleMoves[3].boardPosition.y : -1) + ", " +
                       "Pos_3_X = " + (script.whitePiece[i].possibleMoves.Count > 4 ? script.whitePiece[i].possibleMoves[5].boardPosition.x : -1) + ", " +
                       "Pos_3_Y = " + (script.whitePiece[i].possibleMoves.Count > 4 ? script.whitePiece[i].possibleMoves[5].boardPosition.y : -1) + " " +
                       "WHERE PieceID = " + (i + 1) + ";";                                                                                           
            dbCmd.CommandText = sqlQuery;                                                                                                            
            reader = dbCmd.ExecuteReader();                                                                                                          
        }
        
        //Save black pieces state                                                                                                                    
        for (int i = 0; i < 12; i++) {                                                                                                               
            dbCmd = dbConn.CreateCommand();
            sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Current_xCell = " + script.blackPiece[i].currentCell.boardPosition.x + ", " +
                       "Current_yCell = " + script.blackPiece[i].currentCell.boardPosition.y + ", " +
                       "Promotion = " + script.blackPiece[i].king + ", " +
                       "Captured = " + !script.blackPiece[i].gameObject.activeSelf + ", " +
                       "Is_Enabled = " + script.blackPiece[i].isActiveAndEnabled + ", " +
                       "Move_Two = " + script.blackPiece[i].moveTwo + ", " +
                       "Cell_1_X = " + (script.blackPiece[i].highlightedCells.Count > 0 ? script.blackPiece[i].highlightedCells[0].boardPosition.x : -1) + ", " +
                       "Cell_1_Y = " + (script.blackPiece[i].highlightedCells.Count > 0 ? script.blackPiece[i].highlightedCells[0].boardPosition.y : -1) + ", " +
                       "Cell_2_X = " + (script.blackPiece[i].highlightedCells.Count > 1 ? script.blackPiece[i].highlightedCells[1].boardPosition.x : -1) + ", " +
                       "Cell_2_Y = " + (script.blackPiece[i].highlightedCells.Count > 1 ? script.blackPiece[i].highlightedCells[1].boardPosition.y : -1) + ", " +
                       "Cell_3_X = " + (script.blackPiece[i].highlightedCells.Count > 2 ? script.blackPiece[i].highlightedCells[2].boardPosition.x : -1) + ", " +
                       "Cell_3_Y = " + (script.blackPiece[i].highlightedCells.Count > 2 ? script.blackPiece[i].highlightedCells[2].boardPosition.y : -1) + ", " +
                       "Pos_1_X = " + (script.blackPiece[i].possibleMoves.Count > 0 ? script.blackPiece[i].possibleMoves[1].boardPosition.x : -1) + ", " +
                       "Pos_1_Y = " + (script.blackPiece[i].possibleMoves.Count > 0 ? script.blackPiece[i].possibleMoves[1].boardPosition.y : -1) + ", " +
                       "Pos_2_X = " + (script.blackPiece[i].possibleMoves.Count > 2 ? script.blackPiece[i].possibleMoves[3].boardPosition.x : -1) + ", " +
                       "Pos_2_Y = " + (script.blackPiece[i].possibleMoves.Count > 2 ? script.blackPiece[i].possibleMoves[3].boardPosition.y : -1) + ", " +
                       "Pos_3_X = " + (script.blackPiece[i].possibleMoves.Count > 4 ? script.blackPiece[i].possibleMoves[5].boardPosition.x : -1) + ", " +
                       "Pos_3_Y = " + (script.blackPiece[i].possibleMoves.Count > 4 ? script.blackPiece[i].possibleMoves[5].boardPosition.y : -1) + " " +
                       "WHERE PieceID = " + (i + 13) + ";";
            dbCmd.CommandText = sqlQuery;
            reader = dbCmd.ExecuteReader();
        }

        //Change value to 1
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE GAME_DATA SET Load_State = 1 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    /// <summary>
    /// Clear database.
    /// </summary>
    /// <param name="num">table number</param>
    public void ClearCheckersData(int num) {

        string table = GetTableName(num);

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "UPDATE " + table + " " +
                       "SET " +
                       "Current_xCell = NULL, " +
                       "Current_yCell = NULL, " +
                       "Promotion = NULL, " +
                       "Captured = 0, " +
                       "Is_Enabled = 0, " +
                       "Move_Two = 0, " +
                       "Cell_1_X = -1, " +
                       "Cell_1_Y = -1, " +
                       "Cell_2_X = -1, " +
                       "Cell_2_Y = -1, " +
                       "Cell_3_X = -1, " +
                       "Cell_3_Y = -1, " +
                       "Turn_Color = NULL;";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Update data to database
        dbCmd = dbConn.CreateCommand();
        sqlQuery = "UPDATE GAME_DATA SET Load_State = 0 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    #endregion

    #region Data Checking
    /// <summary>
    /// Set selected GAME_DATA value to 1 to be loaded.
    /// </summary>
    /// <param name="num">table number</param>
    public void SetLoadData(int num) {

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "UPDATE GAME_DATA SET Load_State = 2 WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;
    }

    /// <summary>
    /// Get GAME_DATA value of specific ID.
    /// </summary>
    /// <param name="num">table number</param>
    /// <returns>state of the database - 0: empty, 1: loadable</returns>
    public int GetLoadState(int num) {

        //Create database folder path
        string conn = "URI=file:" + Application.streamingAssetsPath + "/CSCI401 Project.db";

        //Establish connection and open file
        IDbConnection dbConn;
        dbConn = (IDbConnection)new SqliteConnection(conn);
        dbConn.Open();

        //Save data to database
        IDbCommand dbCmd = dbConn.CreateCommand();
        string sqlQuery = "SELECT Load_State FROM GAME_DATA WHERE ID = " + num + ";";
        dbCmd.CommandText = sqlQuery;
        IDataReader reader = dbCmd.ExecuteReader();

        int i = 0;

        while (reader.Read())
            i = reader.GetInt32(0);

        //Close connection
        reader.Close();
        reader = null;
        dbCmd.Dispose();
        dbCmd = null;
        dbConn.Close();
        dbConn = null;

        return i;
    }
    #endregion
}
