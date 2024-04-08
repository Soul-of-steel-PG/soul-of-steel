using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPanel : MonoBehaviour {
    void Start() {
        GameManager.Instance.handPanel = gameObject;
    }
}