using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private List<int> roomIds;

    private const string GAME_SCENE = "Game";
    public int joiningAttempts = 0;

    private List<RoomInfo> _roomList;

    private void Start()
    {
        roomIds = new List<int>();
        _roomList = new List<RoomInfo>();
    }

    // joining

    public void JoinRoom()
    {
        if (PhotonNetwork.CountOfRooms > 0) {
            for (int i = 0; i < 100; i++) {
                foreach (RoomInfo room in _roomList) {
                    if (room.PlayerCount < 2) {
                        PhotonNetwork.JoinRoom(room.Name);
                        return; // Exit the function
                    }
                }
            }
        }

        // If no available room was found, create a new room
        CreateRoom();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        _roomList = roomList;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed joining to a random room");
    }

    // creating

    private void CreateRoom()
    {
        int roomId = _roomList.Count + 1;
        roomIds.Add(roomId);

        Debug.Log($"Creating a new room");
        PhotonNetwork.JoinOrCreateRoom($"Room #{roomId}", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
        PhotonNetwork.LoadLevel(GAME_SCENE);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Failed to create a new room, trying again");

        CreateRoom();
    }
}