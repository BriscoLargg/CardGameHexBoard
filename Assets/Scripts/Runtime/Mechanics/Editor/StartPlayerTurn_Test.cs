﻿using HexCardGame.Runtime.Game;
using NUnit.Framework;

namespace HexCardGame.Runtime.Test
{
    public partial class Mechanics_Test : IStartPlayerTurn
    {
        private IPlayer _player;

        public void OnStartPlayerTurn(IPlayer player)
        {
            _player = player;
            EventReceived = true;
        }

        [Test]
        public void StartPlayerTurn_Test()
        {
            Game.StartPlayerTurn();
            Assert.IsTrue(EventReceived);
            Assert.IsTrue(Game.TurnLogic.CurrentSeatType == _player.Seat);
        }
    }
}