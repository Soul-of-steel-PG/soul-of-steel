using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IPlayerView {
    GameObject GetHandCardsPanel();
    void CleanHandsPanel();
    CardView AddCardToPanel(CardType cardType);
    void InitAddCards(int amount);
    PhotonView GetPv();
    void SelectCards(CardType type, int amount, bool setSelecting = true);
    void ClearPanel(Transform panel);
    PlayerCardsInfo GetDeckInfo();
    bool GetAttackDone();
    void SetAttackDone(bool attackDone);
}

[Serializable]
public class PlayerView : MonoBehaviourPunCallbacks, IPlayerView, IPunObservable {
    [SerializeField] private PhotonView pv;
    [ShowInInspector] public PlayerCardsInfo _deckInfo;
    private PlayerMovement _playerMovement;

    public bool _inAnimation;
    private bool _receivePriority;
    private GameObject _handCardsPanel;
    private bool _myMovementTurn;
    private bool _myEffectTurn;
    private bool _effectTurnDone;
    private bool _movementTurnDone;
    private bool _attackDone;

    public GameObject HandCardsPanel {
        get { return _handCardsPanel ??= GameManager.Instance.handPanel.gameObject; }
    }

    [SerializeField] private GameObject equipmentCardPrefab;
    [SerializeField] private GameObject pilotCardPrefab;
    [SerializeField] private GameObject effectCardPrefab;
    [SerializeField] private GameObject legsCardPrefab;
    [SerializeField] private GameObject armCardPrefab;

    [SerializeField] private TMP_Text playerName;
    [SerializeField] private Image playerDirection;

    private IPlayerController _playerController;
    private IPlayerView _playerViewImplementation;

    public IPlayerController PlayerController {
        get { return _playerController ??= new PlayerController(this); }
    }

    private void Awake() {
        GameManager.Instance.OnGameStartedEvent += TurnOnSprite;
        pv = GetComponent<PhotonView>();
        _playerMovement = GetComponent<PlayerMovement>();
        GameManager.Instance.playerList.Add(this);
        if (pv.IsMine) {
            GameManager.Instance.LocalPlayerInstance = this;
        }

        PlayerController.SetPlayerId(GameManager.Instance.testing ? 0 : pv.Owner.ActorNumber);
    }

    private void Start() {
        if (GameManager.Instance.testing) TurnOnSprite();
        GameManager.Instance.OnPrioritySetEvent += ReceivePriority;
    }

    public void TurnOnSprite() {
        TryGetComponent(out Image image);
        image.enabled = true;
        playerName.gameObject.SetActive(true);
        playerDirection.gameObject.SetActive(true);
        if (!GameManager.Instance.testing) playerName.text = pv.Owner.NickName;

        SetCardsInfo();
    }

    public GameObject GetHandCardsPanel() {
        return HandCardsPanel;
    }

    public void CleanHandsPanel() {
        foreach (Transform t in HandCardsPanel.transform) {
            Destroy(t.gameObject);
        }
    }

    public CardView AddCardToPanel(CardType cardType) {
        GameObject prefab = null;
        Transform parent = null;

        switch (cardType) {
            case CardType.Pilot:
                prefab = pilotCardPrefab;
                break;
            case CardType.Weapon:
            case CardType.Arm:
                prefab = armCardPrefab;
                parent = pv.IsMine
                    ? GameManager.Instance.myEquipmentPanel.transform
                    : GameManager.Instance.enemyEquipmentPanel.transform;
                break;
            case CardType.Armor:
            case CardType.Chest:
            case CardType.Generator:
                prefab = equipmentCardPrefab;
                parent = pv.IsMine
                    ? GameManager.Instance.myEquipmentPanel.transform
                    : GameManager.Instance.enemyEquipmentPanel.transform;
                break;
            case CardType.Legs:
                prefab = legsCardPrefab;
                parent = pv.IsMine
                    ? GameManager.Instance.myEquipmentPanel.transform
                    : GameManager.Instance.enemyEquipmentPanel.transform;
                break;
            case CardType.CampEffect:
            case CardType.Hacking:
                prefab = effectCardPrefab;
                parent = HandCardsPanel.transform;
                break;
            default:
                prefab = pilotCardPrefab;
                Debug.LogError($"Prefab not found");
                break;
        }

        GameObject GO = Instantiate(prefab, parent);

        GO.TryGetComponent(out RectTransform rt);
        rt.sizeDelta = new Vector2(250, 350);
        if (cardType != CardType.Legs && cardType != CardType.Arm && cardType != CardType.Weapon)
            rt.localScale = new Vector3(0.7f, 0.7f, 0.7f);

        GO.TryGetComponent(out CardView card);
        GO.SetActive(pv.IsMine);
        return card;
    }

    public void InitAddCards(int amount) {
        StartCoroutine(PlayerController.AddCards(amount));
    }

    public PhotonView GetPv() {
        return pv;
    }

    public void ClearPanel(Transform panel) {
        foreach (Transform t in panel) {
            Destroy(t.gameObject);
        }
    }

    public PlayerCardsInfo GetDeckInfo() {
        return _deckInfo;
    }

    public bool GetAttackDone() {
        return _attackDone;
    }

    public void SetAttackDone(bool attackDone) {
        _attackDone = attackDone;
    }

    public void SetCardsInfo() {
        if (pv.IsMine) {
            int actorNumber = GameManager.Instance.testing ? 0 : pv.Owner.ActorNumber;
            int count = GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Count;
            int halfCount = count / 2;

            int startIndex = actorNumber == 1 ? 0 : halfCount;

            Debug.Log($" actor number {actorNumber}");
            _deckInfo = Resources.Load<PlayerCardsInfo>($"PlayerCards{actorNumber}");
            _deckInfo.playerCards.Clear();

            if (!GameManager.Instance.testing) {
                // _deckInfo.SetPlayerCards(Enumerable
                //     .Range(startIndex, actorNumber == 1 ? halfCount : count - startIndex)
                //     .ToList());

                // _deckInfo = Resources.Load<PlayerCardsInfo>($"PlayerCards0");
                _deckInfo.SetPlayerCards(new List<int> { 0, 0, 0, 0, 0, 0, 0, 33, 32, 18 });
            }
            else {
                // _deckInfo = Resources.Load<PlayerCardsInfo>($"PlayerCards0");
                _deckInfo.SetPlayerCards(new List<int> { 0, 0, 0, 0, 0, 0, 0, 33, 32, 18 });
            }
        }
    }

    private void OnDestroy() {
        if (GameManager.HasInstance()) {
            GameManager.Instance.OnPrioritySetEvent -= ReceivePriority;
            GameManager.Instance.OnGameStartedEvent -= TurnOnSprite;
        }
    }

    public void DrawCards(int amount, bool fullDraw) {
        PlayerController.DrawCards(amount, fullDraw);
    }

    public void SelectCards(CardType type, int amount, bool setSelecting = true) {
        PlayerController.SelectCards(type, amount, setSelecting);
    }

    public void SelectMovement() {
        PlayerController.SelectMovement();
    }

    public void SelectAttack() {
        PlayerController.SelectAttack();
    }

    public void ReceivePriority(int priority) {
        _receivePriority = true;
    }

    // players communication
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting && pv.IsMine) {
            // selecting cards
            stream.SendNext(PlayerController.GetCardsSelected());
            stream.SendNext(PlayerController.GetPlayerId());

            //priority
            stream.SendNext(GameManager.Instance.currentPriority);

            //effects
            stream.SendNext(EffectManager.Instance.effectTurn);
            stream.SendNext(PlayerController.GetAllEffectsDone());

            //movement
            stream.SendNext(PlayerController.GetMovementSelected());
            stream.SendNext(PlayerController.GetMovementDone());

            //attack
            stream.SendNext(_attackDone);
        }
        else if (stream.IsReading) {
            bool receivedSelection = (bool)stream.ReceiveNext();
            int receivedPlayerId = (int)stream.ReceiveNext();

            int receivedPriority = (int)stream.ReceiveNext();

            int receivedEffectTurn = (int)stream.ReceiveNext();
            bool receivedAllEffectsDone = (bool)stream.ReceiveNext();

            bool receivedMovementSelected = (bool)stream.ReceiveNext();
            bool receivedMovementDone = (bool)stream.ReceiveNext();

            bool receivedAttackDone = (bool)stream.ReceiveNext();

            foreach (PlayerView player in GameManager.Instance.playerList) {
                if (receivedPlayerId == player.pv.Owner.ActorNumber) {
                    player.PlayerController.SetCardsSelected(receivedSelection);
                    player.PlayerController.SetAllEffectsDone(receivedAllEffectsDone);
                    player.PlayerController.SetMovementSelected(receivedMovementSelected);
                    player.PlayerController.SetMovementDone(receivedMovementDone);
                    player.SetAttackDone(receivedAttackDone);
                }
            }

            if (!_myEffectTurn) EffectManager.Instance.effectTurn = receivedEffectTurn;

            if (!PhotonNetwork.IsMasterClient && _receivePriority) {
                GameManager.Instance.currentPriority = receivedPriority;
                _receivePriority = false;
            }
        }
    }

    public void SelectCells(int amount) {
        StartCoroutine(PlayerController.SelectCells(amount));
    }

    public void SetMyEffectTurn(bool myEffectTurn) {
        _myEffectTurn = myEffectTurn;
    }

    public void SetMyMovementTurn(bool myMovementTurn) {
        _myMovementTurn = myMovementTurn;
    }

    public void SetEffectTurnDone(bool effectTurnDone) {
        _effectTurnDone = effectTurnDone;
    }

    public bool GetEffectTurnDone() {
        return _effectTurnDone;
    }

    public void SetMovementTurnDone(bool movementTurnDone) {
        _movementTurnDone = movementTurnDone;
    }

    public bool GetMovementTurnDone() {
        return _movementTurnDone;
    }

    public void DoMove() {
        PlayerController.DoMovement();
    }

    [PunRPC]
    public void RpcSetTurn() {
        GameManager.Instance.movementTurn =
            (GameManager.Instance.movementTurn % GameManager.Instance.playerList.Count) + 1;
    }

    [PunRPC]
    public void RpcSetAttackTurn() {
        GameManager.Instance.attackTurn =
            (GameManager.Instance.attackTurn % GameManager.Instance.playerList.Count) + 1;
    }

    [PunRPC]
    public void RpcReceivedDamage(int damage, int localPlayerId) {
        PlayerController.ReceivedDamage(damage, localPlayerId);
    }

    [PunRPC]
    public void RpcPutMines(int x, int y) {
        GameManager.Instance.boardView.GetBoardStatus()[x][y].CellController.SetType(CellType.Mined);
    }
}