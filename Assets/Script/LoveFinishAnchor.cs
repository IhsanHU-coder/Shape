using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveFinishAnchor : MonoBehaviour
{
    [Header("Finish Settings")]
    [SerializeField] private float suctionSpeed = 8f;

    [Header("Requirement Settings")]
    [Tooltip("Daftar barang bawaan wajib yang harus dipenuhi player sebelum ke sini")]
    [SerializeField] private List<GiftData> requiredGifts = new List<GiftData>();

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem celebrationParticles;

    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasFinished) return;

        CubeGridMovement player = other.GetComponent<CubeGridMovement>();
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (player != null && inventory != null)
        {
            // CEK APAKAH BARANGNYA CUKUP ATAU KURANG
            List<string> missingGiftsInfo = new List<string>();
            bool isAllRequirementsMet = true;

            foreach (GiftData gift in requiredGifts)
            {
                int owned = inventory.GetOwnedAmount(gift);
                if (owned < gift.requiredAmount)
                {
                    isAllRequirementsMet = false;
                    int missingCount = gift.requiredAmount - owned;
                    missingGiftsInfo.Add($"- {gift.giftName} (Kurang {missingCount} lagi)");
                }
            }

            // JIKA SEMUA SYARAT TERPENUHI -> DITERIMA (FINISH)
            if (isAllRequirementsMet)
            {
                hasFinished = true;
                player.StopAllCoroutines();
                player.enabled = false; 
                StartCoroutine(AnimateFinish(player.transform));
            }
            // JIKA KURANG -> DITOLAK MENTAH-MENTAH
            else
            {
                Debug.LogWarning("❌ Circle: 'Maaf, kamu belum siap meminangku!'");
                Debug.LogWarning("Kamu belum membawa atau kekurangan barang berikut:");
                foreach (string info in missingGiftsInfo)
                {
                    Debug.LogWarning(info);
                }

                // EFEK PANTULAN: Karena ditolak cewek, player otomatis galau dan mental mundur ke belakang!
                // Kita gunakan logika memantul yang mirip heartbreak wall agar player dipaksa mencari barang lagi
                player.currentDirection = -player.currentDirection;
                
                // Tambahkan dorongan balik instan agar player menjauh dari zona finish
                player.StopAllCoroutines();
            }
        }
    }

    private IEnumerator AnimateFinish(Transform playerTransform)
    {
        float elapsed = 0f;
        Vector3 startPos = playerTransform.position;
        Quaternion startRot = playerTransform.rotation;

        Vector3 targetPos = new Vector3(transform.position.x, playerTransform.position.y, transform.position.z);
        Quaternion targetRot = transform.rotation;

        if (celebrationParticles != null) celebrationParticles.Play();

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * suctionSpeed;
            playerTransform.position = Vector3.Lerp(startPos, targetPos, elapsed);
            playerTransform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed);
            yield return null;
        }

        playerTransform.position = targetPos;
        playerTransform.rotation = targetRot;
        Debug.Log("💖 SHAPE LOVE! Cinta kalian resmi bersatu karena persiapanmu matang!");
    }
}