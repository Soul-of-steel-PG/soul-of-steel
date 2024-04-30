using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CellType {
    Normal,
    Mined,
    Blocked,
    Shady
}

public interface ICellController {
    void SetType(CellType type);
    CellType GetCellType();
}

public class CellController : ICellController {
    private readonly ICellView _view;
    public CellType CellType { private set; get; }

    public CellController(ICellView view, CellType cellType) {
        _view = view;
        CellType = cellType;
    }

    public void SetType(CellType type) {
        CellType = type;

        switch (CellType) {
            case CellType.Normal:
                _view.SetCellColor(_view.GetOriginalColor());
                break;
            case CellType.Mined:
                _view.SetCellColor(Color.red);
                break;
            case CellType.Shady:
                _view.SetCellColor(new Color(0.49019607843137253f, 0.19607843137254902f, 0.6235294117647059f));
                break;
            default:
                _view.SetCellColor(_view.GetOriginalColor());
                Debug.Log($"not valid type, using normal color");
                break;
        }
    }

    public CellType GetCellType() {
        return CellType;
    }
}