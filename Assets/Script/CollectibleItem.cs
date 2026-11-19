using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Gift")]
    [SerializeField] private GiftData giftData;

    [Header("Effects")]
    [SerializeField] private ParticleSystem pickupParticle;
    [SerializeField] private AudioSource pickupSound;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        CubeGridMovement player = other.GetComponent<CubeGridMovement>();

        if (player == null || giftData == null)
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory == null)
            return;

        collected = true;

        inventory.AddGift(giftData);

        Debug.Log($"🎁 Berhasil mengambil {giftData.giftName}!");

        if (pickupParticle != null)
        {
            ParticleSystem effect = Instantiate(
                pickupParticle,
                transform.position,
                Quaternion.identity);

            Destroy(effect.gameObject, 3f);
        }

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound.clip, transform.position);

        Destroy(gameObject);
    }
}