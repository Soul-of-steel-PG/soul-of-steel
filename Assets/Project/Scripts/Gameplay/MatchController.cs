using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Random = UnityEngine.Random;

public interface IMatchController
{
    IEnumerator PrepareMatch();
}

public class MatchController : IMatchController
{
    private int _matchId;
    private List<string> _matchLog;

    private readonly IMatchView _view;
    private readonly IGameManager _gameManager;

    public MatchController(
        IMatchView view,
        IGameManager gameManager)
    {
        _view = view;
        _gameManager = gameManager;
    }

    private void SetPriority() {
        const int currentPriority = 1;
        _gameManager.SetCurrentPriority(currentPriority);
        _view.SetCurrentPhaseText($"priority = {currentPriority}");
    }

    private void SelectQuadrant() {
        if(_gameManager?.PlayerList == null) return;
        
        _view.SetCurrentPhaseText("Selecting quadrant");

        if(_gameManager.PlayerList.Count == 0) return;
        
        const int upDegrees = 90;
        const int downDegrees = 270;
        foreach (IPlayerView player in _gameManager.PlayerList) {
            Vector2 nextCell = player.PlayerController.GetPlayerId() == 1
                ? Vector2.zero
                : (_gameManager.BoardView.BoardController.GetBoardCount() - 1) * Vector2.one;

            int currentDegrees = player.PlayerController.GetPlayerId() == 1 ? downDegrees : upDegrees;

            player.PlayerController.SetCurrentCell(nextCell);
            player.PlayerController.SetCurrentDegrees(currentDegrees);
            player.MoveToCell(nextCell);
            player.Rotate(currentDegrees);
        }
    }

    public IEnumerator PrepareMatch()
    {
        yield return new WaitForSeconds(2);
        SetPriority();
        yield return new WaitForSeconds(2);
        SelectQuadrant();
        yield return new WaitForSeconds(2);
        GameManager.Instance.PrepareForMatch(_view);
    }

    #region Debug
    #if UNITY_EDITOR
    public void Debug_SetPriority()
    {
        SetPriority();
    }
    
    public void Debug_SelectQuadrant()
    {
        SelectQuadrant();
    }
    #endif
    #endregion
}