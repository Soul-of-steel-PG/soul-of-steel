using System;
using UnityEngine;

public class ScrapPanel : MonoBehaviour {
    public Transform backup;

    private void Start() {
        GameManager.Instance.scrapPanel = this;
    }

    public void SendToBackup() {
        Transform t = transform.GetChild(2);
        t.SetParent(backup);
    }
}