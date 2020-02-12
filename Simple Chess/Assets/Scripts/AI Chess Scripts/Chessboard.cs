using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chessboard : MonoBehaviour
{
    public int halfMove = 0;
    public int fullMoves = 1, currentMove = 0;
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
    public List<GameObject> temporaryCastlingPieceRefs = new List<GameObject>();
    public GameObject temporaryEnPassantPieceRefs;
    public Vector3 pinnedPiece;
    public float bXMax = 8.75f, bXMin = 0f, bZMax = 8.75f, bZMin = 0f, bXSpace = 1.25f, bZSpace = 1.25f;
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
        #region White
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

        //for King and Rooks

        //PAWNS
        for (int i = 8; i < 16; i++)
        {
            pieces[i] = Instantiate(Pawn, chessBoard.transform);
            pieces[i].transform.localPosition = chessBoard2D[i - 8, 1];
            pieces[i].transform.localScale = new Vector3(1, 1, 1);
            pieces[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
            pieces[i].GetComponent<Renderer>().material = white;
            pieces[i].tag = "White";
            pieces[i].AddComponent<Track>();
            piecePosition.Add(chessBoard2D[i - 8, 1], pieces[i]);

        }
        #endregion

        #region Black
        //PAWNS//
        for (int i = 16; i < 24; i++)
        {
            pieces[i] = Instantiate(Pawn, chessBoard.transform);
            pieces[i].transform.localPosition = chessBoard2D[i - 16, 6];
            pieces[i].transform.localScale = new Vector3(1, 1, 1);
            pieces[i].transform.localRotation = new Quaternion(0, 0, 0, 0);
            pieces[i].GetComponent<Renderer>().material = black;
            pieces[i].AddComponent<Track>();
            pieces[i].tag = "Black";
            piecePosition.Add(chessBoard2D[i - 16, 6], pieces[i]);

        }
        //REST OF PIECES//
        pieces[24] = Instantiate(Rook, chessBoard.transform);
        pieces[25] = Instantiate(Knight, chessBoard.transform);
        pieces[26] = Instantiate(Bishop, chessBoard.transform);
        pieces[27] = Instantiate(Queen, chessBoard.transform);
        pieces[28] = Instantiate(King, chessBoard.transform);
        pieces[29] = Instantiate(Bishop, chessBoard.transform);
        pieces[30] = Instantiate(Knight, chessBoard.transform);
        pieces[31] = Instantiate(Rook, chessBoard.transform);

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
        #endregion
    }

    public void GetSpaces(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces, ref List<Vector3> castlingList, ref List<Vector3> enPassantL, ref List<Vector3> diagonalSpaces)
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            PawnMovement(selectedPiece, ref lightList, ref enPassantL, ref temporaryEnPassantPieceRefs, ref diagonalSpaces);
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
            KingMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces, numberOfPossibleMovesW, numberOfPossibleMovesB, ref castlingList, temporaryCastlingPieceRefs);
        }
        else if (selectedPiece.transform.GetChild(0).tag == "Queen")
        {
            QueenMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        }
    }

    public void PawnMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> enPassantL, ref GameObject enemyPawnEnPassant, ref List<Vector3> diagonalSpaces)
    {
        enPassantL.Clear();
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
                                    piecesChecking.Add(startingPos);
                                    piecesPuttingKingInCheck += 1;
                                }

                                else
                                {
                                    numberOfPossibleMovesW++;
                                    lightList.Add(tempMove);
                                }
                            }
                        }
                        diagonalSpaces.Add(tempMove);
                        //Check for En Passant
                        tempMove = startingPos; //resets tempUp
                        tempMove += dir;
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            GameObject temp = piecePosition[tempMove];
                            if (temp.transform.GetChild(0).tag == "Pawn" && temp.tag == "Black")
                            {
                                if (temp.GetComponent<Track>().enPassant == true)
                                {
                                    enPassantL.Add(temp.transform.localPosition + moveForward);
                                    enemyPawnEnPassant = temp;
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
                                    piecesChecking.Add(startingPos);
                                    piecesPuttingKingInCheck += 1;
                                }
                                else
                                {
                                    lightList.Add(tempMove);
                                    numberOfPossibleMovesB++;
                                }
                            }
                        }
                        diagonalSpaces.Add(tempMove);

                        //Check for En Passant
                        tempMove = startingPos; //resets tempUp
                        tempMove -= dir;
                        if (piecePosition.ContainsKey(tempMove))
                        {
                            GameObject temp = piecePosition[tempMove];
                            if (temp.transform.GetChild(0).tag == "Pawn" && temp.tag == "White")
                            {
                                if (temp.GetComponent<Track>().enPassant == true)
                                {
                                    enPassantL.Add(temp.transform.localPosition - moveForward);
                                    enemyPawnEnPassant = temp;
                                }
                            }
                        }
                    }
                }
            }

            else if (selectedPiece.GetComponent<Track>().pinned == true) // Can only move forwards or take a piece that is pinning it
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
                        diagonalSpaces.Add(tempMove);
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
                        diagonalSpaces.Add(tempMove);
                    }
                }
            }
        }

        else if (check == true)
        {
            if (selectedPiece.GetComponent<Track>().pinned == false) //Can only move to block or take a piece pinning.
            {
                if (selectedPiece.tag == "White")
                {
                    //Move forward one
                    tempMove = startingPos + moveForward;
                    if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
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
                            if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
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

                        tempMove = startingPos; //resets tempUp
                        tempMove -= dir;
                        if (piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                        {
                            if (piecePosition.ContainsKey(tempMove))
                            {
                                GameObject temp = piecePosition[tempMove];
                                if (temp.transform.GetChild(0).tag == "Pawn" && temp.tag == "White")
                                {
                                    if (temp.GetComponent<Track>().enPassant == true)
                                    {
                                        numberOfPossibleMovesW++;
                                        enPassantL.Add(temp.transform.localPosition - moveForward);
                                        enemyPawnEnPassant = temp;
                                    }
                                }
                            }
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    //Move forward one
                    tempMove = startingPos - moveForward;
                    if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
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
                            if (lineOfCheck.Contains(tempMove) && piecesPuttingKingInCheck == 1)
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

                        tempMove = startingPos; //resets tempUp
                        tempMove -= dir;
                        if (piecesChecking.Contains(tempMove) && piecesPuttingKingInCheck == 1)
                        {
                            if (piecePosition.ContainsKey(tempMove))
                            {
                                GameObject temp = piecePosition[tempMove];
                                if (temp.transform.GetChild(0).tag == "Pawn" && temp.tag == "Black")
                                {
                                    if (temp.GetComponent<Track>().enPassant == true)
                                    {
                                        numberOfPossibleMovesB++;
                                        enPassantL.Add(temp.transform.localPosition - moveForward);
                                        enemyPawnEnPassant = temp;
                                    }
                                }
                            }
                        }
                    }


                }
            }

            else if (selectedPiece.GetComponent<Track>().pinned == true) // can't move
            {

            }

        }
    }

    public void PawnTakePositions(GameObject pawn, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces, ref List<Vector3> enPassantLL)
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

            //En Passant
            foreach (Vector3 dir in directions)
            {

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

                        //Regular movement with no pieces in the way
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
                                    piecesChecking.Add(startingPos);
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
                                    piecesChecking.Add(startingPos);
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

    void KingMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces, int numOfMovesW, int numOfMovesB, ref List<Vector3> castlingLightList, List<GameObject> tempCastlingPieceRef)
    {
        tempCastlingPieceRef.Clear();
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
        
        if (selectedPiece.GetComponent<Track>().startingPosition == true)
        {
            //cannot castle whilst in check
            if (check == false)
            {
                //check if the rooks are in their starting positions.
                Vector3[] rookDir = new Vector3[2];
                Vector3 tempRookMove;

                rookDir[0] = new Vector3(-1.25f, 0, 0); //Left
                rookDir[1] = new Vector3(1.25f, 0, 0); //Right

                foreach (Vector3 rookDirections in rookDir)
                {
                    bool inLineOfCheck = false;
                    bool movingLeft;
                    tempRookMove = startingPos;
                    tempRookMove += rookDirections;

                    if (tempRookMove.x > startingPos.x)
                    {
                        movingLeft = false;
                    }
                    else
                    {
                        movingLeft = true;
                    }
                    if (selectedPiece.tag == "White")
                    {
                        while (!piecePosition.ContainsKey(tempRookMove) && tempRookMove.x >= bXMin && tempRookMove.z >= bZMin && tempRookMove.x <= bXMax && tempRookMove.z <= bZMax && inLineOfCheck == false)
                        {
                            if (movingLeft == true)
                            {
                                if (UIEngineReference.blackPossiblePositions.Contains(tempRookMove) && tempRookMove.x >= (startingPos.x + (rookDirections.x * 2)))
                                {
                                    inLineOfCheck = true;
                                }
                            }
                            else
                            {
                                if (UIEngineReference.blackPossiblePositions.Contains(tempRookMove) && startingPos.x <= (startingPos.x + (rookDirections.x * 2)))
                                {
                                    inLineOfCheck = true;
                                }
                            }

                            tempRookMove += rookDirections;
                        }
                    }
                    else
                    {
                        while (!piecePosition.ContainsKey(tempRookMove) && tempRookMove.x >= bXMin && tempRookMove.z >= bZMin && tempRookMove.x <= bXMax && tempRookMove.z <= bZMax && inLineOfCheck == false)
                        {
                            if (movingLeft == true)
                            {
                                if (UIEngineReference.whitePossiblePositions.Contains(tempRookMove) && tempRookMove.x >= (startingPos.x + (rookDirections.x * 2)))
                                {
                                    inLineOfCheck = true;
                                }
                            }
                            else
                            {
                                if (UIEngineReference.whitePossiblePositions.Contains(tempRookMove) && startingPos.x <= (startingPos.x + (rookDirections.x * 2)))
                                {
                                    inLineOfCheck = true;
                                }
                            }

                            tempRookMove += rookDirections;
                        }
                    }
                    //check if a rook has been found
                    if (inLineOfCheck == false)
                    {
                        if (piecePosition.ContainsKey(tempRookMove))
                        {
                            if (piecePosition[tempRookMove].transform.GetChild(0).tag == "Rook")
                            {
                                //check if the rook has moved
                                if (piecePosition[tempRookMove].GetComponent<Track>().startingPosition == true)
                                {
                                    castlingLightList.Add(startingPos + (2 * rookDirections));
                                    tempCastlingPieceRef.Add(piecePosition[tempRookMove]);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        foreach (Vector3 dir in directions)
        {
            tempMove = startingPos;
            tempMove += dir;

            if (check == false)
            {
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

            else if (check == true)
            {
                if (selectedPiece.tag == "White")
                {
                    if (!piecePosition.ContainsKey(tempMove) && !UIEngineReference.blackPossiblePositions.Contains(tempMove) && !lineOfCheck.Contains(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                    {
                        numOfMovesW++;
                        lightList.Add(tempMove);
                    }

                    if (piecePosition.ContainsKey(tempMove) && !UIEngineReference.ProtectedBlackPieces.Contains(tempMove))
                    {
                        if (piecePosition[tempMove].tag == "Black")
                        {
                            numOfMovesW++;
                            lightList.Add(tempMove);
                        }
                    }
                }

                if (selectedPiece.tag == "Black")
                {
                    if (!piecePosition.ContainsKey(tempMove) && !UIEngineReference.whitePossiblePositions.Contains(tempMove) && !lineOfCheck.Contains(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                    {
                        numOfMovesB++;
                        lightList.Add(tempMove);
                    }

                    if (piecePosition.ContainsKey(tempMove) && !UIEngineReference.ProtectedWhitePieces.Contains(tempMove))
                    {
                        if (piecePosition[tempMove].tag == "White")
                        {
                            numOfMovesB++;
                            lightList.Add(tempMove);
                        }
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
                                    piecesChecking.Add(startingPos);
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
                                    piecesChecking.Add(startingPos);
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
                                    piecesChecking.Add(startingPos);
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
                                    piecesChecking.Add(startingPos);
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
                                pBPieces.Add(tempMove);
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
                                pBPieces.Add(tempMove);
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
            if (check == true && numberOfPossibleMovesW == 0)
            {
                checkMate = true;
            }
        }
        else
        {
            if (check == true && numberOfPossibleMovesB == 0)
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
    
    public void MoveCounter(ref int currentM)
    {
        currentM++;
        if (currentM % 2 == 0)
        {
            fullMoves++;
        }
    }

    public string GetBoardState()
    {
        string fenString = "";

        //rules 
        /* 1.   Piece placement (from White's perspective). Each rank is described, starting with rank 8 and ending with rank 1; within each rank, 
           the contents of each square are described from file "a" through file "h". Following the Standard Algebraic Notation (SAN), each piece 
           is identified by a single letter taken from the standard English names (pawn = "P", knight = "N", bishop = "B", rook = "R",
           queen = "Q" and king = "K").[1] White pieces are designated using upper-case letters ("PNBRQK") while black pieces use lowercase ("pnbrqk").
           Empty squares are noted using digits 1 through 8 (the number of empty squares), and "/" separates ranks. */ //DONE//

        /* 2.  Active color. "w" means White moves next, "b" means Black moves next. */

        /* 3.  Castling availability. If neither side can castle, this is "-". Otherwise, this has one or more letters: "K" (White can castle kingside), 
           "Q" (White can castle queenside), "k" (Black can castle kingside), and/or "q" (Black can castle queenside). */

        /* 4.   En passant target square in algebraic notation. If there's no en passant target square, this is "-". If a pawn has just made a two-square move, 
            this is the position "behind" the pawn. This is recorded regardless of whether there is a pawn in position to make an en passant capture        */

        /* 5.   Halfmove clock: This is the number of halfmoves since the last capture or pawn advance. 
            This is used to determine if a draw can be claimed under the fifty-move rule.        */

        /* 6.   Fullmove number: The number of the full move. It starts at 1, and is incremented after Black's move.    */


        #region Piece Placement
        int emptySpaceCount = 0;
        for (float z = bZMax; z >= bZMin; z -= 1.25f)
        {
            for (float x = bXMin; x <= bXMax; x += 1.25f)
            {
                //for new line exluding first line
                if (!z.Equals(bZMax) && x == 0)
                {
                    if (emptySpaceCount > 0)
                    {
                        fenString += emptySpaceCount.ToString();
                    }
                    fenString += "/";
                    emptySpaceCount = 0;
                }

                if (piecePosition.ContainsKey(new Vector3(x, 0, z)))
                {
                    if (emptySpaceCount > 0)
                    {
                        fenString += emptySpaceCount.ToString();
                        emptySpaceCount = 0;
                    }
                    fenString += PieceConverter(piecePosition[new Vector3(x, 0, z)].transform.GetChild(0).tag, piecePosition[new Vector3(x, 0, z)].tag);
                }
                else
                {
                    emptySpaceCount++;
                }


            }
        }
        #endregion

        #region Active Colour
        if (whiteToMove == true)
        {
            fenString += " w ";
        }
        else
        {
            fenString += " b ";
        }
        #endregion

        #region Castling Availability
        bool bKingStartingPos = false, wKingStartingPos = false;
        bool wKingSideC = false, wQueenSideC = false, bKingSideC = false, bQueenSideC = false;
        foreach (KeyValuePair<Vector3, GameObject> piece in piecePosition)
        {
            if (piece.Value.tag == "White")
            {
                if (piece.Value.transform.GetChild(0).tag == "King")
                {
                    if (piece.Value.GetComponent<Track>().startingPosition == true)
                    {
                        wKingStartingPos = true;
                    }
                }

                else if (piece.Value.transform.GetChild(0).tag == "Rook")
                {
                    if (piece.Key.x == bXMin)
                    {
                        if (piece.Value.GetComponent<Track>().startingPosition == true)
                        {
                            wKingSideC = true;
                        }
                    }
                    else if (piece.Key.x == bXMax)
                    {
                        if (piece.Value.GetComponent<Track>().startingPosition == true)
                        {
                            wQueenSideC = true;
                        }
                    }
                    
                }
            }

            if (piece.Value.tag == "Black")
            {
                if (piece.Value.transform.GetChild(0).tag == "King")
                {
                    if (piece.Value.GetComponent<Track>().startingPosition == true)
                    {
                        bKingStartingPos = true;
                    }
                }

                else if (piece.Value.transform.GetChild(0).tag == "Rook")
                {
                    if (piece.Key.x == bXMin)
                    {
                        if (piece.Value.GetComponent<Track>().startingPosition == true)
                        {
                            bKingSideC = true;
                        }
                    }
                    else if (piece.Key.x == bXMax)
                    {
                        if (piece.Value.GetComponent<Track>().startingPosition == true)
                        {
                            bQueenSideC = true;
                        }
                    }

                }
            }
        }

        if (wKingStartingPos == true)
        {
            if (wKingSideC == true)
            {
                fenString += "K";
            }
            if (wQueenSideC == true)
            {
                fenString += "Q";
            }
        }
        if (bKingStartingPos == true)
        {
            if (bQueenSideC == true)
            {
                fenString += "k";
            }
            if (bKingSideC == true)
            {
                fenString += "q";
            }
        }

        if (wKingStartingPos == false && bKingStartingPos == false)
        {
            fenString += "-";
        }
        #endregion

        #region En Passant
        Vector3 difference = new Vector3(0, 0, 1.25f);
        bool enPassantFound = false;
        foreach (KeyValuePair<Vector3,GameObject> piece in piecePosition)
        {
            if (piece.Value.tag == "White")
            {
                if (piece.Value.GetComponent<Track>().enPassant == true)
                {
                    //Debug.Log("en passant: " + piece.Value.tag + piece.Value.transform.localPosition.ToString("F2"));
                    fenString += " " + ConvertPositionIntoBoardState(piece.Value.transform.localPosition - difference) + " ";
                    enPassantFound = true;
                    break;
                }
            }
            else
            {
                if (piece.Value.GetComponent<Track>().enPassant == true)
                {
                    //Debug.Log("en passant: " + piece.Value.tag + piece.Value.transform.localPosition.ToString("F2"));
                    fenString += " " + ConvertPositionIntoBoardState(piece.Value.transform.localPosition + difference) + " ";
                    enPassantFound = true;
                    break;
                }
            }
        }

        if (enPassantFound == false)
        {
            fenString += " - ";
        }

        #endregion

        #region Halfmove Clock

        fenString += halfMove.ToString();

        #endregion

        #region Fullmove Number

        fenString += " " + fullMoves.ToString();

        #endregion

        return fenString;

    }

    public string PieceConverter(string pieceName, string pieceColour)
    {
        string s = "";
        if (pieceColour == "Black")
        {
            if (pieceName == "Pawn")
            {
                s = "p";
            }
            else if (pieceName == "Rook")
            {
                s = "r";
            }
            else if (pieceName == "Knight")
            {
                s = "n";
            }
            else if (pieceName == "Bishop")
            {
                s = "b";
            }
            else if (pieceName == "Queen")
            {
                s = "q";
            }
            else if (pieceName == "King")
            {
                s = "k";
            }
        }

        else if (pieceColour == "White")
        {
            if (pieceName == "Pawn")
            {
                s = "P";
            }
            else if (pieceName == "Rook")
            {
                s = "R";
            }
            else if (pieceName == "Knight")
            {
                s = "N";
            }
            else if (pieceName == "Bishop")
            {
                s = "B";
            }
            else if (pieceName == "Queen")
            {
                s = "Q";
            }
            else if (pieceName == "King")
            {
                s = "K";
            }
        }


        return s;

    }

    public string ConvertPositionIntoBoardState(Vector3 currentPos)
    {
        string boardPos = "";

        if (currentPos.x.Equals(bXMin))
        {
            boardPos += "a";
        }
        else if (currentPos.x.Equals((bXMin + bXSpace)))
        {
            boardPos += "b";
        }
        else if (currentPos.x.Equals(bXMin + (2 * bXSpace)))
        {
            boardPos += "c";
        }
        else if (currentPos.x.Equals(bXMin + (3 * bXSpace)))
        {
            boardPos += "d";
        }
        else if (currentPos.x.Equals(bXMin + (4 * bXSpace)))
        {
            boardPos += "e";
        }
        else if (currentPos.x.Equals(bXMin + (5 * bXSpace)))
        {
            boardPos += "f";
        }
        else if (currentPos.x.Equals(bXMin + (6 * bXSpace)))
        {
            boardPos += "g";
        }
        else if (currentPos.x.Equals(bXMin + (7 * bXSpace)))
        {
            boardPos += "h";
        }

        if (currentPos.z.Equals(bZMin))
        {
            boardPos += "1";
        }
        else if (currentPos.z.Equals(bZMin + bZSpace))
        {
            boardPos += "2";
        }
        else if (currentPos.z.Equals(bZMin + (2* bZSpace)))
        {
            boardPos += "3";
        }
        else if (currentPos.z.Equals(bZMin + (3 * bZSpace)))
        {
            boardPos += "4";
        }
        else if (currentPos.z.Equals(bZMin + (4 * bZSpace)))
        {
            boardPos += "5";
        }
        else if (currentPos.z.Equals(bZMin + (5 * bZSpace)))
        {
            boardPos += "6";
        }
        else if (currentPos.z.Equals(bZMin + (6 * bZSpace)))
        {
            boardPos += "7";
        }
        else if (currentPos.z.Equals(bZMin + (7 * bZSpace)))
        {
            boardPos += "8";
        }

        return boardPos;
    }

    public Vector3 ConvertBoardStateIntoPosition(string algebraicNotation)
    {
        Debug.Log(algebraicNotation);
        string[] characters = new string[algebraicNotation.Length];
        for (int i = 0; i < algebraicNotation.Length; i++)
        {
            characters[i] = algebraicNotation[i].ToString();
        }

        string l1 = characters[0];
        string l2 = characters[1];
        Debug.Log("l1: " + l1);
        Debug.Log("l2: " + l2);
        Vector3 position = new Vector3(0,0,0);

        if (l1 == "a")
        {
            position += new Vector3(bXMin, 0, 0);
        }
        else if (l1 == "b")
        {
            position += new Vector3(bXMin + bXSpace, 0, 0);
        }
        else if (l1 == "c")
        {
            position += new Vector3(bXMin + (2* bXSpace), 0);
        }
        else if (l1 == "d")
        {
            position += new Vector3(bXMin + (3 * bXSpace), 0);
        }
        else if (l1 == "e")
        {
            position += new Vector3(bXMin + (4 * bXSpace), 0);
        }
        else if (l1 == "f")
        {
            position += new Vector3(bXMin + (5 * bXSpace), 0);
        }
        else if (l1 == "g")
        {
            position += new Vector3(bXMin + (6 * bXSpace), 0);
        }
        else if (l1 == "h")
        {
            position += new Vector3(bXMin + (7 * bXSpace), 0);
        }

        if (l2 == "1")
        {
            position += new Vector3(0, 0, bZMin);
        }
        else if (l2 == "2")
        {
            position += new Vector3(0, 0, bZMin + bZSpace);
        }
        else if (l2 == "3")
        {
            position += new Vector3(0, 0, bZMin + (2 * bZSpace));
        }
        else if (l2 == "4")
        {
            position += new Vector3(0, 0, bZMin + (3 * bZSpace));
        }
        else if (l2 == "5")
        {
            position += new Vector3(0, 0, bZMin + (4 * bZSpace));
        }
        else if (l2 == "6")
        {
            position += new Vector3(0, 0, bZMin + (5 * bZSpace));
        }
        else if (l2 == "7")
        {
            position += new Vector3(0, 0, bZMin + (6 * bZSpace));
        }
        else if (l2 == "8")
        {
            position += new Vector3(0, 0, bZMin + (7 * bZSpace));
        }

        Debug.Log(position.ToString("f2"));
        return position;
    }
}
