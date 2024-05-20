using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

public class RechargePhase : Phase
{
    private bool _allCardSelected;
    private bool _allEffectsDone;
    private List<CardView> _effectCards;

    public RechargePhase(IMatchView matchView) : base(matchView)
    {
        GameManager.Instance.OnCardSelectedEvent += CardSelected;
        GameManager.Instance.OnCardSelectingFinishedEvent += AllCardsSelected;
        EffectManager.Instance.OnAllEffectsFinishedEvent += SetEffectTurn;

        _effectCards = new List<CardView>();
    }

    public override IEnumerator Start()
    {
        matchView.SetCurrentPhaseText("recharge phase");

        List<CardType> cardTypes = new() {
            CardType.CampEffect,
            CardType.Hacking
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

        // doing cards effects        
        EffectManager.Instance.effectTurn = GameManager.Instance.CurrentPriority;

        foreach (PlayerView p in GameManager.Instance.PlayerList) {
            p.PlayerController.SetAllEffectsDone(false);
            p.SetEffectTurnDone(false);
        }

        while (!_allEffectsDone) {
            if (GameManager.Instance.LocalPlayerInstance.GetEffectTurnDone() ||
                GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId() !=
                EffectManager.Instance.effectTurn) {
                bool localAllEffectsDone = true;
                foreach (PlayerView p in GameManager.Instance.PlayerList) {
                    if (!p.PlayerController.GetAllEffectsDone()) {
                        localAllEffectsDone = false;
                    }
                }

                _allEffectsDone = localAllEffectsDone;
                if (_allEffectsDone) GameManager.Instance.LocalPlayerInstance.PlayerController.SetAllEffectsDone(false);

                yield return null;
            }
            else {
                PlayerView player = GameManager.Instance.PlayerList.Find(p =>
                    p.PlayerController.GetPlayerId() == EffectManager.Instance.effectTurn) as PlayerView;

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
        EffectManager.Instance.IncrementRadarSabotageRoundsCount();
        GameManager.Instance.OnCardSelectedEvent -= CardSelected;
        GameManager.Instance.OnCardSelectingFinishedEvent -= AllCardsSelected;
        EffectManager.Instance.OnAllEffectsFinishedEvent -= SetEffectTurn;
    }

    private void CardSelected(PlayerView playerView, CardView card, bool selected)
    {
        if (selected) _effectCards.Add(card);
        else _effectCards.Remove(card);

        // if (_effectCards.Count >= 1)
        GameManager.Instance.OnSelectingFinished();
    }

    private void AllCardsSelected()
    {
        GameManager.Instance.LocalPlayerInstance.PlayerController.SetCardsSelected(_effectCards.Count >= 1);
    }

    private void SetEffectTurn()
    {
        EffectManager.Instance.effectTurn =
            (EffectManager.Instance.effectTurn % GameManager.Instance.PlayerList.Count) + 1;
    }
}