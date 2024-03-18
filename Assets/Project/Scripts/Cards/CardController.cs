using UnityEngine;

// public enum CardType {
//     Pilot,
//     Weapon,
//     CampEffect
// }

public interface ICardController {
    void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery, bool isCampEffect,
        Sprite imageSource);

    void ShowCard();
    // CardType GetCardType();
}

public class CardController : ICardController {
    private readonly ICardView _view;

    private string _cardName;
    private string _cardDescription;
    private int _scrapCost;
    private int _scrapRecovery;
    private bool _isCampEffect;
    private Sprite _imageSource;

    public CardController(ICardView view) {
        _view = view;
    }

    public void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery, bool isCampEffect,
        Sprite imageSource) {
        _cardName = cardName;
        _cardDescription = cardDescription;
        _scrapCost = scrapCost;
        _scrapRecovery = scrapRecovery;
        _isCampEffect = isCampEffect;
        _imageSource = imageSource;

        SetCardUI();
    }

    protected virtual void SetCardUI() {
        _view.SetCardUI(_cardName, _cardDescription, _scrapCost, _imageSource);
    }

    public virtual void ShowCard() {
        Debug.Log($"opening the card");
    }

    // public abstract CardType GetCardType();
}