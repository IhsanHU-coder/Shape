public struct MusicVolumeChanged
{
    public float Value;

    public MusicVolumeChanged(float value)
    {
        Value = value;
    }
}

public struct DayTimeChanged
{
    public float Value;

    public DayTimeChanged(float value)
    {
        Value = value;
    }
}

public struct ReverseScrollChanged
{
    public bool Value;

    public ReverseScrollChanged(bool value)
    {
        Value = value;
    }
}