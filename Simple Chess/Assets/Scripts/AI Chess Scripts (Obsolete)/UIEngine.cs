using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIEngine : MonoBehaviour
{
    public static bool playerSelectedWhite;
    bool AIThinking;
    public Camera transitionCamera;
    public bool cameraFocusWhite = true;
    bool isCameraMoving = false;
    public TextMeshProUGUI playerTurn;
    public GameObject[] lights, castlingLights, enPassantLights;
    public GameObject selectedPiece, rookCastling, pawnEnPassant;
    public GameObject lightPrefab, board, whiteGraveyard, blackGraveyard;
    Chessboard ChessboardRef;
    Stockfish StockfishRef;
    bool promoting = false;
    string promotionPeice = "";
    public GameObject promotionPanel, queenButton, rookButton, knightButton, bishopButton;
    public Sprite wQueen2D, wRook2D, wBishop2D, wKnight2D, bQueen2D, bRook2D, bBishop2D, bKnight2D;
    public List<Vector3> lightList = new List<Vector3>();
    public List<Vector3> castlingLightList = new List<Vector3>();
    public List<Vector3> EnPassantLightList = new List<Vector3>();
    public List<Vector3> whitePossiblePositions = new List<Vector3>();
    public List<Vector3> blackPossiblePositions = new List<Vector3>();
    public List<Vector3> ProtectedWhitePieces = new List<Vector3>();
    public List<Vector3> ProtectedBlackPieces = new List<Vector3>();
    public GameObject enPassantGORef;
    Vector3 graveYardMoveW = new Vector3(3.0f, 0, -125.0f);
    Vector3 graveYardMoveB = new Vector3(3.0f, 0, -125.0f);
    List<Vector3> TempList1 = new List<Vector3>();
    List<Vector3> TempList2 = new List<Vector3>();
    void Start()
    {
        ChessboardRef = FindObjectOfType<Chessboard>();
        StockfishRef = FindObjectOfType<Stockfish>();
        //setCameraPosition
        if (playerSelectedWhite == true)
        {
            transitionCamera.transform.localPosition = new Vector3(86.6f, 200.9f, -47f);
            transitionCamera.transform.localRotation = Quaternion.Euler(60.58f, 0f, 0f);
        }
        else
        {
            transitionCamera.transform.localPosition = new Vector3(86.6f, 200.9f, -47f);
            transitionCamera.transform.localRotation = Quaternion.Euler(60.58f, 180f, 0f);
        }
        UpdateUI();
    }

    void Update()
    {
        //Players turn
        if (playerSelectedWhite == true && ChessboardRef.whiteToMove == true)
        {
            AIThinking = false;
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
                if (hit)
                {
                    if (hitInfo.transform.tag == "Move")
                    {
                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                        {
                            ChessboardRef.halfMove = 0;
                        }
                        else ChessboardRef.halfMove++;

                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                        {
                            selectedPiece.GetComponent<Track>().startingPosition = false;
                        }
                        EnPassantCheck(selectedPiece, hitInfo.transform.gameObject);
                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, -0.4f, 0)); //checks whether there is a takeable piece.
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        StartCoroutine(PromotionCheck(selectedPiece));
                        if (promoting == true)
                        {
                            StartCoroutine(WaitForPromotion());
                        }
                        else
                        {
                            ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                            //StartCoroutine(AnimateCamera());
                            UpdateUI();
                            DestroyAndClearLights();
                            ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
                        }
                        //Debug.Log(ChessboardRef.GetBoardState());
                    }

                    else if (hitInfo.transform.gameObject.tag == "White")
                    {
                        //White to move
                        if (ChessboardRef.whiteToMove)
                        {
                            if (selectedPiece == hitInfo.transform.gameObject)
                            {
                                DestroyAndClearLights();
                                selectedPiece = null;
                            }

                            else
                            {
                                DestroyAndClearLights();
                                selectedPiece = hitInfo.transform.gameObject;
                                ChessboardRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList1);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                }
                            }
                        }

                        //Black to move
                        else
                        {
                            if (lightList.Count > 0)
                            {
                                if (lightList.Contains(hitInfo.transform.localPosition))
                                {
                                    if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                    {
                                        ChessboardRef.halfMove = 0;
                                    }
                                    else ChessboardRef.halfMove++;

                                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                    {
                                        selectedPiece.GetComponent<Track>().startingPosition = false;
                                    }
                                    ChessboardRef.check = false; // Resets Check
                                    ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                                    TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                    ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                    StartCoroutine(PromotionCheck(selectedPiece));
                                    if (promoting == true)
                                    {
                                        StartCoroutine(WaitForPromotion());
                                    }
                                    else
                                    {
                                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                                        //StartCoroutine(AnimateCamera());
                                        UpdateUI();
                                        DestroyAndClearLights();
                                        ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
                                        //Debug.Log(ChessboardRef.GetBoardState());
                                    }
                                }
                            }
                        }
                    }

                    else if (hitInfo.transform.gameObject.tag == "Black")
                    {
                        //White to move
                        if (ChessboardRef.whiteToMove)
                        {
                            if (lightList.Count > 0)
                            {
                                if (lightList.Contains(hitInfo.transform.localPosition))
                                {
                                    if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                    {
                                        ChessboardRef.halfMove = 0;
                                    }
                                    else ChessboardRef.halfMove++;

                                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                    {
                                        selectedPiece.GetComponent<Track>().startingPosition = false;
                                    }
                                    ChessboardRef.check = false; // Resets Check
                                    ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                                    TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                    ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                    StartCoroutine(PromotionCheck(selectedPiece));
                                    if (promoting == true)
                                    {
                                        StartCoroutine(WaitForPromotion());
                                    }
                                    else
                                    {
                                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                                        //StartCoroutine(AnimateCamera());
                                        UpdateUI();
                                        DestroyAndClearLights();
                                        ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
                                        //Debug.Log(ChessboardRef.GetBoardState());
                                    }
                                }
                            }
                        }

                        //Black to move
                        else
                        {
                            if (selectedPiece == hitInfo.transform.gameObject)
                            {
                                DestroyAndClearLights();
                                selectedPiece = null;
                            }

                            else
                            {
                                DestroyAndClearLights();
                                selectedPiece = hitInfo.transform.gameObject;
                                ChessboardRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                }
                            }
                        }
                    }

                    else if (hitInfo.transform.tag == "Castling")
                    {
                        ChessboardRef.halfMove++;
                        bool moveLeft = false;
                        rookCastling = null;
                        selectedPiece.GetComponent<Track>().startingPosition = false;
                        //gets the correct rook

                        foreach (GameObject rook in ChessboardRef.temporaryCastlingPieceRefs)
                        {
                            if (rook.transform.localPosition.x > hitInfo.transform.localPosition.x)
                            {
                                moveLeft = false;
                                rookCastling = rook;
                            }
                            else
                            {
                                moveLeft = true;
                                rookCastling = rook;
                            }
                        }

                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                        ChessboardRef.piecePosition.Remove(rookCastling.transform.localPosition);// Removes the old rook position + gameobject to the dictionary
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights

                        if (moveLeft == true)
                        {
                            rookCastling.transform.localPosition = hitInfo.transform.localPosition + new Vector3(1.25f, 0, 0);
                            rookCastling.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        }
                        else
                        {
                            rookCastling.transform.localPosition = hitInfo.transform.localPosition - new Vector3(1.25f, 0, 0);
                            rookCastling.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        }

                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                        ChessboardRef.piecePosition.Add(rookCastling.transform.localPosition, rookCastling); // Adds the Rooks new position + gameobject to the dictionary

                        rookCastling.GetComponent<Track>().startingPosition = false;

                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                        // StartCoroutine(AnimateCamera());
                        UpdateUI();
                        DestroyAndClearLights();
                        lightList.Clear();
                        castlingLightList.Clear();
                    }

                    else if (hitInfo.transform.tag == "EnPassant")
                    {
                        ChessboardRef.halfMove = 0;
                        pawnEnPassant = ChessboardRef.temporaryEnPassantPieceRefs;
                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                        TakePieceCheck(pawnEnPassant, new Vector3(0, 0, 0));
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                        //StartCoroutine(AnimateCamera());
                        UpdateUI();
                        DestroyAndClearLights();
                        lightList.Clear();
                        castlingLightList.Clear();
                    }
                    else
                    {
                        DestroyAndClearLights();
                        selectedPiece = null;
                    }
                }
            }
        }
       
        // Run Stockfish
        else if (playerSelectedWhite == false && ChessboardRef.whiteToMove == true && !AIThinking)
        {
            string bestmove = StockfishRef.GetStockfishCommands(ChessboardRef.GetBoardState());

            string[] characters = new string[bestmove.Length];
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = bestmove[i].ToString();
            }

            //Debug.Log(characters[0] + characters[1] + ": " + ChessboardRef.ConvertBoardStateIntoPosition(characters[0] + characters[1]).ToString("f2"));
            //Debug.Log(characters[2] + characters[3] + ": " + ChessboardRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]).ToString("f2"));

            //Get the positons
            Debug.Log("Best Move: "+ bestmove);

            GameObject selectedPiece = ChessboardRef.piecePosition[ChessboardRef.ConvertBoardStateIntoPosition(characters[0] + characters[1])];
            Debug.Log("Selected Piece: " + selectedPiece.transform.GetChild(0).tag + " " + selectedPiece.transform.localPosition.ToString("f2"));

            Vector3 newPositon = ChessboardRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]);
            Debug.Log("New Position: " + newPositon.ToString("f2"));

            StartCoroutine(HighlightAIMove(newPositon, selectedPiece));
        }

        //Player's turn
        else if (playerSelectedWhite == false && ChessboardRef.whiteToMove == false)
        {
            AIThinking = false;
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
                if (hit)
                {
                    if (hitInfo.transform.tag == "Move")
                    {
                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                        {
                            ChessboardRef.halfMove = 0;
                        }
                        else ChessboardRef.halfMove++;

                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                        {
                            selectedPiece.GetComponent<Track>().startingPosition = false;
                        }
                        EnPassantCheck(selectedPiece, hitInfo.transform.gameObject);
                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, -0.4f, 0)); //checks whether there is a takeable piece.
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        StartCoroutine(PromotionCheck(selectedPiece));
                        if (promoting == true)
                        {
                            StartCoroutine(WaitForPromotion());
                        }
                        else
                        {
                            ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                            //StartCoroutine(AnimateCamera());
                            UpdateUI();
                            DestroyAndClearLights();
                            ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
                            //Debug.Log(ChessboardRef.GetBoardState());
                        }
                    }

                    else if (hitInfo.transform.gameObject.tag == "White")
                    {
                        //White to move
                        if (ChessboardRef.whiteToMove)
                        {
                            if (selectedPiece == hitInfo.transform.gameObject)
                            {
                                DestroyAndClearLights();
                                selectedPiece = null;
                            }

                            else
                            {
                                DestroyAndClearLights();
                                selectedPiece = hitInfo.transform.gameObject;
                                ChessboardRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList1);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                }
                            }
                        }

                        //Black to move
                        else
                        {
                            if (lightList.Count > 0)
                            {
                                if (lightList.Contains(hitInfo.transform.localPosition))
                                {
                                    if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                    {
                                        ChessboardRef.halfMove = 0;
                                    }
                                    else ChessboardRef.halfMove++;

                                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                    {
                                        selectedPiece.GetComponent<Track>().startingPosition = false;
                                    }
                                    ChessboardRef.check = false; // Resets Check
                                    ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                                    TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                    ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                    StartCoroutine(PromotionCheck(selectedPiece));
                                    if (promoting == true)
                                    {
                                        StartCoroutine(WaitForPromotion());
                                    }
                                    else
                                    {
                                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                                        //StartCoroutine(AnimateCamera());
                                        UpdateUI();
                                        DestroyAndClearLights();
                                        ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
                                        // Debug.Log(ChessboardRef.GetBoardState());
                                    }
                                }
                            }
                        }
                    }

                    else if (hitInfo.transform.gameObject.tag == "Black")
                    {
                        //White to move
                        if (ChessboardRef.whiteToMove)
                        {
                            if (lightList.Count > 0)
                            {
                                if (lightList.Contains(hitInfo.transform.localPosition))
                                {
                                    if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                    {
                                        ChessboardRef.halfMove = 0;
                                    }
                                    else ChessboardRef.halfMove++;

                                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                    {
                                        selectedPiece.GetComponent<Track>().startingPosition = false;
                                    }
                                    ChessboardRef.check = false; // Resets Check
                                    ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                                    TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                    ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                    StartCoroutine(PromotionCheck(selectedPiece));
                                    if (promoting == true)
                                    {
                                        StartCoroutine(WaitForPromotion());
                                    }
                                    else
                                    {
                                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                                        //StartCoroutine(AnimateCamera());
                                        UpdateUI();
                                        DestroyAndClearLights();
                                        ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
                                        //Debug.Log(ChessboardRef.GetBoardState());
                                    }
                                }
                            }
                        }

                        //Black to move
                        else
                        {
                            if (selectedPiece == hitInfo.transform.gameObject)
                            {
                                DestroyAndClearLights();
                                selectedPiece = null;
                            }

                            else
                            {
                                DestroyAndClearLights();
                                selectedPiece = hitInfo.transform.gameObject;
                                ChessboardRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                }
                            }
                        }
                    }

                    else if (hitInfo.transform.tag == "Castling")
                    {
                        ChessboardRef.halfMove++;
                        bool moveLeft = false;
                        rookCastling = null;
                        selectedPiece.GetComponent<Track>().startingPosition = false;
                        //gets the correct rook

                        foreach (GameObject rook in ChessboardRef.temporaryCastlingPieceRefs)
                        {
                            if (rook.transform.localPosition.x > hitInfo.transform.localPosition.x)
                            {
                                moveLeft = false;
                                rookCastling = rook;
                            }
                            else
                            {
                                moveLeft = true;
                                rookCastling = rook;
                            }
                        }

                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                        ChessboardRef.piecePosition.Remove(rookCastling.transform.localPosition);// Removes the old rook position + gameobject to the dictionary
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights

                        if (moveLeft == true)
                        {
                            rookCastling.transform.localPosition = hitInfo.transform.localPosition + new Vector3(1.25f, 0, 0);
                            rookCastling.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        }
                        else
                        {
                            rookCastling.transform.localPosition = hitInfo.transform.localPosition - new Vector3(1.25f, 0, 0);
                            rookCastling.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        }

                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                        ChessboardRef.piecePosition.Add(rookCastling.transform.localPosition, rookCastling); // Adds the Rooks new position + gameobject to the dictionary

                        rookCastling.GetComponent<Track>().startingPosition = false;

                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                        // StartCoroutine(AnimateCamera());
                        UpdateUI();
                        DestroyAndClearLights();
                        lightList.Clear();
                        castlingLightList.Clear();
                    }

                    else if (hitInfo.transform.tag == "EnPassant")
                    {
                        ChessboardRef.halfMove = 0;
                        pawnEnPassant = ChessboardRef.temporaryEnPassantPieceRefs;
                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old pawn position + gameobject to the dictionary
                        TakePieceCheck(pawnEnPassant, new Vector3(0, 0, 0));
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the pawns new position + gameobject to the dictionary
                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                        //StartCoroutine(AnimateCamera());
                        UpdateUI();
                        DestroyAndClearLights();
                        lightList.Clear();
                        castlingLightList.Clear();
                    }
                    else
                    {
                        DestroyAndClearLights();
                        selectedPiece = null;
                    }
                }
            }
        }

        //Run Stockfish
        else if (playerSelectedWhite == true && ChessboardRef.whiteToMove == false && !AIThinking)
        {

            string bestmove = StockfishRef.GetStockfishCommands(ChessboardRef.GetBoardState());

            string[] characters = new string[bestmove.Length];
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = bestmove[i].ToString();
            }

            //Debug.Log(characters[0] + characters[1] + ": " + ChessboardRef.ConvertBoardStateIntoPosition(characters[0] + characters[1]).ToString("f2"));
            //Debug.Log(characters[2] + characters[3] + ": " + ChessboardRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]).ToString("f2"));

            //Get the positons
            Debug.Log(bestmove);

            GameObject selectedPiece = ChessboardRef.piecePosition[ChessboardRef.ConvertBoardStateIntoPosition(characters[0] + characters[1])];
            Debug.Log("Current Piece: " + selectedPiece.transform.localPosition.ToString("f2"));

            Vector3 newPositon = ChessboardRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]);
            Debug.Log("New Position: " + newPositon);

            StartCoroutine(HighlightAIMove(newPositon, selectedPiece));

            
        }
    }

    void CameraChange()
    {
        cameraFocusWhite = !cameraFocusWhite;

        if (cameraFocusWhite == true)
        {
            transitionCamera.transform.localPosition = new Vector3(86.6f, 200.9f, -47f);
            transitionCamera.transform.localRotation = Quaternion.Euler(60.58f, 0f, 0f);
        }
        else
        {
            transitionCamera.transform.localPosition = new Vector3(86.6f, 200.9f, -47f);
            transitionCamera.transform.localRotation = Quaternion.Euler(60.58f, 180f, 0f);
        }
    }

    IEnumerator AnimateCamera()
    {

        if (cameraFocusWhite == true)
        {
            transitionCamera.gameObject.GetComponent<Animator>().SetTrigger("whiteToBlack");
            cameraFocusWhite = false;
        }
        else
        {
            transitionCamera.gameObject.GetComponent<Animator>().SetTrigger("blackToWhite");
            cameraFocusWhite = true;
        }
        isCameraMoving = true;
        yield return new WaitForSeconds(2.0f);
        isCameraMoving = false;
    }

    void DestroyAndClearLights()
    {
        lightList.Clear();
        castlingLightList.Clear();
        EnPassantLightList.Clear();
        foreach (Transform child in board.transform)
        {
            if (child.tag == "Move")
            {
                Destroy(child.gameObject);
            }

            else if (child.tag == "Castling")
            {
                Destroy(child.gameObject);
            }

            else if (child.tag == "EnPassant")
            {
                Destroy(child.gameObject);
            }
        }
    }

    void TakePieceCheck(GameObject newPos, Vector3 difference)
    {
        Vector3 temp = newPos.transform.localPosition + difference;
        if (ChessboardRef.piecePosition.ContainsKey(temp)) //checks if there is a piece what can be taken
        {
            ChessboardRef.halfMove = 0;
            if (ChessboardRef.piecePosition[temp].tag == "White")
            {
                graveYardMoveW += new Vector3(3.0f,0,20.0f);
                GameObject tempGameO = Instantiate(ChessboardRef.piecePosition[temp].gameObject, whiteGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessboardRef.white;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
                tempGameO.transform.localPosition += graveYardMoveW;
                tempGameO.tag = "Dead";
            }
            else
            {
                graveYardMoveB += new Vector3(3.0f, 0, 20.0f);
                GameObject tempGameO = Instantiate(ChessboardRef.piecePosition[temp].gameObject, blackGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessboardRef.black;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
                tempGameO.transform.localPosition += graveYardMoveB;
                tempGameO.tag = "Dead";
            }
            Destroy(ChessboardRef.piecePosition[temp].gameObject);
            ChessboardRef.piecePosition.Remove(temp);
        }
    }

    void AllPossibleMoves(ref List<Vector3> WPosP, ref List<Vector3> BPosP, ref List<Vector3> pWpieces, ref List<Vector3> pBPieces)
    {
        TempList1.Clear();
        TempList2.Clear();
        List<Vector3> tempDiagonalSpacesW = new List<Vector3>();
        List<Vector3> tempDiagonalSpacesB = new List<Vector3>();
        ChessboardRef.pinnedPiecePath.Clear();
        ChessboardRef.numberOfPossibleMovesW = 0;
        ChessboardRef.numberOfPossibleMovesB = 0;
        ChessboardRef.piecesChecking.Clear();
        ChessboardRef.piecesPuttingKingInCheck = 0;
        ChessboardRef.lineOfCheck.Clear();
        pWpieces.Clear();
        pBPieces.Clear();
        WPosP.Clear();
        BPosP.Clear();

        Dictionary<Vector3, GameObject> tempWhiteList = new Dictionary<Vector3, GameObject>();
        Dictionary<Vector3, GameObject> tempBlackList = new Dictionary<Vector3, GameObject>();

        //Un-pin all pieces to be calculated if they are pinned
        foreach (KeyValuePair<Vector3, GameObject> pPos in ChessboardRef.piecePosition)
        {
            pPos.Value.GetComponent<Track>().pinned = false;

            if (pPos.Value.GetComponent<Track>().enPassant == true)
            {
                if (pPos.Value.GetComponent<Track>().enPassantRound == ChessboardRef.currentMove)
                {
                    pPos.Value.GetComponent<Track>().enPassant = false;
                    enPassantGORef = null;
                }
                else
                {
                    enPassantGORef = pPos.Value;
                }
            }
        }

        //Temporarily seperates the chess pieces by their colour//
        foreach (KeyValuePair<Vector3, GameObject> pPos in ChessboardRef.piecePosition)
        {
            if (pPos.Value.tag == "White")
            {
                tempWhiteList.Add(pPos.Key, pPos.Value);
            }

            if (pPos.Value.tag == "Black")
            {
                tempBlackList.Add(pPos.Key, pPos.Value);
            }
        }

        // If white to move, calculate blacks moves first for chance of check.
        if (ChessboardRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces, ref BPosP, ref BPosP, ref tempDiagonalSpacesB);
            }
            if (ChessboardRef.piecesPuttingKingInCheck > 0)
            {
                ChessboardRef.check = true;
            }

            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces, ref WPosP, ref WPosP, ref tempDiagonalSpacesW);
            }
        }

        else if (!ChessboardRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces, ref WPosP, ref WPosP, ref tempDiagonalSpacesW);
            }

            if (ChessboardRef.piecesPuttingKingInCheck > 0)
            {
                ChessboardRef.check = true;
            }


            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces, ref BPosP, ref BPosP, ref tempDiagonalSpacesB);
            }
        }

        Debug.Log("Number of Possible Moves White: " + ChessboardRef.numberOfPossibleMovesW);
        Debug.Log("Number of Possible Moves Black: " + ChessboardRef.numberOfPossibleMovesB);

        WPosP.AddRange(tempDiagonalSpacesW);
        BPosP.AddRange(tempDiagonalSpacesB);
    }

    void highlightPossibleMoves(List<Vector3> lightlist, List<Vector3> castlingList, List<Vector3> enPassantLL)
    {
        int i = 0;
        if (lightlist.Count > 0)
        {
            lights = new GameObject[lightlist.Count];
            foreach (Vector3 light in lightlist)
            {
                if (!ChessboardRef.blackSpaces.Contains(light)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
                {
                    lights[i] = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
                    lights[i].transform.localPosition = new Vector3(light.x, 0.4f, light.z);
                    lights[i].GetComponent<Light>().intensity = 3;
                    lights[i].tag = "Move";
                    i++;
                }
                else
                {
                    lights[i] = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
                    lights[i].transform.localPosition = new Vector3(light.x, 0.4f, light.z);
                    lights[i].tag = "Move";
                    i++;
                }
            }
        }

        i = 0;
        if (castlingList.Count > 0)
        {
            castlingLights = new GameObject[castlingList.Count];
            foreach (Vector3 castleLight in castlingList)
            {
                if (!ChessboardRef.blackSpaces.Contains(castleLight)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
                {
                    castlingLights[i] = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
                    castlingLights[i].transform.localPosition = new Vector3(castleLight.x, 0.4f, castleLight.z);
                    castlingLights[i].GetComponent<Light>().intensity = 3;
                    castlingLights[i].tag = "Castling";
                    i++;
                }
                else
                {
                    castlingLights[i] = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
                    castlingLights[i].transform.localPosition = new Vector3(castleLight.x, 0.4f, castleLight.z);
                    castlingLights[i].tag = "Castling";
                    i++;
                }
            }
        }

        if (enPassantLL.Count > 0)
        {
            enPassantLights = new GameObject[enPassantLL.Count];
            foreach (Vector3 ePL in enPassantLL)
            {
                if (!ChessboardRef.blackSpaces.Contains(ePL)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
                {
                    enPassantLights[0] = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
                    enPassantLights[0].transform.localPosition = new Vector3(ePL.x, 0.4f, ePL.z);
                    enPassantLights[0].GetComponent<Light>().intensity = 3;
                    enPassantLights[0].tag = "EnPassant";
                }
                else
                {
                    enPassantLights[0] = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
                    enPassantLights[0].transform.localPosition = new Vector3(ePL.x, 0.4f, ePL.z);
                    enPassantLights[0].tag = "EnPassant";
                }
            }
        }  
    }

    void EnPassantCheck(GameObject selectedPiece, GameObject hitInfo) //To make EnPassant True after the Pawn move has been made for one round
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            if (selectedPiece.transform.tag == "White")
            {
                if (selectedPiece.transform.localPosition.z == (hitInfo.transform.localPosition.z - (2* ChessboardRef.bZSpace)))
                {
                    selectedPiece.GetComponent<Track>().enPassant = true;
                    selectedPiece.GetComponent<Track>().enPassantRound = ChessboardRef.currentMove + 1;
                    selectedPiece.GetComponent<Track>().EnPassantPosition = new Vector3(hitInfo.transform.localPosition.x, hitInfo.transform.localPosition.y, hitInfo.transform.localPosition.z - ChessboardRef.bZSpace);
                }
            }
            else if (selectedPiece.transform.tag == "Black")
            {
                if (selectedPiece.transform.localPosition.z == (hitInfo.transform.localPosition.z + (2 * ChessboardRef.bZSpace)))
                {
                    selectedPiece.GetComponent<Track>().enPassant = true;
                    selectedPiece.GetComponent<Track>().enPassantRound = ChessboardRef.currentMove + 1;
                    selectedPiece.GetComponent<Track>().EnPassantPosition = new Vector3(hitInfo.transform.localPosition.x, hitInfo.transform.localPosition.y, hitInfo.transform.localPosition.z + ChessboardRef.bZSpace);
                }
            }
        }
    }

    void UpdateUI()
    {
        if (ChessboardRef.checkMate == true)
        {
            playerTurn.text = "Checkmate!";
        }

        else if (ChessboardRef.check == true)
        {
            playerTurn.text = "Check";
        }

       else if (playerSelectedWhite == true && ChessboardRef.whiteToMove == true)
        {
            playerTurn.text = "Your turn";
        }
        
        else if (playerSelectedWhite == false && ChessboardRef.whiteToMove == false)
        {
            playerTurn.text = "Your turn";
        }

        else if (playerSelectedWhite == false && ChessboardRef.whiteToMove == true)
        {
            playerTurn.text = "AI Thinking";
        }

        else if (playerSelectedWhite == true && ChessboardRef.whiteToMove == false)
        {
            playerTurn.text = "AI Thinking";
        }
    }

    IEnumerator HighlightAIMove(Vector3 LightPos, GameObject selectedP)
    {
        AIThinking = true;

        GameObject lightGO = Instantiate(lightPrefab, ChessboardRef.chessBoard.transform);
        lightGO.transform.localPosition = new Vector3(LightPos.x, 0.4f, LightPos.z);

        lightGO.GetComponent<Light>().intensity = 10;
        lightGO.GetComponent<Light>().color = new Color(0.12f, 1f, 0);

        yield return new WaitForSeconds(2.0f);

        Destroy(lightGO);

        MoveAIPiece(LightPos, selectedP);
    }

    void MoveAIPiece(Vector3 newPosition, GameObject selPiece)
    {
        #region MovePieces

        ChessboardRef.check = false; // Resets Check
        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove;

        ChessboardRef.piecePosition.Remove(selPiece.transform.localPosition);

        //If it contains a piece
        if (ChessboardRef.piecePosition.ContainsKey(newPosition))
        {
            if (selPiece.transform.GetChild(0).tag == "Pawn")
            {
                if (selPiece.tag == "White")
                {
                    if (selPiece.transform.localPosition.z == ChessboardRef.bZMax) //Perform Promotion
                    {
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                        TakePieceCheck(selPiece, new Vector3(0, 0, 0));
                        TakePieceCheck(ChessboardRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        GameObject newPiece = Instantiate(ChessboardRef.Queen, ChessboardRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        TakePieceCheck(ChessboardRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        selPiece.transform.localPosition = newPosition; // moves the piece into the correct position.
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else
                {
                    if (selPiece.transform.localPosition.z == ChessboardRef.bZMin) //Perform Promotion
                    {
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                        TakePieceCheck(selPiece, new Vector3(0, 0, 0));
                        TakePieceCheck(ChessboardRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        GameObject newPiece = Instantiate(ChessboardRef.Queen, ChessboardRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        TakePieceCheck(ChessboardRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        selPiece.transform.localPosition = newPosition; // moves the piece into the correct position.
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
            }
            else
            {
                TakePieceCheck(ChessboardRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                selPiece.transform.localPosition = newPosition; // moves the piece into the correct position.
                ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
            }
        }
        //Doesn't contain a piece
        else
        {
            if (selPiece.transform.GetChild(0).tag == "Pawn") // En Passant
            {
                if (enPassantGORef != null)
                {
                    if (newPosition == enPassantGORef.GetComponent<Track>().EnPassantPosition)
                    {
                        TakePieceCheck(enPassantGORef, new Vector3(0, 0, 0));
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else if(selPiece.tag == "White")
                {
                    if (newPosition.z == ChessboardRef.bZMax) //Promotion
                    {
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                        TakePieceCheck(selPiece, new Vector3(0, 0, 0));
                        GameObject newPiece = Instantiate(ChessboardRef.Queen, ChessboardRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else if (selPiece.tag == "Black") //tag is black
                {
                    if (newPosition.z == ChessboardRef.bZMin) //Promotion
                    {
                        Destroy(selPiece);
                        GameObject newPiece = Instantiate(ChessboardRef.Queen, ChessboardRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
            }
            else if (selPiece.transform.GetChild(0).tag == "King") //Castling
            {
                if (selPiece.tag == "White")
                {
                    //Castle King Side
                    if (selPiece.transform.localPosition.x - newPosition.x == (ChessboardRef.bXSpace*2))
                    {
                        GameObject Rook = ChessboardRef.piecePosition[new Vector3(8.75f, 0, 0)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(-1.25f, 0, 0);
                        ChessboardRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    //Castle Queen Side
                    else if (selPiece.transform.localPosition.x - newPosition.x == -(ChessboardRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessboardRef.piecePosition[new Vector3(0, 0, 0)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(1.25f, 0, 0);
                        ChessboardRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else if (selPiece.tag == "Black")
                {
                    //Castle Queen Side
                    if (selPiece.transform.localPosition.x - newPosition.x == (ChessboardRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessboardRef.piecePosition[new Vector3(0, 0, 8.75f)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(-1.25f, 0, 0);
                        ChessboardRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    //Castle King Side
                    else if (selPiece.transform.localPosition.x - newPosition.x == -(ChessboardRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessboardRef.piecePosition[new Vector3(8.75f, 0, 8.75f)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(1.25f, 0, 0);
                        ChessboardRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
            }
            else
            {
                selPiece.transform.localPosition = newPosition;
                ChessboardRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
            }
        }

        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
        ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);
        UpdateUI();
        #endregion
    }


    IEnumerator PromotionCheck(GameObject pawn)
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            if (selectedPiece.tag == "White")
            {
                if (selectedPiece.transform.localPosition.z == ChessboardRef.bZMax)
                {
                    Vector3 savedPosition = selectedPiece.transform.localPosition;
                    Debug.Log("A");
                    //save current Positons
                    //Perform Promotion
                    playerTurn.gameObject.SetActive(false);
                    promoting = true;
                    DisplayPromotionPanel(selectedPiece.tag);
                    Destroy(selectedPiece);
                    yield return new WaitUntil(() => promoting == false);
                    if (promotionPeice == "Queen")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Queen, ChessboardRef.chessBoard.transform);
                    }
                    else if (promotionPeice == "Rook")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Rook, ChessboardRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Bishop")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Bishop, ChessboardRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Knight")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Knight, ChessboardRef.chessBoard.transform);

                    }
                    selectedPiece.AddComponent<Track>().startingPosition = false;
                    selectedPiece.GetComponent<Renderer>().material = ChessboardRef.white;
                    selectedPiece.tag = "White";
                    selectedPiece.transform.localPosition = savedPosition;
                    selectedPiece.transform.localScale = new Vector3(1, 1, 1);
                    promotionPanel.SetActive(false);
                    playerTurn.gameObject.SetActive(true);

                }
            }
            else if (selectedPiece.tag == "Black")
            {
                if (selectedPiece.transform.localPosition.z == ChessboardRef.bZMin)
                {
                    //save current Positons
                    Vector3 savedPosition = selectedPiece.transform.localPosition;
                    //Perform Promotion
                    playerTurn.gameObject.SetActive(false);
                    promoting = true;
                    DisplayPromotionPanel(selectedPiece.tag);
                    Destroy(selectedPiece);
                    yield return new WaitUntil(() => promoting == false);
                    if (promotionPeice == "Queen")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Queen, ChessboardRef.chessBoard.transform);
                    }
                    else if (promotionPeice == "Rook")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Rook, ChessboardRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Bishop")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Bishop, ChessboardRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Knight")
                    {
                        selectedPiece = Instantiate(ChessboardRef.Knight, ChessboardRef.chessBoard.transform);

                    }
                    selectedPiece.AddComponent<Track>().startingPosition = false;
                    selectedPiece.GetComponent<Renderer>().material = ChessboardRef.black;
                    selectedPiece.tag = "Black";
                    selectedPiece.transform.localPosition = savedPosition;
                    selectedPiece.transform.localScale = new Vector3(1, 1, 1);
                    promotionPanel.SetActive(false);
                    playerTurn.gameObject.SetActive(true);
                }

            }
        }
    }
    IEnumerator WaitForPromotion()
    {
        yield return new WaitUntil(() => promoting == false);

        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
        //StartCoroutine(AnimateCamera());
        UpdateUI();
        DestroyAndClearLights();
        ChessboardRef.MoveCounter(ref ChessboardRef.currentMove);

    }

    public void DisplayPromotionPanel(string colour)
    {
        promotionPanel.SetActive(true);
        if (colour == "White")
        {
            queenButton.GetComponent<Image>().sprite = wQueen2D;
            rookButton.GetComponent<Image>().sprite = wRook2D;
            bishopButton.GetComponent<Image>().sprite = wBishop2D;
            knightButton.GetComponent<Image>().sprite = wKnight2D;
        }
        else if (colour == "Black")
        {
            queenButton.GetComponent<Image>().sprite = bQueen2D;
            rookButton.GetComponent<Image>().sprite = bRook2D;
            bishopButton.GetComponent<Image>().sprite = bBishop2D;
            knightButton.GetComponent<Image>().sprite = bKnight2D;
        }
    }

    public void CreateQueen()
    {
        promoting = false;
        promotionPeice = "Queen";
    }
    public void CreateRook()
    {
        promoting = false;
        promotionPeice = "Rook";
    }
    public void CreateBishop()
    {
        promoting = false;
        promotionPeice = "Bishop";
    }
    public void CreateKnight()
    {
        promoting = false;
        promotionPeice = "Knight";
    }
}
