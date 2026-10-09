using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [Header("Kamera Kayd�rma Ayarlar�")]
    [Tooltip("Kule, kameran�n merkezinden ne kadar yukar� ��karsa kamera hareket etsin?")]
    public float yThreshold = 2.5f;

    [Tooltip("S�n�r ge�ildi�inde kamera tek seferde ne kadar yukar� kays�n?")]
    public float moveUpAmount = 3.0f;

    [Tooltip("Kameran�n yukar� kayma yumu�akl��� (D���k = daha yava�/smooth)")]
    public float smoothSpeed = 4f;

    private float targetY; // Kameran�n gitmek istedi�i yeni Y koordinat�

    void Start()
    {
        // Oyun ba�lad���nda kameran�n ilk y�ksekli�ini hedef olarak belirliyoruz
        targetY = transform.position.y;
    }

    void Update()
    {
        float highestBlockY = GetHighestBlockY();

        // E�er en y�ksek blo�un Y koordinat�, kameran�n tetiklenme s�n�r�n� ge�tiyse
        if (highestBlockY > targetY + yThreshold)
        {
            // Kameran�n yeni hedefini moveUpAmount kadar yukar� ta��!
            targetY += moveUpAmount;
        }

        // Kameray� her frame'de yava��a yeni hedefine do�ru kayd�r (Lerp ile p�r�zs�zle�tirme)
        Vector3 targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
    }

    // Sahnedeki (Block etiketine sahip) en y�ksek blo�u bulan yard�mc� fonksiyon
    float GetHighestBlockY()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        float maxY = -100f; // Referans i�in en dip nokta

        foreach (GameObject block in blocks)
        {
            // E�er inceledi�imiz blok �u anki en y�ksekten daha yukar�daysa, yeni rekor budur
            if (block.transform.position.y > maxY)
            {
                maxY = block.transform.position.y;
            }
        }
        return maxY;
    }
}