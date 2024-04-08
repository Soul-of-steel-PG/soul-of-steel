using System.Collections;
using UnityEngine;

public class ChangePriorityPhase : Phase {
    public ChangePriorityPhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        if (GameManager.Instance.playerList.Count == 0) yield break;

        GameManager.Instance.currentPriority =
            (GameManager.Instance.currentPriority + 1) % GameManager.Instance.playerList.Count;

        matchView.SetCurrentPhaseText("Changing priority phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new RechargePhase(matchView));
    }
}