using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBoardController {
    void GenerateBoardCells(float cellXSize, float cellYSize, float offset, GameObject cellPrefab);
    int GetBoardCount();
}

public class BoardController : IBoardController {
    private readonly IBoardView _view;

    private const int BoardCount = 10;
    private List<QuadrantView> _quadrants;

    public List<List<CellView>> _boardStatus;

    public BoardController(IBoardView view) {
        _view = view;

        _quadrants = new List<QuadrantView>();
        _boardStatus = new List<List<CellView>>();
    }

    public void GenerateBoardCells(float cellXSize, float cellYSize, float offset, GameObject cellPrefab) {
        int xSize = BoardCount;
        int ySize = BoardCount;

        for (int i = 0; i < xSize; i++) {
            _boardStatus.Add(new List<CellView>());
            for (int j = 0; j < ySize; j++) {
                float xCenter = (cellXSize / 2) + (cellXSize * j) + (offset * j);
                float yCenter = (cellYSize / 2) - (cellYSize * i) - (offset * i);
                Transform cellT = _view.InstantiateCellView().transform;
                cellT.SetParent(_view.GetTransform());
                RectTransform rt = (RectTransform)cellT;
                rt.anchorMin = new Vector2(j / 10f, (ySize - (i + 1)) / 10f);
                rt.anchorMax = new Vector2((j + 1) * 0.1f, (ySize - i) / 10f);

                cellT.TryGetComponent(out CellView boardCell);
                boardCell.cellXSize = cellXSize;
                boardCell.cellYSize = cellYSize;
                boardCell.index = new Vector2(j, i);
                boardCell.SetSize();


                cellT.localPosition = new Vector2(xCenter, yCenter);
                
                _boardStatus[i].Add(boardCell);
                cellT.gameObject.name = $"cell_{i}{j}";

                cellT.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        _view.SetBoardStatus(_boardStatus);
    }

    public int GetBoardCount() {
        return BoardCount;
    }
}