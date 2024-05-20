using System.Collections;
using UnityEngine;

public class DrawPhase : Phase {
    public DrawPhase(IMatchView matchView) : base(matchView)
    {
        GameManager.Instance.OnDrawFinishedEvent += FinishDraw;
    }

    public override IEnumerator Start()
    {
        GameManager.Instance.PlayerList.ForEach(player => player.DrawCards(5, true));

        matchView.SetCurrentPhaseText("drawing cards");
        yield break;
    }

    public void FinishDraw()
    {
        if (GameManager.HasInstance()) GameManager.Instance.OnDrawFinishedEvent -= FinishDraw;
        GameManager.Instance.ChangePhase(new ChangePriorityPhase(matchView));
    }
}