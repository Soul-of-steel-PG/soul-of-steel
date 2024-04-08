using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface ICardView {
    void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource);
}

[Serializable]
public abstract class CardView : MonoBehaviour, ICardView, IPointerClickHandler {
    [SerializeField, BoxGroup("Card UI Components")]
    private TMP_Text nameTMP;
    [SerializeField, BoxGroup("Card UI Components")]
    private TMP_Text descriptionTMP;
    [SerializeField, BoxGroup("Card UI Components")]
    private TMP_Text scrapCostTMP;
    [SerializeField, BoxGroup("Card UI Components")]
    private Image imageSourceIMG;
    
    public virtual void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource) {
        nameTMP.text = cardName;
        descriptionTMP.text = cardDescription;
        scrapCostTMP.text = $"{scrapCost}";
        imageSourceIMG.sprite = imageSource;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            ManageRightClick();
        }
    }

    public abstract void ManageRightClick();

    public abstract CardType GetCardType();

    public abstract void InitCard(string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        bool isCampEffect, Sprite imageSource, int health, BoardView defaultMovement);
    
}