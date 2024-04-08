using UnityEngine;

public interface IEquipmentCardController : ICardController {
    void InitEquipmentCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type);

}

public class EquipmentCardController : CardController, IEquipmentCardController {
    private readonly IEquipmentCardView _view;

    public EquipmentCardController(IEquipmentCardView view) : base(view) {
        _view = view;
    }

    public void InitEquipmentCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type) {
        InitCard(cardName, cardDescription, scrapCost, scrapRecovery, isCampEffect, imageSource, type);
    }
    
    public override CardType GetCardType() {
        return Type;
    }
}