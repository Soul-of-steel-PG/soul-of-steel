using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface ICellView {
    void SetCellColor(Color color);
    Color GetOriginalColor();
}

[Serializable]
public class CellView : MonoBehaviour, ICellView, IPointerEnterHandler, IPointerExitHandler,
    IPointerClickHandler {
    [SerializeField] private Outline outline;

    public float cellXSize;
    public float cellYSize;

    public Vector2 index;

    private Image _cellImage;
    private Color _originalColor;

    private ICellController _cellController;

    public ICellController CellController {
        get { return _cellController ??= new CellController(this, CellType.Normal); }
    }

    private void Start() {
        _cellImage = GetComponent<Image>();
        _originalColor = _cellImage.color;
    }

    private void Update() {
        if (CellController.GetCellType() == CellType.Mined) {
        }
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
        EffectManager.Instance.CellSelected(index, CellController.GetCellType() == CellType.Normal);
    }

    public void SetCellColor(Color color) {
        _cellImage.color = color;
    }

    public Color GetOriginalColor() {
        return _originalColor;
    }
}