using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using WebSocketSharp;

[Flags]
public enum AttackType
{
    None,
    StraightLine,
    Square,
    Cone
}


[Serializable]
public class CardInfoSerialized
{
    public List<CardInfoStruct> Sheet1;
    private List<CardInfoStruct> Sheet2;

    [Serializable]
    public class CardInfoStruct
    {
        Dictionary<string, CardType> typeMapping = new() {
            { "Campo", CardType.CampEffect },
            { "hackeo", CardType.Hacking },
            { "Generador", CardType.Generator },
            { "Fabrica", CardType.Generator },
            { "Arma", CardType.Weapon },
            { "Brazo", CardType.Arm },
            { "Pecho", CardType.Chest },
            { "Piernas", CardType.Legs },
            { "Piloto", CardType.Pilot }
        };

        Dictionary<string, AttackType> attackTypeMapping = new() {
            { "Linearecta", global::AttackType.StraightLine },
            { "Cuadrado", global::AttackType.Square },
            { "Cono", global::AttackType.Cone },
        };

        [FoldoutGroup("Card")] public int Id;
        [FoldoutGroup("Card")] public string CardName;

        [OnValueChanged("SetType"), HideInInspector]
        public string Type;

        [FoldoutGroup("Card")] public CardType TypeEnum;

        [TextArea(1, 10), FoldoutGroup("Card/Description")] [FoldoutGroup("Card")]
        public string Description;

        [FoldoutGroup("Card")] public int Cost;
        [FoldoutGroup("Card")] public int Energy;
        [FoldoutGroup("Card")] public int Damage;
        [FoldoutGroup("Card")] public int Shield;
        [FoldoutGroup("Card")] public int Recovery;
        [FoldoutGroup("Card")] public bool IsCampEffect;
        [FoldoutGroup("Card")] public Sprite ImageSource;
        [FoldoutGroup("Card")] public int Health;
        [FoldoutGroup("Card")] public List<Movement> SerializedMovements;
        [FoldoutGroup("Card")] public AttackType AttackTypeEnum;
        [FoldoutGroup("Card")] public int AttackDistance;
        [FoldoutGroup("Card")] public int AttackArea;

        [OnValueChanged("SetMovements"), HideInInspector]
        public string Movements;

        [OnValueChanged("SetAttackType"), HideInInspector]
        public string AttackType;

        public void SetType()
        {
            Type = Type.Replace(" ", "");
            if (Type == "Brazo/Arma") Type = "Arma";
            if (typeMapping.TryGetValue(Type, out CardType enumValue))
            {
                TypeEnum = enumValue;
            }
            else
            {
                throw new ArgumentException($"Invalid CardType Type: {Type}");
            }
        }

        public void SetMovements()
        {
            if (Movements.IsNullOrEmpty() || Movements == "0") return;
            SerializedMovements = Movement.FromString(Movements);
        }

        public void SetAttackType()
        {
            if (AttackType.IsNullOrEmpty() || AttackType == "0")
            {
                AttackTypeEnum = global::AttackType.None;
                return;
            }

            AttackType = AttackType.Replace(" ", "");

            if (attackTypeMapping.TryGetValue(AttackType, out AttackType enumValue))
            {
                AttackTypeEnum = enumValue;
            }
            else
            {
                throw new ArgumentException($"Invalid Attack Type: {AttackType}");
            }
        }
    }
}

[Serializable, CreateAssetMenu(fileName = "Card_Data", menuName = "Card_Data")]
public class CardsDataBase : ScriptableObject
{
    public CardInfoSerialized cardDataBase;

    [Button]
    public void DownloadData()
    {
        Downloader.Instance.LoadInfo(this);
    }
}