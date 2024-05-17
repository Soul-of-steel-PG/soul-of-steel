using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class CardPanel : MonoBehaviour
{
    [BoxGroup("UI Components")][SerializeField] private TMP_Text nameTMP;
    [BoxGroup("UI Components")][SerializeField] private TMP_Text descriptionTMP;
    [BoxGroup("UI Components")][SerializeField] private TMP_Text scrapCostTMP;
    [BoxGroup("UI Components")][SerializeField] private Image imageSourceIMG;

    public void Init(string cardName, string cardDescription,
        int scrapCost, Sprite imageSource) {

        nameTMP.text = cardName;
        descriptionTMP.text = cardDescription;
        scrapCostTMP.text = $"{scrapCost}";
        imageSourceIMG.sprite = imageSource;
    }
}
