using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {
    public TMP_InputField nickname;

    [SerializeField] private GameObject gamePanel;
    [SerializeField] public MatchView _currentGamePanel;

    [SerializeField] private GameObject cardPanel;
    [SerializeField] private CardPanel _currentCardPanel;

    [SerializeField] private GameObject waitingForOpponentPanel;
    [SerializeField] private WaitingForOpponentPanel currentWaitingForOpponentPanel;

    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private SelectionPanel _currentSelectionPanel;

    public MatchView matchView;

    private void Start() {
        if (!GameManager.Instance.testing) nickname.onValueChanged.AddListener(SetNickname);
    }

    private void SetNickname(string nickname) {
        GameManager.Instance.LocalPlayerName = nickname;
    }

    public void ShowWaitingForOpponentPanel(bool activate = true) {
        FindOrInstantiatePanel(ref currentWaitingForOpponentPanel, waitingForOpponentPanel);

        currentWaitingForOpponentPanel.gameObject.SetActive(activate);
    }

    public void ShowGamePanel(bool activate = true) {
        FindOrInstantiatePanel(ref _currentGamePanel, gamePanel);

        _currentGamePanel.transform.GetChild(0).gameObject.SetActive(activate);
        if (activate) _currentGamePanel.PrepareMatch();
    }

    public void ShowCardPanel(string cardName, string cardDescription,
        int scrapCost, Sprite imageSource, bool activate = true) {
        FindOrInstantiatePanel(ref _currentCardPanel, cardPanel);

        _currentCardPanel.Init(cardName, cardDescription, scrapCost, imageSource);
        _currentCardPanel.gameObject.SetActive(activate);
    }

    public void ShowSelectionPanel(int optionsAmount, List<string> optionNames, bool activate = true) {
        FindOrInstantiatePanel(ref _currentSelectionPanel, selectionPanel);

        if (optionNames != null) _currentSelectionPanel.Init(optionsAmount, optionNames);
        _currentSelectionPanel.gameObject.SetActive(activate);
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

    protected override void OnDestroy() {
        if (GameManager.HasInstance()) {
            if (!GameManager.Instance.testing) {
                nickname.onValueChanged.RemoveAllListeners();
            }
        }
    }
}