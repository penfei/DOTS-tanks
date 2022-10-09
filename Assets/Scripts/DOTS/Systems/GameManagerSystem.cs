﻿using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GameManagerSystem : ComponentSystem
{
    private EntityQuery _tankPlayerQuery;
    private ArchetypeChunkComponentType<TankPlayer> _tankPlayerComponentType;

    protected override void OnCreate()
    {
        _tankPlayerQuery = GetEntityQuery(ComponentType.ReadWrite<TankPlayer>());
        _tankPlayerComponentType = GetArchetypeChunkComponentType<TankPlayer>();
        
        // Create GameProgress singleton entity
        var progressEntity = EntityManager.CreateEntity();
        EntityManager.AddComponentData(progressEntity, new GameProgress
        {
            CurrentRound = 1,
        });
    }

    protected override void OnUpdate()
    {
        bool newGame = false;
        bool newRound = false;
        var tankPlayerChunks = _tankPlayerQuery.CreateArchetypeChunkArray(Allocator.TempJob);
        if (tankPlayerChunks.Length == 1)
        {
            if (tankPlayerChunks[0].Count == 1)
            {
                // Exactly one tank left alive. Its owner wins this round.
                var chunkPlayers = tankPlayerChunks[0].GetNativeArray(_tankPlayerComponentType);
                var winningPlayer = chunkPlayers[0];
                winningPlayer.Score += 1;
                
                var gameSettings = GetSingleton<GameSettings>();
                if (winningPlayer.Score == gameSettings.ScoreToWin)
                {
                    // game over, this player wins
                    newGame = true;
                }
                else
                {
                    // round over
                    newRound = true;
                }
            }
        }
        else if (tankPlayerChunks.Length == 0)
        {
            // No tanks left alive. This round is a tie.
            newRound = true;
        }
        tankPlayerChunks.Dispose();

        if (newRound)
        {
            var gameProgress = GetSingleton<GameProgress>();
            gameProgress.CurrentRound += 1;
            SetSingleton(gameProgress);
            // TODO: reactivate tanks
        }
        else if (newGame)
        {
            var gameProgress = GetSingleton<GameProgress>();
            gameProgress.CurrentRound = 1;
            SetSingleton(gameProgress);
        }
    }
}
