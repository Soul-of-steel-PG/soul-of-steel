using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChestCardView : IEquipmentCardView
{
    public void InitCard(int id, string cardName, string cardDescription,
        int scrapCost, int scrapRecovery, Sprite imageSource, CardType type);

    CardType GetCardType();
    void GetEffect();
    void RemoveEffect();
    void DestroyGo();
}

public class ChestCardView : EquipmentCardView, IChestCardView
{
    private IChestCardController _chestCardController;

    public IChestCardController ChestCardController {
        get { return _chestCardController ??= new ChestCardController(this); }
    }

    protected override void Start()
    {
        //CardInfoSerialized.CardInfoStruct cardInfoStruct =
        //    GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Find(c => c.TypeEnum == CardType.Chest);

        //InitCard(cardInfoStruct.Id, cardInfoStruct.CardName, cardInfoStruct.Description,
        //    cardInfoStruct.Cost, cardInfoStruct.Recovery, cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);
    }

    public void InitCard(int id, string cardName, string cardDescription,
        int scrapCost, int scrapRecovery, Sprite imageSource, CardType type)
    {
        ChestCardController.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public override void ManageLeftClick()
    {
        ChestCardController.Select(false);
    }

    public override void ManageRightClick()
    {
        ChestCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting)
    {
        ChestCardController.IsSelecting(isSelecting);
    }

    public override CardType GetCardType()
    {
        return ChestCardController.GetCardType();
    }

    public override bool GetSelected()
    {
        return ChestCardController.GetSelected();
    }

    public override void Select(bool deselect = false)
    {
        ChestCardController.Select(deselect);
    }

    public override void DoEffect(int originId)
    {
        ChestCardController.DoEffect(originId);
    }

    public void GetEffect()
    {
        ChestCardController.GetEffect();
    }

    public void RemoveEffect()
    {
        ChestCardController.RemoveEffect();
    }

    public void DestroyGo()
    {
        Destroy(gameObject);
    }

    public override void Dismiss()
    {
        ChestCardController.DismissCard();
    }

    public override int GetScrapCost()
    {
        return ChestCardController.GetScrapCost();
    }

    public override int GetId()
    {
        return ChestCardController.GetId();
    }
}