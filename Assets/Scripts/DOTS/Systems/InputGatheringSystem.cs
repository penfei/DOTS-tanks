﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class InputGatheringSystem : ComponentSystem, TanksControls.IInGameActions
{
    // TODO - TankControls should be stored in its own singleton somewhere, and just used by systems, rather than owned here.
    public TanksControls TanksControls => tankControls;
    TanksControls tankControls;

    EntityQuery playersQuery;
    
    Entity player1Entity;
    Entity player2Entity;

    protected override void OnCreate()
    {
        // Create input
        tankControls = new TanksControls();
        tankControls.InGame.SetCallbacks(this);
        
        // Query
        playersQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[] { ComponentType.ReadOnly<TankPlayer>() }
        });
    }

    protected override void OnStartRunning()
    {
        if (playersQuery.CalculateEntityCount() == 0)
        {
            throw new InvalidOperationException("no player tanks detected");
        }
        var players = playersQuery.ToEntityArray(Allocator.TempJob);
        ComponentDataFromEntity<TankPlayer> tankPlayers = GetComponentDataFromEntity<TankPlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            TankPlayer tankPlayer = tankPlayers[players[i]];
            if (tankPlayer.PlayerId == 0)
            {
                player1Entity = players[i];
            }
            else if(tankPlayer.PlayerId == 1)
            {
                player2Entity = players[i];
            }
        }
        players.Dispose();
        tankControls.InGame.Enable();
    }

    protected override void OnUpdate()
    {
    }

    public void OnPlayer1Move(InputAction.CallbackContext context)
    {        
        Entity player = player1Entity;
        PlayerInputState playerInputState = EntityManager.GetComponentData<PlayerInputState>(player);
        playerInputState.Move = context.ReadValue<Vector2>();
        EntityManager.SetComponentData(player, playerInputState);        
    }

    public void OnPlayer2Move(InputAction.CallbackContext context)
    {
        Entity player = player2Entity;
        PlayerInputState playerInputState = EntityManager.GetComponentData<PlayerInputState>(player);
        playerInputState.Move = context.ReadValue<Vector2>();
        EntityManager.SetComponentData(player, playerInputState); 
    }

    public void OnPlayer1Shoot(InputAction.CallbackContext context)
    {
        Entity player = player1Entity;
        PlayerInputState playerInputState = EntityManager.GetComponentData<PlayerInputState>(player);
        playerInputState.Firing = context.ReadValue<float>();
        EntityManager.SetComponentData(player, playerInputState);
    }

    public void OnPlayer2Shoot(InputAction.CallbackContext context)
    {
        Entity player = player2Entity;
        PlayerInputState playerInputState = EntityManager.GetComponentData<PlayerInputState>(player);
        playerInputState.Firing = context.ReadValue<float>();
        EntityManager.SetComponentData(player, playerInputState);
    }
}
