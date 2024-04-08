using System.Collections;
using UnityEngine;

public class PrincipalPhase : Phase {
    public PrincipalPhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("principal phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new MovementPhase(matchView));

        yield break;
    }
}
