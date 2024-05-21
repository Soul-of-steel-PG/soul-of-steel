using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public interface IMatchView {
    void SetCurrentPhaseText(string text);
    void StopPhase(Phase phase);
}

public class MatchView : MonoBehaviour, IMatchView {
    [SerializeField] private TMP_Text currentPhaseText;
    [SerializeField] private TMP_Text enemyLifeText;
    [SerializeField] private TMP_Text priorityText;
    [SerializeField] private TMP_Text orientationText;

    private IMatchController _matchController;

    public IMatchController MatchController {
        get { return _matchController ??= new MatchController(this, GameManager.Instance); }
    }

    private void Awake()
    {
        UIManager.Instance.matchView = this;
        UIManager.Instance._currentGamePanel = this;
        GameManager.Instance.ExecutePhases += ExecutePhases;
    }

    private void Start()
    {
        if (GameManager.Instance.testing) PrepareMatch();
        GameManager.Instance.isFirstRound = true;
    }

    private void Update()
    {
        IPlayerView p = GameManager.Instance.PlayerList.Find(p =>
            p.PlayerController.GetPlayerId() == GameManager.Instance.CurrentPriority);

        if (p != null)
        {
            priorityText.text = $"{p.GetPlayerName()} tiene la prioridad";
        }

        if (GameManager.Instance.LocalPlayerInstance != null)
        {
            string orientationString = "";

            switch (GameManager.Instance.LocalPlayerInstance.PlayerController.GetCurrentDegrees())
            {
                case 180:
                    orientationString = "izquierda";
                    break;
                case 0:
                    orientationString = "derecha";
                    break;
                case 90:
                    orientationString = "arriba";
                    break;
                case 270:
                    orientationString = "abajo";
                    break;
            }

            orientationText.text = $"Orientacion: {orientationString}";
        }

        Debug.Log($"{PhotonNetwork.GetPing()}");
    }

    [Button]
    public void PrepareMatch()
    {
        StartCoroutine(MatchController.PrepareMatch());
    }

    public void SetCurrentPhaseText(string text)
    {
        currentPhaseText.text = text;
    }

    private void ExecutePhases(Phase phase)
    {
        StartCoroutine(phase.Start());
    }

    public void StopPhase(Phase phase)
    {
        StopCoroutine(phase.Start());
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance()) GameManager.Instance.ExecutePhases -= ExecutePhases;
    }

    public void DisconnectPlayer()
    {
        if (!GameManager.Instance.testing) GameManager.Instance.PhotonGame.DisconnectPlayer();
    }

    public void OpenMenuPanel()
    {
        UIManager.Instance.ShowMenuPanel();
    }

    public void UpdateEnemyLifeTMP(int amount)
    {
        if (GameManager.Instance.testing) return;
        enemyLifeText.text = $"Vida enemiga: {amount}";
    }
}