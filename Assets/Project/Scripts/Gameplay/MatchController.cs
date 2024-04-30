using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Random = UnityEngine.Random;

public interface IMatchController {
    IEnumerator PrepareMatch();
}

public class MatchController : IMatchController {
    private int _matchId;
    private List<string> _matchLog;

    private readonly IMatchView _view;

    public MatchController(IMatchView view) {
        _view = view;
    }

    private void ThrowPriorityDice() {
        if (PhotonNetwork.IsMasterClient) {
            GameManager.Instance.currentPriority = Random.Range(1, 3);
            // Debug.Log($"master priority {GameManager.Instance.currentPriority}");
        }

        GameManager.Instance.OnPrioritySet(GameManager.Instance.currentPriority);

        _view.SetCurrentPhaseText($"Throwing priority dice, result = {GameManager.Instance.currentPriority}");
    }

    private void SelectQuadrant() {
        _view.SetCurrentPhaseText("Selecting quadrant");

        foreach (PlayerView p in GameManager.Instance.playerList) {
            Vector2 nextCell = p.PlayerController.GetPlayerId() == 1
                ? Vector2.zero
                : new Vector2(GameManager.Instance.boardView.BoardController.GetBoardCount() - 1,
                    GameManager.Instance.boardView.BoardController.GetBoardCount() - 1);

            int currentDegrees = p.PlayerController.GetPlayerId() == 1 ? 270 : 90;

            p.PlayerController.SetCurrentCell(nextCell);
            p.PlayerController.SetCurrentDegrees(currentDegrees);
            p.GetComponent<PlayerMovement>().MoveToCell(nextCell, currentDegrees);
        }
    }

    public IEnumerator PrepareMatch() {
        yield return new WaitForSeconds(2);
        ThrowPriorityDice();
        yield return new WaitForSeconds(2);
        SelectQuadrant();
        yield return new WaitForSeconds(2);
        GameManager.Instance.PrepareForMatch(_view);
        _view.SetCurrentPhaseText("shuffling decks");
    }
}