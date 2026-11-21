using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class PlayerCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GameContext.SceneEvents.Publish(
                new PlayerWaterCollision(transform.position)
            );
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            GameContext.SceneEvents.Publish(
                new PlayerWallCollision(transform.position)
            );
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Rock"))
        {
            GameContext.SceneEvents.Publish(
                new PlayerRockCollision(transform.position)
            );
        }
    }
}
}