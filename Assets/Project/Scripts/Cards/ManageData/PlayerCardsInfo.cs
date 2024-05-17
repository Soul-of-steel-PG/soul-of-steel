using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable, CreateAssetMenu(fileName = "PlayerCards_", menuName = "PlayerCards")]
public class PlayerCardsInfo : ScriptableObject {
    public List<CardInfoSerialized.CardInfoStruct> playerCards;

    [Button]
    public void SetPlayerCards(List<int> cardIds) {
        playerCards ??= new List<CardInfoSerialized.CardInfoStruct>();

        playerCards.Clear();

        foreach (int cardId in cardIds) {
            CardInfoSerialized.CardInfoStruct card =
                GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Find(c => c.Id == cardId);
            //Debug.Log($"{card.CardName} with id {card.Id}");
            playerCards.Add(card);
        }
    }
}