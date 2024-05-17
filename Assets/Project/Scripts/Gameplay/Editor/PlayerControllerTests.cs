using System;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace SoulOfSteelTests
{
    public class PlayerControllerTests
    {
        private IPlayerView _mockView;
        private IGameManager _mockGameManager;

        [SetUp]
        public void BeforeTest()
        {
            _mockView = Substitute.For<IPlayerView>();
            _mockGameManager = Substitute.For<IGameManager>();
        }

        private PlayerController CreateSystem()
        {
            return new PlayerController(_mockView, _mockGameManager);
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
            const int initialScrapPoints = 15;
            const int cardCost = 5;
            PlayerController systemUnderTest = CreateSystem();

            // Act.
            bool result = systemUnderTest.TryPayingForCard(cardCost);

            // Assert.
            Assert.AreEqual(systemUnderTest.Debug_GetScrapPoints(), initialScrapPoints - cardCost);
            Assert.IsTrue(result);
        }

        [Test]
        public void TryPayingForCard_EnoughScrapPoints_ScrapPointsDoesNotChange_AndReturnsFalse()
        {
            // Arrange.
            const int initialScrapPoints = 15;
            const int cardCost = 20;

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            bool result = systemUnderTest.TryPayingForCard(cardCost);

            // Assert.
            Assert.AreEqual(systemUnderTest.Debug_GetScrapPoints(), initialScrapPoints);
            Assert.IsFalse(result);
        }

        #endregion

        #region EquipCard

        [Test]
        public void EquipCard_WhenCardInfoStructIsNull_DoesNothing()
        {
            // Arrange.
            _mockGameManager.GetCardFromDataBaseByIndex(0).ReturnsNull();

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(0);

            // Assert.
            _mockView.DidNotReceive().AddCardToPanel(Arg.Any<CardType>());
            LogAssert.Expect(LogType.Error, "CARD NOT FOUND");
        }

        [Test]
        public void EquipCard_WhenCardInfoStructIsNotNull_SetTheCorrespondingCard(
            [NUnit.Framework.Range(0, 30)] int index)
        {
            // Arrange.
            var cardData = Resources.Load<CardsDataBase>("Presets/Card_Data");
            var cardInfoStruct = cardData.cardDataBase.Sheet1.Find(c => c.Id == index);
            _mockGameManager.GetCardFromDataBaseByIndex(index).Returns(cardInfoStruct);

            // Act.
            PlayerController systemUnderTest = CreateSystem();
            systemUnderTest.EquipCard(index);

            // Assert.
            bool shouldAddCardToPanel = cardInfoStruct != null && (cardInfoStruct.TypeEnum is CardType.Arm
                or CardType.Weapon
                or CardType.Legs
                or CardType.Armor
                or CardType.Chest);

            if (shouldAddCardToPanel)
            {
                _mockView.Received(1).AddCardToPanel(cardInfoStruct.TypeEnum);
            }
            else
            {
                _mockView.DidNotReceive().AddCardToPanel(Arg.Any<CardType>());
            }
        }

        #endregion
    }
}