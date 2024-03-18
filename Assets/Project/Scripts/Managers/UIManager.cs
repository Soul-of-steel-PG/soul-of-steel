using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {
    [SerializeField] private GameObject cardPanel;
    private CardPanel _currentCardPanel;
    
    public void ShowCard(bool activate, string cardName, string cardDescription,
        int scrapCost, Sprite imageSource) {
        if (_currentCardPanel == null) {
            _currentCardPanel = Instantiate(cardPanel).GetComponent<CardPanel>();
        }
        
        _currentCardPanel.Init(cardName, cardDescription, scrapCost, imageSource);
        _currentCardPanel.gameObject.SetActive(activate);
    }
}
