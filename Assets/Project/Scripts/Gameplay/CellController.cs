using System;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CellType {
    Normal
}

public interface ICellController {
}

public class CellController : ICellController {
    private readonly ICellView _view;
    private CellType _type;

    public CellController(ICellView view, CellType type) {
        _view = view;
        _type = type;
    }
}