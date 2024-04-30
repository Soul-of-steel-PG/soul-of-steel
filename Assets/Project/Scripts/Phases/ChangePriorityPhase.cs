using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ChangePriorityPhase : Phase {
    public ChangePriorityPhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        if (GameManager.Instance.playerList.Count == 0) yield break;

        if (PhotonNetwork.IsMasterClient) {
            GameManager.Instance.currentPriority =
                (GameManager.Instance.currentPriority % GameManager.Instance.playerList.Count) + 1;
            // Debug.Log($"master priority {GameManager.Instance.currentPriority}");
        }

        GameManager.Instance.OnPrioritySet(GameManager.Instance.currentPriority);

        matchView.SetCurrentPhaseText("Changing priority phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new RechargePhase(matchView));
    }
}