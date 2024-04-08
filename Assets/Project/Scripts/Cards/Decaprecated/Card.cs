using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public abstract class Card : MonoBehaviour, IPointerClickHandler {
    [BoxGroup("Card UI Components")]
    [SerializeField] private TMP_Text nameTMP;
    [SerializeField] private TMP_Text descriptionTMP;
    [SerializeField] private TMP_Text scrapCostTMP;
    [SerializeField] private Image imageSourceIMG;
    
    [BoxGroup("Card Properties")]
    [SerializeField] private string cardName;
    [SerializeField] private string cardDescription;
    [SerializeField] private int scrapCost;
    [SerializeField] private int scrapRecovery;
    [SerializeField] private bool isCampEffect;
    [SerializeField] private Sprite imageSource;

    protected void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery, bool isCampEffect, Sprite imageSource) {
        this.cardName = cardName;
        this.cardDescription = cardDescription;
        this.scrapCost = scrapCost;
        this.scrapRecovery = scrapRecovery;
        this.isCampEffect = isCampEffect;
        this.imageSource = imageSource;
        
        SetCardUI();
    }
    
    protected virtual void SetCardUI() {
        nameTMP.text = cardName;
        descriptionTMP.text = cardDescription;
        scrapCostTMP.text = $"{scrapCost}";
        imageSourceIMG.sprite = imageSource;
    }

    /*protected virtual void ShowCard(string name, string description, int scrapCost, Sprite imageSource) {
        ShowCard(true, name, description, scrapCost, imageSource );
    }*/
    
    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
        }
    }
}