using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public interface IGameManager {
    int CurrentPriority { get; }
    void SetCurrentPriority(int currentPriority);
    List<IPlayerView> PlayerList { get; }
    IBoardView BoardView { get; set; }
    IHandPanel HandPanel { get; set; }
    IHandPanel MiddlePanel { get; set; }
    CardInfoSerialized.CardInfoStruct GetCardFromDataBaseByIndex(int index);
    CardInfoSerialized.CardInfoStruct GetCardFromDataBaseByType(CardType type);
    void AddPlayerToThePlayerList(IPlayerView playerView);
    bool ValidateHealthStatus();
    void OnSelectionConfirmed(int id);
    bool GetTesting();
    void OnLocalAttackDone();
    void OnMovementSelected(Movement getDefaultMovement, PlayerView view, int getMovementIterations);
    IPlayerView LocalPlayerInstance { get; set; }
    void OnCardSelected(IPlayerView playerView, ICardView card, bool selected);
    ScrapPanel ScrapPanel { get; set; }
}

public class GameManager : MonoBehaviourSingleton<GameManager>, IGameManager {
    public CardsDataBase cardDataBase;

    public bool testing;
    public bool isFirstRound;

    public int movementTurn;
    public int attackTurn;

    public IHandPanel HandPanel { get; set; }
    public IHandPanel MiddlePanel { get; set; }
    public ScrapPanel ScrapPanel { get; set; }

    public EquipmentPanel myEquipmentPanel;
    public EquipmentPanel enemyEquipmentPanel;

    public Phase CurrentPhase { get; private set; }
    public IPlayerView LocalPlayerInstance { get; set; }
    public PhotonGame PhotonGame { get; set; }

    public PilotCardView LocalPilotCardView;
    public string LocalPlayerName;

    public int CurrentPriority { get; set; }
    public IBoardView BoardView { get; set; }
    public List<IPlayerView> PlayerList { get; set; }

    public Phase CurrenPhase;
    public bool gameOver;

    #region Events

    public event Action OnMasterServerConnected;
    public event Action<Phase> ExecutePhases;
    public event Action OnDrawFinishedEvent;
    public event Action<Vector2> OnCellClickedEvent;
    public event Action OnGameStartedEvent;
    public event Action<IPlayerView, ICardView, bool> OnCardSelectedEvent; // card has been selected or deselected
    public event Action OnCardSelectingFinishedEvent; // all cards has been selected
    public event Action<int> OnPrioritySetEvent;

    //movement events
    public event Action<Movement, PlayerView, int> OnMovementSelectedEvent;
    public event Action OnMovementFinishedEvent;
    public event Action<int> OnSelectionConfirmedEvent;
    public event Action OnAllMovementSelectedEvent;
    public event Action OnMovementTurnDoneEvent;

    // battle events
    public event Action OnAllAttacksSelectedEvent;
    public event Action OnLocalAttackDoneEvent;

    #endregion

    #region EventsInvokes

    public void OnLocalAttackDone()
    {
        OnLocalAttackDoneEvent?.Invoke();
    }

    public void OnAllAttackSelected()
    {
        OnAllAttacksSelectedEvent?.Invoke();
    }

    public void OnMovementTurnDone()
    {
        OnMovementTurnDoneEvent?.Invoke();
    }

    public void OnAllMovementSelected()
    {
        OnAllMovementSelectedEvent?.Invoke();
    }

    public void OnSelectionConfirmed(int id)
    {
        OnSelectionConfirmedEvent?.Invoke(id);
    }

    public bool GetTesting()
    {
        return testing;
    }

    public void OnMovementFinished()
    {
        OnMovementFinishedEvent?.Invoke();
    }

    public void OnMovementSelected(Movement movement, PlayerView player, int iterations)
    {
        OnMovementSelectedEvent?.Invoke(movement, player, iterations);
    }

    public void SetCurrentPriority(int currentPriority)
    {
        CurrentPriority = currentPriority;
        OnPrioritySetEvent?.Invoke(currentPriority);
    }

    // BORRAR.
    public void OnPrioritySet(int priority)
    {
        OnPrioritySetEvent?.Invoke(priority);
    }


    public void OnCardSelected(IPlayerView playerView, ICardView card, bool selected)
    {
        OnCardSelectedEvent?.Invoke(playerView, card, selected);
    }

    public void OnSelectingFinished()
    {
        OnCardSelectingFinishedEvent?.Invoke();
    }

    public void OnCellClicked(Vector2 index)
    {
        OnCellClickedEvent?.Invoke(index);
    }

    public void OnGameStarted()
    {
        StartCoroutine(OnGameStartedCoroutine());
    }

    public IEnumerator OnGameStartedCoroutine()
    {
        while (PlayerList.Count < 2 && !testing)
        {
            yield return null;
        }

        OnGameStartedEvent?.Invoke();
    }

    public void OnDrawFinished()
    {
        OnDrawFinishedEvent?.Invoke();
    }

    public void OnConnectedToServer()
    {
        OnMasterServerConnected?.Invoke();
    }

    public void ChangePhase(Phase phase)
    {
        if (gameOver) return;
        CurrentPhase = phase;
        ExecutePhases?.Invoke(CurrentPhase);
    }

    #endregion

    public bool ValidateHealthStatus()
    {
        foreach (IPlayerView playerView in PlayerList)
        {
            if (playerView.PlayerController.GetCurrenHealth() <= 0)
            {
                Debug.Log($"current phase {CurrenPhase}");
                Debug.Log($"player view id {playerView.PlayerController.GetPlayerId()}");
                Debug.Log($"player health {playerView.PlayerController.GetCurrenHealth()}");
                gameOver = true;
                UIManager.Instance.SetText(
                    $"JUEGO TERMINADO, {playerView.GetPlayerName()} perdio");
                CurrenPhase.End();
                StartCoroutine(BackToMenuFinishingGame());
                return false;
            }
        }

        return true;
    }

    IEnumerator BackToMenuFinishingGame()
    {
        yield return new WaitForSeconds(3);

        PhotonGame.DisconnectPlayer();
    }

    public void ResetValues()
    {
        testing = false;
        isFirstRound = false;
        movementTurn = 0;
        attackTurn = 0;
        CurrentPhase = default(Phase);
        CurrentPriority = 0;
        CurrenPhase = default(Phase);
        gameOver = false;
    }

    public void PrepareForMatch(IMatchView matchView)
    {
        PlayerList.ForEach(player => player.PlayerController.ShuffleDeck(true, false));
        ChangePhase(new DrawPhase(matchView));
    }

    public CardInfoSerialized.CardInfoStruct GetCardFromDataBaseByIndex(int index)
    {
        CardInfoSerialized.CardInfoStruct cardInfoStruct = cardDataBase.cardDataBase.Sheet1
            .Find(c => c.Id == index);
        return cardInfoStruct;
    }

    public CardInfoSerialized.CardInfoStruct GetCardFromDataBaseByType(CardType type)
    {
        CardInfoSerialized.CardInfoStruct cardInfoStruct = cardDataBase.cardDataBase.Sheet1
            .Find(c => c.TypeEnum == CardType.Pilot);

        return cardInfoStruct;
    }

    public void AddPlayerToThePlayerList(IPlayerView playerView)
    {
        PlayerList ??= new List<IPlayerView>();

        PlayerList.Add(playerView);
    }

    protected override void OnDestroy()
    {
        Instance = null;
    }
}