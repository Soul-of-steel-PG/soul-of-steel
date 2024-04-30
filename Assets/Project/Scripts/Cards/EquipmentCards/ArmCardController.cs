using System;
using System.Collections.Generic;
using UnityEngine;

public interface IArmCardController : IEquipmentCardController {
    void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        int damage, AttackType attackType, int attackDistance, int attackArea, Sprite imageSource, CardType type);

    void SelectAttack();
    int GetDamage();
}

public class ArmCardController : EquipmentCardController, IArmCardController {
    private readonly IArmCardView _view;

    private int _damage;
    private AttackType _attackType;
    private int _attackDistance;
    private int _attackArea;

    private List<Vector2> currentCellsShaded;

    public ArmCardController(IArmCardView view) : base(view) {
        _view = view;
    }

    public void InitCard(int id, string cardName, string cardDescription, int scrapCost, int scrapRecovery,
        int damage, AttackType attackType, int attackDistance, int attackArea, Sprite imageSource, CardType type) {
        _damage = damage;
        _attackType = attackType;
        _attackDistance = attackDistance;
        _attackArea = attackArea;

        PlayerView currentPlayer = GameManager.Instance.LocalPlayerInstance;

        if (Type == CardType.Arm) {
            PilotCardView currentPilotCard = currentPlayer.PlayerController.GetPilotCard();
            _damage = currentPilotCard.PilotCardController.GetDefaultDamage();
            _attackDistance = 1;
            _attackArea = 1;
            _attackType = AttackType.StraightLine;
        }

        base.InitCard(id, cardName, cardDescription, scrapCost, scrapRecovery, imageSource, type);
    }

    public void SelectAttack() {
        currentCellsShaded = new List<Vector2>();
        currentCellsShaded.Clear();
        PlayerView currentPlayer = GameManager.Instance.LocalPlayerInstance;
        BoardView currentBoardView = GameManager.Instance.boardView;

        int direction = currentPlayer.PlayerController.GetCurrentDegrees();
        Vector2 cellToSelect = currentPlayer.PlayerController.GetCurrentCell();

        switch (_attackType) {
            case AttackType.StraightLine:
                for (int i = 0; i < _attackDistance; i++) {
                    switch (direction) {
                        case 180:
                            cellToSelect.x -= 1;
                            break;
                        case 0:
                            cellToSelect.x += 1;
                            break;
                        case 90:
                            cellToSelect.y -= 1;
                            break;
                        case 270:
                            cellToSelect.y += 1;
                            break;
                    }

                    Vector2 index = new(
                        Mathf.Clamp(cellToSelect.x, 0, currentBoardView.BoardController.GetBoardCount() - 1),
                        Mathf.Clamp(cellToSelect.y, 0, currentBoardView.BoardController.GetBoardCount() - 1));

                    currentCellsShaded.Add(index);
                    GameManager.Instance.boardView.SetBoardStatusCellType(index, CellType.Shady);
                }

                GameManager.Instance.OnCellClickedEvent += currentPlayer.PlayerController.DoAttack;

                break;
            default:
                Debug.Log($"type not implemented {_attackType} card name: {CardName}");
                break;
        }

        GameManager.Instance.OnLocalAttackDoneEvent += UnShadeCells;
    }

    public int GetDamage() {
        return _damage;
    }

    public void UnShadeCells() {
        foreach (Vector2 cellIndex in currentCellsShaded) {
            GameManager.Instance.boardView.SetBoardStatusCellType(cellIndex, CellType.Normal);
        }

        GameManager.Instance.OnLocalAttackDoneEvent -= UnShadeCells;
    }
}