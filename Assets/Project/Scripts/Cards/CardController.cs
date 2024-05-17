using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

[Flags]
public enum CardType
{
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

public interface ICardController
{
    CardType GetCardType();
    public void ManageRightClick();
    void PrintInfo();
    void Select(bool deselect = false);
    void IsSelecting(bool isSelecting);
    bool GetSelected();
    void DoEffect(int originId);
    void DismissCard();
    int GetId();

    int GetScrapCost();
}

public abstract class CardController : ICardController
{
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

    protected CardController(ICardView view)
    {
        _view = view;
    }

    public virtual void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        Sprite imageSource, CardType type)
    {
        //Debug.Log($"Card controller init card {cardName} with id {id}");
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

    protected virtual void SetCardUI()
    {
        _view.SetCardUI(CardName, CardDescription, ScrapCost, ImageSource);
    }

    protected virtual void ShowCard()
    {
        UIManager.Instance.ShowCardPanel(CardName, CardDescription, ScrapCost, ImageSource);
    }

    public virtual void ManageRightClick()
    {
        ShowCard();
    }

    public void PrintInfo()
    {
        string s = CardName + "\n";
        s += CardDescription + "\n";
        s += $"{ScrapCost}\n";
        Debug.Log(s);
    }

    public abstract CardType GetCardType();

    public void Select(bool deselect = false)
    {
        if (GameManager.Instance.LocalPlayerInstance._inAnimation) return;
        if (GameManager.Instance.LocalPlayerInstance.PlayerController.GetCardsSelected() && !_selected) return;

        if (_isSelecting)
        {
            if (!GameManager.Instance.LocalPlayerInstance.PlayerController.TryPayingForCard(GetScrapCost())) return;
            _selected = !deselect && !_selected;
            _view.SelectAnimation(_selected);

            switch (Type)
            {
                case CardType.Weapon:
                case CardType.Arm:
                    GameManager.Instance.OnCardSelected(GameManager.Instance.LocalPlayerInstance,
                        _view.GetGameObject().GetComponent<ArmCardView>(), _selected);
                    break;
                case CardType.CampEffect:
                case CardType.Hacking:
                    GameManager.Instance.OnCardSelected(GameManager.Instance.LocalPlayerInstance,
                        _view.GetGameObject().GetComponent<EffectCardView>(), _selected);
                    break;
                case CardType.Generator:
                    break;
                case CardType.Legs:
                    GameManager.Instance.OnCardSelected(GameManager.Instance.LocalPlayerInstance,
                        _view.GetGameObject().GetComponent<LegsCardView>(), _selected);
                    break;
                case CardType.Armor:
                case CardType.Chest:
                    GameManager.Instance.OnCardSelected(GameManager.Instance.LocalPlayerInstance,
                        _view.GetGameObject().GetComponent<EquipmentCardView>(), _selected);
                    break;
                default:
                    Debug.Log($"CARD NOT IMPLEMENTED");
                    break;
            }
        }
        else if (!_isSelecting && _selected)
        {
            _selected = !deselect;
        }
    }


    public void DismissCard()
    {
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

    public int GetId()
    {
        return Id;
    }

    public void IsSelecting(bool isSelecting)
    {
        _isSelecting = isSelecting;
    }

    public bool GetSelected()
    {
        return _selected;
    }

    public virtual void DoEffect(int originId)
    {
        // Debug.Log($"doing effect from {CardName}");
    }

    public int GetScrapCost()
    {
        return ScrapCost;
    }
}