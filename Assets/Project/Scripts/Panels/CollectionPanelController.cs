using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CardInfoSerialized;

public class CollectionPanelController : MonoBehaviour
{
    [Header("Card")]
    [SerializeField] private GameObject _cardGO;

    [Header("Contents")]
    [SerializeField] private Transform _allCardsPanelContent;
    [SerializeField] private Transform _attackCardsPanelContent;
    [SerializeField] private Transform _armorCardsPanelContent;
    [SerializeField] private Transform _spellsCardsPanelContent;
    [SerializeField] private Transform _fieldCardsPanelContent;

    private List<int> playerCards = new List<int> { 30, 36, 34, 35, 23, 6, 0, 35, 32, 20, 0, 35, 23, 35, 37, 0, 6, 18, 20, 22, 23, 24, 26, 27, 28, 29, 30, 31, 32, 34, 35, 36, 37, 38 };
//[Header("Buttons")]
//[SerializeField] private Button Show

private void Start()
    {
        InstantiateCards();
    }

    [Button("SpawnCards")]
    public void InstantiateCards()
    {
        //Recorrer lista de cartas del GameManager
        List<CardInfoStruct> cards = GameManager.Instance.cardDataBase.cardDataBase.Sheet1;

        foreach (var card in cards)
        {
            if (!playerCards.Contains(card.Id))
            {
                continue;
            }
            InstantiateCard(_allCardsPanelContent, card);

            switch (card.TypeEnum)
            {
                case CardType.Weapon:
                case CardType.Arm:
                    InstantiateCard(_attackCardsPanelContent, card);
                    break;
                case CardType.Armor:
                case CardType.Chest:
                case CardType.Legs:
                    InstantiateCard(_armorCardsPanelContent, card);
                    break;
                case CardType.CampEffect:
                case CardType.Hacking:
                case CardType.Generator:
                    InstantiateCard(_spellsCardsPanelContent, card);
                    break;
                default:
                    break;
            }
        }
    }

    private void InstantiateCard(Transform parent, CardInfoStruct cardData)
    {
        IBaseCardUI baseCardUI = Instantiate(_cardGO, parent).GetComponent<IBaseCardUI>();
        baseCardUI.SetCardUI(cardData);
    }
}

