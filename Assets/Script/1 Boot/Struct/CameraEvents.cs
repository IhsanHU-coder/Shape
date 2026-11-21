using UnityEngine;

public struct CameraBoostChanged
{
    public bool IsBoosting;

    public CameraBoostChanged(bool isBoosting)
    {
        IsBoosting = isBoosting;
    }
}

public struct CameraFreezeRequested { }

public struct CameraUnfreezeRequested { }

public struct CameraCutsceneRequested
{
    public Transform Target;

    public CameraCutsceneRequested(Transform target)
    {
        Target = target;
    }
}

public struct CameraFinishRequested { }