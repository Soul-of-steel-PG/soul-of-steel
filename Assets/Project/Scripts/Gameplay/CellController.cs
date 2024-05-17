using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CellType
{
    Normal,
    Mined,
    Blocked,
    Shady,
    Barrier,
    Tower
}

public interface ICellController
{
    void SetType(CellType type);
    CellType GetCellType();
    bool GetIsMined();
    bool GetIsBarrier();
    bool GetIsTower();

    void HideCell();

    void ShowCell();
}

public class CellController : ICellController
{
    private readonly ICellView _view;
    public CellType CellType { private set; get; }

    private bool _isMined;
    private bool _isBarrier;
    private bool _isTower;

    public CellController(ICellView view, CellType cellType)
    {
        _view = view;
        CellType = cellType;
    }

    public void SetType(CellType type)
    {
        CellType = type;

        switch (CellType) {
            case CellType.Normal:
                _isMined = false;
                _view.SetCellColor(_view.GetOriginalColor());
                break;
            case CellType.Mined:
                _view.SetCellColor(Color.red);
                _isMined = true;
                break;
            case CellType.Shady:
                _view.SetCellColor(new Color(0.49019607843137253f, 0.19607843137254902f, 0.6235294117647059f));
                break;
            case CellType.Barrier:
                _isBarrier = true;
                _view.SetCellColor(new Color(0.6509434f, 0.355801f, 0.1934407f));
                break;
            case CellType.Tower:
                _isTower = true;
                _view.SetCellColor(new Color(0.5529411764705883f, 0.6705882352941176f, 0.20784313725490197f));
                break;
            default:
                _view.SetCellColor(_view.GetOriginalColor());
                Debug.Log($"not valid type, using normal color");
                break;
        }
    }

    public void HideCell()
    {
        _view.SetCellColor(new Color(0.1803921568627451f, 0.15294117647058825f, 0.34901960784313724f));
    }

    public void ShowCell()
    {
        switch (CellType)
        {
            case CellType.Normal:
                _view.SetCellColor(_view.GetOriginalColor());
                break;
            case CellType.Mined:
                _view.SetCellColor(Color.red);
                break;
            case CellType.Shady:
                _view.SetCellColor(new Color(0.49019607843137253f, 0.19607843137254902f, 0.6235294117647059f));
                break;
            case CellType.Barrier:
                _view.SetCellColor(new Color(0.6509434f, 0.355801f, 0.1934407f));
                break;
            case CellType.Tower:
                _view.SetCellColor(new Color(0.5529411764705883f, 0.6705882352941176f, 0.20784313725490197f));
                break;
            default:
                _view.SetCellColor(_view.GetOriginalColor());
                Debug.Log($"not valid type, using normal color");
                break;
        }
    }

    public CellType GetCellType()
    {
        return CellType;
    }

    public bool GetIsMined()
    {
        return _isMined;
    }

    public bool GetIsBarrier()
    {
        return _isBarrier;
    }

    public bool GetIsTower()
    {
        return _isTower;
    }
}