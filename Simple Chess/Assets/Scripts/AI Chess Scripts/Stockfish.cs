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

    void Start()
    {
        //UnityEngine.Debug.Log(GetBestMove("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1"));
    }

    public string GetStockfishCommands(string forsythEdwardsNotationString)
    {
        return GetBestMove(forsythEdwardsNotationString);
    }

    public string GetBestMove(string forsythEdwardsNotationString)
    {
        var p = new Process();
        p.StartInfo.FileName = Application.dataPath + "/stockfish-11-win/Windows/stockfish_20011801_32bit.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        string setupString = "position fen " + forsythEdwardsNotationString;
        p.StandardInput.WriteLine("uci");
        //Waits for the Console to return "uciok"
        string standard_output;

        while ((standard_output = p.StandardOutput.ReadLine()) != null)
        {
            if (standard_output.Contains("uciok"))
            {
                p.StandardInput.WriteLine(setupString);
                break;
            }
        }

        string processString = "go movetime 5000";
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

    //String GetBestMove(String forsythEdwardsNotationString)
    //{
    //    var p = new Process();
    //    p.StartInfo.FileName = "C:/Users/kspre/Desktop/stockfish-10-win/stockfish-10-win/Windows/stockfish_10_x64.exe";
    //    p.StartInfo.UseShellExecute = false;
    //    p.StartInfo.RedirectStandardInput = true;
    //    p.StartInfo.RedirectStandardOutput = true;
    //    p.StartInfo.RedirectStandardError = true;
    //    //   p.StartInfo.CreateNoWindow = true;
    //    p.Start();
    //    String setupString = "position fen " + forsythEdwardsNotationString;
    //    p.StandardInput.WriteLine("uci");
    //    String processString = "go movetime 5000";
    //    //Waits for the Console to return "uciok"
    //    string standard_output;
    //    while ((standard_output = p.StandardOutput.ReadLine()) != null)
    //    {
    //        if (standard_output.Contains("uciok"))
    //        {
    //            UnityEngine.Debug.Log("A");
    //            p.StandardInput.WriteLine(setupString);
    //            p.StandardInput.WriteLine(processString);
    //            break;
    //        }
    //    }

    //    // Process for 5 seconds


    //    // Process 20 deep
    //    // String processString = "go depth 20";

    //    // int skillLevelValue = 10;
    //    // string setOptionLevel = "setopiton name skill level "+ skillLevelValue.ToString();



    //    //String bestMoveInAlgebraicNotation = p.StandardOutput.ReadLine();

    //    while ((standard_output = p.StandardOutput.ReadLine()) != null)
    //    {
    //        if (standard_output.Contains("bestmove"))
    //        {
    //            UnityEngine.Debug.Log("B");
    //            bestMoveInAlgebraicNotation = p.StandardOutput.ReadLine();
    //            p.WaitForExit();
    //            p.Close();
    //            break;
    //        }
    //    }

    //    return bestMoveInAlgebraicNotation;
    //}


    /*
    private StreamReader strmReader;
    private StreamWriter strmWriter;
    private Process engineProcess;

    private IDisposable engineListener;
    public event Action<string> EngineMessage;

    
    public void SendCommand(string command)
    {
        if (strmWriter != null && command != UciCommands.uci)
        {
            strmWriter.WriteLine(command);
        }
    }

    public void StopEngine()
    {
        if (engineProcess != null & !engineProcess.HasExited)
        {
            engineListener.Dispose();
            strmReader.Close();
            strmWriter.Close();
        }
    }
    
    public void StartEngine()
    {
        FileInfo engine = new FileInfo(Path.Combine(Environment.CurrentDirectory, "stockfish_8_x64.exe"));
        if (engine.Exists && engine.Extension == ".exe")
        {
            engineProcess = new Process();
            engineProcess.StartInfo.FileName = engine.FullName;
            engineProcess.StartInfo.UseShellExecute = false;
            engineProcess.StartInfo.RedirectStandardInput = true;
            engineProcess.StartInfo.RedirectStandardOutput = true;
            engineProcess.StartInfo.RedirectStandardError = true;
            engineProcess.StartInfo.CreateNoWindow = true;

            engineProcess.Start();

            strmWriter = engineProcess.StandardInput;
            strmReader = engineProcess.StandardOutput;

            engineListener = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1)).Subscribe(s => ReadEngineMessages());

            strmWriter.WriteLine(UciCommands.uci);
            strmWriter.WriteLine(UciCommands.isready);
            strmWriter.WriteLine(UciCommands.ucinewgame);
        }
        else
        {
            throw new FileNotFoundException();
        }
    }

    private void ReadEngineMessages()
    {
        var message = strmReader.ReadLine();
        if (message != string.Empty)
        {
            EngineMessage?.Invoke(message);
        }
    }






    */






