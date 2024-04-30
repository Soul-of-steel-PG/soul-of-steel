using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public interface IMatchView {
    void SetCurrentPhaseText(string text);
}

public class MatchView : MonoBehaviour, IMatchView {
    [SerializeField] private TMP_Text currentPhaseText;

    private IMatchController _matchController;

    public IMatchController MatchController {
        get { return _matchController ??= new MatchController(this); }
    }

    private void Awake() {
        UIManager.Instance.matchView = this;
        UIManager.Instance._currentGamePanel = this;
        GameManager.Instance.ExecutePhases += ExecutePhases;
    }

    private void Start() {
        if (GameManager.Instance.testing) PrepareMatch();
    }

    [Button]
    public void PrepareMatch() {
        StartCoroutine(MatchController.PrepareMatch());
    }

    public void SetCurrentPhaseText(string text) {
        currentPhaseText.text = text;
    }

    private void ExecutePhases(Phase phase) {
        StartCoroutine(phase.Start());
    }

    private void OnDestroy() {
        if (GameManager.HasInstance()) GameManager.Instance.ExecutePhases -= ExecutePhases;
    }
}