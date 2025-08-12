using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Objek yang akan diikuti oleh kamera (pemain).")]
    public Transform target;

    [Tooltip("Seberapa mulus kamera mengikuti target. Nilai kecil = lebih responsif, nilai besar = lebih 'tertinggal'.")]
    public float smoothTime = 0.2f;

    [Tooltip("Jarak kamera dari target. Atur Z ke -10 untuk game 2D.")]
    public Vector3 offset = new Vector3(0, 2, -10);

    // Variabel internal untuk menyimpan kecepatan pergerakan kamera
    private Vector3 velocity = Vector3.zero;

    // LateUpdate dipanggil setiap frame, SETELAH semua fungsi Update selesai.
    // Ini penting untuk kamera agar tidak ada getaran (jitter).
    private void LateUpdate()
    {
        // Jika tidak ada target, jangan lakukan apa-apa.
        if (target == null)
        {
            return;
        }

        // Tentukan posisi tujuan kamera (posisi target + offset)
        Vector3 targetPosition = target.position + offset;

        // Gerakkan kamera secara mulus dari posisi saat ini ke posisi tujuan
        // menggunakan SmoothDamp.
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
