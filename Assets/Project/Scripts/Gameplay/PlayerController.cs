using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public interface IPlayerController
{
    void SetPlayerId(int id);
    void DrawCards(int amount, bool fullDraw);
    void EquipCard(int index);
    void ShuffleDeck(bool firstTime, bool shuffle);
    void SelectAttack();
    void SelectMovement();
    void SelectCards(List<CardType> types, int amount, bool setIsSelecting = true);
    void ReceivedDamage(int damage, int localPlayerId);
    IEnumerator AddCards(int amount);
    int GetPlayerId();
    bool GetCardsSelected();
    void SetCardsSelected(bool cardsSelected);
    IEnumerator SelectCells(int amount);
    bool GetMoving();
    void SetMoving(bool moving);
    bool GetDoingEffect();
    void SetDoingEffect(bool doingEffect);
    bool GetAllEffectsDone();
    void SetAllEffectsDone(bool allEffectsDone);
    void SetCurrentCell(Vector2 currentCell);
    Vector2 GetCurrentCell();
    void SetExtraDamage(int extraDamage);
    void SetCurrentDegrees(int currentDegrees);
    int GetCurrentDegrees();
    bool GetMovementSelected();
    void SetMovementSelected(bool movementSelected);
    bool GetMovementDone();
    void SetMovementDone(bool movementDone);
    void DoMovement();
    PilotCardView GetPilotCard();
    void DoAttack(Vector2 index);
    int GetCurrenHealth();
    void SetHealth(int amount);
    bool TryPayingForCard(int cardCost);

    void AddToScrapValue(int value);
    void SubtractFromScrapValue(int value);
}

public class PlayerController : IPlayerController
{
    #region Attributes

    private readonly IPlayerView _view;
    private readonly IGameManager _gameManager;

    private readonly PlayerCardsInfo _shuffledDeck;
    private readonly List<ICardView> _hand;

    private int _playerId;
    private int _health;
    private int _scrapPoints;
    private List<CardView> _scrapPile;
    private List<CardView> _factory;
    private IPilotCardView _pilot;
    private ILegsCardView _legs;
    private IArmCardView _arm;
    private IArmCardView _weapon;
    private IChestCardView _bodyArmor;

    private bool _movementSelected;
    private bool _movementDone;

    private bool _cardsSelected;
    private bool _moving;
    private bool _doingEffect;
    private bool _allEffectsDone;
    private int _extraDamage = 0;
    private int _currentMovementId;

    private List<Vector2> _cellsSelected;

    private Vector2 _currentCell;
    private int _currentDegrees;
    private int _currentDamage;

    #endregion

    public PlayerController(IPlayerView view, IGameManager gameManager)
    {
        _view = view;
        _gameManager = gameManager;

        _hand = new List<ICardView>();
        _shuffledDeck = ScriptableObject.CreateInstance<PlayerCardsInfo>();
        _moving = false;
        _currentDegrees = 270;
        _currentCell = new Vector2(5, 5);
        _scrapPoints = 15;
    }

    #region Cards Management

    public void DrawCards(int amount, bool fullDraw)
    {
        if (fullDraw)
        {
            // not destroying the select animation reference
            _gameManager.HandPanel.ResetAnimationReferenceParent();
            _view.CleanHandsPanel();
            _hand.Clear();
        }

        _view.InitAddCards(amount);
    }

    public IEnumerator AddCards(int amount)
    {
        while (amount > 0)
        {
            yield return new WaitForSeconds(0.5f);
            UIManager.Instance.SetText($"adding cards {amount}");
            int index = 0;
            CardInfoSerialized.CardInfoStruct cardInfoStruct = _shuffledDeck.playerCards[index];

            ICardView card = null;

            if (cardInfoStruct.TypeEnum != CardType.Pilot)
            {
                card = _view.AddCardToPanel(cardInfoStruct.TypeEnum, true);
            }

            switch (cardInfoStruct.TypeEnum)
            {
                case CardType.Pilot:
                    Debug.Log($"Cannot have pilot card here");
                    continue;
                case CardType.CampEffect:
                case CardType.Hacking:
                case CardType.Generator:
                    ((IEffectCardView)card)?.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
                        cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery,
                        cardInfoStruct.IsCampEffect, cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);
                    break;
                case CardType.Legs:
                    ((ILegsCardView)card)?.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
                        cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery,
                        cardInfoStruct.SerializedMovements, cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);
                    break;
                case CardType.Weapon:
                case CardType.Arm:
                    ((IArmCardView)card)?.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
                        cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery, cardInfoStruct.Damage,
                        cardInfoStruct.AttackTypeEnum, cardInfoStruct.AttackDistance, cardInfoStruct.AttackArea,
                        cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);
                    break;
                case CardType.Armor:
                case CardType.Chest:
                    ((IChestCardView)card)?.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
                        cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery,
                        cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            _hand.Add(card);
            _shuffledDeck.playerCards.RemoveAt(index);
            if (_shuffledDeck.playerCards.Count == 0) ShuffleDeck(false, false);
            amount--;
        }

        GameManager.Instance.OnDrawFinished();
    }

    public bool TryPayingForCard(int cardCost)
    {
        int finalScrapAmount = _scrapPoints - cardCost;

        if (finalScrapAmount >= 0)
        {
            _scrapPoints = finalScrapAmount;
            return true;
        }

        return false;
    }

    public void EquipCard(int index)
    {
        CardInfoSerialized.CardInfoStruct cardInfoStruct = _gameManager.GetCardFromDataBaseByIndex(index);

        if (cardInfoStruct == null)
        {
            Debug.LogError($"CARD NOT FOUND");
            return;
        }

        switch (cardInfoStruct.TypeEnum)
        {
            case CardType.Arm:
            case CardType.Weapon:
                SetArmCard(cardInfoStruct);
                break;
            case CardType.Legs:
                SetLegsCard(cardInfoStruct);
                break;
            case CardType.Armor:
            case CardType.Chest:
                SetArmorCard(cardInfoStruct);
                break;
        }
    }

    public void ShuffleDeck(bool firstTime, bool shuffle)
    {
        List<CardInfoSerialized.CardInfoStruct> temporalDeck = _view.GetDeckInfo().playerCards.ToList();
        if (shuffle)
        {
            int n = temporalDeck.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (temporalDeck[k], temporalDeck[n]) = (temporalDeck[n], temporalDeck[k]);
            }
        }

        _shuffledDeck.playerCards = temporalDeck;

        if (firstTime)
        {
            SetPilotCard();
        }

        _shuffledDeck.playerCards.Remove(_shuffledDeck.playerCards.Find(p => p.TypeEnum == CardType.Pilot));
    }

    public void SelectCards(List<CardType> type, int amount, bool setSelecting = true)
    {
        bool has = false;
        foreach (CardView card in _hand)
        {
            if (type.Contains(card.GetCardType()))
            {
                has = true;
                card.SetIsSelecting(setSelecting);
                GameManager.Instance.ValidateHealthStatus();
            }
        }

        if (!has)
        {
            SetCardsSelected(true);
        }
    }

    #endregion

    #region SelectThings

    public void SelectAttack()
    {
        _currentDamage = _extraDamage;

        if (!_view.GetPv().IsMine) return;

        if (_weapon == null && _arm != null)
        {
            _currentDamage += _arm.ArmCardController.GetDamage();
            _arm.SelectAttack();
        }
        else if (_weapon == null && _arm == null)
        {
            _currentDamage += _pilot.PilotCardController.GetDefaultDamage();
            _pilot.SelectAttack();
        }
        else if (_weapon != null)
        {
            _currentDamage += _weapon.ArmCardController.GetDamage();
            _weapon.SelectAttack();
        }
    }

    public void SelectMovement()
    {
        if (!_view.GetPv().IsMine) return;

        GameManager.Instance.OnSelectionConfirmedEvent += OnMovementSelected;
        if (_legs == null && _pilot != null)
        {
            GameManager.Instance.OnSelectionConfirmed(0);
        }
        else
        {
            _legs.SelectMovement();
        }
    }

    public void OnMovementSelected(int movementId)
    {
        SetMovementSelected(true);
        _currentMovementId = movementId;
        GameManager.Instance.OnAllMovementSelectedEvent += OnAllMovementSelected;
    }

    public void OnAllMovementSelected()
    {
        GameManager.Instance.OnSelectionConfirmedEvent -= OnMovementSelected;
        GameManager.Instance.OnAllMovementSelectedEvent -= OnAllMovementSelected;
    }

    public IEnumerator SelectCells(int amount)
    {
        int currentAmount = amount;
        _cellsSelected = new List<Vector2>();
        _cellsSelected.Clear();

        EffectManager.Instance.OnSelectedCellEvent += CellSelected;

        while (currentAmount > 0)
        {
            yield return null;
            currentAmount = amount - _cellsSelected.Count;
        }

        EffectManager.Instance.OnSelectedCellEvent -= CellSelected;

        EffectManager.Instance.CellsSelected(_cellsSelected);
    }

    private void CellSelected(Vector2 index, bool select)
    {
        if (select) _cellsSelected.Add(index);
        else _cellsSelected.Remove(index);
    }

    #endregion

    #region Actions

    public void DoAttack(Vector2 index)
    {
        if (GameManager.Instance.BoardView.GetBoardStatus()[(int)index.y][(int)index.x].CellController.GetCellType() !=
            CellType.Shady) return;

        PlayerView otherPlayer =
            GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() != _playerId) as PlayerView;

        if (GameManager.Instance.testing)
            otherPlayer =
                GameManager.Instance.PlayerList.Find(p => p.PlayerController.GetPlayerId() == _playerId) as PlayerView;

        if (otherPlayer?.PlayerController.GetCurrentCell() == index)
        {
            if (!GameManager.Instance.testing)
            {
                otherPlayer.photonView.RPC("RpcReceivedDamage", RpcTarget.AllBuffered, 2,
                    otherPlayer.PlayerController.GetPlayerId());
            }
        }

        GameManager.Instance.OnLocalAttackDone();
        _view.SetAttackDone(true);
        GameManager.Instance.OnCellClickedEvent -= DoAttack;
    }

    public void ReceivedDamage(int damage, int localPlayerId)
    {
        if (localPlayerId == _playerId)
        {
            _health -= damage;
            _pilot.SetHealthTMP(_health);
        }
    }

    public void DoMovement()
    {
        if (_legs == null && _pilot != null)
        {
            GameManager.Instance.OnMovementSelected(_pilot.PilotCardController.GetDefaultMovement(),
                (PlayerView)_view, GetMovementIterations());
        }
        else
        {
            GameManager.Instance.OnMovementSelected(_legs.LegsCardController.GetMovements()[_currentMovementId],
                (PlayerView)_view, GetMovementIterations());
        }
    }

    #endregion

    #region Setting Cards

    private void SetArmCard(CardInfoSerialized.CardInfoStruct cardInfoStruct)
    {
        IArmCardView card = (IArmCardView)_view.AddCardToPanel(cardInfoStruct.TypeEnum);
        
        card.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName, cardInfoStruct.Description,
            cardInfoStruct.Cost, cardInfoStruct.Recovery, cardInfoStruct.Damage, cardInfoStruct.AttackTypeEnum,
            cardInfoStruct.AttackDistance, cardInfoStruct.AttackArea, cardInfoStruct.ImageSource,
            cardInfoStruct.TypeEnum);


        if (card.GetCardType() == CardType.Arm)
        {
            if (_arm != null)
            {
                _arm.RemoveEffect();
                _arm.DestroyGo();
            }

            _arm = card;
            _arm.GetEffect();
        }
        else if (card.GetCardType() == CardType.Weapon)
        {
            if (_weapon != null)
            {
                _weapon.RemoveEffect();
                _weapon.DestroyGo();
            }

            _weapon = card;
            _weapon.GetEffect();
        }
    }

    private void SetLegsCard(CardInfoSerialized.CardInfoStruct cardInfoStruct)
    {
        ILegsCardView card = (ILegsCardView)_view.AddCardToPanel(cardInfoStruct.TypeEnum);

        card.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
            cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery,
            cardInfoStruct.SerializedMovements, cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);

        if (_legs != null)
        {
            _legs.DestroyGo();
        }

        _legs = card;
    }

    private void SetArmorCard(CardInfoSerialized.CardInfoStruct cardInfoStruct)
    {
        IChestCardView card = (IChestCardView)_view.AddCardToPanel(cardInfoStruct.TypeEnum);

        card.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
            cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery,
            cardInfoStruct.ImageSource, cardInfoStruct.TypeEnum);

        //Remove previous effect
        if (_bodyArmor != null)
        {
            _bodyArmor.RemoveEffect();
            _bodyArmor.DestroyGo();
        }

        _bodyArmor = card;

        _bodyArmor.GetEffect();
    }

    private void SetPilotCard()
    {
        CardInfoSerialized.CardInfoStruct cardInfoStruct =
            _shuffledDeck.playerCards.Find(c => c.TypeEnum == CardType.Pilot);

        IPilotCardView card = (IPilotCardView)_view.AddCardToPanel(cardInfoStruct.TypeEnum);

        card.InitCard(cardInfoStruct.Id, cardInfoStruct.CardName,
            cardInfoStruct.Description, cardInfoStruct.Cost, cardInfoStruct.Recovery,
            cardInfoStruct.ImageSource, cardInfoStruct.Health, cardInfoStruct.TypeEnum,
            cardInfoStruct.SerializedMovements[0], cardInfoStruct.Damage);
        _pilot = card;
        _health = _pilot.PilotCardController.GetHealth();
        if (_view.GetPv().IsMine) _pilot.SetHealthTMP(_health);
        else UIManager.Instance.matchView.UpdateEnemyLifeTMP(_health);
    }

    #endregion

    #region Setters And Getters

    private int GetMovementIterations()
    {
        if (EffectManager.Instance.doubleMovementEffectActive)
        {
            EffectManager.Instance.SetDoubleMovementEffectActive(false);
            return 2;
        }

        return 1;
    }

    public void AddToScrapValue(int valueToAdd)
    {
        _scrapPoints += valueToAdd;
    }

    public void SubtractFromScrapValue(int valueToSubtract)
    {
        _scrapPoints -= valueToSubtract;
    }

    public bool GetMoving()
    {
        return _moving;
    }

    public void SetMoving(bool moving)
    {
        _moving = moving;
    }

    public bool GetDoingEffect()
    {
        return _doingEffect;
    }

    public void SetDoingEffect(bool doingEffect)
    {
        _doingEffect = doingEffect;
    }

    public bool GetAllEffectsDone()
    {
        return _allEffectsDone;
    }

    public void SetAllEffectsDone(bool allEffectsDone)
    {
        _allEffectsDone = allEffectsDone;
    }

    public void SetCurrentCell(Vector2 currentCell)
    {
        _currentCell = currentCell;
    }

    public Vector2 GetCurrentCell()
    {
        return _currentCell;
    }

    public void SetCurrentDegrees(int currentDegrees)
    {
        _currentDegrees = currentDegrees;
    }

    public int GetCurrentDegrees()
    {
        return _currentDegrees;
    }

    public bool GetMovementSelected()
    {
        return _movementSelected;
    }

    public void SetMovementSelected(bool movementSelected)
    {
        _movementSelected = movementSelected;
    }

    public bool GetMovementDone()
    {
        return _movementDone;
    }

    public void SetMovementDone(bool movementDone)
    {
        _movementDone = movementDone;
    }

    public void SetPlayerId(int id)
    {
        _playerId = id;
    }

    public int GetPlayerId()
    {
        return _playerId;
    }

    public bool GetCardsSelected()
    {
        return _cardsSelected;
    }

    public void SetCardsSelected(bool cardsSelected)
    {
        _cardsSelected = cardsSelected;
    }

    public void SetExtraDamage(int extraDamage)
    {
        _extraDamage = extraDamage;
    }

    public int GetCurrenHealth()
    {
        return _health;
    }

    public void SetHealth(int amount)
    {
        _health = amount;
        if (_playerId != GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId())
        {
            UIManager.Instance.matchView.UpdateEnemyLifeTMP(_health);
        }
    }

    public PilotCardView GetPilotCard()
    {
        return (PilotCardView)_pilot;
    }

    #endregion

    #region Debug

#if UNITY_EDITOR
    public int Debug_GetScrapPoints()
    {
        return _scrapPoints;
    }

    public void Debug_SetArmCard(CardInfoSerialized.CardInfoStruct cardInfoStruct)
    {
        SetArmCard(cardInfoStruct);
    }
#endif

    #endregion
}