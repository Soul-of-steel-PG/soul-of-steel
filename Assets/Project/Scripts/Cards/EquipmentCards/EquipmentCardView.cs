using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IEquipmentCardView : ICardView {
}

public class EquipmentCardView : CardView, IEquipmentCardView {
    private IEquipmentCardController _equipmentCardController;

    protected virtual void Start() {
        InitCard(0, "Cota de espinas",
            "Devuelve el daño",
            Random.Range(3, 7),
            Random.Range(3, 7),
            null,
            CardType.Armor);
    }

    public override bool GetSelected() {
        return EquipmentCardController.GetSelected();
    }

    public override void Select(bool deselect = false) {
        EquipmentCardController.Select(deselect);
    }

    public override void Dismiss() {
        EquipmentCardController.DismissCard();
    }

    public override void DoEffect(int originId) {
        EquipmentCardController.DoEffect(originId);
    }

    public void InitCard(int id, string cardName, string cardDescription,
        int scrapCost, int scrapRecovery, Sprite imageSource, CardType type) {
        // Debug.Log($"for children is recommended to not use this method");

        EquipmentCardController.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public IEquipmentCardController EquipmentCardController {
        get { return _equipmentCardController ??= new EquipmentCardController(this); }
    }

    public override void ManageLeftClick() {
        EquipmentCardController.Select(false);
    }

    public override void ManageRightClick() {
        EquipmentCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting) {
        EquipmentCardController.IsSelecting(isSelecting);
    }

    public override CardType GetCardType() {
        return EquipmentCardController.GetCardType();
    }
}