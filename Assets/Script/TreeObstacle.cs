using UnityEngine;

public class TreeObstacle : MonoBehaviour
{
    private Rigidbody rb;
    private bool isBroken = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Di awal game, pastikan pohon kokoh (isKinematic = true)
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        else
        {
            Debug.LogError($"Rigidbody tidak ditemukan di {gameObject.name}! Silakan tambahkan Rigidbody.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Jika sudah hancur/tumbang, jangan proses lagi
        if (isBroken) return;

        // Cek apakah yang menabrak adalah Player
        CubeGridMovement playerMovement = other.GetComponent<CubeGridMovement>();

        if (playerMovement != null)
        {
            // Ambil kecepatan saat ini dari script gerakan player
            float playerSpeed = playerMovement.currentVelocity;

            if (playerSpeed >= 5f)
            {
                // JIKA KECEPATAN LEBIH DARI ATAU SAMA DEGAN 5: Pohon hancur/tumbang
                isBroken = true;
                rb.isKinematic = false;

                // Opsional: Beri sedikit dorongan ke pohon ke arah gerakan player agar efek tumbangnya bagus
                Vector3 pushDirection = playerMovement.currentDirection;
                rb.AddForce(pushDirection * playerSpeed * 2f, ForceMode.Impulse);
                
                Debug.Log($"Pohon ditabrak! Kecepatan player {playerSpeed} (>= 5). Pohon Tumbang!");
            }
            else
            {
                // JIKA KECEPATAN KURANG DARI 5: Pohon tetap kokoh, player mati / reset
                rb.isKinematic = true;
                
                // Memicu fungsi retry/kematian di player karena menabrak pohon dalam keadaan pelan
                TriggerPlayerDeath(playerMovement);
                
                Debug.Log($"Pohon ditabrak pelan! Kecepatan player {playerSpeed} (< 5). Player Mati!");
            }
        }
    }

    private void TriggerPlayerDeath(CubeGridMovement player)
    {
        // Menggunakan logika kematian yang mirip dengan batu (Rock) di script kamu
        player.StopAllCoroutines();
        
        // Panggil tombol retry dari komponen bawaan script player kamu
        // Karena variabel btnAnim di script player bersifat private, pastikan game manager kamu menghandle ini
        // Atau kita bisa panggil method langsung jika metodenya public.
        
        // Contoh mematikan movement player:
        player.currentVelocity = 0f;
        player.currentDirection = Vector3.zero;
        player.isMoving = false;
        player.enabled = false;
    }
}