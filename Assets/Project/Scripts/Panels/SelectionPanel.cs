using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour {
    public Image image;
    public GameObject togglePrefab;
    public Transform checkboxPanel;
    [HideInInspector] public List<Toggle> toggleList;

    public int currentId;

    public void Init(int optionsAmount, Sprite imageSource, List<string> optionNames = null)
    {
        foreach (Transform t in checkboxPanel)
        {
            Destroy(t.gameObject);
        }

        toggleList.Clear();

        for (int i = 0; i < optionsAmount; i++)
        {
            toggleList.Add(Instantiate(togglePrefab, checkboxPanel).GetComponent<Toggle>());
        }

        if (optionNames != null)
        {
            for (int i = 0; i < optionNames.Count; i++)
            {
                toggleList[i].GetComponentInChildren<TMP_Text>().text = optionNames[i];
            }
        }

        foreach (Toggle toggle in toggleList)
        {
            toggle.onValueChanged.AddListener((toggleObject) => OnToggleSelected(toggle, toggleObject));
        }

        if (imageSource != null)
        {
            image.sprite = imageSource;
        }
    }

    private void OnToggleSelected(Toggle toggle, bool on)
    {
        if (on == false) return;
        for (int i = 0; i < toggleList.Count; i++)
        {
            if (toggleList[i] == toggle)
            {
                currentId = i;
                toggleList[i].isOn = true;
            }
            else
            {
                toggleList[i].isOn = false;
            }
        }
    }

    public void ConfirmButton()
    {
        foreach (Toggle toggle in toggleList)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

        GameManager.Instance.OnSelectionConfirmed(currentId);
        UIManager.Instance.ShowSelectionPanel(0, null, null, false);
    }
}