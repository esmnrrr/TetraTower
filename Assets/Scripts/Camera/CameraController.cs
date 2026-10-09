using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Kamera Döndürme Ayarları")]
    public float rotationSpeed = 200f; // Dönüş hızı

    void Update()
    {
        // Fare tekerleğine (Middle Mouse - Buton 2) basılı tutuluyorsa
        if (Input.GetMouseButton(2))
        {
            // Farenin X eksenindeki hareketini al
            float mouseX = Input.GetAxis("Mouse X");

            // Pivot objesini kendi etrafında (Y ekseninde) döndür
            transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}