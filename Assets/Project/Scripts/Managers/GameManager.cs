using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    public CardsDataBase cardDataBase;

    public bool testing;

    public int movementTurn;
    public int attackTurn;

    public HandPanel handPanel;
    public HandPanel middlePanel;
    public ScrapPanel scrapPanel;

    public EquipmentPanel myEquipmentPanel;
    public EquipmentPanel enemyEquipmentPanel;

    public Phase CurrentPhase { get; private set; }
    public PlayerView LocalPlayerInstance { get; set; }
    public string LocalPlayerName;

    public int currentPriority; // player Id
    public BoardView boardView;
    public List<PlayerView> playerList;

    #region Events

    public event Action OnMasterServerConnected;
    public event Action<Phase> ExecutePhases;
    public event Action OnDrawFinishedEvent;
    public event Action<Vector2> OnCellClickedEvent;
    public event Action OnGameStartedEvent;
    public event Action<CardView, bool> OnCardSelectedEvent; // card has been selected or deselected
    public event Action OnCardSelectingFinishedEvent; // all cards has been selected
    public event Action<int> OnPrioritySetEvent;

    //movement events
    public event Action<Movement, PlayerView> OnMovementSelectedEvent;
    public event Action OnMovementFinishedEvent;
    public event Action<int> OnSelectionConfirmedEvent;
    public event Action OnAllMovementSelectedEvent;
    public event Action OnMovementTurnDoneEvent;

    // battle events
    public event Action OnAllAttacksSelectedEvent;
    public event Action OnLocalAttackDoneEvent;

    #endregion

    #region EventsInvokes

    public void OnLocalAttackDone() {
        OnLocalAttackDoneEvent?.Invoke();
    }

    public void OnAllAttackSelected() {
        OnAllAttacksSelectedEvent?.Invoke();
    }

    public void OnMovementTurnDone() {
        OnMovementTurnDoneEvent?.Invoke();
    }

    public void OnAllMovementSelected() {
        OnAllMovementSelectedEvent?.Invoke();
    }

    public void OnSelectionConfirmed(int id) {
        OnSelectionConfirmedEvent?.Invoke(id);
    }

    public void OnMovementFinished() {
        OnMovementFinishedEvent?.Invoke();
    }

    public void OnMovementSelected(Movement movement, PlayerView player) {
        OnMovementSelectedEvent?.Invoke(movement, player);
    }

    public void OnPrioritySet(int priority) {
        OnPrioritySetEvent?.Invoke(priority);
    }


    public void OnCardSelected(CardView card, bool selected) {
        OnCardSelectedEvent?.Invoke(card, selected);
    }

    public void OnSelectingFinished() {
        OnCardSelectingFinishedEvent?.Invoke();
    }

    public void OnCellClicked(Vector2 index) {
        OnCellClickedEvent?.Invoke(index);
    }

    public void OnGameStarted() {
        StartCoroutine(OnGameStartedCoroutine());
    }

    public IEnumerator OnGameStartedCoroutine() {
        while (playerList.Count < 2 && !testing) {
            yield return null;
        }

        OnGameStartedEvent?.Invoke();
    }

    public void OnDrawFinished() {
        OnDrawFinishedEvent?.Invoke();
    }

    public void OnConnectedToServer() {
        OnMasterServerConnected?.Invoke();
    }

    public void ChangePhase(Phase phase) {
        CurrentPhase = phase;
        ExecutePhases?.Invoke(CurrentPhase);
    }

    #endregion

    public void ValidateHealthStatus() {
        foreach (PlayerView playerView in playerList) {
            if (playerView.PlayerController.GetCurrenHealth() <= 0) {
                Debug.Log($"JUEGO TERMINADO, player {playerView.PlayerController.GetPlayerId()} has lost");
            }
        }
    }

    public void PrepareForMatch(IMatchView matchView) {
        playerList.ForEach(player => player.PlayerController.ShuffleDeck(true));
        ChangePhase(new DrawPhase(matchView));
    }

    protected override void OnDestroy() {
        Instance = null;
    }
}