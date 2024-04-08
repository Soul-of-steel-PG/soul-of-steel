using System;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IEquipmentCardView : ICardView {
}

public class EquipmentCardView : CardView, IEquipmentCardView {
    private IEquipmentCardController _equipmentCardController;

    private void Start() {
        EquipmentCardController.InitEquipmentCard("Cota de espinas",
            "Devuelve el daño",
            Random.Range(3, 7),
            Random.Range(3, 7),
            false,
            null,
            CardType.Armor);
    }

    public override void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, int health, BoardView defaultMovement) {
        EquipmentCardController.InitEquipmentCard(cardName, cardDescription, scrapCost, scrapRecovery,
            isCampEffect, imageSource, CardType.Pilot);
    }

    public IEquipmentCardController EquipmentCardController {
        get { return _equipmentCardController ??= new EquipmentCardController(this); }
    }

    public override void ManageRightClick() {
        EquipmentCardController.ManageRightClick();
    }

    public override CardType GetCardType() {
        return EquipmentCardController.GetCardType();
    }
}