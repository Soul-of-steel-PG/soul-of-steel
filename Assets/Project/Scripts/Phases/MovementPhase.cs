using System.Collections;
using Photon.Pun;
using UnityEngine;

public class MovementPhase : Phase {
    private bool _allMovementsSelected;
    private bool _allMovementDone;

    public MovementPhase(IMatchView matchView) : base(matchView) {
        GameManager.Instance.OnMovementFinishedEvent += MovementFinished;
        GameManager.Instance.OnMovementTurnDoneEvent += SetTurn;
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("movement phase");

        foreach (PlayerView p in GameManager.Instance.playerList) {
            p.PlayerController.SetMovementDone(false);
            p.SetMovementTurnDone(false);
            p.SetMyMovementTurn(false);
            p.PlayerController.SetMovementSelected(false);
        }

        GameManager.Instance.playerList.ForEach(p => p.SelectMovement());

        while (!_allMovementsSelected) {
            bool localAllMovementSelected = true;
            foreach (PlayerView player in GameManager.Instance.playerList) {
                if (!player.PlayerController.GetMovementSelected()) {
                    localAllMovementSelected = false;
                    break;
                }
            }

            _allMovementsSelected = localAllMovementSelected;

            yield return null;
        }

        GameManager.Instance.OnAllMovementSelected();
        GameManager.Instance.movementTurn = GameManager.Instance.currentPriority;


        while (!_allMovementDone) {
            if (GameManager.Instance.LocalPlayerInstance.GetMovementTurnDone() ||
                GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId() !=
                GameManager.Instance.movementTurn) {
                bool localAllMovementDone = true;
                foreach (PlayerView p in GameManager.Instance.playerList) {
                    if (!p.PlayerController.GetMovementDone()) {
                        localAllMovementDone = false;
                    }
                }

                _allMovementDone = localAllMovementDone;
                if (_allMovementDone)
                    GameManager.Instance.LocalPlayerInstance.PlayerController.SetMovementDone(false);

                yield return null;
            }
            else {
                PlayerView player = GameManager.Instance.playerList.Find(p =>
                    p.PlayerController.GetPlayerId() == GameManager.Instance.movementTurn);

                player.SetMyMovementTurn(true);

                player.DoMove();

                do {
                    yield return null;
                } while (player.PlayerController.GetMoving());

                if (!GameManager.Instance.testing) player.photonView.RPC("RpcSetTurn", RpcTarget.AllBuffered);
                player.SetMovementTurnDone(true);
            }

            if (GameManager.Instance.LocalPlayerInstance.GetMovementTurnDone()) {
                GameManager.Instance.LocalPlayerInstance.PlayerController.SetMovementDone(true);
            }
        }

        GameManager.Instance.OnMovementFinished();
        GameManager.Instance.OnMovementTurnDoneEvent -= SetTurn;
    }

    public void MovementFinished() {
        GameManager.Instance.ChangePhase(new BattlePhase(matchView));
        GameManager.Instance.OnMovementFinishedEvent -= MovementFinished;
    }

    public void SetTurn() {
        GameManager.Instance.movementTurn =
            (GameManager.Instance.movementTurn % GameManager.Instance.playerList.Count) + 1;
    }
}