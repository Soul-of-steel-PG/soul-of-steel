using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;

public class PhotonGame : MonoBehaviourPunCallbacks {
    private Player[] _players;
    private int _playerNumber;
    private GameObject _playerGameObject;

    public PhotonView pv;

    private const string PLAYER_PREFAB_PATH = "Player";
    private const string MAIN_MENU_SCENE = "MainMenu";

    private void Start() {
        pv = GetComponent<PhotonView>();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        /*_players = PhotonNetwork.PlayerList;
        _playerNumber = _players.Length;

        Debug.Log($"Player number {_playerNumber} joined, enteredroom");

        PhotonNetwork.NickName = _playerNumber.ToString();

        _playerGameObject = PhotonNetwork.Instantiate("Player", transform.position, Quaternion.identity, 0);*/
    }

    public override void OnJoinedRoom() {
        SpawnPlayer();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log($"A player left de room");
        PhotonNetwork.LoadLevel(MAIN_MENU_SCENE);
        PhotonNetwork.LeaveRoom();
    }

    private void SpawnPlayer() {
        _players = PhotonNetwork.PlayerList;
        _playerNumber = _players.Length;

        Debug.Log($"Player number {_playerNumber} joined");

        PhotonNetwork.NickName = _playerNumber.ToString();

        _playerGameObject = PhotonNetwork.Instantiate(PLAYER_PREFAB_PATH, transform.position, Quaternion.identity, 0);
    }
}