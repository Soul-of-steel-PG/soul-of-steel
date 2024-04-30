using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public PhotonView pv;
    public Movement currentMovement;

    private void Start() {
        pv = GetComponent<PhotonView>();
        GameManager.Instance.OnMovementSelectedEvent += DoMove;
    }

    private void DoMove(Movement movement, PlayerView player) {
        if (GameManager.Instance.LocalPlayerInstance.PlayerController.GetMoving()) return;
        player.PlayerController.SetMoving(true);
        currentMovement = movement;
        StartCoroutine(StartMoving(movement, player));
    }

    private IEnumerator StartMoving(Movement movement, PlayerView player) {
        int currentDegrees = player.PlayerController.GetCurrentDegrees();
        Vector2 currentCell = player.PlayerController.GetCurrentCell();

        List<Movement.MovementInfo> steps = movement.steps;
        int nextDegrees = movement.degrees[0];

        foreach (Movement.MovementInfo step in steps) {
            int currentSteps = step.Steps;
            Vector2 nextCell = currentCell;

            while (currentSteps > 0) {
                yield return new WaitForSeconds(0.5f);

                int adjustedDirection;

                switch (step.Direction) {
                    case "↑":
                        adjustedDirection = currentDegrees;
                        break;
                    case "→":
                        adjustedDirection = (currentDegrees - 90) % 360;
                        break;
                    case "←":
                        adjustedDirection = (currentDegrees + 90) % 360;
                        break;
                    default:
                        Debug.Log($"not valid direction");
                        yield break;
                }

                switch (adjustedDirection) {
                    case 180:
                        nextCell.x -= 1;
                        break;
                    case 0:
                        nextCell.x += 1;
                        break;
                    case 90:
                        nextCell.y -= 1;
                        break;
                    case 270:
                        nextCell.y += 1;
                        break;
                }

                nextCell = new Vector2(
                    Mathf.Clamp(nextCell.x, 0, GameManager.Instance.boardView.BoardController.GetBoardCount() - 1),
                    Mathf.Clamp(nextCell.y, 0, GameManager.Instance.boardView.BoardController.GetBoardCount() - 1));

                MoveToCell(nextCell, adjustedDirection);
                currentSteps--;
                yield return null;
            }

            player.PlayerController.SetCurrentCell(nextCell);
            player.PlayerController.SetCurrentDegrees(nextDegrees);
            player.transform.rotation = Quaternion.Euler(0, 0, nextDegrees);
            player.PlayerController.SetMoving(false);
            GameManager.Instance.OnMovementTurnDone();

            if (GameManager.Instance.boardView.GetBoardStatus()[(int)nextCell.y][(int)nextCell.x].CellController
                    .GetCellType() == CellType.Blocked) {
                if (!GameManager.Instance.testing) {
                    player.photonView.RPC("RpcReceivedDamage", RpcTarget.AllBuffered, 2,
                        player.PlayerController.GetPlayerId());
                }
                else {
                    player.PlayerController.ReceivedDamage(3, player.PlayerController.GetPlayerId());
                }

                nextCell = new Vector2(nextCell.x + 1, nextCell.y);
                MoveToCell(nextCell);
                player.PlayerController.SetCurrentCell(nextCell);
            }
        }
    }

    public void MoveToCell(Vector2 index, int adjustedDirection = -1) {
        if (pv.IsMine) {
            transform.position = GameManager.Instance.boardView.GetCellPos(index);
            // if (adjustedDirection != -1) transform.rotation = Quaternion.Euler(0, 0, adjustedDirection);

            CellView currentCell = GameManager.Instance.boardView.GetBoardStatus()[(int)index.y][(int)index.x];

            if (currentCell.CellController.GetCellType() == CellType.Mined) {
                PlayerView currentPlayer = GetComponent<PlayerView>();
                if (!GameManager.Instance.testing) {
                    currentPlayer.photonView.RPC("RpcReceivedDamage", RpcTarget.AllBuffered, 3,
                        currentPlayer.PlayerController.GetPlayerId());
                }
                else {
                    currentPlayer.PlayerController.ReceivedDamage(3, currentPlayer.PlayerController.GetPlayerId());
                }

                currentCell.CellController.SetType(CellType.Normal);
            }
        }
    }

    private void OnDestroy() {
        if (GameManager.HasInstance()) GameManager.Instance.OnMovementSelectedEvent -= DoMove;
    }
}