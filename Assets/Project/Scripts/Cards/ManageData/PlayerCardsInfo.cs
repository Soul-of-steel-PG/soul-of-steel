using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IPlayerCardsInfo {
    public List<CardInfoSerialized.CardInfoStruct> PlayerCards { get; }
}

[Serializable, CreateAssetMenu(fileName = "PlayerCards_", menuName = "PlayerCards")]
public class PlayerCardsInfo : ScriptableObject, IPlayerCardsInfo {
    public List<CardInfoSerialized.CardInfoStruct> playerCards;

    public List<CardInfoSerialized.CardInfoStruct> PlayerCards => playerCards;

    [Button]
    public void SetPlayerCards(List<int> cardIds)
    {
        playerCards ??= new List<CardInfoSerialized.CardInfoStruct>();

        playerCards.Clear();

        foreach (int cardId in cardIds)
        {
            CardInfoSerialized.CardInfoStruct card =
                GameManager.Instance.cardDataBase.cardDataBase.Sheet1.Find(c => c.Id == cardId);
            //Debug.Log($"{card.CardName} with id {card.Id}");
            playerCards.Add(card);
        }
    }
}