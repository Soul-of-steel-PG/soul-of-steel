using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private Button playButton;

    private void Start() {
        GameManager.Instance.OnMasterServerConnected += OnMasterServerConnected;
    }

    public void OnExitButton() => Application.Quit();

    private void OnMasterServerConnected() {
        playButton.interactable = true;
    }

    private void OnDestroy() {
        if (GameManager.HasInstance()) GameManager.Instance.OnMasterServerConnected -= OnMasterServerConnected;
    }
}