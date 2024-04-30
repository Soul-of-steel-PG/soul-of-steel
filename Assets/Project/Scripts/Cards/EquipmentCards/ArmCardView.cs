using System.Collections.Generic;
using UnityEngine;

public interface IArmCardView : IEquipmentCardView {
}

public class ArmCardView : EquipmentCardView, IArmCardView {
    private IArmCardController _armCardController;

    public IArmCardController ArmCardController {
        get { return _armCardController ??= new ArmCardController(this); }
    }

    protected override void Start() {
        CardInfoSerialized.CardInfoStruct cardInfoStruct =
            GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Find(c =>
                (c.TypeEnum == CardType.Arm || c.TypeEnum == CardType.Weapon));

        InitCard(cardInfoStruct.Id, cardInfoStruct.CardName, cardInfoStruct.Description,
            cardInfoStruct.Cost, cardInfoStruct.Recovery,
            cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);

        GameManager.Instance.LocalPlayerInstance.PlayerController.SetArmCard(this);
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        int damage, AttackType attackType, int attackDistance, int attackArea, Sprite imageSource, CardType type) {
        ArmCardController.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, damage, attackType,
            attackDistance, attackArea, imageSource, type);
    }

    public override void ManageLeftClick() {
        ArmCardController.Select(false);
    }

    public override void ManageRightClick() {
        ArmCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting) {
        ArmCardController.IsSelecting(isSelecting);
    }

    public override CardType GetCardType() {
        return ArmCardController.GetCardType();
    }

    public override bool GetSelected() {
        return ArmCardController.GetSelected();
    }

    public override void Select(bool deselect = false) {
        ArmCardController.Select(deselect);
    }

    public override void DoEffect(int originId) {
        ArmCardController.DoEffect(originId);
    }

    public override void Dismiss() {
        ArmCardController.DismissCard();
    }

    public void SelectAttack() {
        ArmCardController.SelectAttack();
    }
}