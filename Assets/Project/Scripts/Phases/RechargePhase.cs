using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class RechargePhase : Phase {
    private bool _allCardSelected;
    private bool _allEffectsDone;
    private List<CardView> _effectCards;

    public RechargePhase(IMatchView matchView) : base(matchView) {
        GameManager.Instance.OnCardSelectedEvent += CardSelected;
        GameManager.Instance.OnCardSelectingFinishedEvent += AllCardsSelected;
        EffectManager.Instance.OnAllEffectsFinishedEvent += SetEffectTurn;

        _effectCards = new List<CardView>();
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("recharge phase");

        // selecting cards
        GameManager.Instance.playerList.ForEach(player => player.SelectCards(CardType.CampEffect, 1));

        while (!_allCardSelected) {
            bool localAllSelected = true;
            foreach (PlayerView player in GameManager.Instance.playerList) {
                if (!player.PlayerController.GetCardsSelected()) {
                    localAllSelected = false;
                    break;
                }
            }

            _allCardSelected = localAllSelected;

            yield return null;
        }

        GameManager.Instance.playerList.ForEach(player => player.SelectCards(CardType.CampEffect, 1, false));

        // doing cards effects        
        EffectManager.Instance.effectTurn = GameManager.Instance.currentPriority;

        foreach (PlayerView p in GameManager.Instance.playerList) {
            p.PlayerController.SetAllEffectsDone(false);
            p.SetEffectTurnDone(false);
        }

        while (!_allEffectsDone) {
            if (GameManager.Instance.LocalPlayerInstance.GetEffectTurnDone() ||
                GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId() !=
                EffectManager.Instance.effectTurn) {
                bool localAllEffectsDone = true;
                foreach (PlayerView p in GameManager.Instance.playerList) {
                    if (!p.PlayerController.GetAllEffectsDone()) {
                        localAllEffectsDone = false;
                    }
                }

                _allEffectsDone = localAllEffectsDone;
                if (_allEffectsDone) GameManager.Instance.LocalPlayerInstance.PlayerController.SetAllEffectsDone(false);

                yield return null;
            }
            else {
                PlayerView player = GameManager.Instance.playerList.Find(p =>
                    p.PlayerController.GetPlayerId() == EffectManager.Instance.effectTurn);

                player.SetMyEffectTurn(true);

                foreach (CardView card in _effectCards) {
                    card.DoEffect(GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId());

                    do {
                        yield return null;
                    } while (player.PlayerController.GetDoingEffect());
                }

                foreach (CardView card in _effectCards) {
                    card.SetIsSelecting(false);
                    card.Select(true);
                    card.Dismiss();
                }

                player.SetMyEffectTurn(false);
                player.SetEffectTurnDone(true);
            }

            if (GameManager.Instance.LocalPlayerInstance.GetEffectTurnDone()) {
                GameManager.Instance.LocalPlayerInstance.PlayerController.SetAllEffectsDone(true);
            }
        }

        GameManager.Instance.LocalPlayerInstance.PlayerController.SetCardsSelected(false);

        _effectCards.Clear();
        GameManager.Instance.ChangePhase(new PrincipalPhase(matchView));
        GameManager.Instance.OnCardSelectedEvent -= CardSelected;
        GameManager.Instance.OnCardSelectingFinishedEvent -= AllCardsSelected;
        EffectManager.Instance.OnAllEffectsFinishedEvent -= SetEffectTurn;
    }

    private void CardSelected(CardView card, bool selected) {
        if (selected) _effectCards.Add(card);
        else _effectCards.Remove(card);

        // if (_effectCards.Count >= 1)
        GameManager.Instance.OnSelectingFinished();
    }

    private void AllCardsSelected() {
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetCardsSelected(_effectCards.Count >= 1);
    }

    public override IEnumerator End() {
        yield break;
    }

    private void SetEffectTurn() {
        EffectManager.Instance.effectTurn =
            (EffectManager.Instance.effectTurn % GameManager.Instance.playerList.Count) + 1;
    }
}