using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonGame : MonoBehaviourPunCallbacks
{
    private Photon.Realtime.Player[] _players;
    private int _playerNumber;
    private GameObject _playerGameObject;

    public PhotonView pv;

    private const string PLAYER_PREFAB_PATH = "PlayerCanvas";
    private const string MAIN_MENU_SCENE = "MainMenu";

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        GameManager.Instance.PhotonGame = this;

        UIManager.Instance.ShowWaitingForOpponentPanel();
    }

    public override void OnJoinedRoom()
    {
        // Debug.Log($"OnJoinedRoom() called by PUN: {PhotonNetwork.CurrentRoom.Name}");

        SpawnPlayer();
        StartGame();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        StartGame();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"A player left the room");
        PhotonNetwork.LoadLevel(MAIN_MENU_SCENE);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // SceneManager.LoadScene(0);
    }

    private void SpawnPlayer()
    {
        _players = PhotonNetwork.PlayerList;
        _playerNumber = _players.Length;

        PhotonNetwork.NickName = GameManager.Instance.LocalPlayerName;

        PhotonNetwork.Instantiate(PLAYER_PREFAB_PATH, transform.position, Quaternion.identity, 0).transform
            .GetChild(0)
            .TryGetComponent(out PlayerView currentPlayer);

        currentPlayer.PlayerController.SetPlayerId(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void StartGame()
    {
        if (PhotonNetwork.PlayerList.Length == 2 || GameManager.Instance.testing) {
            GameManager.Instance.OnGameStarted();

            UIManager.Instance.ShowWaitingForOpponentPanel(false);
            UIManager.Instance.ShowGamePanel();
        }
    }

    public void DisconnectPlayer()
    {
        StartCoroutine(DisconnectAndLoad());
    }

    private IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom) {
            yield return null;
        }

        PhotonNetwork.LoadLevel(MAIN_MENU_SCENE);                                                                                                                                                                                                                                                                               
    }
}