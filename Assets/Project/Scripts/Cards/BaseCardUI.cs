using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CardInfoSerialized;

public interface IBaseCardUI
{
    void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource);
    void SetCardUI(CardInfoStruct cardData);
}

public class BaseCardUI : MonoBehaviour, IBaseCardUI, IPointerClickHandler
{
    private CardInfoStruct currentCardData = new CardInfoStruct();

    [SerializeField, BoxGroup("Card UI Components")]
    private TMP_Text nameTMP;

    [SerializeField, BoxGroup("Card UI Components")]
    private TMP_Text descriptionTMP;

    [SerializeField, BoxGroup("Card UI Components")]
    private TMP_Text scrapCostTMP;

    [SerializeField, BoxGroup("Card UI Components")]
    private Image imageSourceIMG;

    public virtual void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource)
    {
        nameTMP.text = cardName;
        descriptionTMP.text = cardDescription;
        if (scrapCostTMP != null) scrapCostTMP.text = $"{scrapCost}";
        imageSourceIMG.sprite = imageSource;
    }

    public void SetCardUI(CardInfoStruct cardData)
    {
        currentCardData = cardData;
        SetCardUI(cardData.CardName, cardData.Description, cardData.Cost, cardData.ImageSource);
    }

    private void OnCardSelected()
    {
        UIManager.Instance.ShowCardPanel(currentCardData.CardName, currentCardData.Description, currentCardData.Cost, currentCardData.ImageSource);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnCardSelected();
        }
    }
}
