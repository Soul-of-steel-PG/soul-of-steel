using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "Card_", menuName = "Card")]
public class CardsInfo : ScriptableObject {
    public CardsInfo() {
    }

    public TextAsset textAssetData;

    public int playerId;
    public List<CardInfoStruct> playerCards;

    public void GetPlayerInfo() {
        string[] data = textAssetData.text.Split(new string[] { ";", "\n" }, StringSplitOptions.None);
    }

    [Serializable]
    public struct CardInfoStruct {
        public int id;
        public string cardName;
        public CardType type;
        public string cardDescription;
        public int cost;
        public int energy;
        public int damage;
        public int shield;
        public int recovery;
        public bool isCampEffect;
        public Sprite imageSource;
        public int health;
        public BoardView defaultMovement;
    }

    [Button]
    public void DownloadData() {
        Downloader.Instance.LoadInfo(this);
    }


    private void OnEnable() {
    }

    private void OnDisable() {
    }
}