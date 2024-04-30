using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.Serialization;

public interface IBoardView {
    Transform GetTransform();
    float GetXBoardSize();
    float GetYBoardSize();
    GameObject InstantiateCellView();
    List<List<CellView>> GetBoardStatus();
    void SetBoardStatus(List<List<CellView>> board);
}

public class BoardView : MonoBehaviour, IBoardView {
    [OnValueChanged("GenerateBoard")] public float xBoardSize;
    [OnValueChanged("GenerateBoard")] public float yBoardSize;
    [OnValueChanged("GenerateBoard")] public float offset;
    public GameObject cellPrefab;

    [ShowInInspector] private List<List<CellView>> _boardStatus = new List<List<CellView>>();

    private IBoardController _boardController;

    public IBoardController BoardController {
        get { return _boardController ??= new BoardController(this); }
    }

    private void Awake() {
        SetBoardStatusInPlaymode();
    }

    private void Start() {
        GameManager.Instance.boardView = this;
    }

    public Transform GetTransform() {
        return transform;
    }

    public float GetXBoardSize() {
        return xBoardSize;
    }

    public float GetYBoardSize() {
        return xBoardSize;
    }

    [Button]
    public void GenerateBoard() {
        DestroyTransformChildren();

        BoardController.GenerateBoardCells(xBoardSize / BoardController.GetBoardCount(),
            yBoardSize / BoardController.GetBoardCount(), offset, cellPrefab);
    }

    private void DestroyTransformChildren() {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++) {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public GameObject InstantiateCellView() {
        return Instantiate(cellPrefab);
    }

    public List<List<CellView>> GetBoardStatus() {
        return _boardStatus;
    }

    public Vector3 GetCellPos(Vector2 index) {
        return _boardStatus[(int)index.y][(int)index.x].transform.position;
    }

    public void SetBoardStatus(List<List<CellView>> board) {
        _boardStatus = board;
    }

    public void SetBoardStatusCellType(Vector2 index, CellType cellType) {
        _boardStatus[(int)index.y][(int)index.x].CellController.SetType(cellType);
    }

    public void SetBoardStatusInPlaymode() {
        _boardStatus = new List<List<CellView>>();

        for (int i = 0; i < transform.childCount / BoardController.GetBoardCount(); i++) {
            _boardStatus.Add(new List<CellView>());
            for (int j = 0; j < BoardController.GetBoardCount(); j++) {
                _boardStatus[i].Add(transform.GetChild(j + i * 10).GetComponent<CellView>());
                if (_boardStatus[i].Count == 10) break;
            }
        }
    }
}