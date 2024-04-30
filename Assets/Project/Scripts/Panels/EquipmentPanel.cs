using System;
using UnityEngine;

public class EquipmentPanel : MonoBehaviour {
    public bool isEnemy;

    private void Start() {
        if (isEnemy) GameManager.Instance.enemyEquipmentPanel = this;
        else GameManager.Instance.myEquipmentPanel = this;
    }
}