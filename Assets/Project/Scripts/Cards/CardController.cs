using UnityEngine;

public enum CardType {
    Pilot,
    Weapon,
    Armor,
    CampEffect
}

public interface ICardController {
    CardType GetCardType();
    public void ManageRightClick();
    void PrintInfo();
}

public abstract class CardController : ICardController {
    private readonly ICardView _view;

    private int _scrapRecovery;
    private bool _isCampEffect;

    protected CardType Type;
    protected string CardName { get; private set; }
    protected string CardDescription { get; private set; }
    protected int ScrapCost { get; private set; }
    protected Sprite ImageSource { get; private set; }

    protected CardController(ICardView view) {
        _view = view;
    }

    protected void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type) {
        CardName = cardName;
        CardDescription = cardDescription;
        ScrapCost = scrapCost;
        _scrapRecovery = scrapRecovery;
        _isCampEffect = isCampEffect;
        ImageSource = imageSource;
        Type = type;

        SetCardUI();
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
}