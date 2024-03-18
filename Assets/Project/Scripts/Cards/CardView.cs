using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ICardView {
    void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource);
}

[Serializable]
public class CardView : MonoBehaviour, ICardView {
    [BoxGroup("Card UI Components")]
    [SerializeField] private TMP_Text nameTMP;
    [SerializeField] private TMP_Text descriptionTMP;
    [SerializeField] private TMP_Text scrapCostTMP;
    [SerializeField] private Image imageSourceIMG;
    
    private ICardController _cardController;

    public ICardController CardController {
        get { return _cardController ??= new CardController(this); }
    }
    
    public virtual void SetCardUI(string cardName, string cardDescription, int scrapCost, Sprite imageSource) {
        nameTMP.text = cardName;
        descriptionTMP.text = cardDescription;
        scrapCostTMP.text = $"{scrapCost}";
        imageSourceIMG.sprite = imageSource;
    }
}

public class Test {
    public void TestMethod() {
        var cardView = new CardView();
        cardView.CardController.InitCard("Card Name", "Card Description", 10, 10, false, null);
    }    
}