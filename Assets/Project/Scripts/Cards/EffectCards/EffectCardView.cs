using System.Collections.Generic;
using UnityEngine;


public interface IEffectCardView : ICardView
{
    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type);
}

public class EffectCardView : CardView, IEffectCardView
{
    private IEffectCardController _effectCardController;

    public IEffectCardController EffectCardController {
        get { return _effectCardController ??= new EffectCardController(this); }
    }

    public override void ManageLeftClick()
    {
        EffectCardController.Select(false);
    }

    public override void ManageRightClick()
    {
        EffectCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting)
    {
        EffectCardController.IsSelecting(isSelecting);
    }

    public override CardType GetCardType()
    {
        return EffectCardController.GetCardType();
    }

    public override bool GetSelected()
    {
        return EffectCardController.GetSelected();
    }

    public override void Select(bool deselect = false)
    {
        EffectCardController.Select(deselect);
    }

    public override void Dismiss()
    {
        EffectCardController.DismissCard();
    }

    public override void DoEffect(int originId)
    {
        EffectCardController.DoEffect(originId);
    }

    public override int GetId()
    {
        return EffectCardController.GetId();
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, CardType type)
    {
        EffectCardController.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, isCampEffect,
            imageSource, type);
    }

    public override int GetScrapCost()
    {
        return EffectCardController.GetScrapCost();
    }
}