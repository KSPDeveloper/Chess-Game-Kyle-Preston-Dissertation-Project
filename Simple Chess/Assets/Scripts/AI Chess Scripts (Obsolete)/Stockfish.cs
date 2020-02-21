using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Linq;

public class Stockfish : MonoBehaviour
{
    string bestMoveInAlgebraicNotation;
    public static int skillLevelValue = 10;
    public static int moveTime = 5000;
    void Start()
    {
        if (moveTime > 20000)
        {
            moveTime = 20000;
        }
        else if (moveTime < 0)
        {
            moveTime = 1000;
        }
        //rnbq1rk1/pp4pp/1np5/2bP1pN1/2P1p3/2N5/PPB2PPP/R1BQK2R w KQ - 0 1 (Example of Castling)
        //UnityEngine.Debug.Log(GetBestMove("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            UnityEngine.Debug.Log("Movetime: " + moveTime + ": + skillL: " + skillLevelValue);
        }
    }

    public string GetStockfishCommands(string forsythEdwardsNotationString)
    {
        return GetBestMove(forsythEdwardsNotationString);
    }

    public string GetBestMove(string forsythEdwardsNotationString)
    {
        //UnityEngine.Debug.Log(forsythEdwardsNotationString);
        var p = new Process();
        p.StartInfo.FileName = Application.dataPath + "/stockfish-11-win/Windows/stockfish_20011801_32bit.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        string setupString = "position fen " + forsythEdwardsNotationString;
        string standard_output;
        string setOptionLevel = "setoption name skill level value "+ skillLevelValue.ToString();

        #region Engine Set Up
        p.StandardInput.WriteLine("uci");
        while ((standard_output = p.StandardOutput.ReadLine()) != null)
        {
            if (standard_output.Contains("uciok"))
            {
                p.StandardInput.WriteLine(setOptionLevel);
                p.StandardInput.WriteLine(setupString);
                break;
            }
        }
        #endregion
        string processString = "go movetime " + moveTime;
        p.StandardInput.WriteLine(processString);

        while ((standard_output = p.StandardOutput.ReadLine()) != null)
        {
            if (standard_output.Contains("bestmove"))
            {
                bestMoveInAlgebraicNotation = standard_output.Substring(9);
                p.Close();
                break;
            }
        }
        return bestMoveInAlgebraicNotation;
    }
}






