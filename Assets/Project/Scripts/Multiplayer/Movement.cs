using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviour {
    public PhotonView pv;

    private BoardView _board;

    private void Start() {
        pv = GetComponent<PhotonView>();
        GameManager.Instance.OnCellClickedEvent += OnMovement;
    }

    private void Update() {
    }

    private void OnMovement(Vector2 index) {
        if (pv.IsMine) {
            transform.position = GameManager.Instance.boardView.GetCellPos(index);
        }
    }

    private void OnDestroy() {
        if (GameManager.HasInstance()) GameManager.Instance.OnCellClickedEvent -= OnMovement;
    }
}
