using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NSubstitute.ReturnsExtensions;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace SoulOfSteelTests {
    public class CardControllerTests {
        private ICardView _mockView;
        private IGameManager _mockGameManager;
        private IUIManager _mockUIManager;

        [SetUp]
        public void BeforeTest()
        {
            _mockView = Substitute.For<ICardView>();
            _mockGameManager = Substitute.For<IGameManager>();
            _mockUIManager = Substitute.For<IUIManager>();
        }

        private CardController CreateSystem()
        {
            return Substitute.For<CardController>(_mockView, _mockGameManager, _mockUIManager);
        }

        [Test]
        public void Select_InAnimation_ReturnsEarly()
        {
            // Arrange
            _mockGameManager.LocalPlayerInstance.Returns(Substitute.For<IPlayerView>());

            // Act
            CardController systemUnderTest = CreateSystem();
            systemUnderTest.Select();

            // Assert
            _mockView.DidNotReceive().SelectAnimation(Arg.Any<bool>());
        }

        [Test]
        public void Select_CardsAlreadySelectedAndNotCurrentlySelected_ReturnsEarly()
        {
            // Arrange
            _mockGameManager.LocalPlayerInstance.Returns(Substitute.For<IPlayerView>());
            _mockGameManager.LocalPlayerInstance.PlayerController.GetCardsSelected().Returns(true);
            // Act
            CardController systemUnderTest = CreateSystem();
            systemUnderTest.Select();

            // Assert
            _mockView.DidNotReceive().SelectAnimation(Arg.Any<bool>());
        }

        [Test]
        public void Select_NotSelectingAndAlreadySelected_DeselectsCard()
        {
            // Arrange
            _mockGameManager.LocalPlayerInstance.Returns(Substitute.For<IPlayerView>());
            _mockGameManager.LocalPlayerInstance.PlayerController.GetCardsSelected().Returns(true);

            // Act
            CardController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetSelected(true);
            systemUnderTest.Debug_SetIsSelecting(false);
            systemUnderTest.Select(true);

            // Assert
            Assert.IsFalse(systemUnderTest.GetSelected());
        }

        [Test]
        public void Select_SelectingAndCantAffordCard_ReturnsEarly()
        {
            // Arrange
            _mockGameManager.LocalPlayerInstance.Returns(Substitute.For<IPlayerView>());
            _mockGameManager.LocalPlayerInstance.PlayerController.GetCardsSelected().Returns(false);
            _mockGameManager.LocalPlayerInstance.PlayerController.TryPayingForCard(Arg.Any<int>()).Returns(false);

            // Act
            CardController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetIsSelecting(true);
            systemUnderTest.Select();

            // Assert
            _mockView.DidNotReceive().SelectAnimation(Arg.Any<bool>());
        }

        [Test]
        public void Select_SelectingAndCanAffordCard_TogglesSelectedAndCallsSelectAnimation()
        {
            // Arrange
            _mockGameManager.LocalPlayerInstance.Returns(Substitute.For<IPlayerView>());
            _mockGameManager.LocalPlayerInstance.PlayerController.GetCardsSelected().Returns(false);
            _mockGameManager.LocalPlayerInstance.PlayerController.TryPayingForCard(Arg.Any<int>()).Returns(true);

            // Act
            CardController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetIsSelecting(true);
            systemUnderTest.Select();

            // Assert
            Assert.IsTrue(systemUnderTest.GetSelected());
            _mockView.Received(1).SelectAnimation(true);
        }

        [Test]
        public void Select_CardTypeSelectionLogic()
        {
            // Arrange
            _mockGameManager.LocalPlayerInstance.Returns(Substitute.For<IPlayerView>());
            _mockGameManager.LocalPlayerInstance.PlayerController.GetCardsSelected().Returns(false);
            _mockGameManager.LocalPlayerInstance.PlayerController.TryPayingForCard(Arg.Any<int>()).Returns(true);
            var mockCardView = Substitute.For<IArmCardView>();
            _mockView.GetCardView().Returns(mockCardView);

            // Act
            CardController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetIsSelecting(true);
            systemUnderTest.Debug_SetType(CardType.Arm);
            systemUnderTest.Select();

            // Assert
            // Verify correct behavior based on card type (Weapon, Arm, CampEffect, Hacking, Legs, Armor, Chest)
            // Example for Weapon type:
            _mockGameManager.Received().OnCardSelected(_mockGameManager.LocalPlayerInstance,
                mockCardView, true);
        }
    }
}