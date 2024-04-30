using UnityEngine;

public interface IEffectCardController : ICardController {
    void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type);
}

public class EffectCardController : CardController, IEffectCardController {
    private readonly IEffectCardView _view;

    private bool _isCampEffect;

    public EffectCardController(IEffectCardView view) : base(view) {
        _view = view;
    }

    public override CardType GetCardType() {
        return Type;
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type) {
        _isCampEffect = isCampEffect;
        base.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public override void DoEffect(int originId) {
        base.DoEffect(originId);
        Debug.Log($"putting mines");

        switch (Id) {
            case 0:
                EffectManager.Instance.PutMines(originId, 3);
                break;
        }
    }
}