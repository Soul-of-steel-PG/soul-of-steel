﻿using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public interface IPilotCardView : ICardView {
    void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource, int health);
    void SelectAttack();

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        Sprite imageSource, int health, CardType type, Movement defaultMovement, int defaultDamage);

    IPilotCardController PilotCardController { get; }
    void SetHealthTMP(int value);
}

[Serializable]
public class PilotCardView : CardView, IPilotCardView {
    [Header("Pilot Card UI Components")] [SerializeField]
    private TMP_Text healthTMP;

    [SerializeField] private bool isPanelPilot;

    private IPilotCardController _pilotCardController;

    public IPilotCardController PilotCardController {
        get { return _pilotCardController ??= new PilotCardController(this, GameManager.Instance, UIManager.Instance); }
    }

    private void Start()
    {
        if (isPanelPilot) GameManager.Instance.LocalPilotCardView = this;
    }

    public override bool GetSelected()
    {
        return PilotCardController.GetSelected();
    }

    public override void Select(bool deselect = false)
    {
        PilotCardController.Select(deselect);
    }

    public override ICardView GetCardView()
    {
        return this;
    }

    public override void Dismiss()
    {
        PilotCardController.DismissCard();
    }

    public override int GetId()
    {
        return PilotCardController.GetId();
    }

    public override void DoEffect(int originId)
    {
        PilotCardController.DoEffect(originId);
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        Sprite imageSource, int health, CardType type, Movement defaultMovement, int defaultDamage)
    {
        PilotCardController.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, health,
            defaultMovement, type, defaultDamage);
    }

    public void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource, int _health)
    {
        base.SetCardUI(cardName, cardDescription, scrapCost, imageSource);

        if (healthTMP != null) healthTMP.text = $"Vida: {_health}";
    }

    public void SelectAttack()
    {
        PilotCardController.SelectAttack();
    }

    public override void ManageLeftClick()
    {
        PilotCardController.Select(false);
    }

    public override void ManageRightClick()
    {
        PilotCardController.ManageRightClick();
    }

    public override void SetIsSelecting(bool isSelecting)
    {
        PilotCardController.IsSelecting(isSelecting);
    }

    public override string GetCardName()
    {
        return PilotCardController.GetCardName();
    }

    public override CardType GetCardType()
    {
        return PilotCardController.GetCardType();
    }

    public void SetHealthTMP(int value)
    {
        healthTMP.text = $"Vida: {value}";
    }

    public override int GetScrapCost()
    {
        return PilotCardController.GetScrapCost();
    }
}