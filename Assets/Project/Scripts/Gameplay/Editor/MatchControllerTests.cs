using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using UnityEngine;

namespace SoulOfSteelTests {
    public class MatchControllerTests {
        private IMatchView _mockView;
        private IGameManager _mockGameManager;

        [SetUp]
        public void BeforeTest() {
            _mockView = Substitute.For<IMatchView>();
            _mockGameManager = Substitute.For<IGameManager>();
        }

        private MatchController CreateSystem() {
            return new MatchController(_mockView, _mockGameManager);
        }

        #region SetPriority

        [Test]
        public void SetPriority_SetsPriorityTo1_AndSetsCurrentPhaseText() {
            // Act.
            MatchController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SetPriority();

            // Assert.
            _mockGameManager.Received(1).SetCurrentPriority(1);
            _mockView.Received(1).SetCurrentPhaseText("priority = 1");
        }

        #endregion

        #region SelectQuadrant
        
        [Test]
        public void SelectQuadrant_WhenPlayerListIsNull_DoesNothing() {
            // Arrange.
            _mockGameManager.PlayerList.ReturnsNull();

            // Act.
            MatchController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SelectQuadrant();

            // Assert.
            _mockView.DidNotReceive().SetCurrentPhaseText(Arg.Any<string>());
        }
        
        [Test]
        public void SelectQuadrant_WhenPlayerListIsEmpty_SetsCurrentPhaseText() {
            // Arrange.
            _mockGameManager.PlayerList.Returns(new List<IPlayerView>());

            // Act.
            MatchController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SelectQuadrant();

            // Assert.
            _mockView.Received(1).SetCurrentPhaseText(Arg.Any<string>());
        }

        [Test]
        public void SelectQuadrant_WhenPlayerListIsNotEmpty_SetsCurrentPhaseText_AndMovesPlayerToCorrespondingCell(
            [Values(0, 1, 2)] int playerId, [Values(0, 1, 2)] int boardCount) {
            // Arrange.
            IPlayerView mockPlayer = Substitute.For<IPlayerView>();
            mockPlayer.PlayerController.Returns(Substitute.For<IPlayerController>());
            mockPlayer.PlayerController.GetPlayerId().Returns(playerId);
            _mockGameManager.PlayerList.Returns(new List<IPlayerView> {mockPlayer});
            
            _mockGameManager.BoardView.Returns(Substitute.For<IBoardView>());
            _mockGameManager.BoardView.BoardController.Returns(Substitute.For<IBoardController>());
            _mockGameManager.BoardView.BoardController.GetBoardCount().Returns(boardCount);
            
            // Act.
            MatchController systemUnderTest = CreateSystem();
            systemUnderTest.Debug_SelectQuadrant();
            
            // Assert.
            const int upDegrees = 90;
            const int downDegrees = 270;
            Vector2 nextCell = playerId == 1 ? Vector2.zero : (boardCount - 1) * Vector2.one;
            int currentDegrees =  playerId == 1 ? downDegrees : upDegrees;

            mockPlayer.PlayerController.Received(1).SetCurrentCell(nextCell);
            mockPlayer.PlayerController.Received(1).SetCurrentDegrees(currentDegrees);
            mockPlayer.Received(1).MoveToCell(nextCell);
            mockPlayer.Received(1).Rotate(currentDegrees);
        }

        #endregion
    }
}