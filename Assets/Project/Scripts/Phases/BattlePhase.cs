using System.Collections;
using UnityEngine;

public class BattlePhase : Phase {
    public BattlePhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("battle phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new FinalPhase(matchView));

        yield break;
    }
}
