using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public interface IMatchController {
    IEnumerator PrepareMatch();
}

public class MatchController : IMatchController {
    
    private int _matchId;
    private List<string> _matchLog;
    
    private readonly IMatchView _view;
    
    public MatchController(IMatchView view) {
        _view = view;
    }

    private void ThrowPriorityDice() {
        GameManager.Instance.currentPriority = Random.Range(0, 1);
        _view.SetCurrentPhaseText($"Throwing priority dice, result={GameManager.Instance.currentPriority}");
    }

    private void SelectQuadrant() {
        _view.SetCurrentPhaseText("Selecting quadrant");
    }

    public IEnumerator PrepareMatch() {
        ThrowPriorityDice();
        yield return new WaitForSeconds(2);
        SelectQuadrant();
        yield return new WaitForSeconds(2);
        GameManager.Instance.PrepareForMatch(_view);
        _view.SetCurrentPhaseText("shuffling decks");
    }
}
