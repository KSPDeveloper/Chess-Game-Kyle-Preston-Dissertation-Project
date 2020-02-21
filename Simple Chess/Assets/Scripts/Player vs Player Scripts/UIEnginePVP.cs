using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIEnginePVP : MonoBehaviour
{
    #region AIGame Global Variables
    public static bool AIGame = true;
    public static bool playerSelectedWhite = false;
    bool AIThinking;
    public GameObject enPassantGORef;
    #endregion

    #region Bools
    public bool cameraFocusWhite = true;
    bool promoting = false;
    bool isCameraMoving = false;
    bool firstMove;
    #endregion
    #region Lists
    public List<Vector3> lightList = new List<Vector3>();
    public List<Vector3> castlingLightList = new List<Vector3>();
    public List<Vector3> EnPassantLightList = new List<Vector3>();
    public List<Vector3> whitePossiblePositions = new List<Vector3>();
    public List<Vector3> blackPossiblePositions = new List<Vector3>();
    public List<Vector3> ProtectedWhitePieces = new List<Vector3>();
    public List<Vector3> ProtectedBlackPieces = new List<Vector3>();
    List<Vector3> TempList1 = new List<Vector3>();
    List<Vector3> TempList2 = new List<Vector3>();
    #endregion
    #region Other
    public Camera transitionCamera;
    public TextMeshProUGUI playerTurn;
    public Sprite wQueen2D, wRook2D, wBishop2D, wKnight2D, bQueen2D, bRook2D, bBishop2D, bKnight2D;
    Vector3 graveYardMoveW = new Vector3(3.0f, 0, -125.0f);
    Vector3 graveYardMoveB = new Vector3(3.0f, 0, -125.0f);
    Stockfish StockfishRef;
    ChessBoardPVP ChessBoardPVPRef;
    #endregion
    #region GameObjects
    public GameObject[] lights, castlingLights, enPassantLights;
    public GameObject selectedPiece, rookCastling, pawnEnPassant;
    public GameObject lightPrefab, board, whiteGraveyard, blackGraveyard;
    public GameObject promotionPanel, queenButton, rookButton, knightButton, bishopButton;
    #endregion

    int tempInt = 0;
    int numberOfPMW, numberOfPMB;
    string promotionPeice = "";

  
    void Start()
    {
        ChessBoardPVPRef = FindObjectOfType<ChessBoardPVP>();
        StockfishRef = FindObjectOfType<Stockfish>();
        firstMove = true;
        UpdateUI();
    }

    void Update()
    {
        if (AIGame == true)
        {
            //Players turn
            if (playerSelectedWhite == true && ChessBoardPVPRef.whiteToMove == true && ChessBoardPVPRef.checkMate == false)
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
                                ChessBoardPVPRef.halfMove = 0;
                            }
                            else ChessBoardPVPRef.halfMove++;

                            if (selectedPiece.GetComponent<Track>().startingPosition == true)
                            {
                                selectedPiece.GetComponent<Track>().startingPosition = false;
                            }
                            EnPassantCheck(selectedPiece, hitInfo.transform.localPosition);
                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, -0.4f, 0)); //checks whether there is a takeable piece.
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                            selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                            selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                            StartCoroutine(PromotionCheck(selectedPiece));
                            if (promoting == true)
                            {
                                StartCoroutine(WaitForPromotion());
                            }
                            else
                            {
                                ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                //StartCoroutine(AnimateCamera());
                                DestroyAndClearLights();
                            }
                            //Debug.Log(ChessBoardPVPRef.GetBoardState());
                        }

                        else if (hitInfo.transform.gameObject.tag == "White")
                        {
                            //White to move
                            if (ChessBoardPVPRef.whiteToMove)
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
                                    ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2, ref tempInt, ref tempInt);
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
                                            ChessBoardPVPRef.halfMove = 0;
                                        }
                                        else ChessBoardPVPRef.halfMove++;

                                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                        {
                                            selectedPiece.GetComponent<Track>().startingPosition = false;
                                        }
                                        ChessBoardPVPRef.check = false; // Resets Check
                                        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                        ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                        StartCoroutine(PromotionCheck(selectedPiece));
                                        if (promoting == true)
                                        {
                                            StartCoroutine(WaitForPromotion());
                                        }
                                        else
                                        {
                                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                            //StartCoroutine(AnimateCamera());
                                            DestroyAndClearLights();
                                            //Debug.Log(ChessBoardPVPRef.GetBoardState());
                                        }
                                    }
                                    else
                                    {
                                        DestroyAndClearLights();
                                        selectedPiece = null;
                                    }
                                }
                            }
                        }

                        else if (hitInfo.transform.gameObject.tag == "Black")
                        {
                            //White to move
                            if (ChessBoardPVPRef.whiteToMove)
                            {
                                if (lightList.Count > 0)
                                {
                                    if (lightList.Contains(hitInfo.transform.localPosition))
                                    {
                                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                        {
                                            ChessBoardPVPRef.halfMove = 0;
                                        }
                                        else ChessBoardPVPRef.halfMove++;

                                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                        {
                                            selectedPiece.GetComponent<Track>().startingPosition = false;
                                        }
                                        ChessBoardPVPRef.check = false; // Resets Check
                                        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                        ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                        StartCoroutine(PromotionCheck(selectedPiece));
                                        if (promoting == true)
                                        {
                                            StartCoroutine(WaitForPromotion());
                                        }
                                        else
                                        {
                                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                            DestroyAndClearLights();
                                            //Debug.Log(ChessBoardPVPRef.GetBoardState());
                                        }
                                    }
                                    else
                                    {
                                        DestroyAndClearLights();
                                        selectedPiece = null;
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
                                    ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2, ref tempInt, ref tempInt);
                                    if (lightList.Count != 0)
                                    {
                                        highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                    }
                                }
                            }
                        }

                        else if (hitInfo.transform.tag == "Castling")
                        {
                            ChessBoardPVPRef.halfMove++;
                            bool moveLeft = false;
                            rookCastling = null;
                            selectedPiece.GetComponent<Track>().startingPosition = false;
                            //gets the correct rook

                            foreach (GameObject rook in ChessBoardPVPRef.temporaryCastlingPieceRefs)
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

                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                            ChessBoardPVPRef.piecePosition.Remove(rookCastling.transform.localPosition);// Removes the old rook position + gameobject to the dictionary
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

                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                            ChessBoardPVPRef.piecePosition.Add(rookCastling.transform.localPosition, rookCastling); // Adds the Rooks new position + gameobject to the dictionary

                            rookCastling.GetComponent<Track>().startingPosition = false;

                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            DestroyAndClearLights();
                        }

                        else if (hitInfo.transform.tag == "EnPassant")
                        {
                            ChessBoardPVPRef.halfMove = 0;
                            pawnEnPassant = ChessBoardPVPRef.temporaryEnPassantPieceRefs;
                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                            TakePieceCheck(pawnEnPassant, new Vector3(0, 0, 0));
                            selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                            selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            DestroyAndClearLights();
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
            else if (playerSelectedWhite == false && ChessBoardPVPRef.whiteToMove == true && !AIThinking && ChessBoardPVPRef.checkMate == false)
            {
                string bestmove = StockfishRef.GetStockfishCommands(ChessBoardPVPRef.GetBoardState());

                string[] characters = new string[bestmove.Length];
                for (int i = 0; i < characters.Length; i++)
                {
                    characters[i] = bestmove[i].ToString();
                }

                //Debug.Log(characters[0] + characters[1] + ": " + ChessBoardPVPRef.ConvertBoardStateIntoPosition(characters[0] + characters[1]).ToString("f2"));
                //Debug.Log(characters[2] + characters[3] + ": " + ChessBoardPVPRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]).ToString("f2"));

                //Get the positons
                //Debug.Log("Best Move: " + bestmove);

                GameObject selectedPiece = ChessBoardPVPRef.piecePosition[ChessBoardPVPRef.ConvertBoardStateIntoPosition(characters[0] + characters[1])];
                //Debug.Log("Selected Piece: " + selectedPiece.transform.GetChild(0).tag + " " + selectedPiece.transform.localPosition.ToString("f2"));

                Vector3 newPositon = ChessBoardPVPRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]);
                //Debug.Log("New Position: " + newPositon.ToString("f2"));

                StartCoroutine(HighlightAIMove(newPositon, selectedPiece));
            }

            //Player's turn
            else if (playerSelectedWhite == false && ChessBoardPVPRef.whiteToMove == false && ChessBoardPVPRef.checkMate == false)
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
                                ChessBoardPVPRef.halfMove = 0;
                            }
                            else ChessBoardPVPRef.halfMove++;

                            if (selectedPiece.GetComponent<Track>().startingPosition == true)
                            {
                                selectedPiece.GetComponent<Track>().startingPosition = false;
                            }
                            EnPassantCheck(selectedPiece, hitInfo.transform.localPosition);
                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, -0.4f, 0)); //checks whether there is a takeable piece.
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                            selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                            selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                            StartCoroutine(PromotionCheck(selectedPiece));
                            if (promoting == true)
                            {
                                StartCoroutine(WaitForPromotion());
                            }
                            else
                            {
                                ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                ChessBoardPVPRef.CheckMateCheck(numberOfPMW, numberOfPMB);
                                DestroyAndClearLights();
                                //Debug.Log(ChessBoardPVPRef.GetBoardState());
                            }
                        }

                        else if (hitInfo.transform.gameObject.tag == "White")
                        {
                            //White to move
                            if (ChessBoardPVPRef.whiteToMove)
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
                                    ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2, ref tempInt, ref tempInt);
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
                                            ChessBoardPVPRef.halfMove = 0;
                                        }
                                        else ChessBoardPVPRef.halfMove++;

                                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                        {
                                            selectedPiece.GetComponent<Track>().startingPosition = false;
                                        }
                                        ChessBoardPVPRef.check = false; // Resets Check
                                        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                        ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                        StartCoroutine(PromotionCheck(selectedPiece));
                                        if (promoting == true)
                                        {
                                            StartCoroutine(WaitForPromotion());
                                        }
                                        else
                                        {
                                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                            DestroyAndClearLights();
                                            // Debug.Log(ChessBoardPVPRef.GetBoardState());
                                        }
                                    }
                                }
                            }
                        }

                        else if (hitInfo.transform.gameObject.tag == "Black")
                        {
                            //White to move
                            if (ChessBoardPVPRef.whiteToMove)
                            {
                                if (lightList.Count > 0)
                                {
                                    if (lightList.Contains(hitInfo.transform.localPosition))
                                    {
                                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                        {
                                            ChessBoardPVPRef.halfMove = 0;
                                        }
                                        else ChessBoardPVPRef.halfMove++;

                                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                        {
                                            selectedPiece.GetComponent<Track>().startingPosition = false;
                                        }
                                        ChessBoardPVPRef.check = false; // Resets Check
                                        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                        ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                        StartCoroutine(PromotionCheck(selectedPiece));
                                        if (promoting == true)
                                        {
                                            StartCoroutine(WaitForPromotion());
                                        }
                                        else
                                        {
                                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                            DestroyAndClearLights();
                                            //Debug.Log(ChessBoardPVPRef.GetBoardState());
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
                                    ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2, ref tempInt, ref tempInt);
                                    if (lightList.Count != 0)
                                    {
                                        highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                    }
                                }
                            }
                        }

                        else if (hitInfo.transform.tag == "Castling")
                        {
                            ChessBoardPVPRef.halfMove++;
                            bool moveLeft = false;
                            rookCastling = null;
                            selectedPiece.GetComponent<Track>().startingPosition = false;
                            //gets the correct rook

                            foreach (GameObject rook in ChessBoardPVPRef.temporaryCastlingPieceRefs)
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

                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                            ChessBoardPVPRef.piecePosition.Remove(rookCastling.transform.localPosition);// Removes the old rook position + gameobject to the dictionary
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

                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                            ChessBoardPVPRef.piecePosition.Add(rookCastling.transform.localPosition, rookCastling); // Adds the Rooks new position + gameobject to the dictionary

                            rookCastling.GetComponent<Track>().startingPosition = false;

                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            DestroyAndClearLights();
                        }

                        else if (hitInfo.transform.tag == "EnPassant")
                        {
                            ChessBoardPVPRef.halfMove = 0;
                            pawnEnPassant = ChessBoardPVPRef.temporaryEnPassantPieceRefs;
                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old pawn position + gameobject to the dictionary
                            TakePieceCheck(pawnEnPassant, new Vector3(0, 0, 0));
                            selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                            selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the pawns new position + gameobject to the dictionary
                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            DestroyAndClearLights();
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
            else if (playerSelectedWhite == true && ChessBoardPVPRef.whiteToMove == false && !AIThinking && ChessBoardPVPRef.checkMate == false)
            {
                string bestmove = StockfishRef.GetStockfishCommands(ChessBoardPVPRef.GetBoardState());

                string[] characters = new string[bestmove.Length];
                for (int i = 0; i < characters.Length; i++)
                {
                    characters[i] = bestmove[i].ToString();
                }

                //Get the positons
                //Debug.Log(bestmove);

                GameObject selectedPiece = ChessBoardPVPRef.piecePosition[ChessBoardPVPRef.ConvertBoardStateIntoPosition(characters[0] + characters[1])];
                //Debug.Log("Current Piece: " + selectedPiece.transform.localPosition.ToString("f2"));

                Vector3 newPositon = ChessBoardPVPRef.ConvertBoardStateIntoPosition(characters[2] + characters[3]);
                //Debug.Log("New Position: " + newPositon);

                StartCoroutine(HighlightAIMove(newPositon, selectedPiece));


            }
        }
        else
        {
            if (isCameraMoving == false)
            {
                if (Input.GetMouseButtonDown(0) && !ChessBoardPVPRef.checkMate)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
                    if (hit)
                    {
                        if (hitInfo.transform.tag == "Move")
                        {
                            if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                            {
                                ChessBoardPVPRef.halfMove = 0;
                            }
                            else ChessBoardPVPRef.halfMove++;

                            if (selectedPiece.GetComponent<Track>().startingPosition == true)
                            {
                                selectedPiece.GetComponent<Track>().startingPosition = false;
                            }
                            EnPassantCheck(selectedPiece, hitInfo.transform.localPosition);
                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, -0.4f, 0)); //checks whether there is a takeable piece.
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                            selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                            selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                            StartCoroutine(PromotionCheck(selectedPiece));
                            if (promoting == true)
                            {
                                StartCoroutine(WaitForPromotion());
                            }
                            else
                            {
                                ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                //StartCoroutine(AnimateCamera());
                                DestroyAndClearLights();
                                // Debug.Log(ChessBoardPVPRef.GetBoardState());
                            }
                        }

                        else if (hitInfo.transform.gameObject.tag == "White")
                        {
                            //White to move
                            if (ChessBoardPVPRef.whiteToMove)
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
                                    ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList1, ref tempInt, ref tempInt);
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
                                            ChessBoardPVPRef.halfMove = 0;
                                        }
                                        else ChessBoardPVPRef.halfMove++;

                                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                        {
                                            selectedPiece.GetComponent<Track>().startingPosition = false;
                                        }
                                        ChessBoardPVPRef.check = false; // Resets Check
                                        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                        ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                        StartCoroutine(PromotionCheck(selectedPiece));
                                        if (promoting == true)
                                        {
                                            StartCoroutine(WaitForPromotion());
                                        }
                                        else
                                        {
                                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                            //StartCoroutine(AnimateCamera());
                                            DestroyAndClearLights();
                                            Debug.Log(ChessBoardPVPRef.GetBoardState());
                                        }
                                    }
                                }
                            }
                        }

                        else if (hitInfo.transform.gameObject.tag == "Black")
                        {
                            //White to move
                            if (ChessBoardPVPRef.whiteToMove)
                            {
                                if (lightList.Count > 0)
                                {
                                    if (lightList.Contains(hitInfo.transform.localPosition))
                                    {
                                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                                        {
                                            ChessBoardPVPRef.halfMove = 0;
                                        }
                                        else ChessBoardPVPRef.halfMove++;

                                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                        {
                                            selectedPiece.GetComponent<Track>().startingPosition = false;
                                        }
                                        ChessBoardPVPRef.check = false; // Resets Check
                                        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                        ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                        StartCoroutine(PromotionCheck(selectedPiece));
                                        if (promoting == true)
                                        {
                                            StartCoroutine(WaitForPromotion());
                                        }
                                        else
                                        {
                                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                            //StartCoroutine(AnimateCamera());
                                            DestroyAndClearLights();
                                            Debug.Log(ChessBoardPVPRef.GetBoardState());
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
                                    ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList, ref TempList2, ref tempInt, ref tempInt);
                                    if (lightList.Count != 0)
                                    {
                                        highlightPossibleMoves(lightList, castlingLightList, EnPassantLightList);
                                    }
                                }
                            }
                        }

                        else if (hitInfo.transform.tag == "Castling")
                        {
                            ChessBoardPVPRef.halfMove++;
                            bool moveLeft = false;
                            rookCastling = null;
                            selectedPiece.GetComponent<Track>().startingPosition = false;
                            //gets the correct rook

                            foreach (GameObject rook in ChessBoardPVPRef.temporaryCastlingPieceRefs)
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

                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                            ChessBoardPVPRef.piecePosition.Remove(rookCastling.transform.localPosition);// Removes the old rook position + gameobject to the dictionary
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

                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                            ChessBoardPVPRef.piecePosition.Add(rookCastling.transform.localPosition, rookCastling); // Adds the Rooks new position + gameobject to the dictionary

                            rookCastling.GetComponent<Track>().startingPosition = false;

                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            //StartCoroutine(AnimateCamera());
                            DestroyAndClearLights();
                        }

                        else if (hitInfo.transform.tag == "EnPassant")
                        {
                            ChessBoardPVPRef.halfMove = 0;
                            pawnEnPassant = ChessBoardPVPRef.temporaryEnPassantPieceRefs;
                            ChessBoardPVPRef.check = false; // Resets Check
                            ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove; //alternates between white and black to move
                            ChessBoardPVPRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old king position + gameobject to the dictionary
                            TakePieceCheck(pawnEnPassant, new Vector3(0, 0, 0));
                            selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                            selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                            ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the Kings new position + gameobject to the dictionary
                            AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                            //StartCoroutine(AnimateCamera());
                            DestroyAndClearLights();
                        }

                        else
                        {
                            DestroyAndClearLights();
                            selectedPiece = null;
                        }
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            int i = 0;
            foreach (Vector3 V in blackPossiblePositions)
            {
                Debug.Log("Black Position " + i + ": "+  V.ToString("f2"));
                i++;
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            int i = 0;
            foreach (Vector3 V in whitePossiblePositions)
            {
                Debug.Log("Black Position " + i + ": " + V.ToString("f2"));
                i++;
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (enPassantGORef != null)
            {
                Debug.Log("Y");
            }
            else
            {
                Debug.Log("N");
            }
        }
    }

    #region Camera
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

    #endregion
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
        if (ChessBoardPVPRef.piecePosition.ContainsKey(temp)) //checks if there is a piece what can be taken
        {
            ChessBoardPVPRef.halfMove = 0;
            if (ChessBoardPVPRef.piecePosition[temp].tag == "White")
            {
                graveYardMoveW += new Vector3(3.0f, 0, 20.0f);
                GameObject tempGameO = Instantiate(ChessBoardPVPRef.piecePosition[temp].gameObject, whiteGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessBoardPVPRef.white;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
                tempGameO.transform.localPosition += graveYardMoveW;
                tempGameO.tag = "Dead";
            }
            else
            {
                graveYardMoveB += new Vector3(3.0f, 0, 20.0f);
                GameObject tempGameO = Instantiate(ChessBoardPVPRef.piecePosition[temp].gameObject, blackGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessBoardPVPRef.black;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
                tempGameO.transform.localPosition += graveYardMoveB;
                tempGameO.tag = "Dead";
            }
            Destroy(ChessBoardPVPRef.piecePosition[temp].gameObject);
            ChessBoardPVPRef.piecePosition.Remove(temp);
        }
    }

    void AllPossibleMoves(ref List<Vector3> WPosP, ref List<Vector3> BPosP, ref List<Vector3> pWpieces, ref List<Vector3> pBPieces)
    {
        #region Variables + Clears
        Dictionary<Vector3, GameObject> tempWhiteList = new Dictionary<Vector3, GameObject>();
        Dictionary<Vector3, GameObject> tempBlackList = new Dictionary<Vector3, GameObject>();
        List<Vector3> tempDiagonalSpacesW = new List<Vector3>();
        List<Vector3> tempDiagonalSpacesB = new List<Vector3>();
        List<Vector3> tempWPosP1 = new List<Vector3>();
        List<Vector3> tempWPosP2 = new List<Vector3>();
        List<Vector3> tempWPosP3 = new List<Vector3>();
        List<Vector3> tempBPosP1 = new List<Vector3>();
        List<Vector3> tempBPosP2 = new List<Vector3>();
        List<Vector3> tempBPosP3 = new List<Vector3>();
        numberOfPMW = 0;
        numberOfPMB = 0;
        ChessBoardPVPRef.pinnedPiecePath.Clear();
        ChessBoardPVPRef.piecesChecking.Clear();
        ChessBoardPVPRef.piecesPuttingKingInCheck = 0;
        ChessBoardPVPRef.lineOfCheck.Clear();
        pWpieces.Clear();
        pBPieces.Clear();
        WPosP.Clear();
        BPosP.Clear();
        #endregion

        #region Un-pin Pieces
        //Un-pin all pieces to be calculated if they are pinned
        foreach (KeyValuePair<Vector3, GameObject> pPos in ChessBoardPVPRef.piecePosition)
        {
            pPos.Value.GetComponent<Track>().pinned = false;

            if (pPos.Value.GetComponent<Track>().enPassant == true)
            {
                if (pPos.Value.GetComponent<Track>().enPassantRound == ChessBoardPVPRef.currentMove)
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
        #endregion

        #region Seperate Pieces by Colour
        //Temporarily seperates the chess pieces by their colour//
        foreach (KeyValuePair<Vector3, GameObject> pPos in ChessBoardPVPRef.piecePosition)
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
        #endregion

        #region If White to move
        // If white to move, calcualte blacks moves first for chance of check.
        if (ChessBoardPVPRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref tempBPosP1, ref pWpieces, ref pBPieces, ref tempBPosP2, ref tempBPosP3, ref tempDiagonalSpacesB, ref numberOfPMW, ref numberOfPMB);
            }
            if (ChessBoardPVPRef.piecesPuttingKingInCheck > 0)
            {
                ChessBoardPVPRef.check = true;
            }
            BPosP.AddRange(tempBPosP1);
            BPosP.AddRange(tempBPosP2);
            BPosP.AddRange(tempBPosP3);
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref tempWPosP1, ref pWpieces, ref pBPieces, ref tempWPosP2, ref tempWPosP3, ref tempDiagonalSpacesW, ref numberOfPMW, ref numberOfPMB);
            }
            WPosP.AddRange(tempWPosP1);
            WPosP.AddRange(tempWPosP2);
            WPosP.AddRange(tempWPosP3);
        }
        #endregion

        #region If Black to move
        else if (!ChessBoardPVPRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref tempWPosP1, ref pWpieces, ref pBPieces, ref tempWPosP2, ref tempWPosP3, ref tempDiagonalSpacesW, ref numberOfPMW, ref numberOfPMB);
            }
            if (ChessBoardPVPRef.piecesPuttingKingInCheck > 0)
            {
                ChessBoardPVPRef.check = true;
            }
            WPosP.AddRange(tempWPosP1);
            WPosP.AddRange(tempWPosP2);
            WPosP.AddRange(tempWPosP3);
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref tempBPosP1, ref pWpieces, ref pBPieces, ref tempBPosP3, ref tempBPosP3, ref tempDiagonalSpacesB, ref numberOfPMW, ref numberOfPMB);
            }
            BPosP.AddRange(tempBPosP1);
            BPosP.AddRange(tempBPosP2);
            BPosP.AddRange(tempBPosP3);
        }
        #endregion


        //Debug.Log("Old Number of Possible Moves White: " + numberOfPMW);
        //Debug.Log("Old Number of Possible Moves Black: " + numberOfPMB);

        numberOfPMW = WPosP.Count;
        numberOfPMB = BPosP.Count;

        //Debug.Log("New Number of Possible Moves White: " + numberOfPMW);
        //Debug.Log("New Number of Possible Moves Black: " + numberOfPMB);

        ChessBoardPVPRef.CheckMateCheck(numberOfPMW, numberOfPMB);
        ChessBoardPVPRef.MoveCounter(ref ChessBoardPVPRef.currentMove);

        WPosP.AddRange(tempDiagonalSpacesW);
        BPosP.AddRange(tempDiagonalSpacesB);
        
        if (firstMove)
        {
            firstMove = !firstMove;
        }
        UpdateUI();
    }

    void highlightPossibleMoves(List<Vector3> lightlist, List<Vector3> castlingList, List<Vector3> enPassantLL)
    {
        int i = 0;
        if (lightlist.Count > 0)
        {
            lights = new GameObject[lightlist.Count];
            foreach (Vector3 light in lightlist)
            {
                if (!ChessBoardPVPRef.blackSpaces.Contains(light)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
                {
                    lights[i] = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
                    lights[i].transform.localPosition = new Vector3(light.x, 0.4f, light.z);
                    lights[i].GetComponent<Light>().intensity = 3;
                    lights[i].tag = "Move";
                    i++;
                }
                else
                {
                    lights[i] = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
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
                if (!ChessBoardPVPRef.blackSpaces.Contains(castleLight)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
                {
                    castlingLights[i] = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
                    castlingLights[i].transform.localPosition = new Vector3(castleLight.x, 0.4f, castleLight.z);
                    castlingLights[i].GetComponent<Light>().intensity = 3;
                    castlingLights[i].tag = "Castling";
                    i++;
                }
                else
                {
                    castlingLights[i] = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
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
                if (!ChessBoardPVPRef.blackSpaces.Contains(ePL)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
                {
                    enPassantLights[0] = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
                    enPassantLights[0].transform.localPosition = new Vector3(ePL.x, 0.4f, ePL.z);
                    enPassantLights[0].GetComponent<Light>().intensity = 3;
                    enPassantLights[0].tag = "EnPassant";
                }
                else
                {
                    enPassantLights[0] = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
                    enPassantLights[0].transform.localPosition = new Vector3(ePL.x, 0.4f, ePL.z);
                    enPassantLights[0].tag = "EnPassant";
                }
            }
        }
    }

    void EnPassantCheck(GameObject selectedPiece, Vector3 newPosition)
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            if (selectedPiece.transform.tag == "White")
            {
                if (selectedPiece.transform.localPosition.z == (newPosition.z - (2 * ChessBoardPVPRef.bZSpace)))
                {
                    selectedPiece.GetComponent<Track>().enPassant = true;
                    selectedPiece.GetComponent<Track>().enPassantRound = ChessBoardPVPRef.currentMove + 1;
                }
            }
            else if (selectedPiece.transform.tag == "Black")
            {
                if (selectedPiece.transform.localPosition.z == (newPosition.z + (2 * ChessBoardPVPRef.bZSpace)))
                {
                    selectedPiece.GetComponent<Track>().enPassant = true;
                    selectedPiece.GetComponent<Track>().enPassantRound = ChessBoardPVPRef.currentMove + 1;
                }
            }
        }
    }

    void UpdateUI()
    {
        if (AIGame)
        {
            if (!firstMove)
            {
                if (whitePossiblePositions.Count == 0 || blackPossiblePositions.Count == 0 && ChessBoardPVPRef.check == false)
                {
                    playerTurn.text = "Draw!";
                }
                else
                {
                    if (ChessBoardPVPRef.checkMate == true)
                    {
                        playerTurn.text = "Checkmate!";
                    }

                    else if (ChessBoardPVPRef.check == true)
                    {
                        playerTurn.text = "Check";
                    }

                    else if (playerSelectedWhite == true && ChessBoardPVPRef.whiteToMove == true)
                    {
                        playerTurn.text = "Your turn";
                    }

                    else if (playerSelectedWhite == false && ChessBoardPVPRef.whiteToMove == false)
                    {
                        playerTurn.text = "Your turn";
                    }

                    else if (playerSelectedWhite == false && ChessBoardPVPRef.whiteToMove == true)
                    {
                        playerTurn.text = "AI Thinking";
                    }

                    else if (playerSelectedWhite == true && ChessBoardPVPRef.whiteToMove == false)
                    {
                        playerTurn.text = "AI Thinking";
                    }
                }
            }
            else
            {
                playerTurn.text = "White to make the First Move";
            }
        }
        else
        {
            if (!firstMove)
            {
                if (whitePossiblePositions.Count == 0 || blackPossiblePositions.Count == 0 && ChessBoardPVPRef.check == false && firstMove == false)
                {
                    playerTurn.text = "Draw!";
                }
                else
                {
                    if (ChessBoardPVPRef.checkMate == true)
                    {
                        playerTurn.text = "Checkmate!";
                    }

                    else if (ChessBoardPVPRef.check == true)
                    {
                        playerTurn.text = "Check";
                    }

                    else if (ChessBoardPVPRef.whiteToMove)
                    {
                        playerTurn.text = "White to move";
                    }

                    else if (!ChessBoardPVPRef.whiteToMove)
                    {
                        playerTurn.text = "Black to move";
                    }
                }
            }
            else
            {
                playerTurn.text = "White to make the First Move";
            }
        }
    }

    #region Promotion
    IEnumerator PromotionCheck(GameObject pawn)
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            if (selectedPiece.tag == "White")
            {
                if (selectedPiece.transform.localPosition.z == ChessBoardPVPRef.bZMax)
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
                        selectedPiece = Instantiate(ChessBoardPVPRef.Queen, ChessBoardPVPRef.chessBoard.transform);
                    }
                    else if (promotionPeice == "Rook")
                    {
                        selectedPiece = Instantiate(ChessBoardPVPRef.Rook, ChessBoardPVPRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Bishop")
                    {
                        selectedPiece = Instantiate(ChessBoardPVPRef.Bishop, ChessBoardPVPRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Knight")
                    {
                        selectedPiece = Instantiate(ChessBoardPVPRef.Knight, ChessBoardPVPRef.chessBoard.transform);

                    }
                    selectedPiece.AddComponent<Track>().startingPosition = false;
                    selectedPiece.GetComponent<Renderer>().material = ChessBoardPVPRef.white;
                    selectedPiece.tag = "White";
                    selectedPiece.transform.localPosition = savedPosition;
                    selectedPiece.transform.localScale = new Vector3(1, 1, 1);
                    promotionPanel.SetActive(false);
                    playerTurn.gameObject.SetActive(true);

                }
            }
            else if (selectedPiece.tag == "Black")
            {
                if (selectedPiece.transform.localPosition.z == ChessBoardPVPRef.bZMin)
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
                        selectedPiece = Instantiate(ChessBoardPVPRef.Queen, ChessBoardPVPRef.chessBoard.transform);
                    }
                    else if (promotionPeice == "Rook")
                    {
                        selectedPiece = Instantiate(ChessBoardPVPRef.Rook, ChessBoardPVPRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Bishop")
                    {
                        selectedPiece = Instantiate(ChessBoardPVPRef.Bishop, ChessBoardPVPRef.chessBoard.transform);

                    }
                    else if (promotionPeice == "Knight")
                    {
                        selectedPiece = Instantiate(ChessBoardPVPRef.Knight, ChessBoardPVPRef.chessBoard.transform);

                    }
                    selectedPiece.AddComponent<Track>().startingPosition = false;
                    selectedPiece.GetComponent<Renderer>().material = ChessBoardPVPRef.black;
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

        ChessBoardPVPRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
        //StartCoroutine(AnimateCamera());
        DestroyAndClearLights();
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
    #endregion

    IEnumerator HighlightAIMove(Vector3 LightPos, GameObject selectedP)
    {
        AIThinking = true;

        GameObject lightGO = Instantiate(lightPrefab, ChessBoardPVPRef.chessBoard.transform);
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

        ChessBoardPVPRef.check = false; // Resets Check
        ChessBoardPVPRef.whiteToMove = !ChessBoardPVPRef.whiteToMove;

        ChessBoardPVPRef.piecePosition.Remove(selPiece.transform.localPosition);

        //If it contains a piece
        if (ChessBoardPVPRef.piecePosition.ContainsKey(newPosition))
        {
            if (selPiece.transform.GetChild(0).tag == "Pawn")
            {
                if (selPiece.tag == "White")
                {
                    if (selPiece.transform.localPosition.z == ChessBoardPVPRef.bZMax) //Perform Promotion
                    {
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                        TakePieceCheck(selPiece, new Vector3(0, 0, 0));
                        TakePieceCheck(ChessBoardPVPRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        GameObject newPiece = Instantiate(ChessBoardPVPRef.Queen, ChessBoardPVPRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        TakePieceCheck(ChessBoardPVPRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        selPiece.transform.localPosition = newPosition; // moves the piece into the correct position.
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else
                {
                    if (selPiece.transform.localPosition.z == ChessBoardPVPRef.bZMin) //Perform Promotion
                    {
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                        TakePieceCheck(selPiece, new Vector3(0, 0, 0));
                        TakePieceCheck(ChessBoardPVPRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        GameObject newPiece = Instantiate(ChessBoardPVPRef.Queen, ChessBoardPVPRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        TakePieceCheck(ChessBoardPVPRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                        selPiece.transform.localPosition = newPosition; // moves the piece into the correct position.
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
            }
            else
            {
                TakePieceCheck(ChessBoardPVPRef.piecePosition[newPosition], new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                selPiece.transform.localPosition = newPosition; // moves the piece into the correct position.
                ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
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
                        Debug.Log("V");
                        TakePieceCheck(enPassantGORef, new Vector3(0, 0, 0));
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else if (selPiece.tag == "White")
                {
                    if (newPosition.z == ChessBoardPVPRef.bZMax) //Promotion
                    {
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                        TakePieceCheck(selPiece, new Vector3(0, 0, 0));
                        GameObject newPiece = Instantiate(ChessBoardPVPRef.Queen, ChessBoardPVPRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        EnPassantCheck(selPiece, newPosition);
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else if (selPiece.tag == "Black") //tag is black
                {
                    if (newPosition.z == ChessBoardPVPRef.bZMin) //Promotion
                    {
                        Destroy(selPiece);
                        GameObject newPiece = Instantiate(ChessBoardPVPRef.Queen, ChessBoardPVPRef.chessBoard.transform);
                        newPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(newPiece.transform.localPosition, newPiece);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
            }
            else if (selPiece.transform.GetChild(0).tag == "King") //Castling
            {
                if (selPiece.tag == "White")
                {
                    //Castle King Side
                    if (selPiece.transform.localPosition.x - newPosition.x == (ChessBoardPVPRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessBoardPVPRef.piecePosition[new Vector3(8.75f, 0, 0)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(-1.25f, 0, 0);
                        ChessBoardPVPRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    //Castle Queen Side
                    else if (selPiece.transform.localPosition.x - newPosition.x == -(ChessBoardPVPRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessBoardPVPRef.piecePosition[new Vector3(0, 0, 0)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(1.25f, 0, 0);
                        ChessBoardPVPRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
                else if (selPiece.tag == "Black")
                {
                    //Castle Queen Side
                    if (selPiece.transform.localPosition.x - newPosition.x == (ChessBoardPVPRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessBoardPVPRef.piecePosition[new Vector3(0, 0, 8.75f)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(-1.25f, 0, 0);
                        ChessBoardPVPRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    //Castle King Side
                    else if (selPiece.transform.localPosition.x - newPosition.x == -(ChessBoardPVPRef.bXSpace * 2))
                    {
                        GameObject Rook = ChessBoardPVPRef.piecePosition[new Vector3(8.75f, 0, 8.75f)]; //locates the correct rook
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Remove(Rook.transform.localPosition);
                        Rook.transform.localPosition = newPosition + new Vector3(1.25f, 0, 0);
                        ChessBoardPVPRef.piecePosition.Add(Rook.transform.localPosition, Rook);
                    }
                    else
                    {
                        selPiece.transform.localPosition = newPosition;
                        ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
                    }
                }
            }
            else
            {
                selPiece.transform.localPosition = newPosition;
                ChessBoardPVPRef.piecePosition.Add(selPiece.transform.localPosition, selPiece);
            }
        }

        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
        #endregion
    }
}
