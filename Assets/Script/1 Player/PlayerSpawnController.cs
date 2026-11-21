using UnityEngine;
using PapayaKit.Core;

namespace Papayakit.Ihsan
{
public class PlayerSpawnController : MonoBehaviour
{
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private void Start()
    {
        _spawnPosition = transform.position;
        _spawnRotation = transform.rotation;
    }

    public void Respawn()
    {
        StopAllCoroutines();

        transform.position = _spawnPosition;
        transform.rotation = _spawnRotation;

        GameContext.SceneEvents.Publish(new PlayerRespawnRequested());
        GameContext.SceneEvents.Publish(new CameraUnfreezeRequested());
    }
}
}