using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ServerManager : MonoBehaviourPunCallbacks {
    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start() {
        if (PhotonNetwork.IsConnected) return;
        
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log($"It is going to connect to the master server");
    }

    public override void OnConnectedToMaster() {
        Debug.Log($"It has been connected to the master server");
        GameManager.Instance.OnConnectedToServer();
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log($"Disconnected from the server");
    }
}