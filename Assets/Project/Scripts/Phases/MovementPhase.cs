using System.Collections;
using Photon.Pun;
using UnityEngine;

public class MovementPhase : Phase {
    private bool _allMovementsSelected;
    private bool _allMovementDone;

    public MovementPhase(IMatchView matchView) : base(matchView)
    {
        GameManager.Instance.OnMovementFinishedEvent += MovementFinished;
        GameManager.Instance.OnMovementTurnDoneEvent += SetTurn;
    }

    public override IEnumerator Start()
    {
        matchView.SetCurrentPhaseText("movement phase");
        _allMovementDone = false;
        _allMovementsSelected = false;

        foreach (PlayerView p in GameManager.Instance.PlayerList)
        {
            p.PlayerController.SetMovementDone(false);
            p.SetMovementTurnDone(false);
            p.SetMyMovementTurn(false);
            p.PlayerController.SetMovementSelected(false);
        }

        GameManager.Instance.PlayerList.ForEach(p => p.SelectMovement());

        while (!_allMovementsSelected)
        {
            bool localAllMovementSelected = true;
            foreach (PlayerView player in GameManager.Instance.PlayerList)
            {
                if (!player.PlayerController.GetMovementSelected())
                {
                    localAllMovementSelected = false;
                    break;
                }
            }

            _allMovementsSelected = localAllMovementSelected;

            yield return null;
        }

        GameManager.Instance.OnAllMovementSelected();
        GameManager.Instance.movementTurn = GameManager.Instance.CurrentPriority;


        while (!_allMovementDone)
        {
            if (GameManager.Instance.LocalPlayerInstance.GetMovementTurnDone() ||
                GameManager.Instance.LocalPlayerInstance.PlayerController.GetPlayerId() !=
                GameManager.Instance.movementTurn)
            {
                bool localAllMovementDone = true;
                foreach (PlayerView p in GameManager.Instance.PlayerList)
                {
                    if (!p.PlayerController.GetMovementDone())
                    {
                        localAllMovementDone = false;
                    }
                }

                _allMovementDone = localAllMovementDone;
                if (_allMovementDone)
                    GameManager.Instance.LocalPlayerInstance.PlayerController.SetMovementDone(false);

                yield return null;
            }
            else
            {
                PlayerView player = GameManager.Instance.PlayerList.Find(p =>
                    p.PlayerController.GetPlayerId() == GameManager.Instance.movementTurn) as PlayerView;

                player.SetMyMovementTurn(true);

                player.DoMove();

                do
                {
                    yield return null;
                } while (player.PlayerController.GetMoving());

                if (!GameManager.Instance.testing) player.photonView.RPC("RpcSetTurn", RpcTarget.AllBuffered);
                player.SetMovementTurnDone(true);
            }

            if (GameManager.Instance.LocalPlayerInstance.GetMovementTurnDone())
            {
                GameManager.Instance.LocalPlayerInstance.PlayerController.SetMovementDone(true);
            }
        }

        GameManager.Instance.OnMovementFinished();
        GameManager.Instance.OnMovementTurnDoneEvent -= SetTurn;
    }

    public void MovementFinished()
    {
        GameManager.Instance.OnMovementFinishedEvent -= MovementFinished;
        GameManager.Instance.ChangePhase(new BattlePhase(matchView));
    }

    public void SetTurn()
    {
        GameManager.Instance.movementTurn =
            (GameManager.Instance.movementTurn % GameManager.Instance.PlayerList.Count) + 1;
    }
}