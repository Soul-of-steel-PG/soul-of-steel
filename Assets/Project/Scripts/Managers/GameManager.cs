using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviourSingleton<GameManager> {
    public bool testing;

    public GameObject handPanel;
    
    public Phase CurrentPhase { get; private set; }
    public GameObject LocalPlayerInstance { get; set; }

    public int currentPriority; // player Id
    public BoardView boardView;
    public List<PlayerView> playerList;

    public event Action OnMasterServerConnected;
    public event Action<Phase> ExecutePhases;
    public event Action OnDrawFinishedEvent;
    public event Action<string> OnDataDownloadedEvent;
    public event Action<Vector2> OnCellClickedEvent;

    public void OnCellClicked(Vector2 index) {
        OnCellClickedEvent?.Invoke(index);
    }

    public void OnDataDownloaded(string data) {
        OnDataDownloadedEvent?.Invoke(data);
    }

    public void OnDrawFinished() {
        OnDrawFinishedEvent?.Invoke();
    }

    public void OnConnectedToServer() {
        OnMasterServerConnected?.Invoke();
    }

    public void ApplyDamage(int playerId) {
        
    }

    public void ValidateHealthStatus() {
        
    }

    public void ExecuteEffect() {
        
    }

    public void PrepareForMatch(IMatchView matchView) {
        playerList.ForEach(player => player.PlayerController.ShuffleDeck());
        ChangePhase(new DrawPhase(matchView));
    }

    public void ChangePhase(Phase phase) {
        CurrentPhase = phase;
        ExecutePhases?.Invoke(CurrentPhase);
    }

    protected override void OnDestroy() {
        Instance = null;
    }
}