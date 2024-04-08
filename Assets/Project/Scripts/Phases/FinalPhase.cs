﻿using System.Collections;
using UnityEngine;

public class FinalPhase : Phase {
    public FinalPhase(IMatchView matchView) : base(matchView) {
    }

    public override IEnumerator Start() {
        matchView.SetCurrentPhaseText("final phase");

        yield return new WaitForSeconds(3);

        GameManager.Instance.ChangePhase(new DrawPhase(matchView));

        yield break;
    }
}