using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Kamus data untuk menyimpan: <Data Barang, Jumlah yang Dimiliki>
    private Dictionary<GiftData, int> currentInventory = new Dictionary<GiftData, int>();

    public void AddGift(GiftData gift)
    {
        if (currentInventory.ContainsKey(gift))
        {
            currentInventory[gift]++;
        }
        else
        {
            currentInventory.Add(gift, 1);
        }
    }

    public int GetOwnedAmount(GiftData gift)
    {
        if (currentInventory.ContainsKey(gift))
        {
            return currentInventory[gift];
        }
        return 0;
    }
}