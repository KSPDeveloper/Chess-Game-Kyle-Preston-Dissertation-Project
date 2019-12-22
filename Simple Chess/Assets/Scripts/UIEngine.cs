using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIEngine : MonoBehaviour
{
    public TextMeshProUGUI playerTurn;
    public GameObject selectedPiece;
    public GameObject lightPrefab, board, whiteGraveyard, blackGraveyard;
    Chessboard ChessboardReference;
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
        ChessboardReference = FindObjectOfType<Chessboard>();
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
        if (Input.GetMouseButtonDown(0))
        {
            DestroyLights();
            lightList.Clear();
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity);
            if (hit)
            {
                //if a piece is selected, preform the move function//
                if (hitInfo.transform.tag == "Move")
                {
                    if (selectedPiece.GetComponent<Track>())
                    {
                        selectedPiece.GetComponent<Track>().startingPosition = false;
                    }
                    ChessboardReference.whiteToMove = !ChessboardReference.whiteToMove; //alternates between white and black to move
                    TakePieceCheck(hitInfo.transform.gameObject); //checks whether there is a takeable piece.
                    ChessboardReference.piecePosition.Remove(selectedPiece.transform.localPosition);// Removes the old position + gameobject to the dictionary
                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                    selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0); // Removes the height change applied onto the highlights
                    ChessboardReference.piecePosition.Add(selectedPiece.transform.localPosition, selectedPiece); // Adds the new position + gameobject to the dictionary
                    AllPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                    UpdateUI();
                    KingCheck();
                }

                //White
                else if (hitInfo.transform.gameObject.tag == "White")
                {
                    if (selectedPiece == hitInfo.transform.gameObject)
                    {
                        selectedPiece = null;
                    }
                    else
                    {
                        selectedPiece = hitInfo.transform.gameObject;


                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                        {
                            if (ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Bishop")
                        {
                            if (ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Knight")
                        {
                            if (ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Rook")
                        {
                            if (ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Queen")
                        {
                            if (ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "King")
                        {
                            if (ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }

                        }
                    }
                }

                //Black
                else if (hitInfo.transform.gameObject.tag == "Black")
                {
                    if (selectedPiece == hitInfo.transform.gameObject)
                    {
                        selectedPiece = null;
                    }
                    else
                    {
                        selectedPiece = hitInfo.transform.gameObject;


                        if (selectedPiece.transform.GetChild(0).tag == "Pawn")
                        {
                            if (!ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Bishop")
                        {
                            if (!ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Knight")
                        {
                            if (!ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Rook")
                        {
                            if (!ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "Queen")
                        {
                            if (!ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }

                        else if (selectedPiece.transform.GetChild(0).tag == "King")
                        {
                            if (!ChessboardReference.whiteToMove)
                            {
                                ChessboardReference.GetSpaces(selectedPiece, ref lightList, ref TempList1, ref TempList2);
                                if (lightList.Count != 0)
                                {
                                    highlightPossibleMoves(lightList);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                selectedPiece = null;
            }
        }
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

    void TakePieceCheck(GameObject newPos)
    {
        Vector3 temp = newPos.transform.localPosition - new Vector3(0, 0.4f, 0);

        if (ChessboardReference.piecePosition.ContainsKey(temp)) //checks if there is a piece what can be taken
        {
            if (ChessboardReference.piecePosition[(temp)].tag == "White")
            {
                GameObject tempGameO = Instantiate(ChessboardReference.piecePosition[temp].gameObject, whiteGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessboardReference.white;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
            }
            else
            { 
                GameObject tempGameO = Instantiate(ChessboardReference.piecePosition[temp].gameObject, blackGraveyard.transform);
                tempGameO.GetComponent<Renderer>().material = ChessboardReference.black;
                tempGameO.transform.localScale = new Vector3(20, 20, 20);
            }
            Destroy(ChessboardReference.piecePosition[temp].gameObject);
            ChessboardReference.piecePosition.Remove(temp);
        }
        /*

        foreach (Transform child in board.transform)
        {
            if (child.transform.localPosition == temp)
            {
                if (child.tag == "White")
                {
                    GameObject tempGameO = Instantiate(child.gameObject, whiteGraveyard.transform);
                    tempGameO.GetComponent<Renderer>().material = ChessboardReference.white;
                    tempGameO.transform.localScale = new Vector3(20, 20, 20);
                    Destroy(child.gameObject);
                }
                if (child.tag == "Black")
                {
                    GameObject tempGameO = Instantiate(child.gameObject, blackGraveyard.transform);
                    tempGameO.GetComponent<Renderer>().material = ChessboardReference.black;
                    tempGameO.transform.localScale = new Vector3(20, 20, 20);
                    Destroy(child.gameObject);
                }
            }
        }
        */
    }

    void AllPossibleMoves(ref List<Vector3> WPosP, ref List<Vector3> BPosP, ref List<Vector3> pWpieces, ref List<Vector3> pBPieces)
    {
        pWpieces.Clear();
        pBPieces.Clear();
        WPosP.Clear();
        BPosP.Clear();
        foreach (KeyValuePair<Vector3, GameObject> pPos in ChessboardReference.piecePosition)
        {
            if (!pPos.Value.transform.GetChild(0).tag.Contains("King")) //Ensures the kings moves are not added to the list
            {
                if (pPos.Value.tag == "White")
                {
                    if (!pPos.Value.transform.GetChild(0).tag.Contains("Pawn"))
                    {
                        ChessboardReference.GetSpaces(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces);
                    }
                    else
                    { 
                        ChessboardReference.PawnTakePositions(pPos.Value, ref WPosP, ref pWpieces, ref pBPieces);
                    }
                }

                if (pPos.Value.tag == "Black")
                {
                    if (!pPos.Value.transform.GetChild(0).tag.Contains("Pawn"))
                    {
                        ChessboardReference.GetSpaces(pPos.Value, ref BPosP, ref ProtectedWhitePieces, ref ProtectedBlackPieces);
                    }
                    else
                    {
                        ChessboardReference.PawnTakePositions(pPos.Value, ref BPosP, ref pWpieces, ref pBPieces);
                    }
                }
            }
            
        }
        /*
        foreach (Transform pieces in board.transform)
        {
            if (pieces.tag == "White")
            {
                ChessboardReference.GetSpaces(pieces.gameObject, ref WPosP);
            }
            if (pieces.tag == "Black")
            {
                ChessboardReference.GetSpaces(pieces.gameObject, ref BPosP);
            }
        }
        */
    }

    void highlightPossibleMoves(List<Vector3> lightlist)
    {
        lights = new GameObject[lightList.Count];
        int i = 0;
        foreach (Vector3 light in lightList)
        {
            if (!ChessboardReference.blackSpaces.Contains(light)) //this is for the black squares on the board as the lighting requires a different intensity to be seen.
            {
                lights[i] = Instantiate(lightPrefab, ChessboardReference.chessBoard.transform);
                lights[i].transform.localPosition = new Vector3(light.x, 0.4f, light.z);
                lights[i].GetComponent<Light>().intensity = 3;
                lights[i].tag = "Move";
                i++;
            }
            else
            {
                lights[i] = Instantiate(lightPrefab, ChessboardReference.chessBoard.transform);
                lights[i].transform.localPosition = new Vector3(light.x, 0.4f, light.z);
                lights[i].tag = "Move";
                i++;
            }
        }
    }

    void UpdateUI()
    {
        if (ChessboardReference.whiteToMove)
        {
            playerTurn.text = "White to move";
        }

        else if (!ChessboardReference.whiteToMove)
        {
            playerTurn.text = "Black to move";
        }
    }

    void KingCheck()
    {

    }
}
