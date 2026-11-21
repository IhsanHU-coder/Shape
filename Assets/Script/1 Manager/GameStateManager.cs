using UnityEngine;
using PapayaKit.Core;

public enum GameState
{
    Playing,
    Paused,
    Dead,
    Finished
}

namespace Papayakit.Ihsan
{
public class GameStateManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    private void OnEnable()
    {
        GameContext.SceneEvents.Subscribe<GamePausedChanged>(OnPauseChanged);
        GameContext.SceneEvents.Subscribe<PlayerDeathRequested>(OnPlayerDeath);
        GameContext.SceneEvents.Subscribe<GameWinRequested>(OnGameWin);
    }

    private void Start()
    {
        SetState(GameState.Playing);
    }

    private void OnPauseChanged(GamePausedChanged evt)
    {
        if (CurrentState == GameState.Finished ||
            CurrentState == GameState.Dead)
            return;

        SetState(evt.IsPaused ? GameState.Paused : GameState.Playing);
    }

    private void OnPlayerDeath(PlayerDeathRequested evt)
    {
        SetState(GameState.Dead);
    }

    private void OnGameWin(GameWinRequested evt)
    {
        SetState(GameState.Finished);

        GameContext.SceneEvents.Publish(new GameTimerStopped());
        GameContext.SceneEvents.Publish(new GameFinished());
        GameContext.SceneEvents.Publish(new CameraFinishRequested());
    }

    private void SetState(GameState state)
    {
        CurrentState = state;
    }

    private void OnDisable()
    {
        GameContext.SceneEvents.Unsubscribe<GamePausedChanged>(OnPauseChanged);
        GameContext.SceneEvents.Unsubscribe<PlayerDeathRequested>(OnPlayerDeath);
        GameContext.SceneEvents.Unsubscribe<GameWinRequested>(OnGameWin);
    }
}
}