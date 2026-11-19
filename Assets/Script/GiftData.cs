using UnityEngine;

[CreateAssetMenu(fileName = "NewGift", menuName = "ShapeLove/New Gift")]
public class GiftData : ScriptableObject
{
    public string giftName;

    [TextArea]
    public string description;

    public Sprite icon;

    public int requiredAmount = 1;

    public Color giftColor = Color.white;
}