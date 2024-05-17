using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincipalPhase : Phase
{
    private bool _allCardSelected;
    private EquipmentCardView _localEquipmentCard;
    private int _localId;
    private EquipmentCardView _enemyEquipmentCard;
    private int _enemyId;

    public PrincipalPhase(IMatchView matchView) : base(matchView)
    {
        GameManager.Instance.OnCardSelectedEvent += CardSelected;
        GameManager.Instance.OnCardSelectingFinishedEvent += AllCardsSelected;
    }

    public override IEnumerator Start()
    {
        matchView.SetCurrentPhaseText("principal phase");

        List<CardType> cardTypes = new() {
            CardType.Legs,
            CardType.Arm,
            CardType.Armor,
            CardType.Weapon,
            CardType.Chest
        };

        // selecting cards
        GameManager.Instance.PlayerList.ForEach(player => player.SelectCards(cardTypes, 1));

        while (!_allCardSelected) {
            bool localAllSelected = true;
            foreach (PlayerView player in GameManager.Instance.PlayerList) {
                if (!player.PlayerController.GetCardsSelected()) {
                    localAllSelected = false;
                    break;
                }
            }

            _allCardSelected = localAllSelected;

            yield return null;
        }

        GameManager.Instance.PlayerList.ForEach(player => player.SelectCards(cardTypes, 1, false));

        // equipping cards         

        yield return new WaitForSeconds(1);

        SetCardVariables(_localEquipmentCard, true);
        SetCardVariables(_enemyEquipmentCard, false);

        yield return new WaitForSeconds(0.5f);


        foreach (PlayerView playerView in GameManager.Instance.PlayerList) {
            if (playerView.PlayerController.GetPlayerId() ==
                GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId()) {
                playerView.PlayerController.EquipCard(_localId);
            }
            else {
                //playerView.PlayerController.EquipCard(_enemyId);
            }
        }

        GameManager.Instance.LocalPlayerInstance.PlayerController.SetCardsSelected(false);

        GameManager.Instance.ChangePhase(new MovementPhase(matchView));
        GameManager.Instance.OnCardSelectedEvent -= CardSelected;
        GameManager.Instance.OnCardSelectingFinishedEvent -= AllCardsSelected;
    }

    private void CardSelected(PlayerView view, CardView card, bool selected)
    {
        if (view.PlayerController.GetPlayerId() ==
            GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId() || GameManager.Instance.testing) {
            _localId = card.GetId();
            _localEquipmentCard = (EquipmentCardView)card;
        }
        else {
            _enemyId = card.GetId();
            _enemyEquipmentCard = (EquipmentCardView)card;
        }

        // if (_effectCards.Count >= 1)
        GameManager.Instance.OnSelectingFinished();
    }

    private void AllCardsSelected()
    {
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetCardsSelected(_localEquipmentCard != null);
    }

    private void SetCardVariables(CardView card, bool local)
    {
        card?.SetIsSelecting(false);
        card?.Select(true);
        ((EquipmentCardView)card)?.EquipmentCardController.EquipCardAnimation(local);
    }
}