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

        #region DismissCard

        [Test]
        public void DismissCard_MovesCardToScrapPanel()
        {
            // Arrange
            var cardController = CreateSystem();
            var _mockScrapPanel = Substitute.For<IScrapPanel>();
            _mockScrapPanel.GetTransform().Returns(Arg.Any<Transform>());
            _mockScrapPanel.GetTransform().TransformPoint(Arg.Any<Vector3>()).Returns(Vector3.zero);

            // Mocking DOTween's DOMove and OnComplete
            Tween tween = Substitute.For<Tween>();
            _mockScrapPanel.GetTransform().DOMove(_mockScrapPanel.GetTransform().position, 0.5f).Returns(tween);

            // Act
            cardController.DismissCard();

            // Assert
            _gameObject.transform.Received(1).DOMove(Vector3.zero, 0.5f);
            Assert.AreSame(scrapPanelTransform, _gameObject.transform.parent);
            Assert.AreEqual(2, _gameObject.transform.GetSiblingIndex());
        }

        [Test]
        public void DismissCard_AnimationSequence()
        {
            // Arrange
            var cardController = CreateSystem();
            var scrapPanelTransform = new GameObject().transform;
            _mockScrapPanel.GetTransform().Returns(scrapPanelTransform);
            scrapPanelTransform.TransformPoint(Arg.Any<Vector3>()).Returns(Vector3.zero);

            // Mocking DOTween's DOMove and OnComplete
            Tween tween = Substitute.For<Tween>();
            _gameObject.transform.DOMove(Vector3.zero, 0.5f).Returns(tween);
            tween.OnComplete(Arg.Do<TweenCallback>(callback => callback.Invoke())).Returns(tween);

            // Act
            cardController.DismissCard();

            // Assert
            _gameObject.transform.Received(1).DOMove(Vector3.zero, 0.5f);
            Assert.IsTrue(_mockPlayerView._inAnimation);
            tween.Received(1).OnComplete(Arg.Any<TweenCallback>());
        }

        [Test]
        public void DismissCard_SetsInAnimationToTrue()
        {
            // Arrange
            var cardController = CreateSystem();
            var scrapPanelTransform = new GameObject().transform;
            _mockScrapPanel.GetTransform().Returns(scrapPanelTransform);
            scrapPanelTransform.TransformPoint(Arg.Any<Vector3>()).Returns(Vector3.zero);

            // Mocking DOTween's DOMove and OnComplete
            Tween tween = Substitute.For<Tween>();
            _gameObject.transform.DOMove(Vector3.zero, 0.5f).Returns(tween);
            tween.OnComplete(Arg.Do<TweenCallback>(callback => callback.Invoke())).Returns(tween);

            // Act
            cardController.DismissCard();

            // Assert
            Assert.IsTrue(_mockPlayerView._inAnimation);
        }

        [Test]
        public void DismissCard_SetsInAnimationToFalse()
        {
            // Arrange
            var cardController = CreateSystem();
            var scrapPanelTransform = new GameObject().transform;
            _mockScrapPanel.GetTransform().Returns(scrapPanelTransform);
            scrapPanelTransform.TransformPoint(Arg.Any<Vector3>()).Returns(Vector3.zero);

            // Mocking DOTween's DOMove and OnComplete
            Tween tween = Substitute.For<Tween>();
            _gameObject.transform.DOMove(Vector3.zero, 0.5f).Returns(tween);
            tween.OnComplete(Arg.Do<TweenCallback>(callback => callback.Invoke())).Returns(tween);

            // Act
            cardController.DismissCard();

            // Simulate OnComplete callback
            // tween.OnCompleteReceived().Invoke();

            // Assert
            Assert.IsFalse(_mockPlayerView._inAnimation);
        }

        [Test]
        public void DismissCard_SetsDismissTextSizes()
        {
            // Arrange
            var cardController = CreateSystem();
            var scrapPanelTransform = new GameObject().transform;
            _mockScrapPanel.GetTransform().Returns(scrapPanelTransform);
            scrapPanelTransform.TransformPoint(Arg.Any<Vector3>()).Returns(Vector3.zero);

            // Mocking DOTween's DOMove and OnComplete
            Tween tween = Substitute.For<Tween>();
            _gameObject.transform.DOMove(Vector3.zero, 0.5f).Returns(tween);
            tween.OnComplete(Arg.Do<TweenCallback>(callback => callback.Invoke())).Returns(tween);

            // Act
            cardController.DismissCard();

            // Simulate OnComplete callback
            // tween.OnCompleteReceived().Invoke();

            // Assert
            _mockView.Received(1).SetDismissTextSizes();
        }

        #endregion
    }
}