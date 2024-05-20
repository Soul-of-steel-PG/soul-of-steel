using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ChangePriorityPhase : Phase {
    public ChangePriorityPhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        if (GameManager.Instance.PlayerList.Count == 0) yield break;

        if (!GameManager.Instance.isFirstRound) {
            if (PhotonNetwork.IsMasterClient) {
                GameManager.Instance.CurrentPriority =
                    (GameManager.Instance.CurrentPriority % GameManager.Instance.PlayerList.Count) + 1;
                // Debug.Log($"master priority {GameManager.Instance.CurrentPriority}");
            }

            GameManager.Instance.OnPrioritySet(GameManager.Instance.CurrentPriority);

            matchView.SetCurrentPhaseText("Changing priority phase");
        }

        yield return new WaitForSeconds(2);
        GameManager.Instance.ChangePhase(new RechargePhase(matchView));
    }
}