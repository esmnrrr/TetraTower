using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Sars�nt� Ayarlar�")]
    public float shakeDuration = 0.15f;  // Ne kadar s�re sallanacak?
    public float shakeMagnitude = 0.1f;  // Ne kadar �iddetli sallanacak?

    // KURAL 1: Obje aktifle�ti�inde sinyale (radyo frekans�na) abone ol
    private void OnEnable()
    {
        GameManager.OnScreenShake += StartShake;
    }

    // KURAL 2: Obje kapan�rsa veya silinirse abonelikten ��k (Hata almamak i�in �ok �nemli!)
    private void OnDisable()
    {
        GameManager.OnScreenShake -= StartShake;
    }

    // Sinyal geldi�inde tetiklenecek fonksiyon
    private void StartShake()
    {
        // E�er zaten sallan�yorsa iptal et ve ba�tan ba�la (�st �ste binmeleri engeller)
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    // Ger�ek sars�nt�y� yapan zamanlay�c� fonksiyon
    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Rastgele X ve Y y�nlerinde minik kaymalar olu�tur
            float xOffset = Random.Range(-1f, 1f) * shakeMagnitude;
            float yOffset = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = new Vector3(originalPos.x + xOffset, originalPos.y + yOffset, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // Bir sonraki frame'e (kareye) kadar bekle
        }

        // Sars�nt� bitince kameray� tam olarak eski m�kemmel yerine oturt
        transform.position = originalPos;
    }
}