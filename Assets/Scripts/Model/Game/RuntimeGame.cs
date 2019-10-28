﻿using System.Collections.Generic;
using HexCardGame.Model.GameBoard;
using HexCardGame.Model.GamePool;
using HexCardGame.Model.TurnLogic;

namespace HexCardGame.Model.Game
{
    /// <summary>  Simple concrete Game Implementation. TODO: Consider to break this class down into small partial classes. </summary>
    public class RuntimeGame : IGame
    {
        #region Constructor

        public RuntimeGame(IGameController controller, List<IPlayer> players, GameParameters gameParameters,
            EventsDispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            TurnLogic = new TurnMechanics(players);
            BattleFsm = new BattleFsm(controller, this, gameParameters, dispatcher);

            ProcessPreStartGame = new PreStartGameMechanics(this);
            ProcessStartGame = new StartGameMechanics(this);
            ProcessStartPlayerTurn = new StartPlayerTurnMechanics(this);
            ProcessFinishPlayerTurn = new FinishPlayerTurnMechanics(this);
            ProcessFinishGame = new FinishGameMechanics(this);
            Board = new Board(gameParameters, dispatcher);
            Pool = new Pool(gameParameters, dispatcher);

            var libData = new Dictionary<PlayerId, List<object>>
            {
                {PlayerId.User, new List<object>()},
                {PlayerId.Enemy, new List<object>()}
            };
            Library = new Library(libData, Dispatcher);
        }

        #endregion


        #region Properties
    
        public EventsDispatcher Dispatcher { get; }
        public BattleFsm BattleFsm { get; }
        public bool IsGameStarted { get; set; }
        public bool IsGameFinished { get; set; }
        public bool IsTurnInProgress { get; set; }

        #region Mechanics

        public ITurnLogic TurnLogic { get; }
        PreStartGameMechanics ProcessPreStartGame { get; }
        StartGameMechanics ProcessStartGame { get; }
        StartPlayerTurnMechanics ProcessStartPlayerTurn { get; }
        FinishPlayerTurnMechanics ProcessFinishPlayerTurn { get; }
        FinishGameMechanics ProcessFinishGame { get; }
        Board Board { get; }
        Pool Pool { get; }
        Library Library { get; }
        #endregion

        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region Execution

        public void PreStartGame() => ProcessPreStartGame.Execute();

        public void StartGame() => ProcessStartGame.Execute();

        public void StartCurrentPlayerTurn() => ProcessStartPlayerTurn.Execute();

        public void FinishCurrentPlayerTurn() => ProcessFinishPlayerTurn.Execute();

        public void ExecuteAiTurn(PlayerId id)
        {
        }

        public void ForceWin(PlayerId id)
        {
            var player = TurnLogic.GetPlayer(id);
            ProcessFinishGame.Execute(player);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------------
    }
}