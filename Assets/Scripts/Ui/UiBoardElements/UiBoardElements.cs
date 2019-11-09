﻿using Game.Ui;
using HexCardGame.Runtime;
using HexCardGame.Runtime.Game;
using UnityEngine;
using UnityEngine.Tilemaps;
using Logger = Tools.Logger;

namespace HexCardGame.UI
{
    public class UiBoardElements : UiEventListener, ICreateBoardElement
    {
        Tilemap TileMap { get; set; }

        void ICreateBoardElement.OnCreateBoardElement(PlayerId id, BoardElement boardElement, Vector3Int position,
            CardHand card)
        {
            Logger.Log<UiBoard>("Create Board Element at: " + TileMap.WorldToCell(position));
            TileMap.SetTile(position, boardElement.Data.Tile);
        }

        protected override void Awake()
        {
            base.Awake();
            TileMap = GetComponentInChildren<Tilemap>();
        }

        Vector3 GetWorldPosition(Vector3Int position) => TileMap.GetCellCenterWorld(position);
    }
}