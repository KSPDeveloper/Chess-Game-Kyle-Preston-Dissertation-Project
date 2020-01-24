﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIEngine : MonoBehaviour
{
    public Camera transitionCamera;
    public bool cameraFocusWhite = true;
    bool isCameraMoving = false;
    public TextMeshProUGUI playerTurn;
    public GameObject selectedPiece;
    public GameObject lightPrefab, board, whiteGraveyard, blackGraveyard;
    Chessboard ChessboardRef;
    public List<Vector3> lightList = new List<Vector3>();
    public GameObject[] lights;
    public List<Vector3> whitePossiblePositions = new List<Vector3>();
    public List<Vector3> blackPossiblePositions = new List<Vector3>();
    public List<Vector3> ProtectedWhitePieces = new List<Vector3>();
    public List<Vector3> ProtectedBlackPieces = new List<Vector3>();

    List<Vector3> TempList1 = new List<Vector3>();
    List<Vector3> TempList2 = new List<Vector3>();

    string[] piecesWSP = { "Pawn", "King", "Rook" };
    Vector3[] possiblePositions;

    void Start()
    {
        ChessboardRef = FindObjectOfType<Chessboard>();
        Vector3[] possiblePositions = new Vector3[15];
        possiblePositions[0] = new Vector3();
        possiblePositions[1] = new Vector3();
        possiblePositions[2] = new Vector3();
        possiblePositions[3] = new Vector3();
        possiblePositions[4] = new Vector3();
        possiblePositions[5] = new Vector3();
        possiblePositions[6] = new Vector3();
        possiblePositions[7] = new Vector3();
        possiblePositions[8] = new Vector3();
        possiblePositions[9] = new Vector3();
        possiblePositions[10] = new Vector3();
        possiblePositions[11] = new Vector3();
        possiblePositions[12] = new Vector3();
        possiblePositions[13] = new Vector3();
        possiblePositions[14] = new Vector3();
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
                        if (selectedPiece.GetComponent<Track>().startingPosition == true)
                        {
                            selectedPiece.GetComponent<Track>().startingPosition = false;
                        }
                        ChessboardRef.check = false; // Resets Check
                        ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                        TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, -0.4f, 0)); //checks whether there is a takeable piece.
                        ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                        selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                        selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                        ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                        AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                        ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                        StartCoroutine(AnimateCamera());
                        UpdateUI();
                        DestroyLights();
                        lightList.Clear();
                    }

                    else if (hitInfo.transform.gameObject.tag == "White")
                    {
                        //White to move
                        if (ChessboardRef.whiteToMove)
                        {
                            if (selectedPiece == hitInfo.transform.gameObject)
                            {
                                DestroyLights();
                                lightList.Clear();
                                selectedPiece = null;
                            }

                            else
                            {
                                DestroyLights();
                                lightList.Clear();
                                selectedPiece = hitInfo.transform.gameObject;
                                ChessboardRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
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
                                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                    {
                                        selectedPiece.GetComponent<Track>().startingPosition = false;
                                    }
                                    ChessboardRef.check = false; // Resets Check
                                    ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                                    TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                    ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                    ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                    AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                    ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                                    StartCoroutine(AnimateCamera());
                                    UpdateUI();
                                    DestroyLights();
                                    lightList.Clear();
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
                                    if (selectedPiece.GetComponent<Track>().startingPosition == true)
                                    {
                                        selectedPiece.GetComponent<Track>().startingPosition = false;
                                    }
                                    ChessboardRef.check = false; // Resets Check
                                    ChessboardRef.whiteToMove = !ChessboardRef.whiteToMove; //alternates between white and black to move
                                    TakePieceCheck(hitInfo.transform.gameObject, new Vector3(0, 0, 0)); //checks whether there is a takeable piece.
                                    ChessboardRef.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                                    ChessboardRef.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                                    AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                                    ChessboardRef.CheckMateCheck(ChessboardRef.numberOfPossibleMovesW, ChessboardRef.numberOfPossibleMovesB);
                                    StartCoroutine(AnimateCamera());
                                    UpdateUI();
                                    DestroyLights();
                                    lightList.Clear();
                                }
                            }
                        }

                        //Black to move
                        else
                        {
                            if (selectedPiece == hitInfo.transform.gameObject)
                            {
                                DestroyLights();
                                lightList.Clear();
                                selectedPiece = null;
                            }

                            else
                            {
                                DestroyLights();
                                lightList.Clear();
                                selectedPiece = hitInfo.transform.gameObject;
                                ChessboardRef.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }
                    }

                    else
                    {
                        DestroyLights();
                        lightList.Clear();
                        selectedPiece = null;
                    }
                }
            }
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

    void DestroyLights()
    {
        foreach (Transform child in board.transform)
        {
            if (child.tag == "Move")
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
            if (ChessboardRef.piecePosition[temp].tag == "White")
            {
                GameObject tempGameO = Instantiate(ChessboardRef.piecePosition[temp].gameObject, whiteGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessboardRef.white;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
            }
            else
            { 
                GameObject tempGameO = Instantiate(ChessboardRef.piecePosition[temp].gameObject, blackGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessboardRef.black;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
            }
            Destroy(ChessboardRef.piecePosition[temp].gameObject);
            ChessboardRef.piecePosition.Remove(temp);
        }
    }

    void AllPossibleMoves(ref List<Vector3> WPosP, ref List<Vector3> BPosP, ref List<Vector3> pWpieces, ref List<Vector3> pBPieces)
    {
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

        //If white to move, calcualte blacks moves first for chance of check.
        if (ChessboardRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces);
            }

            if (ChessboardRef.piecesPuttingKingInCheck > 0)
            {
                ChessboardRef.check = true;
            }

            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces);
            }
        }

        else if (!ChessboardRef.whiteToMove)
        {
            foreach (KeyValuePair<Vector3, GameObject> pPos in tempWhiteList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces);
            }

            if (ChessboardRef.piecesPuttingKingInCheck > 0)
            {
                ChessboardRef.check = true;
            }


            foreach (KeyValuePair<Vector3, GameObject> pPos in tempBlackList)
            {
                ChessboardRef.GetSpaces(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces);
            }
        }
        
        //Debug.Log("Number of Possible Moves White: " + ChessboardRef.numberOfPossibleMovesW);
        //Debug.Log("Number of Possible Moves Black: " + ChessboardRef.numberOfPossibleMovesB);

        
    }

    void highlightPossibleMoves(List<Vector3> lightlist)
    {
        lights = new GameObject[lightList.Count];
        int i = 0;
        foreach (Vector3 light in lightList)
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

        else if (ChessboardRef.whiteToMove)
        {
            playerTurn.text = "White to move";
        }

        else if (!ChessboardRef.whiteToMove)
        {
            playerTurn.text = "Black to move";
        }
    }
}
