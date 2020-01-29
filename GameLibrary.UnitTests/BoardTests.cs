using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameLibrary.UnitTests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void MatchingPattern_NobodyHasPattern_Returns0()
        {
            // Arrange
            Board board = new Board(7, 6);

            // Act
            int result = board.MatchingPattern(new int[] { 0, 0, 0, 0, 1, 1, 2 });

            // Assert
            Assert.Equals(result, 0);
        }

        [TestMethod]
        public void MatchingPattern_Player1HasPattern_Returns1()
        {
            // Arrange
            Board board = new Board(7, 6);

            // Act
            int result = board.MatchingPattern(new int[] { 2, 1, 1, 1, 1, 2, 0 });

            // Assert
            Assert.Equals(result, 1);
        }

        [TestMethod]
        public void MatchingPattern_Player2HasPattern_Returns2()
        {
            // Arrange
            Board board = new Board(7, 6);

            // Act
            int result = board.MatchingPattern(new int[] { 1, 0, 2, 2, 2, 2, 1 });

            // Assert
            Assert.Equals(result, 1);
        }
    }
}
