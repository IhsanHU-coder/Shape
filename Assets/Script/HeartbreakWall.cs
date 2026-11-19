using UnityEngine;

public class HeartbreakWall : MonoBehaviour
{
    [Header("Bounce Settings")]
    [Tooltip("Persentase kecepatan yang dipertahankan saat memantul (0 = berhenti, 1 = secepat sebelumnya)")]
    [SerializeField] private float bounceRetention = 0.75f;

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah yang menabrak adalah kubus player
        CubeGridMovement player = other.GetComponent<CubeGridMovement>();
        
        if (player != null)
        {
            // PANTULKAN MOMENTUM PLAYER
            // 1. Balikkan arah gerakan saat ini secara instan
            player.currentDirection = -player.currentDirection;

            // 2. Akses kecepatan kubus saat ini melalui reflection atau buat currentVelocity jadi public
            // (Pastikan kamu sudah mengubah 'currentVelocity' di CubeGridMovement.cs menjadi PUBLIC)
            
            // Ambil script movement player dan modifikasi kecepatannya
            System.Reflection.FieldInfo velocityField = typeof(CubeGridMovement).GetField("currentVelocity", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            
            if (velocityField != null)
            {
                float currentV = (float)velocityField.GetValue(player);
                
                // Kurangi sedikit kecepatannya sebagai efek benturan keras, lalu balikkan energinya
                float bouncedV = currentV * bounceRetention;
                
                // Pastikan minimal ada tenaga pantulan agar tidak macet di dalam dinding
                if (bouncedV < 6f) bouncedV = 6f; 

                velocityField.SetValue(player, bouncedV);
            }

            // 3. Paksa kubus keluar dari coroutine lamanya agar langsung memantul ke belakang di frame yang sama
            player.StopAllCoroutines();
            
            // Akses bool isMoving agar kubus bisa bergerak lagi ke arah sebaliknya
            System.Reflection.FieldInfo isMovingField = typeof(CubeGridMovement).GetField("isMoving", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (isMovingField != null) isMovingField.SetValue(player, false);

            // Sedikit efek guncangan visual (Opsional jika mau ditambah partikel nanti)
            Debug.Log("💔 Heartbreak! Player memantul!");
        }
    }
}