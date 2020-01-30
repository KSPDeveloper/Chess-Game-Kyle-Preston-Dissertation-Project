using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIEnginePVP : MonoBehaviour
{
    bool promoting = false;
    string promotionPeice = ""; 
    public Camera transitionCamera;
    public bool cameraFocusWhite = true;
    bool isCameraMoving = false;
    public TextMeshProUGUI playerTurn;
    public GameObject[] lights, castlingLights, enPassantLights;
    public GameObject selectedPiece, rookCastling, pawnEnPassant;
    public GameObject lightPrefab, board, whiteGraveyard, blackGraveyard;
    public GameObject promotionPanel, queenButton, rookButton, knightButton, bishopButton;
    public Sprite wQueen2D, wRook2D, wBishop2D, wKnight2D, bQueen2D, bRook2D, bBishop2D, bKnight2D;
    ChessBoardPVP ChessBoardPVPRef;
    public List<Vector3> lightList = new List<Vector3>();
    public List<Vector3> castlingLightList = new List<Vector3>();
    public List<Vector3> EnPassantLightList = new List<Vector3>();
    public List<Vector3> whitePossiblePositions = new List<Vector3>();
    public List<Vector3> blackPossiblePositions = new List<Vector3>();
    public List<Vector3> ProtectedWhitePieces = new List<Vector3>();
    public List<Vector3> ProtectedBlackPieces = new List<Vector3>();
    Vector3 graveYardMoveW = new Vector3(3.0f, 0, -125.0f);
    Vector3 graveYardMoveB = new Vector3(3.0f, 0, -125.0f);
    bool temp = false;
    List<Vector3> TempList1 = new List<Vector3>();
    List<Vector3> TempList2 = new List<Vector3>();
    void Start()
    {
        ChessBoardPVPRef = FindObjectOfType<ChessBoardPVP>();
        UpdateUI();
    }

    void Update()
    {
        if (isCameraMoving == false)
        {
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
                        EnPassantCheck(selectedPiece, hitInfo.transform.gameObject);
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
                            ChessBoardPVPRef.CheckMateCheck(ChessBoardPVPRef.numberOfPossibleMovesW, ChessBoardPVPRef.numberOfPossibleMovesB);
                            //StartCoroutine(AnimateCamera());
                            UpdateUI();
                            DestroyAndClearLights();
                            ChessBoardPVPRef.MoveCounter(ref ChessBoardPVPRef.currentMove);

                            Debug.Log(ChessBoardPVPRef.GetBoardState());
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
                                ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList);
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
                                        ChessBoardPVPRef.CheckMateCheck(ChessBoardPVPRef.numberOfPossibleMovesW, ChessBoardPVPRef.numberOfPossibleMovesB);
                                        //StartCoroutine(AnimateCamera());
                                        UpdateUI();
                                        DestroyAndClearLights();
                                        ChessBoardPVPRef.MoveCounter(ref ChessBoardPVPRef.currentMove);
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
                                        ChessBoardPVPRef.CheckMateCheck(ChessBoardPVPRef.numberOfPossibleMovesW, ChessBoardPVPRef.numberOfPossibleMovesB);
                                        //StartCoroutine(AnimateCamera());
                                        UpdateUI();
                                        DestroyAndClearLights();
                                        ChessBoardPVPRef.MoveCounter(ref ChessBoardPVPRef.currentMove);
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
                                ChessBoardPVPRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2, ref castlingLightList, ref EnPassantLightList);
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
                        ChessBoardPVPRef.CheckMateCheck(ChessBoardPVPRef.numberOfPossibleMovesW, ChessBoardPVPRef.numberOfPossibleMovesB);
                        //StartCoroutine(AnimateCamera());
                        UpdateUI();
                        DestroyAndClearLights();
                        lightList.Clear();
                        castlingLightList.Clear();
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
                        ChessBoardPVPRef.CheckMateCheck(ChessBoardPVPRef.numberOfPossibleMovesW, ChessBoardPVPRef.numberOfPossibleMovesB);
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


        if (temp == false)
        {
            Debug.Log(ChessBoardPVPRef.GetBoardState());
            temp = true;
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
        ChessBoardPVPRef.pinnedPiecePath.Clear();
        ChessBoardPVPRef.numberOfPossibleMovesW = 0;
        ChessBoardPVPRef.numberOfPossibleMovesB = 0;
        ChessBoardPVPRef.piecesChecking.Clear();
        ChessBoardPVPRef.piecesPuttingKingInCheck = 0;
        ChessBoardPVPRef.lineOfCheck.Clear();
        pWpieces.Clear();
        pBPieces.Clear();
        WPosP.Clear();
        BPosP.Clear();

        Dictionary<Vector3, GameObject> tempWhiteList = new Dictionary<Vector3, GameObject>();
        Dictionary<Vector3, GameObject> tempBlackList = new Dictionary<Vector3, GameObject>();

        //Un-pin all pieces to be calculated if they are pinned
        foreach (KeyValuePair<Vector3, GameObject> pPos in ChessBoardPVPRef.piecePosition)
        {
            pPos.Value.GetComponent<Track>().pinned = false;

            if (pPos.Value.GetComponent<Track>().enPassant == true)
            {
                if (pPos.Value.GetComponent<Track>().enPassantRound == ChessBoardPVPRef.currentMove)
                {
                    pPos.Value.GetComponent<Track>().enPassant = false;
                }
            }
        }

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

        // If white to move, calcualte blacks moves first for chance of check.
        if (ChessBoardPVPRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces, ref BPosP, ref BPosP);
            }
            if (ChessBoardPVPRef.piecesPuttingKingInCheck > 0)
            {
                ChessBoardPVPRef.check = true;
            }

            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces, ref WPosP, ref WPosP);
            }
        }

        else if (!ChessBoardPVPRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces, ref WPosP, ref WPosP);
            }

            if (ChessBoardPVPRef.piecesPuttingKingInCheck > 0)
            {
                ChessBoardPVPRef.check = true;
            }


            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessBoardPVPRef.GetSpaces(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces, ref BPosP, ref BPosP);
            }
        }

        //Debug.Log("Number of Possible Moves White: " + ChessboardRef.numberOfPossibleMovesW);
        //Debug.Log("Number of Possible Moves Black: " + ChessboardRef.numberOfPossibleMovesB);
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

    void EnPassantCheck(GameObject selectedPiece, GameObject hitInfo)
    {
        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
        {
            if (selectedPiece.transform.tag == "White")
            {
                if (selectedPiece.transform.localPosition.z == (hitInfo.transform.localPosition.z - (2 * ChessBoardPVPRef.bZSpace)))
                {
                    selectedPiece.GetComponent<Track>().enPassant = true;
                    selectedPiece.GetComponent<Track>().enPassantRound = ChessBoardPVPRef.currentMove + 1;
                }
            }
            else if (selectedPiece.transform.tag == "Black")
            {
                if (selectedPiece.transform.localPosition.z == (hitInfo.transform.localPosition.z + (2 * ChessBoardPVPRef.bZSpace)))
                {
                    selectedPiece.GetComponent<Track>().enPassant = true;
                    selectedPiece.GetComponent<Track>().enPassantRound = ChessBoardPVPRef.currentMove + 1;
                }
            }
        }
    }

    void UpdateUI()
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
        ChessBoardPVPRef.CheckMateCheck(ChessBoardPVPRef.numberOfPossibleMovesW, ChessBoardPVPRef.numberOfPossibleMovesB);
        //StartCoroutine(AnimateCamera());
        UpdateUI();
        DestroyAndClearLights();
        ChessBoardPVPRef.MoveCounter(ref ChessBoardPVPRef.currentMove);

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
