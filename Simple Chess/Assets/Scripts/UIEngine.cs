using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIEngine : MonoBehaviour
{
    public TextMeshProUGUI playerTurn;
    public GameObject selectedPiece;
    public GameObject lightPrefab, board, whiteGraveyard, blackGraveyard;
    public Chessboard ChessboardReference;
    public List<Vector3> lightList = new List<Vector3>();
    public GameObject[] lights;
    public List<Vector3> whitePiecesPositions = new List<Vector3>();
    public List<Vector3> blackPiecesPositions = new List<Vector3>();
    public List<Vector3> whitePossiblePositions = new List<Vector3>();
    public List<Vector3> blackPossiblePositions = new List<Vector3>();
    string[] piecesWSP = { "Pawn", "King" , "Rook" };
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
    }

    void Update()
    {
        if (ChessboardReference.whiteToMove)
        {
            playerTurn.text = "White to move";
        }

        else if (!ChessboardReference.whiteToMove)
        {
            playerTurn.text = "Black to move";
        }
        
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
                    takePiece(hitInfo.transform.gameObject);
                    selectedPiece.transform.localPosition = hitInfo.transform.localPosition;
                    selectedPiece.transform.localPosition -= new Vector3(0, 0.4f, 0);
                    ChessboardReference.checkBoard(ref whitePiecesPositions, ref blackPiecesPositions);
                    allPossibleMoves(ref whitePossiblePositions, ref blackPossiblePositions);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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
                                ChessboardReference.getSpaces(selectedPiece, ref lightList);
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

    void takePiece(GameObject newPos)
    {
        Vector3 Pos = newPos.transform.localPosition - new Vector3(0, 0.4f, 0);
        foreach (Transform child in board.transform)
        {
            if (child.transform.localPosition == Pos)
            {
                if (child.tag == "White")
                {
                    GameObject temp = Instantiate(child.gameObject, whiteGraveyard.transform);
                    temp.GetComponent<Renderer>().material = ChessboardReference.white;
                    temp.transform.localScale = new Vector3(20, 20, 20);
                    Destroy(child.gameObject);
                }
                if (child.tag == "Black")
                {
                    GameObject temp = Instantiate(child.gameObject, blackGraveyard.transform);
                    temp.GetComponent<Renderer>().material = ChessboardReference.black;
                    temp.transform.localScale = new Vector3(20, 20, 20);
                 //   temp.transform.localPosition = new Vector3()
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void allPossibleMoves(ref List<Vector3> WPosP, ref List<Vector3> BPosP)
    {
        WPosP.Clear();
        BPosP.Clear();
        foreach (Transform pieces in board.transform)
        {
            if (pieces.tag == "White")
            {
                ChessboardReference.getSpaces(pieces.gameObject, ref WPosP);
            }
            if (pieces.tag == "Black")
            {
                ChessboardReference.getSpaces(pieces.gameObject, ref BPosP);
            }
        }
    }

    void highlightPossibleMoves(List<Vector3> lightlist)
    {
        lights = new GameObject[lightList.Count];
        int i = 0;
        foreach (Vector3 light in lightList)
        {
            if (!ChessboardReference.blackSpaces.Contains(light))
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
}
