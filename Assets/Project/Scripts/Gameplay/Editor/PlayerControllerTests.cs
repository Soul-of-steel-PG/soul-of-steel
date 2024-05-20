using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Photon.Pun;
using SRF;
using Random = UnityEngine.Random;

namespace SoulOfSteelTests {
    public class PlayerControllerTests {
        private IPlayerView _mockView;
        private IGameManager _mockGameManager;
        private IEffectManager _mockEffectManager;
        private IUIManager _mockUIManager;

        [SetUp]
        public void BeforeTest()
        {
            _mockView = Substitute.For<IPlayerView>();
            _mockGameManager = Substitute.For<IGameManager>();
            _mockEffectManager = Substitute.For<IEffectManager>();
            _mockUIManager = Substitute.For<IUIManager>();
        }

        private PlayerController CreateSystem()
        {
            return new PlayerController(_mockView, _mockGameManager, _mockEffectManager, _mockUIManager);
        }

        #region DrawCards

        [Test]
        public void DrawCards_WhenFullDrawFalse_InitAddCards(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int amount)
        {
            // Arrange.
            _mockGameManager.HandPanel.Returns(Substitute.For<IHandPanel>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.DrawCards(amount, false);

            // Assert.
            _mockGameManager.HandPanel.DidNotReceive().ResetAnimationReferenceParent();
            _mockView.Received(1).InitAddCards(amount);
        }

        [Test]
        public void DrawCards_WhenFullDrawTrue_InitAddCards(
            [NUnit.Framework.Range(0, 9)] int amount)
        {
            // Arrange.
            _mockGameManager.HandPanel.Returns(Substitute.For<IHandPanel>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.DrawCards(amount, true);

            // Assert.
            _mockGameManager.HandPanel.Received(1).ResetAnimationReferenceParent();
            _mockView.Received(1).CleanHandsPanel();
            _mockView.Received(1).InitAddCards(amount);
        }

        #endregion

        #region TryPayingForCard

        [Test]
        public void TryPayingForCard_EnoughScrapPoints_DecreaseScrapPoints_AndReturnsTrue()
        {
            // Arrange.
            PlayerController systemUnderTest = CreateSystem();
            int initialScrapPoint = systemUnderTest.Debug_GetScrapPoints();
            const int cardCost = 5;

            // Act.
            bool result = systemUnderTest.TryPayingForCard(cardCost);

            // Assert.
            Assert.AreEqual(systemUnderTest.Debug_GetScrapPoints(), initialScrapPoint - cardCost);
            Assert.IsTrue(result);
        }

        [Test]
        public void TryPayingForCard_EnoughScrapPoints_ScrapPointsDoesNotChange_AndReturnsFalse()
        {
            // Arrange.
            PlayerController systemUnderTest = CreateSystem();
            int initialScrapPoint = systemUnderTest.Debug_GetScrapPoints();
            int cardCost = initialScrapPoint + 5;

            // Act.
            bool result = systemUnderTest.TryPayingForCard(cardCost);

            // Assert.
            Assert.AreEqual(systemUnderTest.Debug_GetScrapPoints(), initialScrapPoint);
            Assert.IsFalse(result);
        }

        #endregion

        #region EquipCard

        [Test]
        public void EquipCard_WhenCardInfoStructIsNull_LogsCardNotFoundError()
        {
            // Arrange.
            const int index = 1;
            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns((CardInfoSerialized.CardInfoStruct)null);
            PlayerController systemUnderTest = CreateSystem();

            // Act.
            systemUnderTest.EquipCard(index);

            // Assert.
            LogAssert.Expect(LogType.Error, "CARD NOT FOUND");
        }

        [Test]
        public void EquipCard_WhenCardTypeIsArm_SetsArmCard()
        {
            // Arrange.
            const int index = 1;
            var cardInfoStruct = new CardInfoSerialized.CardInfoStruct {
                Id = 1,
                CardName = "Test Arm Card",
                Description = "Test Description",
                Cost = 10,
                Recovery = 2,
                Damage = 5,
                AttackTypeEnum = AttackType.StraightLine,
                AttackDistance = 2,
                AttackArea = 1,
                ImageSource = null,
                TypeEnum = CardType.Arm
            };

            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns(cardInfoStruct);
            _mockView.AddCardToPanel(CardType.Arm).Returns(Substitute.For<IArmCardView>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(index);

            // Assert.

            _mockView.Received(1).AddCardToPanel(CardType.Arm);
        }

        [Test]
        public void EquipCard_WhenCardTypeIsWeapon_SetsArmCard()
        {
            // Arrange.
            const int index = 1;
            var cardInfoStruct = new CardInfoSerialized.CardInfoStruct {
                Id = 1,
                CardName = "Test Weapon Card",
                Description = "Test Description",
                Cost = 10,
                Recovery = 2,
                Damage = 5,
                AttackTypeEnum = AttackType.StraightLine,
                AttackDistance = 2,
                AttackArea = 1,
                ImageSource = null,
                TypeEnum = CardType.Weapon
            };

            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns(cardInfoStruct);
            _mockView.AddCardToPanel(CardType.Weapon).Returns(Substitute.For<IArmCardView>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(index);

            // Assert.

            _mockView.Received(1).AddCardToPanel(CardType.Weapon);
        }

        [Test]
        public void EquipCard_WhenCardTypeIsLegs_SetsLegsCard()
        {
            // Arrange.
            const int index = 1;
            var cardInfoStruct = new CardInfoSerialized.CardInfoStruct {
                Id = 1,
                CardName = "Test Legs Card",
                Description = "Test Description",
                Cost = 10,
                Recovery = 2,
                Damage = 5,
                AttackTypeEnum = AttackType.StraightLine,
                AttackDistance = 2,
                AttackArea = 1,
                ImageSource = null,
                TypeEnum = CardType.Legs
            };

            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns(cardInfoStruct);
            _mockView.AddCardToPanel(CardType.Legs).Returns(Substitute.For<ILegsCardView>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(index);

            // Assert.

            _mockView.Received(1).AddCardToPanel(CardType.Legs);
        }

        [Test]
        public void EquipCard_WhenCardTypeIsArmor_SetsArmorCard()
        {
            // Arrange.
            const int index = 1;
            var cardInfoStruct = new CardInfoSerialized.CardInfoStruct {
                Id = 1,
                CardName = "Test Armor Card",
                Description = "Test Description",
                Cost = 10,
                Recovery = 2,
                Damage = 5,
                AttackTypeEnum = AttackType.StraightLine,
                AttackDistance = 2,
                AttackArea = 1,
                ImageSource = null,
                TypeEnum = CardType.Armor
            };

            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns(cardInfoStruct);
            _mockView.AddCardToPanel(CardType.Armor).Returns(Substitute.For<IChestCardView>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(index);

            // Assert.

            _mockView.Received(1).AddCardToPanel(CardType.Armor);
        }

        [Test]
        public void EquipCard_WhenCardTypeIsChest_SetsArmorCard()
        {
            // Arrange.
            const int index = 1;
            var cardInfoStruct = new CardInfoSerialized.CardInfoStruct {
                Id = 1,
                CardName = "Test Chest Card",
                Description = "Test Description",
                Cost = 10,
                Recovery = 2,
                Damage = 5,
                AttackTypeEnum = AttackType.StraightLine,
                AttackDistance = 2,
                AttackArea = 1,
                ImageSource = null,
                TypeEnum = CardType.Chest
            };

            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns(cardInfoStruct);
            _mockView.AddCardToPanel(CardType.Chest).Returns(Substitute.For<IChestCardView>());

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(index);

            // Assert.

            _mockView.Received(1).AddCardToPanel(CardType.Chest);
        }

        #endregion

        #region ShuffleDeck

        [Test]
        public void ShuffleDeck_WithoutShuffle_RemainsUnchanged()
        {
            // Arrange
            var mockPlayerCardsInfo = Substitute.For<IPlayerCardsInfo>();
            var cardInfo1 = new CardInfoSerialized.CardInfoStruct
                { Id = 1, CardName = "Card1", TypeEnum = CardType.Pilot };
            var cardInfo2 = new CardInfoSerialized.CardInfoStruct
                { Id = 2, CardName = "Card2", TypeEnum = CardType.Weapon };
            var cardInfo3 = new CardInfoSerialized.CardInfoStruct
                { Id = 3, CardName = "Card3", TypeEnum = CardType.Arm };
            mockPlayerCardsInfo.PlayerCards.Returns(new List<CardInfoSerialized.CardInfoStruct>() {
                cardInfo1,
                cardInfo2,
                cardInfo3
            });
            _mockView.GetDeckInfo().Returns(mockPlayerCardsInfo);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.ShuffleDeck(false, false);

            // Assert
            CollectionAssert.AreEqual(
                new List<CardInfoSerialized.CardInfoStruct> { cardInfo2, cardInfo3 },
                systemUnderTest.Debug_GetShuffledDeck().playerCards);
        }

        [Test]
        public void ShuffleDeck_WithShuffle_ShufflesTheDeck()
        {
            // Arrange
            var mockPlayerCardsInfo = Substitute.For<IPlayerCardsInfo>();
            var cardInfo1 = new CardInfoSerialized.CardInfoStruct
                { Id = 1, CardName = "Card1", TypeEnum = CardType.Pilot };
            var cardInfo2 = new CardInfoSerialized.CardInfoStruct
                { Id = 2, CardName = "Card2", TypeEnum = CardType.Weapon };
            var cardInfo3 = new CardInfoSerialized.CardInfoStruct
                { Id = 3, CardName = "Card3", TypeEnum = CardType.Arm };
            mockPlayerCardsInfo.PlayerCards.Returns(new List<CardInfoSerialized.CardInfoStruct>() {
                cardInfo1,
                cardInfo2,
                cardInfo3
            });
            _mockView.GetDeckInfo().Returns(mockPlayerCardsInfo);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.ShuffleDeck(false, true);

            // Assert
            CollectionAssert.AreNotEqual(
                new List<CardInfoSerialized.CardInfoStruct> { cardInfo1, cardInfo2, cardInfo3 },
                systemUnderTest.Debug_GetShuffledDeck().playerCards);
        }

        [Test]
        public void ShuffleDeck_IsFirstTime_SetsPilotCard_AndRemovesPilotCardFromShuffledDeck()
        {
            // Arrange
            var mockPlayerCardsInfo = Substitute.For<IPlayerCardsInfo>();
            var cardInfo1 = new CardInfoSerialized.CardInfoStruct
                { Id = 1, CardName = "Card1", TypeEnum = CardType.Pilot };
            var cardInfo2 = new CardInfoSerialized.CardInfoStruct
                { Id = 2, CardName = "Card2", TypeEnum = CardType.Weapon };
            var cardInfo3 = new CardInfoSerialized.CardInfoStruct
                { Id = 3, CardName = "Card3", TypeEnum = CardType.Arm };
            mockPlayerCardsInfo.PlayerCards.Returns(new List<CardInfoSerialized.CardInfoStruct>() {
                cardInfo1,
                cardInfo2,
                cardInfo3
            });
            _mockView.GetDeckInfo().Returns(mockPlayerCardsInfo);
            _mockView.AddCardToPanel(CardType.Pilot).Returns(Substitute.For<IPilotCardView>());

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.ShuffleDeck(true, false);

            // Assert
            _mockView.Received(1).AddCardToPanel(CardType.Pilot);
            Assert.IsFalse(systemUnderTest.Debug_GetShuffledDeck().PlayerCards
                .Exists(card => card.TypeEnum == CardType.Pilot));
        }

        #endregion

        #region SelectCards

        [Test]
        public void SelectCards_NoCardsInHand_CallsSetCardsSelectedTrue()
        {
            // Arrange
            var types = new List<CardType> { CardType.Weapon };

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.SelectCards(types, 1);

            // Assert
            Assert.IsTrue(systemUnderTest.GetCardsSelected());
        }

        [Test]
        public void SelectCards_NoMatchingCardTypes_CallsSetCardsSelectedTrue()
        {
            // Arrange
            var types = new List<CardType> { CardType.Weapon };
            var mockCard = Substitute.For<ICardView>();
            mockCard.GetCardType().Returns(CardType.Pilot);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_AddCardToHand(mockCard); // Helper method to add a card to _hand
            systemUnderTest.SelectCards(types, 1);

            // Assert
            Assert.IsTrue(systemUnderTest.GetCardsSelected());
        }

        [Test]
        public void SelectCards_MatchingCardTypes_CallsSetIsSelecting()
        {
            // Arrange
            var types = new List<CardType> { CardType.Weapon };
            var mockCard = Substitute.For<ICardView>();
            mockCard.GetCardType().Returns(CardType.Weapon);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_AddCardToHand(mockCard); // Helper method to add a card to _hand
            systemUnderTest.SelectCards(types, 1);

            // Assert
            mockCard.Received(1).SetIsSelecting(Arg.Any<bool>());
        }

        [Test]
        public void SelectCards_MultipleMatchingCardTypes_CallsSetIsSelectingOnAllMatchingCards()
        {
            // Arrange
            var types = new List<CardType> { CardType.Weapon, CardType.Arm, CardType.Chest };
            var mockCard = Substitute.For<ICardView>();
            mockCard.GetCardType().Returns(types.Random());

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_AddCardToHand(mockCard); // Helper method to add a card to _hand
            systemUnderTest.Debug_AddCardToHand(mockCard); // Helper method to add a card to _hand
            systemUnderTest.Debug_AddCardToHand(mockCard); // Helper method to add a card to _hand
            systemUnderTest.SelectCards(types, 1);

            // Assert
            mockCard.Received(3).SetIsSelecting(Arg.Any<bool>());
        }

        #endregion

        #region SelectAttack

        [Test]
        public void SelectAttack_CurrentPlayerIsNotMine_ReturnsEarly()
        {
            // Arrange
            var mockArm = Substitute.For<IArmCardView>();
            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetArm(mockArm, true);
            systemUnderTest.SelectAttack();

            // Assert
            Assert.AreEqual(0, systemUnderTest.GetCurrentDamage());
            systemUnderTest.Debug_GetWeapon().DidNotReceive().SelectAttack();
        }

        [Test]
        public void SelectAttack_WeaponIsNullAndArmIsNotNull_IncrementsDamageAndCallsSelectAttackOnArm()
        {
            // Arrange
            var mockArm = Substitute.For<IArmCardView>();
            _mockView.PhotonViewIsMine.Returns(true);
            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetArm(mockArm, false);
            systemUnderTest.SelectAttack();

            // Assert
            Assert.AreEqual(0, systemUnderTest.GetCurrentDamage());
            mockArm.Received(1).SelectAttack();
        }

        [Test]
        public void SelectAttack_WeaponAndArmAreNull_IncrementsDamageAndCallsSelectAttackOnPilot()
        {
            // Arrange
            var mockPilot = Substitute.For<IPilotCardView>();
            mockPilot.PilotCardController.GetDefaultDamage().Returns(3);
            _mockView.PhotonViewIsMine.Returns(true);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetPilot(mockPilot);
            systemUnderTest.SelectAttack();

            // Assert
            Assert.AreEqual(3, systemUnderTest.GetCurrentDamage());
            mockPilot.Received(1).SelectAttack();
        }

        [Test]
        public void SelectAttack_WeaponIsNotNull_IncrementsDamageAndCallsSelectAttackOnWeapon()
        {
            // Arrange
            var mockWeapon = Substitute.For<IArmCardView>();
            mockWeapon.ArmCardController.GetDamage().Returns(7);
            _mockView.PhotonViewIsMine.Returns(true);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetArm(mockWeapon, true);
            systemUnderTest.SelectAttack();

            // Assert
            Assert.AreEqual(7, systemUnderTest.GetCurrentDamage());
            mockWeapon.Received(1).SelectAttack();
        }

        [Test]
        public void SelectAttack_ExtraDamageIsSet_InitializesCurrentDamageWithExtraDamage()
        {
            // Arrange
            _mockView.PhotonViewIsMine.Returns(true);
            var mockPilot = Substitute.For<IPilotCardView>();
            mockPilot.PilotCardController.Returns(Substitute.For<IPilotCardController>());
            mockPilot.PilotCardController.GetDefaultDamage().Returns(0);

            // Act
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetPilot(mockPilot);
            systemUnderTest.SetExtraDamage(10);
            systemUnderTest.SelectAttack();

            // Assert
            Assert.AreEqual(10, systemUnderTest.GetCurrentDamage());
        }

        #endregion
    }
}