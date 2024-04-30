using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

[Flags]
public enum CardType {
    Pilot,
    Weapon,
    Armor,
    CampEffect,
    Hacking,
    Generator,
    Arm,
    Legs,
    Chest
}

public interface ICardController {
    CardType GetCardType();
    public void ManageRightClick();
    void PrintInfo();
    void Select(bool deselect = false);
    void IsSelecting(bool isSelecting);
    bool GetSelected();
    void DoEffect(int originId);
    void DismissCard();
}

public abstract class CardController : ICardController {
    private readonly ICardView _view;

    private Vector3 startingPos;
    private bool _isSelecting;
    private bool _selected;
    private int _scrapRecovery;

    protected CardType Type;
    protected string CardName { get; private set; }
    protected string CardDescription { get; private set; }
    protected int ScrapCost { get; private set; }
    protected Sprite ImageSource { get; private set; }
    protected int Id;

    protected CardController(ICardView view) {
        _view = view;
    }

    public virtual void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        Sprite imageSource, CardType type) {
        Id = id;
        CardName = cardName;
        CardDescription = cardDescription;
        ScrapCost = scrapCost;
        _scrapRecovery = scrapRecovery;
        ImageSource = imageSource;
        Type = type;

        SetCardUI();
        startingPos = _view.GetGameObject().transform.position;
    }

    protected virtual void SetCardUI() {
        _view.SetCardUI(CardName, CardDescription, ScrapCost, ImageSource);
    }

    protected virtual void ShowCard() {
        UIManager.Instance.ShowCardPanel(CardName, CardDescription, ScrapCost, ImageSource);
    }

    public virtual void ManageRightClick() {
        ShowCard();
    }

    public void PrintInfo() {
        string s = CardName + "\n";
        s += CardDescription + "\n";
        s += $"{ScrapCost}\n";
        Debug.Log(s);
    }

    public abstract CardType GetCardType();

    public void Select(bool deselect = false) {
        if (GameManager.Instance.LocalPlayerInstance._inAnimation) return;
        if (GameManager.Instance.LocalPlayerInstance.PlayerController.GetCardsSelected() && !_selected) return;

        if (_isSelecting) {
            _selected = !deselect && !_selected;
            SelectAnimation(_selected);
            GameManager.Instance.OnCardSelected(_view.GetGameObject().GetComponent<CardView>(), _selected);
        }
        else if (!_isSelecting && _selected) {
            _selected = !deselect;
        }
    }

    private void SelectAnimation(bool select) {
        Transform animationReference = GameManager.Instance.handPanel.animationReference;
        Transform parent = GameManager.Instance.handPanel.transform.parent;

        animationReference.SetParent(parent);

        Transform t = _view.GetGameObject().transform;
        t.SetParent(parent);

        Vector3 endPos = select
            ? GameManager.Instance.middlePanel.transform.position
            : animationReference.position;

        if (select) animationReference.SetParent(GameManager.Instance.handPanel.transform);

        GameManager.Instance.LocalPlayerInstance._inAnimation = true;
        t.DOMove(endPos, 0.5f).OnComplete(() => {
            t.SetParent(select
                ? GameManager.Instance.middlePanel.transform
                : GameManager.Instance.handPanel.transform);

            if (!select) {
                animationReference.SetParent(GameManager.Instance.handPanel.transform);
            }

            GameManager.Instance.LocalPlayerInstance._inAnimation = false;
        });
    }

    public void DismissCard() {
        Transform t = _view.GetGameObject().transform;
        ScrapPanel scrapPanel = GameManager.Instance.scrapPanel;

        Vector3 endPos = scrapPanel.transform.TransformPoint(scrapPanel.transform.position);

        GameManager.Instance.LocalPlayerInstance._inAnimation = true;
        t.DOMove(endPos, 0.5f).OnComplete(() => {
            scrapPanel.SendToBackup();
            t.localScale = scrapPanel.transform.GetChild(0).localScale;
            t.SetParent(scrapPanel.transform);
            t.SetSiblingIndex(2);
            _view.SetDismissTextSizes();

            GameManager.Instance.LocalPlayerInstance._inAnimation = false;
        });
    }

    public void IsSelecting(bool isSelecting) {
        _isSelecting = isSelecting;
    }

    public bool GetSelected() {
        return _selected;
    }

    public virtual void DoEffect(int originId) {
        // Debug.Log($"doing effect from {CardName}");
    }
}