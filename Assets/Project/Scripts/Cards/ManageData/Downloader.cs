using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

public class Downloader : MonoBehaviourSingleton<Downloader> {
    private const string GoogleSheetDocId = "15zQYSLFn7ehC5R-8KlKORhvuqxqNjIEXrH08lllOvBA";
    private const string Url = "https://docs.google.com/spreadsheets/d/" + GoogleSheetDocId + "/export?format=xlsx";

    public void LoadInfo(CardsInfo cardsInfo) {
        StartCoroutine(DownloadData(cardsInfo));
    }

    private IEnumerator DownloadData(CardsInfo cardsInfo) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(Url)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError($"Download error: {webRequest.error}");
                string downloadedData = PlayerPrefs.GetString("LastDataDownloaded", null);
                Debug.Log($"Using stale data: {downloadedData}");
                yield break;
            }

            if (webRequest.result != UnityWebRequest.Result.Success) {
                Debug.LogError("Unknown error while downloading data");
                yield break;
            }

            Debug.Log("Download success");

            byte[] downloadedBytes = webRequest.downloadHandler.data;

            // Check if the downloaded data is a ZIP archive
            if (IsZipArchive(downloadedBytes)) {
                // Decompress the ZIP archive
                byte[] decompressedBytes = DecompressZip(downloadedBytes);
                if (decompressedBytes == null) {
                    Debug.LogError("Failed to decompress ZIP archive");
                    yield break;
                }

                // Process the decompressed data
                StartCoroutine(ProcessData(decompressedBytes, cardsInfo));
            }
            else {
                // Process the downloaded data directly
                StartCoroutine(ProcessData(downloadedBytes, cardsInfo));
            }
        }
    }

    private bool IsZipArchive(byte[] data) {
        // Check if the first two bytes of the data match the ZIP file header
        return data.Length > 3 && data[0] == 0x50 && data[1] == 0x4B && data[2] == 0x03 && data[3] == 0x04;
    }

    private byte[] DecompressZip(byte[] data) {
        try {
            using (MemoryStream compressedStream = new MemoryStream(data))
            using (var zipArchive =
                   new System.IO.Compression.ZipArchive(compressedStream, System.IO.Compression.ZipArchiveMode.Read)) {
                // Assuming there's only one entry in the ZIP archive
                var entry = zipArchive.Entries[0];
                using (var entryStream = entry.Open()) {
                    using (MemoryStream decompressedStream = new MemoryStream()) {
                        entryStream.CopyTo(decompressedStream);
                        return decompressedStream.ToArray();
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error decompressing ZIP archive: {e.Message}");
            return null;
        }
    }

    private IEnumerator ProcessData(byte[] data, CardsInfo cardsInfo) {
        if (data == null || data.Length == 0) {
            Debug.LogError("Downloaded data is null or empty, cannot process");
            yield break;
        }

        yield return null; // Process the data asynchronously

        Debug.Log($"Processing data: {data}");
        // Implement your data processing logic here
    }
}