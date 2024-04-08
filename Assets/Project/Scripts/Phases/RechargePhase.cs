using System.Collections;
using UnityEngine;

public class RechargePhase : Phase {
    public RechargePhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("recharge phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new PrincipalPhase(matchView));

        yield break;
    }
}
