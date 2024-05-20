using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IEquipmentCardView : ICardView {
    void DestroyGo(GameObject go);
}

public class EquipmentCardView : CardView, IEquipmentCardView {
    private IEquipmentCardController _equipmentCardController;

    //if init on start
    [SerializeField] private bool start;

    protected virtual void Start()
    {
        if (start)
        {
            CardInfoSerialized.CardInfoStruct cardInfoStruct =
                GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Find(c =>
                    c.Id == 29);

            if (cardInfoStruct == null)
            {
                Debug.LogError("Carta no encontrada");
            }
            else
            {
                InitCard(cardInfoStruct.Id, cardInfoStruct.CardName, cardInfoStruct.Description, cardInfoStruct.Cost,
                    cardInfoStruct.Recovery, cardInfoStruct.Shield, cardInfoStruct.ImageSource,
                    cardInfoStruct.TypeEnum);
            }
        }
    }

    public override bool GetSelected()
    {
        return EquipmentCardController.GetSelected();
    }

    public override int GetId()
    {
        return EquipmentCardController.GetId();
    }

    public override void Select(bool deselect = false)
    {
        EquipmentCardController.Select(deselect);
    }

    public override void Dismiss()
    {
        EquipmentCardController.DismissCard();
    }

    public override void DoEffect(int originId)
    {
        EquipmentCardController.DoEffect(originId);
    }

    public void InitCard(int id, string cardName, string cardDescription,
        int scrapCost, int scrapRecovery, int shieldValue, Sprite imageSource, CardType type)
    {
        // Debug.Log($"for children is recommended to not use this method");

        EquipmentCardController.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, shieldValue,
            imageSource, type);
    }

    public IEquipmentCardController EquipmentCardController {
        get {
            return _equipmentCardController ??=
                new EquipmentCardController(this, GameManager.Instance, UIManager.Instance);
        }
    }

    public override void ManageLeftClick()
    {
        EquipmentCardController.Select(false);
    }

    public override void ManageRightClick()
    {
        EquipmentCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting)
    {
        EquipmentCardController.IsSelecting(isSelecting);
    }

    public override string GetCardName()
    {
        return EquipmentCardController.GetCardName();
    }

    public override CardType GetCardType()
    {
        return EquipmentCardController.GetCardType();
    }

    public void DestroyGo(GameObject go)
    {
        Destroy(go);
    }

    public override int GetScrapCost()
    {
        return EquipmentCardController.GetScrapCost();
    }
}