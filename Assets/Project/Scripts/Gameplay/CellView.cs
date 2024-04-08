using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface ICellView {
}

[Serializable]
public class CellView : MonoBehaviour, ICellView, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] private Outline outline;
    
    public float cellXSize;
    public float cellYSize;

    public Vector2 index;

    private ICellController _cellController;

    public ICellController CellController {
        get { return _cellController ??= new CellController(this, CellType.Normal); }
    }

    private void Start() {
    }

    public void SetSize() {
        TryGetComponent(out RectTransform recTransform);

        recTransform.sizeDelta = new Vector2(cellXSize, cellYSize);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        outline.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData) {
        GameManager.Instance.OnCellClicked(index);
    }
}