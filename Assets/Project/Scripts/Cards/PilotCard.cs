using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class PilotCard : Card {
    [Header("Pilot Card UI Components")]
    [SerializeField] private TMP_Text healthTMP;

    [Header("Pilot Properties")]
    private int _health;
    private int _defaultDamage;
    private Board _defaultMovement;
    [Space(20)]public Vector2 position;

    private void Start() {
        InitializePilotCard("Charizard",
            "esta carta está rotisima no hay nada que hacer contra ella",
            6,
            9,
            false,
            null,
            100,
            null);
    }

    public void InitializePilotCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, int health, Board defaultMovement, int defaultDamage = 0) {
        _health = health;
        _defaultMovement = defaultMovement;
        _defaultDamage = defaultDamage;
        
        /* Init card method called at the end because I am calling SetCardUI from it,
           and in this class I am modifying the SetCardUI*/
        InitCard(cardName, cardDescription, scrapCost, scrapRecovery, isCampEffect, imageSource);
    }

    protected override void SetCardUI() {
        base.SetCardUI();

        if(healthTMP != null) healthTMP.text = $"Vida: {_health}";
    }
}
