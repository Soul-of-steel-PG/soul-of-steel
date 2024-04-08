using System.Collections;
using UnityEngine;

public class MovementPhase : Phase {
    public MovementPhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("movement phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new BattlePhase(matchView));

        yield break;
    }
}
