using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class Downloader : MonoBehaviourSingleton<Downloader> {
    private const string GoogleJsonId = "1_Ta--WuJJ-xkT9oDxIOj4k-yfWRjk9Wu";
    private const string Url = "https://drive.google.com/uc?export=download&id=" + GoogleJsonId;

    public CardsDataBase data;

    [Button]
    public void LoadInfo(CardsDataBase playerCardsInfo) {
        if (playerCardsInfo == null) playerCardsInfo = data;
        StartCoroutine(DownloadData(playerCardsInfo));
    }

    private IEnumerator DownloadData(CardsDataBase playerCardsInfo) {
        UnityWebRequest request = UnityWebRequest.Get(Url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success) {
            Debug.LogError($"Error fetching data: {request.error}");
        }
        else {
            // Get the JSON response as a string
            string jsonResponse = request.downloadHandler.text;
            jsonResponse = jsonResponse.Replace("\n", "");

            StartCoroutine(ProcessData(jsonResponse, playerCardsInfo));
        }
    }


    private IEnumerator ProcessData(string data, CardsDataBase playerCardsInfo) {
        if (string.IsNullOrEmpty(data)) {
            Debug.LogError("Downloaded data is null or empty, cannot process");
            yield break;
        }

        yield return null; // Process the data asynchronously

        Debug.Log($"Processing data: {data}");
        // Implement your data processing logic here

        yield return playerCardsInfo.cardDataBase = JsonUtility.FromJson<CardInfoSerialized>(data);

        foreach (CardInfoSerialized.CardInfoStruct card in playerCardsInfo.cardDataBase.Sheet1) {
            card.SetType();
            card.SetMovements();
            card.SetAttackType();
        }
    }
}