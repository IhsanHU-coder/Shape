using UnityEngine;

public struct PlayerScrollInput
{
    public float ScrollValue;
    public bool IsLeftCtrlPressed;

    public PlayerScrollInput(float scrollValue, bool isLeftCtrlPressed)
    {
        ScrollValue = scrollValue;
        IsLeftCtrlPressed = isLeftCtrlPressed;
    }
}

public struct PlayerMovementChanged
{
    public Vector3 Direction;
    public float Velocity;
    public bool IsMoving;

    public PlayerMovementChanged(
        Vector3 direction,
        float velocity,
        bool isMoving)
    {
        Direction = direction;
        Velocity = velocity;
        IsMoving = isMoving;
    }
}

public struct PlayerWaterCollision
{
    public Vector3 Position;

    public PlayerWaterCollision(Vector3 position)
    {
        Position = position;
    }
}

public struct PlayerRockCollision
{
    public Vector3 Position;

    public PlayerRockCollision(Vector3 position)
    {
        Position = position;
    }
}

public struct PlayerWallCollision
{
    public Vector3 Position;

    public PlayerWallCollision(Vector3 position)
    {
        Position = position;
    }
}

public struct PlayerDeathRequested { }

public struct PlayerRespawnRequested { }