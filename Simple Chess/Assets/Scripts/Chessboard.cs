using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chessboard : MonoBehaviour
{
    public bool checkMate = false;
    public int numberOfPossibleMovesB, numberOfPossibleMovesW;
    public UIEngine UIEngineReference;
    public bool whiteToMove = true, check = false;
    public Material white, black;
    public GameObject Pawn, Knight, Queen, Bishop, Rook, King, chessBoard;
    public bool startingPosition = true;
    public List<Vector3> whiteSpaces, blackSpaces, lineOfCheck, piecesChecking, pinnedPieces; // piecesChecking is a list which contains the positons of the pieces putting the king into check
    public Dictionary<Vector3, List<Vector3>> pinnedPiecePath = new Dictionary<Vector3, List<Vector3>>();  // Piece Being Pinned, Path it is being pinned on //
    public GameObject[] pieces = new GameObject[32];
    public Vector3[,] chessBoard2D = new Vector3[8, 8];
    public Dictionary<Vector3, GameObject> piecePosition = new Dictionary<Vector3, GameObject>();
    public Vector3 pinnedPiece;
    float bXMax = 8.75f, bXMin = 0f, bZMax = 8.75f, bZMin = 0f;
    public int piecesPuttingKingInCheck = 0;

    void Start()
    {
        float xPos = 0.0f;
        bool bSquare = true;
        UIEngineReference = FindObjectOfType<UIEngine>();
        //creates the 2D array for the chess board
        for (int i = 0; i < 8; i++)
        {
            float zPos = 0.0f;
            for (int f = 0; f < 8; f++)
            {
                chessBoard2D[i, f] = new Vector3(xPos, 0, zPos);
                if (bSquare)
                {
                    blackSpaces.Add(new Vector3(xPos, 0, zPos));
                }
                zPos += 1.25f;

                bSquare = !bSquare;
            }
            bSquare = !bSquare;
            xPos = xPos + 1.25f;
        }

        InitiatePieces();
    }

    void InitiatePieces()
    {
        //WHITE
        //REST OF PIECES
        pieces[0] = Instantiate(Rook, chessBoard.transform);
        pieces[1] = Instantiate(Knight, chessBoard.transform);
        pieces[2] = Instantiate(Bishop, chessBoard.transform);
        pieces[3] = Instantiate(King, chessBoard.transform);
        pieces[4] = Instantiate(Queen, chessBoard.transform);
        pieces[5] = Instantiate(Bishop, chessBoard.transform);
        pieces[6] = Instantiate(Knight, chessBoard.transform);
        pieces[7] = Instantiate(Rook, chessBoard.transform);

        for (int i = 0; i < 8; i++)
        {
            pieces[i].transform.localPosition = chessBoard2D[i, 0];
            pieces[i].transform.localScale = new Vector3(1, 1, 1);
            pieces[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
            pieces[i].GetComponent<Renderer>().material = white;
            pieces[i].AddComponent<Track>();
            pieces[i].tag = "White";
            piecePosition.Add(chessBoard2D[i, 0], pieces[i]);
        }

        //for Knights only
        pieces[1].transform.localRotation = new Quaternion(0, 270, 0, 1);
        pieces[6].transform.localRotation = new Quaternion(0, 270, 0, 1);

        //PAWNS
        for (int i = 8; i < 16; i++)
        {
            pieces[i] = Instantiate(Pawn, chessBoard.transform);
            pieces[i].transform.localPosition = chessBoard2D[i - 8, 1];
            pieces[i].transform.localScale = new Vector3(1, 1, 1);
            pieces[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
            pieces[i].GetComponent<Renderer>().material = white;
            pieces[i].tag = "White";
            pieces[i].AddComponent<Track>().startingPosition = true;
            piecePosition.Add(chessBoard2D[i - 8, 1], pieces[i]);

        }


        //BLACK//

        //PAWNS//
        for (int i = 16; i < 24; i++)
        {
            pieces[i] = Instantiate(Pawn, chessBoard.transform);
            pieces[i].transform.localPosition = chessBoard2D[i - 16, 6];
            pieces[i].transform.localScale = new Vector3(1, 1, 1);
            pieces[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
            pieces[i].GetComponent<Renderer>().material = black;
            pieces[i].AddComponent<Track>().startingPosition = true;
            pieces[i].tag = "Black";
            piecePosition.Add(chessBoard2D[i - 16, 6], pieces[i]);

        }
        //REST OF PIECES//
        pieces[24] = Instantiate(Rook, chessBoard.transform);
        pieces[24].AddComponent<Track>().startingPosition = true;
        pieces[25] = Instantiate(Knight, chessBoard.transform);
        pieces[26] = Instantiate(Bishop, chessBoard.transform);
        pieces[27] = Instantiate(Queen, chessBoard.transform);
        pieces[28] = Instantiate(King, chessBoard.transform);
        pieces[28].AddComponent<Track>().startingPosition = true;
        pieces[29] = Instantiate(Bishop, chessBoard.transform);
        pieces[30] = Instantiate(Knight, chessBoard.transform);
        pieces[31] = Instantiate(Rook, chessBoard.transform);
        pieces[31].AddComponent<Track>().startingPosition = true;

        for (int i = 24; i < 32; i++)
        {
            pieces[i].transform.localPosition = chessBoard2D[i - 24, 7];
            pieces[i].transform.localScale = new Vector3(1, 1, 1);
            pieces[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
            pieces[i].GetComponent<Renderer>().material = black;
            pieces[i].AddComponent<Track>();
            pieces[i].tag = "Black";
            piecePosition.Add(chessBoard2D[i - 24, 7], pieces[i]);
        }
    }

    public void GetSpaces(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            PawnMovement(selectedPiece, ref lightList);
        }

        else if (selectedPiece.transform.GetChild(0).tag == "Rook")
        {
            RookMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        }

        else if (selectedPiece.transform.GetChild(0).tag == "Knight")
        {
            KnightMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        }

        else if (selectedPiece.transform.GetChild(0).tag == "Bishop")
        {
            BishopMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        }

        else if (selectedPiece.transform.GetChild(0).tag == "King")
        {
            KingMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces, numberOfPossibleMovesW, numberOfPossibleMovesB);
        }
        else if (selectedPiece.transform.GetChild(0).tag == "Queen")
        {
            QueenMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        }
    }

    public void PawnMovement(GameObject selectedPiece, ref List<Vector3> lightList)
    {
        Vector3 moveForward = new Vector3(0, 0, 1.25f);
        Vector3 startingPos = selectedPiece.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[2];
        directions[0] = new Vector3(1.25f, 0, 0); //move right
        directions[1] = new Vector3(-1.25f, 0, 0); //move left

        if (check == false)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) // Normal movement
            {
                if (selectedPiece.tag == "White")
                {
                    //move forward one//
                    tempMove = startingPos + moveForward;
                    if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                    {
                        numberOfPossibleMovesW++;
                        lightList.Add(tempMove);
                    }

                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                    {
                        //move forward two
                        tempMove = startingPos;
                        tempMove += moveForward;
                        if (!piecePosition.ContainsKey(tempMove))
                        {
                            tempMove += moveForward;
                            if (!piecePosition.ContainsKey(tempMove))
                            {
                                numberOfPossibleMovesW++;
                                lightList.Add(tempMove);
                            }
                        }
                    }

                    //Up Left / Right
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos; //resets tempUp
                        tempMove += moveForward + dir; // gets forward one, right / left one position
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesChecking.Add(tempMove);
                                    piecesPuttingKingInCheck += 1;
                                }

                                else
                                {
                                    numberOfPossibleMovesW++;
                                    lightList.Add(tempMove);
                                }
                            }
                        }
                    }
                }

                else if (selectedPiece.tag == "Black")
                {
                    //move forward one//
                    startingPos = selectedPiece.transform.localPosition;
                    tempMove = startingPos - moveForward;
                    if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                    {
                        lightList.Add(tempMove);
                        numberOfPossibleMovesB++;
                    }

                    //Used only for when the pawn has not moved from its stating position
                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                    {
                        //move forward 2
                        tempMove = startingPos;
                        tempMove -= moveForward;
                        if (!piecePosition.ContainsKey(tempMove))
                        {
                            tempMove -= moveForward;
                            if (!piecePosition.ContainsKey(tempMove))
                            {
                                lightList.Add(tempMove);
                                numberOfPossibleMovesB++;
                            }
                        }
                    }

                    //Up Left / Right
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos; //resets tempUp
                        tempMove -= moveForward - dir; // gets forward one, right / left one position

                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "White")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesChecking.Add(tempMove);
                                    piecesPuttingKingInCheck += 1;
                                }
                                else
                                {
                                    lightList.Add(tempMove);
                                    numberOfPossibleMovesB++;
                                }
                            }
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) // Can only move forwards or take a piece that is pinning it
            {
                if (selectedPiece.tag == "White")
                {
                    //move forward one//
                    tempMove = startingPos + moveForward;
                    if (pinnedPiecePath[startingPos].Contains(tempMove) && !piecePosition.ContainsKey(tempMove)) // checks if the path forwards has a space to move into that is not the piece pinning it.
                    {
                        lightList.Add(tempMove);
                        numberOfPossibleMovesW++;
                    }

                    //move forward two
                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                    {
                        tempMove = startingPos;
                        tempMove += moveForward;
                        if (!piecePosition.ContainsKey(tempMove))
                        {
                            tempMove += moveForward;
                            if (pinnedPiecePath[startingPos].Contains(tempMove) && !piecePosition.ContainsKey(tempMove))
                            {
                                lightList.Add(tempMove);
                                numberOfPossibleMovesW++;
                            }
                        }
                    }

                    //Up Left / Right
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos; //resets tempUp
                        tempMove += moveForward + dir; // gets forward one, right / left one position
                        if (pinnedPiecePath[startingPos].Contains(tempMove) && piecePosition.ContainsKey(tempMove))
                        {
                            lightList.Add(tempMove);
                            numberOfPossibleMovesW++;
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //move forward one//
                    tempMove = startingPos - moveForward;
                    if (pinnedPiecePath[startingPos].Contains(tempMove) && !piecePosition.ContainsKey(tempMove)) // checks if the path forwards has a space to move into that is not the piece pinning it.
                    {
                        numberOfPossibleMovesB++;
                        lightList.Add(tempMove);
                    }

                    //move forward two
                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                    {
                        tempMove = startingPos;
                        tempMove -= moveForward;
                        if (!piecePosition.ContainsKey(tempMove))
                        {
                            tempMove -= moveForward;
                            if (pinnedPiecePath[startingPos].Contains(tempMove) && !piecePosition.ContainsKey(tempMove))
                            {
                                numberOfPossibleMovesB++;
                                lightList.Add(tempMove);
                            }
                        }
                    }

                    //Up Left / Right
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos; //resets tempUp
                        tempMove -= moveForward - dir; // gets forward one, right / left one position
                        if (pinnedPiecePath[startingPos].Contains(tempMove) && piecePosition.ContainsKey(tempMove))
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }
                    }
                }
            }
        }

        if (check == true)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) //Can only move to block or take a piece pinning.
            {
                if (selectedPiece.tag == "White")
                {
                    //Move forward one
                    tempMove = startingPos + moveForward;
                    if (lineOfCheck.Contains(tempMove) || piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                    {
                        lightList.Add(tempMove);
                        numberOfPossibleMovesW++;
                    }

                    //move forward two (starting pos only)
                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                    {
                        tempMove = startingPos;
                        tempMove += moveForward;
                        if (!piecePosition.ContainsKey(tempMove))
                        {
                            tempMove += moveForward;
                            if (lineOfCheck.Contains(tempMove) || piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                            {
                                numberOfPossibleMovesW++;
                                lightList.Add(tempMove);
                            }
                        }
                    }
                    // Take the piece checking
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos; //resets tempUp
                        tempMove += moveForward + dir; // gets forward one, right / left one position
                        if (piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //Move forward one
                    tempMove = startingPos - moveForward;
                    if (lineOfCheck.Contains(tempMove) || piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                    {
                        numberOfPossibleMovesB++;
                        lightList.Add(tempMove);
                    }

                    //move forward two (starting pos only)
                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                    {
                        tempMove = startingPos;
                        tempMove -= moveForward;
                        if (!piecePosition.ContainsKey(tempMove))
                        {
                            tempMove += moveForward;
                            if (lineOfCheck.Contains(tempMove) || piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                            {
                                numberOfPossibleMovesB++;
                                lightList.Add(tempMove);
                            }
                        }
                    }
                    // Take the piece checking
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos; //resets tempUp
                        tempMove -= moveForward - dir; // gets forward one, right / left one position
                        if (piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) // can't move
            {

            }

        }
    }

    public void PawnTakePositions(GameObject pawn, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        Vector3 moveForward = new Vector3(0, 0, 1.25f);
        Vector3 startingPos = pawn.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[2];
        directions[0] = new Vector3(1.25f, 0, 0); //move right
        directions[1] = new Vector3(-1.25f, 0, 0); //move left
        if (pawn.tag == "White")
        {
            //Up Left / Right
            foreach (Vector3 dir in directions)
            {
                tempMove = startingPos; //resets tempUp
                tempMove += moveForward + dir; // gets forward one, right / left one position
                if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    numberOfPossibleMovesW++;
                    lightList.Add(tempMove);
                }

                else if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
                        pWPieces.Add(tempMove);
                    }
                }
            }
        }

        if (pawn.tag == "Black")
        {
            //Down Left / Right
            foreach (Vector3 dir in directions)
            {
                tempMove = startingPos; //resets tempUp
                tempMove -= moveForward - dir; // gets forward one, right / left one position
                if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    numberOfPossibleMovesB++;
                    lightList.Add(tempMove);
                }

                else if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        pBPieces.Add(tempMove);
                    }
                }

            }
        }
    }

    void RookMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        int wPossibleMoves = 0;
        int bPossibleMoves = 0;
        bool pieceFound = false;
        Vector3 startingPos = selectedPiece.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[4];
        directions[0] = new Vector3(1.25f, 0, 0);  // moveForward
        directions[1] = new Vector3(-1.25f, 0, 0); // moveBackward
        directions[2] = new Vector3(0, 0, 1.25f);  // moveRight
        directions[3] = new Vector3(0, 0, -1.25f); // moveLeft

        if (check == true)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) //Movement can only be made when blocking, or taking a piece checking the king.
            {
                if (selectedPiece.tag == "White")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);

                    //CHECK FOR BLOCK OR TAKE
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;
                        //regular movement with no pieces in the way
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                            {
                                numberOfPossibleMovesW++;
                                lightList.Add(tempMove);
                            }
                            tempMove += dir;
                        }

                        if (piecesChecking.Contains(tempMove))
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);

                    //CHECK FOR BLOCK OR TAKE
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                            {
                                numberOfPossibleMovesB++;
                                lightList.Add(tempMove);
                            }
                            tempMove -= dir;
                        }

                        if (piecesChecking.Contains(tempMove))
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) // Movement cannot be made
            {
                if (selectedPiece.tag == "White")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);
                }

                if (selectedPiece.tag == "Black")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);
                }
            }
        }

        if (check == false)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) //Full mobility of the piece
            {
                if (selectedPiece.tag == "White")
                {
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos); //adds the position of the piece to the list.
                        tempMove = startingPos;
                        tempMove += dir;
                        //regular movement with no pieces in the way
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                            tempMoves.Add(tempMove);
                            tempMove += dir;
                        }
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesPuttingKingInCheck += 1;
                                    piecesChecking.Add(tempMove);
                                    Vector3 tmp = tempMove;
                                    for (int i = 0; tmp != selectedPiece.transform.localPosition; i++)
                                    {
                                        tmp -= dir;
                                        if (tmp != selectedPiece.transform.localPosition)
                                        {
                                            lineOfCheck.Add(tmp);
                                        }
                                    }
                                }
                                else
                                {
                                    numberOfPossibleMovesW++;
                                    lightList.Add(tempMove);
                                    pinnedPiece = tempMove;
                                    pieceFound = true;
                                }
                            }

                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove += dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove += dir;
                            }

                            if (piecePosition.ContainsKey(tempMove) && !pinnedPiecePath.ContainsKey(pinnedPiece) && !pinnedPiecePath.ContainsValue(tempMoves))
                            {
                                if (piecePosition[tempMove].tag == "Black" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>(); // This is used to track the piece being pinned (this) and the path 
                        tempMoves.Add(startingPos); //adds the position of the piece to the list.
                        tempMove = startingPos;
                        tempMove -= dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesB++;
                            tempMoves.Add(tempMove);
                            lightList.Add(tempMove);
                            tempMove -= dir;
                        }
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "White")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesPuttingKingInCheck += 1;
                                    piecesChecking.Add(tempMove);
                                    Vector3 tmp = tempMove;
                                    for (int i = 0; tmp != selectedPiece.transform.localPosition; i++)
                                    {
                                        tmp += dir;
                                        if (tmp != selectedPiece.transform.localPosition)
                                        {
                                            lineOfCheck.Add(tmp);
                                        }
                                    }
                                }

                                else
                                {
                                    numberOfPossibleMovesB++;
                                    lightList.Add(tempMove);
                                    pinnedPiece = tempMove;
                                    pieceFound = true;
                                }
                            }

                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pBPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove -= dir;
                            //Checks for if the piece before is pinned
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove -= dir;
                            }

                            if (piecePosition.ContainsKey(tempMove) && !pinnedPiecePath.ContainsKey(pinnedPiece) && !pinnedPiecePath.ContainsValue(tempMoves))
                            {
                                if (piecePosition[tempMove].tag == "White" && piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) // Movement limited to moving along the rank of the line of sight of the piece pinning or take the pinning piece
            {
                if (selectedPiece.tag == "White")
                {
                    //Move Along the Pinned Rank
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;
                        //Regular movement with no pieces in the way
                        if (pinnedPiecePath.ContainsKey(startingPos))
                        {
                            while (pinnedPiecePath[startingPos].Contains(tempMove))
                            {
                                numberOfPossibleMovesW++;
                                lightList.Add(tempMove);
                                tempMove += dir;
                            }
                        }
                    }

                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);
                }

                if (selectedPiece.tag == "Black")
                {
                    //Move Along the Pinned Rank
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        //Regular movement with no pieces in the way
                        if (pinnedPiecePath.ContainsKey(startingPos))
                        {
                            while (pinnedPiecePath[startingPos].Contains(tempMove))
                            {
                                numberOfPossibleMovesB++;
                                lightList.Add(tempMove);
                                tempMove -= dir;
                            }
                        }
                    }

                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);
                }
            }
        }

    }

    void KingMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces, int numOfMovesW, int numOfMovesB)
    {
        Vector3 startingPos = selectedPiece.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[8];
        directions[0] = new Vector3(-1.25f, 0, -1.25f); //down left
        directions[1] = new Vector3(0, 0, -1.25f); //down
        directions[2] = new Vector3(1.25f, 0, -1.25f); //down right
        directions[3] = new Vector3(1.25f, 0, 0); //right
        directions[4] = new Vector3(1.25f, 0, 1.25f); //right up
        directions[5] = new Vector3(0, 0, 1.25f); //up
        directions[6] = new Vector3(-1.25f, 0, 1.25f); //up left
        directions[7] = new Vector3(-1.25f, 0, 0); //left

        foreach (Vector3 dir in directions)
        {
            tempMove = startingPos;
            tempMove += dir;

            if (selectedPiece.tag == "White")
            {
                if (!piecePosition.ContainsKey(tempMove) && !UIEngineReference.blackPossiblePositions.Contains(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    numOfMovesW++;
                    lightList.Add(tempMove);
                }

                if (piecePosition.ContainsKey(tempMove) && !UIEngineReference.ProtectedBlackPieces.Contains(tempMove)) //checks to see if there is a piece in the moveable space
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        numOfMovesW++;
                        lightList.Add(tempMove);
                    }

                    if (piecePosition[tempMove].tag == "White")
                    {
                        pWPieces.Add(tempMove);
                    }
                }
            }

            if (selectedPiece.tag == "Black")
            {
                if (!piecePosition.ContainsKey(tempMove) && !UIEngineReference.whitePossiblePositions.Contains(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    numOfMovesB++;
                    lightList.Add(tempMove);
                }

                if (piecePosition.ContainsKey(tempMove) && !UIEngineReference.ProtectedWhitePieces.Contains(tempMove)) //checks to see if there is a piece in the moveable space
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
                        numOfMovesB++;
                        lightList.Add(tempMove);
                    }

                    if (piecePosition[tempMove].tag == "Black")
                    {
                        pBPieces.Add(tempMove);
                    }
                }
            }
        }
    }

    void BishopMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        bool pieceFound = false;
        Vector3 startingPos = selectedPiece.transform.localPosition;
        Vector3[] directions = new Vector3[4];
        directions[0] = new Vector3(1.25f, 0, 1.25f); //moveUpRight
        directions[1] = new Vector3(-1.25f, 0, 1.25f); //moveUpLeft
        directions[2] = new Vector3(1.25f, 0, -1.25f); //moveDownRight
        directions[3] = new Vector3(-1.25f, 0, -1.25f); //moveDownLeft
        Vector3 tempMove;

        if (check == true)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) //Can block the check or take the piece checking 
            {
                if (selectedPiece.tag == "White")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos); // adds the orignal piece
                        tempMove = startingPos;
                        tempMove += dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            tempMoves.Add(tempMove);
                            tempMove += dir;
                        }
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pinnedPiece = tempMove;
                                pieceFound = true;
                            }

                            //Protected Piece
                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove += dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove += dir;
                            }

                            if (piecePosition.ContainsKey(tempMove))
                            {
                                if (piecePosition[tempMove].tag == "Black" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }

                    //CHECK FOR BLOCK OR TAKE
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                            {
                                numberOfPossibleMovesW++;
                                lightList.Add(tempMove);
                            }
                            tempMove += dir;
                        }

                        if (piecesChecking.Contains(tempMove))
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                        }

                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos); // adds the orignal piece
                        tempMove = startingPos;
                        tempMove -= dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesB++;
                            tempMoves.Add(tempMove);
                            tempMove -= dir;
                        }
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "White")
                            {
                                pinnedPiece = tempMove;
                                pieceFound = true;
                            }

                            //Protected Piece
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove -= dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove -= dir;
                            }

                            if (piecePosition.ContainsKey(tempMove))
                            {
                                if (piecePosition[tempMove].tag == "White" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }

                    //CHECK FOR BLOCK OR TAKE
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                            {
                                numberOfPossibleMovesB++;
                                lightList.Add(tempMove);
                            }
                            tempMove -= dir;
                        }

                        if (piecesChecking.Contains(tempMove))
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }
                    }

                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) //Movement cannot be made
            {
                if (selectedPiece.tag == "White")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos); // adds the orignal piece
                        tempMove = startingPos;
                        tempMove += dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            tempMoves.Add(tempMove);
                            tempMove += dir;
                        }
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pinnedPiece = tempMove;
                                pieceFound = true;
                            }

                            //Protected Piece
                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove += dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove += dir;
                            }

                            if (piecePosition.ContainsKey(tempMove))
                            {
                                if (piecePosition[tempMove].tag == "Black" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //CHECK FOR PIN + PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos); // adds the orignal piece
                        tempMove = startingPos;
                        tempMove -= dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            tempMoves.Add(tempMove);
                            tempMove -= dir;
                        }
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "White")
                            {
                                pinnedPiece = tempMove;
                                pieceFound = true;
                            }

                            //Protected Piece
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove -= dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove -= dir;
                            }

                            if (piecePosition.ContainsKey(tempMove))
                            {
                                if (piecePosition[tempMove].tag == "White" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (check == false)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) // Normal movement
            {
                if (selectedPiece.tag == "White")
                {
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos);
                        tempMove = startingPos;
                        tempMove += dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                            tempMoves.Add(tempMove);
                            tempMove += dir;
                        }

                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesPuttingKingInCheck += 1;
                                    piecesChecking.Add(tempMove);
                                    Vector3 tmp = tempMove;
                                    for (int i = 0; tmp != startingPos; i++)
                                    {
                                        tmp -= dir;
                                        if (tmp != selectedPiece.transform.localPosition)
                                        {
                                            lineOfCheck.Add(tmp);
                                        }
                                    }
                                }
                                else
                                {
                                    numberOfPossibleMovesW++;
                                    lightList.Add(tempMove);
                                    pinnedPiece = tempMove;
                                    pieceFound = true;
                                }
                            }

                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove += dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove += dir;
                            }

                            if (piecePosition.ContainsKey(tempMove) && !pinnedPiecePath.ContainsKey(pinnedPiece) && !pinnedPiecePath.ContainsValue(tempMoves))
                            {
                                if (piecePosition[tempMove].tag == "Black" && piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    foreach (Vector3 dir in directions)
                    {
                        List<Vector3> tempMoves = new List<Vector3>();
                        tempMoves.Add(startingPos);
                        tempMove = startingPos;
                        tempMove -= dir;
                        while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesB++;
                            tempMoves.Add(tempMove);
                            lightList.Add(tempMove);
                            tempMove -= dir;
                        }

                        if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "White")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesPuttingKingInCheck += 1;
                                    piecesChecking.Add(tempMove);
                                    Vector3 tmp = tempMove;
                                    for (int i = 0; tmp != startingPos; i++)
                                    {
                                        tmp += dir;
                                        if (tmp != selectedPiece.transform.localPosition)
                                        {
                                            lineOfCheck.Add(tmp);
                                        }
                                    }
                                }
                                else
                                {
                                    numberOfPossibleMovesB++;
                                    lightList.Add(tempMove);
                                    pinnedPiece = tempMove;
                                    pieceFound = true;
                                }
                            }
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pBPieces.Add(tempMove);
                            }
                        }

                        if (pieceFound == true)
                        {
                            tempMove -= dir;
                            while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                            {
                                tempMoves.Add(tempMove);
                                tempMove -= dir;
                            }

                            if (piecePosition.ContainsKey(tempMove) && !pinnedPiecePath.ContainsKey(pinnedPiece) && !pinnedPiecePath.ContainsValue(tempMoves))
                            {
                                if (piecePosition[tempMove].tag == "White" && piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    Debug.Log("Accessed");
                                    piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                                    pinnedPiecePath.Add(pinnedPiece, tempMoves);
                                }
                            }
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) //can only move in the path of the piece pinning or take the piece.
            {
                if (selectedPiece.tag == "White")
                {
                    //Move Along the Pinned Rank
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;
                        //Regular movement with no pieces in the way
                        if (pinnedPiecePath.ContainsKey(startingPos))
                        {
                            while (pinnedPiecePath[startingPos].Contains(tempMove))
                            {
                                numberOfPossibleMovesW++;
                                lightList.Add(tempMove);
                                tempMove += dir;
                            }
                        }
                    }

                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);
                }

                if (selectedPiece.tag == "Black")
                {
                    //Move Along the Pinned Rank
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        //Regular movement with no pieces in the way
                        if (pinnedPiecePath.ContainsKey(startingPos))
                        {
                            while (pinnedPiecePath[startingPos].Contains(tempMove))
                            {
                                numberOfPossibleMovesB++;
                                lightList.Add(tempMove);
                                tempMove -= dir;
                            }
                        }
                    }

                    //CHECK FOR PIN + PROTECTED PIECES
                    CheckPinAndProtectedPieces(selectedPiece, directions, startingPos, ref pWPieces, ref pBPieces);
                }
            }
        }
    }

    void KnightMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        Vector3 startingPos = selectedPiece.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[8];
        directions[0] = new Vector3(-1.25f, 0, -2.5f); // Down Down left
        directions[1] = new Vector3(1.25f, 0, -2.5f); // Down Down Right
        directions[2] = new Vector3(2.5f, 0, -1.25f); // Right Right Down
        directions[3] = new Vector3(2.5f, 0, 1.25f); // Right Right Up
        directions[4] = new Vector3(1.25f, 0, 2.5f); // Up Up Right
        directions[5] = new Vector3(-1.25f, 0, 2.5f); // Up Up Left
        directions[6] = new Vector3(-2.5f, 0, 1.25f); // Left Left Up
        directions[7] = new Vector3(-2.5f, 0, -1.25f); // Left Left Down

        if (check == false)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) //Normal movement
            {
                if (selectedPiece.tag == "White")
                {
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;
                        if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                        }

                        else if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesPuttingKingInCheck += 1;
                                    piecesChecking.Add(tempMove);
                                }

                                else
                                {
                                    numberOfPossibleMovesW++;
                                    lightList.Add(tempMove);
                                }
                            }

                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }

                        else if (piecePosition.ContainsKey(tempMove))
                        {
                            if (piecePosition[tempMove].tag == "White")
                            {
                                if (piecePosition[tempMove].transform.GetChild(0).tag == "King")
                                {
                                    piecesPuttingKingInCheck += 1;
                                    piecesChecking.Add(tempMove);
                                }

                                else
                                {
                                    numberOfPossibleMovesB++;
                                    lightList.Add(tempMove);
                                }
                            }


                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pBPieces.Add(tempMove);
                            }
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) // Can't move
            {
                if (selectedPiece.tag == "White")
                {
                    //PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;

                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            //Protected Piece
                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            //Protected Piece
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }
                    }
                }
            }
        }

        if (check == true)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false)
            {
                if (selectedPiece.tag == "White")
                {
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;

                        if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                        }

                        if (piecesChecking.Contains(tempMove))
                        {
                            numberOfPossibleMovesW++;
                            lightList.Add(tempMove);
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;

                        if (lineOfCheck.Contains(tempMove))
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }

                        if (piecesChecking.Contains(tempMove))
                        {
                            numberOfPossibleMovesB++;
                            lightList.Add(tempMove);
                        }
                    }
                }
            }

            if (selectedPiece.GetComponent<Track>().pinned == true) // Can't move
            {
                if (selectedPiece.tag == "White")
                {
                    // PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove += dir;

                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            //Protected Piece
                            if (piecePosition[tempMove].tag == "White")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //PROTECTED PIECES
                    foreach (Vector3 dir in directions)
                    {
                        tempMove = startingPos;
                        tempMove -= dir;
                        // checks if the next space has a takeable piece on it
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            //Protected Piece
                            if (piecePosition[tempMove].tag == "Black")
                            {
                                pWPieces.Add(tempMove);
                            }
                        }
                    }
                }
            }
        }
    }

    void QueenMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        RookMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        BishopMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
    }

    public void CheckMateCheck(int nOMW, int nOMB)
    {
        if (whiteToMove)
        {
            if (numberOfPossibleMovesW == 0)
            {
                checkMate = true;
            }
        }
        else
        {
            if (numberOfPossibleMovesB == 0)
            {
                checkMate = true;
            }
        }

    }

    void CheckPinAndProtectedPieces(GameObject selectedPiece, Vector3[] directions, Vector3 startingPos, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        bool pieceFound = false;
        if (selectedPiece.tag == "White")
        {
            foreach (Vector3 dir in directions)
            {
                List<Vector3> tempMoves = new List<Vector3>();
                tempMoves.Add(startingPos); // adds the orignal piece
                Vector3 tempMove = startingPos;
                tempMove += dir;
                //regular movement with no pieces in the way
                while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    tempMoves.Add(tempMove);
                    tempMove += dir;
                }
                // checks if the next space has a takeable piece on it
                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        pinnedPiece = tempMove;
                        pieceFound = true;
                    }

                    //Protected Piece
                    if (piecePosition[tempMove].tag == "White")
                    {
                        pWPieces.Add(tempMove);
                    }
                }

                if (pieceFound == true)
                {
                    tempMove += dir;
                    while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                    {
                        tempMoves.Add(tempMove);
                        tempMove += dir;
                    }

                    if (piecePosition.ContainsKey(tempMove) && !pinnedPiecePath.ContainsKey(pinnedPiece) && !pinnedPiecePath.ContainsValue(tempMoves))
                    {
                        if (piecePosition[tempMove].tag == "Black" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                        {
                            piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                            pinnedPiecePath.Add(pinnedPiece, tempMoves);
                        }
                    }
                }
            }
        }

        if (selectedPiece.tag == "Black")
        {
            foreach (Vector3 dir in directions)
            {
                List<Vector3> tempMoves = new List<Vector3>();
                tempMoves.Add(startingPos); // adds the orignal piece
                Vector3 tempMove = startingPos;
                tempMove -= dir;
                //regular movement with no pieces in the way
                while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    tempMoves.Add(tempMove);
                    tempMove -= dir;
                }
                // checks if the next space has a takeable piece on it
                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
                        pinnedPiece = tempMove;
                        pieceFound = true;
                    }

                    //Protected Piece
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        pWPieces.Add(tempMove);
                    }
                }

                if (pieceFound == true)
                {
                    tempMove -= dir;
                    while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                    {
                        tempMoves.Add(tempMove);
                        tempMove -= dir;
                    }

                    if (piecePosition.ContainsKey(tempMove))
                    {
                        if (piecePosition[tempMove].tag == "White" && piecePosition[tempMove].transform.GetChild(0).tag == "King") // checks if the next piece is the opponents king
                        {
                            piecePosition[pinnedPiece].GetComponent<Track>().pinned = true;
                            pinnedPiecePath.Add(pinnedPiece, tempMoves);
                        }
                    }
                }
            }
        }
    }
}
