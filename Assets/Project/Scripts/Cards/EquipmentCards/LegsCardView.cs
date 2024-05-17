using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ILegsCardView : IEquipmentCardView
{
    void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        List<Movement> movements, Sprite imageSource, CardType type);

    void SelectMovement();
    ILegsCardController LegsCardController { get; }
    void DestroyGo();
}

public class LegsCardView : EquipmentCardView, ILegsCardView
{
    private ILegsCardController _legsCardController;

    public ILegsCardController LegsCardController {
        get { return _legsCardController ??= new LegsCardController(this); }
    }

    public void DestroyGo()
    {
        Destroy(gameObject);
    }

    protected override void Start()
    {
        //CardInfoSerialized.CardInfoStruct cardInfoStruct =
        //    GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Find(c => c.TypeEnum == CardType.Legs);

        //InitCard(cardInfoStruct.Id, cardInfoStruct.CardName, cardInfoStruct.Description,
        //    cardInfoStruct.Cost, cardInfoStruct.Recovery, cardInfoStruct.SerializedMovements,
        //    cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        List<Movement> movements, Sprite imageSource, CardType type)
    {
        LegsCardController.InitCard(id, cardName, cardDescription, scrapCost,
            scrapRecovery, movements, imageSource, type);
    }

    public override void ManageLeftClick()
    {
        LegsCardController.Select(false);
    }

    public override void ManageRightClick()
    {
        LegsCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting)
    {
        LegsCardController.IsSelecting(isSelecting);
    }

    public override CardType GetCardType()
    {
        return LegsCardController.GetCardType();
    }

    public override bool GetSelected()
    {
        return LegsCardController.GetSelected();
    }

    public override void Select(bool deselect = false)
    {
        LegsCardController.Select(deselect);
    }

    public override void DoEffect(int originId)
    {
        LegsCardController.DoEffect(originId);
    }

    public override void Dismiss()
    {
        LegsCardController.DismissCard();
    }

    public void SelectMovement()
    {
        LegsCardController.SelectMovement();
    }

    public override int GetScrapCost()
    {
        return LegsCardController.GetScrapCost();
    }

    public override int GetId()
    {
        return LegsCardController.GetId();
    }
}