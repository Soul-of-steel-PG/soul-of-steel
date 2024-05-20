using System;
using UnityEngine;

public interface IScrapPanel {
    Transform GetTransform();
    void SendToBackup();
}

public class ScrapPanel : MonoBehaviour, IScrapPanel {
    public Transform backup;

    private void Start()
    {
        GameManager.Instance.ScrapPanel = this;
    }

    public void SendToBackup()
    {
        Transform t = transform.GetChild(2);
        t.SetParent(backup);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}