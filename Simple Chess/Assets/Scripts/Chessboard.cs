using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chessboard : MonoBehaviour
{
    int numberOfPossibleMovesB, numberOfPossibleMovesW;
    public UIEngine UIEngineReference;
    public bool whiteToMove = true;
    public Material white, black;
    public GameObject Pawn, Knight, Queen, Bishop, Rook, King, chessBoard;
    public bool startingPosition = true;
    public List<Vector3> whiteSpaces, blackSpaces;
    public GameObject[] pieces = new GameObject[32];
    public Vector3[,] chessBoard2D = new Vector3[8, 8];
    public Dictionary<Vector3, GameObject> piecePosition = new Dictionary<Vector3, GameObject>();
    float bXMax = 8.75f, bXMin = 0f, bZMax = 8.75f, bZMin = 0f;
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
            pieces[i].tag = "Black";
            piecePosition.Add(chessBoard2D[i - 24, 7], pieces[i]);
        }
        //CheckBoard(ref UIEngineReference.whitePiecesPositions, ref UIEngineReference.blackPiecesPositions);
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

    public void PawnMovement(GameObject pawn, ref List<Vector3> lightList)
    {
        Vector3 moveForward = new Vector3(0, 0, 1.25f);
        Vector3 temp = pawn.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[2];
        directions[0] = new Vector3(1.25f, 0, 0); //move right
        directions[1] = new Vector3(-1.25f, 0, 0); //move left
        if (pawn.tag == "White")
        {
            //move forward one//
            tempMove = temp + moveForward;
            if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
            {
                lightList.Add(tempMove);
            }

            if (pawn.GetComponent<Track>().startingPosition == true)
            {
                //move forward two
                tempMove = temp;
                tempMove += moveForward;
                if (!piecePosition.ContainsKey(tempMove))
                {
                    tempMove += moveForward;
                    if (!piecePosition.ContainsKey(tempMove))
                    {
                        lightList.Add(tempMove);
                    }
                }
            }

            //Up Left / Right
            foreach (Vector3 dir in directions)
            {
                tempMove = temp; //resets tempUp
                tempMove += moveForward + dir; // gets forward one, right / left one position
                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        lightList.Add(tempMove);
                    }
                }
            }
        }

        else if (pawn.tag == "Black")
        {
            //move forward one//
            temp = pawn.transform.localPosition;
            tempMove = temp - moveForward;
            if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
            {
                lightList.Add(tempMove);
            }

            //Used only for when the pawn has not moved from its stating position
            if (pawn.GetComponent<Track>().startingPosition == true)
            {
                //move forward 2
                tempMove = temp;
                tempMove -= moveForward;
                if (!piecePosition.ContainsKey(tempMove))
                {
                    tempMove -= moveForward;
                    if (!piecePosition.ContainsKey(tempMove))
                    {
                        lightList.Add(tempMove);
                    }
                }
            }

            //Up Left / Right
            foreach (Vector3 dir in directions)
            {
                tempMove = temp; //resets tempUp
                tempMove -= moveForward - dir; // gets forward one, right / left one position

                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
                        lightList.Add(tempMove);
                    }
                }
            }
        }

    }

    public void PawnTakePositions(GameObject pawn, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        Vector3 moveForward = new Vector3(0, 0, 1.25f);
        Vector3 temp = pawn.transform.localPosition;
        Vector3 tempMove;
        Vector3[] directions = new Vector3[2];
        directions[0] = new Vector3(1.25f, 0, 0); //move right
        directions[1] = new Vector3(-1.25f, 0, 0); //move left
        if (pawn.tag == "White")
        {
            //Up Left / Right
            foreach (Vector3 dir in directions)
            {
                tempMove = temp; //resets tempUp
                tempMove += moveForward + dir; // gets forward one, right / left one position
                if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
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
                tempMove = temp; //resets tempUp
                tempMove -= moveForward - dir; // gets forward one, right / left one position
                if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
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
        Vector3 temp = selectedPiece.transform.localPosition;
        Vector3 tempMove = temp;
        Vector3[] directions = new Vector3[4];
        directions[0] = new Vector3(1.25f, 0, 0); // moveForward
        directions[1] = new Vector3(-1.25f, 0, 0); // moveBackward
        directions[2] = new Vector3(0, 0, 1.25f); // moveRight
        directions[3] = new Vector3(0, 0, -1.25f); // moveLeft

        if (selectedPiece.tag == "White")
        {
            foreach (Vector3 dir in directions)
            {
                tempMove = temp;
                tempMove += dir;
                //regular movement with no pieces in the way

                while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    lightList.Add(tempMove);
                    tempMove += dir;
                }
                // checks if the next space has a takeable piece on it
                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        lightList.Add(tempMove);
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
                tempMove = temp;
                tempMove -= dir;
                while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    lightList.Add(tempMove);
                    tempMove -= dir;
                }
                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
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

    void KingMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces, int numOfMovesW, int numOfMovesB)
    {
        Vector3 temp = selectedPiece.transform.localPosition;
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
            tempMove = temp;
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
                    if (piecePosition[tempMove].tag == "White" && !UIEngineReference.whitePossiblePositions.Contains(tempMove))
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
        Vector3 temp = selectedPiece.transform.localPosition;
        Vector3[] directions = new Vector3[4];
        directions[0] = new Vector3(1.25f, 0, 1.25f); //moveUpRight
        directions[1] = new Vector3(-1.25f, 0, 1.25f); //moveUpLeft
        directions[2] = new Vector3(1.25f, 0, -1.25f); //moveDownRight
        directions[3] = new Vector3(-1.25f, 0, -1.25f); //moveDownLeft
        Vector3 tempMove = temp;
        if (selectedPiece.tag == "White")
        {
            foreach (Vector3 dir in directions)
            {
                tempMove = temp;
                tempMove += dir;
                while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    lightList.Add(tempMove);
                    tempMove += dir;
                }

                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        lightList.Add(tempMove);
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
                tempMove = temp;
                tempMove -= dir;
                while (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    lightList.Add(tempMove);
                    tempMove -= dir;
                }

                if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
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

    void KnightMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        Vector3 temp = selectedPiece.transform.localPosition;
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

        if (selectedPiece.tag == "White")
        {
            foreach (Vector3 dir in directions)
            {
                tempMove = temp;
                tempMove += dir;
                if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    lightList.Add(tempMove);
                }

                else if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "Black")
                    {
                        lightList.Add(tempMove);
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
                tempMove = temp;
                tempMove -= dir;
                if (!piecePosition.ContainsKey(tempMove) && tempMove.x >= bXMin && tempMove.z >= bZMin && tempMove.x <= bXMax && tempMove.z <= bZMax)
                {
                    lightList.Add(tempMove);
                }

                else if (piecePosition.ContainsKey(tempMove))
                {
                    if (piecePosition[tempMove].tag == "White")
                    {
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

    void QueenMovement(GameObject selectedPiece, ref List<Vector3> lightList, ref List<Vector3> pWPieces, ref List<Vector3> pBPieces)
    {
        RookMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
        BishopMovement(selectedPiece, ref lightList, ref pWPieces, ref pBPieces);
    }

    /*
    public void CheckBoard(ref List<Vector3> WPP, ref List<Vector3> BPP)
    {
        BPP.Clear();
        WPP.Clear();
        foreach (Transform inPlayPieces in chessBoard.transform)
        {
            if (inPlayPieces.tag == "White")
            {
                WPP.Add(inPlayPieces.transform.localPosition);
            }

            if (inPlayPieces.tag == "Black")
            {
                BPP.Add(inPlayPieces.transform.localPosition);
            }
        }
    }
    */

}
