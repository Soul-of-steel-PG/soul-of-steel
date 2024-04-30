using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanel : MonoBehaviour {
    [SerializeField] private bool isMiddle;
    public Transform animationReference;

    void Start() {
        if (isMiddle) GameManager.Instance.middlePanel = this;
        else GameManager.Instance.handPanel = this;
    }
}