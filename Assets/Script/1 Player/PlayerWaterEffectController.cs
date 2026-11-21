using System;
using PapayaKit.Core;
using UnityEngine;

namespace Papayakit.Ihsan
{
public class PlayerWaterEffectController : MonoBehaviour, IPostInitializable, IDisposable
{
    [SerializeField] private GameObject waterSplashParticle;
    [SerializeField] private Transform player;

    public void PostInitialize()
    {
        GameContext.SceneEvents.Subscribe<PlayerWaterCollision>(OnWaterCollision);
    }

    private void OnWaterCollision(PlayerWaterCollision evt)
    {
        GameContext.SceneEvents.Publish(new CameraFreezeRequested());
        GameContext.SceneEvents.Publish(new RetryButtonRequested());

        GameObject splash = Instantiate(
            waterSplashParticle,
            player != null ? player.position : evt.Position,
            Quaternion.identity
        );

        Destroy(splash, 3f);
    }

    public void Dispose()
    {
        GameContext.SceneEvents.Unsubscribe<PlayerWaterCollision>(OnWaterCollision);
    }
}
}