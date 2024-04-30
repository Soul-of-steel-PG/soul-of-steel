using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BattlePhase : Phase {
    private bool _allAttacksDone;

    public BattlePhase(IMatchView matchView) : base(matchView) {
        GameManager.Instance.OnAllAttacksSelectedEvent += BattleFinished;
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("battle phase");

        yield return new WaitForSeconds(1);

        GameManager.Instance.attackTurn = GameManager.Instance.currentPriority;

        foreach (PlayerView p in GameManager.Instance.playerList) {
            p.SetAttackDone(false);
        }

        while (!_allAttacksDone) {
            if (GameManager.Instance.LocalPlayerInstance.GetAttackDone() ||
                GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId() !=
                GameManager.Instance.attackTurn) {
                bool localAttackDoneSelected = true;
                foreach (PlayerView player in GameManager.Instance.playerList) {
                    if (!player.GetAttackDone()) {
                        localAttackDoneSelected = false;
                        break;
                    }
                }

                _allAttacksDone = localAttackDoneSelected;

                yield return null;
            }
            else {
                GameManager.Instance.LocalPlayerInstance.SelectAttack();

                while (!GameManager.Instance.LocalPlayerInstance.GetAttackDone()) {
                    yield return null;
                }

                GameManager.Instance.ValidateHealthStatus();

                if (!GameManager.Instance.testing)
                    GameManager.Instance.LocalPlayerInstance.photonView.RPC("RpcSetAttackTurn", RpcTarget.AllBuffered);
            }
        }

        GameManager.Instance.OnAllAttackSelected();
    }

    public void BattleFinished() {
        GameManager.Instance.ChangePhase(new FinalPhase(matchView));
        GameManager.Instance.OnAllAttacksSelectedEvent -= BattleFinished;
    }
}