using System;
using PapayaKit.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Papayakit.Ihsan
{
public class PlayerInputService : MonoService, IPostInitializable, IDisposable
{
    private bool _isPaused;
    private bool _reverseScroll;

    public void PostInitialize()
    {
        GameContext.SceneEvents.Subscribe<GamePausedChanged>(OnPauseChanged);
        GameContext.SceneEvents.Subscribe<ReverseScrollChanged>(OnReverseChanged);
    }

    private void Awake()
    {
        _reverseScroll = PlayerPrefs.GetInt("ReverseScroll", 0) == 1;
    }

    private void Update()
    {
        if (Mouse.current == null || Keyboard.current == null)
            return;

        if (_isPaused)
            return;

        Vector2 scroll = Mouse.current.scroll.ReadValue();

        if (_reverseScroll)
            scroll.y = -scroll.y;

        if (Mathf.Abs(scroll.y) > 0.1f)
        {
            GameContext.SceneEvents.Publish(
                new PlayerScrollInput(
                    scroll.y,
                    Keyboard.current.leftCtrlKey.isPressed
                )
            );
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GameContext.SceneEvents.Publish(new PauseButtonRequested());
        }
    }

    private void OnPauseChanged(GamePausedChanged evt)
    {
        _isPaused = evt.IsPaused;
    }

    private void OnReverseChanged(ReverseScrollChanged evt)
    {
        _reverseScroll = evt.Value;
    }

    public void Dispose()
    {
        GameContext.SceneEvents.Unsubscribe<GamePausedChanged>(OnPauseChanged);
        GameContext.SceneEvents.Unsubscribe<ReverseScrollChanged>(OnReverseChanged);
    }
}
}