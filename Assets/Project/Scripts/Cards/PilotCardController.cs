using System.Collections.Generic;
using UnityEngine;

public interface IPilotCardController : ICardController {
    void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery, Sprite imageSource,
        int health, Movement defaultMovement, CardType type, int defaultDamage = 0);

    Movement GetDefaultMovement();
    int GetDefaultDamage();
    int GetHealth();
}

public class PilotCardController : CardController, IPilotCardController {
    private readonly IPilotCardView _view;

    [Header("Pilot Properties")] private int _health;
    private int _defaultDamage;
    private Movement _defaultMovement;
    [Space(20)] public Vector2 position;

    public PilotCardController(IPilotCardView view) : base(view) {
        _view = view;
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        Sprite imageSource, int health, Movement defaultMovement, CardType type,
        int defaultDamage = 1) {
        _health = health;
        _defaultMovement = defaultMovement;
        _defaultDamage = defaultDamage;

        /* Init card method called at the end because I am calling SetCardUI from it,
           and in this class I am modifying the SetCardUI*/
        base.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public Movement GetDefaultMovement() {
        return _defaultMovement;
    }

    public int GetDefaultDamage() {
        return _defaultDamage;
    }

    public int GetHealth() {
        return _health;
    }

    protected override void SetCardUI() {
        _view.SetCardUI(CardName, CardDescription, ScrapCost, ImageSource, _health);
    }

    public override CardType GetCardType() {
        return Type;
    }
}