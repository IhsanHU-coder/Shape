using System;
using PapayaKit.Core;
using UnityEngine;

namespace Papayakit.Ihsan
{
public class PlayerDeathController : MonoBehaviour, IPostInitializable, IDisposable
{
    [Header("References")]
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private MeshRenderer playerRenderer;

    private bool _isDead;

    public void PostInitialize()
    {
        GameContext.SceneEvents.Subscribe<PlayerRockCollision>(OnRockCollision);
        GameContext.SceneEvents.Subscribe<PlayerRespawnRequested>(OnRespawn);
    }

    private void OnRockCollision(PlayerRockCollision evt)
    {
        if (_isDead)
            return;

        _isDead = true;

        GameContext.SceneEvents.Publish(new PlayerDeathRequested());
        GameContext.SceneEvents.Publish(new CameraFreezeRequested());
        GameContext.SceneEvents.Publish(new RetryButtonRequested());

        if (bloodParticle != null)
        {
            GameObject blood = Instantiate(
                bloodParticle,
                evt.Position,
                Quaternion.identity
            );

            Destroy(blood, 3f);
        }

        if (playerRenderer != null)
            playerRenderer.enabled = false;
    }

    private void OnRespawn(PlayerRespawnRequested evt)
    {
        _isDead = false;

        if (playerRenderer != null)
            playerRenderer.enabled = true;
    }

    public void Dispose()
    {
        GameContext.SceneEvents.Unsubscribe<PlayerRockCollision>(OnRockCollision);
        GameContext.SceneEvents.Unsubscribe<PlayerRespawnRequested>(OnRespawn);
    }
}
}