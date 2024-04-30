using System.Collections;
using UnityEngine;

public class DrawPhase : Phase {
    public DrawPhase(IMatchView matchView) : base(matchView) {
        GameManager.Instance.OnDrawFinishedEvent += FinishDraw;
    }

    public override IEnumerator Start() {
        GameManager.Instance.playerList.ForEach(player => player.DrawCards(5, true));

        matchView.SetCurrentPhaseText("drawing cards");
        yield break;
    }

    public void FinishDraw() {
        GameManager.Instance.ChangePhase(new ChangePriorityPhase(matchView));

        if (GameManager.HasInstance()) GameManager.Instance.OnDrawFinishedEvent -= FinishDraw;
    }

    public override IEnumerator End() {
        yield break;
    }
}