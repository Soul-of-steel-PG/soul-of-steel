using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private MatchView _currentGamePanel;
    
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private CardPanel _currentCardPanel;

    [SerializeField] private GameObject waitingForOpponentPanel;
    [SerializeField] private Canvas _currentWaitingForOpponentPanel;

    public MatchView matchView;

    public void ShowWaitingForOpponentPanel(bool activate = true) {
        FindOrInstantiatePanel(ref _currentWaitingForOpponentPanel, waitingForOpponentPanel);

        _currentWaitingForOpponentPanel.gameObject.SetActive(activate);
    }

    public void ShowGamePanel(bool activate = true) {
        FindOrInstantiatePanel(ref _currentGamePanel, gamePanel);

        _currentGamePanel.gameObject.SetActive(activate);
    }

    public void ShowCardPanel(string cardName, string cardDescription,
        int scrapCost, Sprite imageSource, bool activate = true) {
        FindOrInstantiatePanel(ref _currentCardPanel, cardPanel);

        _currentCardPanel.Init(cardName, cardDescription, scrapCost, imageSource);
        _currentCardPanel.gameObject.SetActive(activate);
    }

    private static void FindOrInstantiatePanel<T>(ref T panel, GameObject prefab) where T : Component {
        if (panel != null) return;

        panel = FindObjectOfType<T>();

        if (panel != null) return;
        Instantiate(prefab).TryGetComponent(out panel);
    }

    public void SetText(string text) {
        matchView.SetCurrentPhaseText(text);
    }
}
