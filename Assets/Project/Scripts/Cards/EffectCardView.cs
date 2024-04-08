using UnityEngine;

public interface IEffectCardView : ICardView {
}

public class EffectCardView : CardView, IEffectCardView {
    private IEffectCardController _effectCardController;

    public IEffectCardController EffectCardController {
        get { return _effectCardController ??= new EffectCardController(this); }
    }

    public override void ManageRightClick() {
        EffectCardController.ManageRightClick();
    }

    public override CardType GetCardType() {
        return EffectCardController.GetCardType();
    }

    public override void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, int health, BoardView defaultMovement) {
    }
}