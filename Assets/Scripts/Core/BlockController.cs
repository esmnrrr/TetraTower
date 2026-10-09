using UnityEngine;

public class BlockController : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasBeenPlaced = false;
    private bool isDragging = false;

    [Header("Döndürme Ayarları")]
    public float scrollRotationStep = 15f; // Sağ/sol tık veya tekerlek için derece

    // 3D uzayda fareyi takip etmek için kameraya olan derinlik mesafesi
    private float zDistanceToCamera;
    private Vector3 offset;

    [Header("Ghost Önizleme")]
    public Material ghostMaterial;
    private GameObject ghostObj;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Obje ilk doğduğunda havada asılı kalması için Kinematic yapıyoruz
        rb.isKinematic = true; 
    }

    void Update()
    {
        if (hasBeenPlaced) return;

        // Parça havadayken fare tekerleği (scroll) ile Z ekseninde döndürme
        if (isDragging)
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScroll > 0)
            {
                transform.Rotate(0, 0, scrollRotationStep, Space.World);
            }
            else if (mouseScroll < 0)
            {
                transform.Rotate(0, 0, -scrollRotationStep, Space.World);
            }

            // Sağ tık ile 90 derece döndürme (Flip) - GDD'deki mekanik
            if (Input.GetMouseButtonDown(1)) 
            {
                transform.Rotate(0, 90f, 0, Space.World);
            }
        }

        // Eğer parça taşınıyorsa ve hayalet varsa, onu aşağı yansıt
        if (isDragging && ghostObj != null)
        {
            ghostObj.transform.rotation = transform.rotation; // Dönüşü eşitle

            RaycastHit hit;
            // Objeden dümdüz aşağıya doğru bir ışın gönderiyoruz
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
            {
                // Hayaleti ışının çarptığı yere (platforma veya kuleye) yerleştir
                ghostObj.transform.position = hit.point;
            }
        }
    }

    void OnMouseDown()
    {
        if (hasBeenPlaced) return;

        isDragging = true;
        
        // Objenin kameraya olan Z derinliğini hesaplıyoruz
        zDistanceToCamera = Camera.main.WorldToScreenPoint(transform.position).z;
        
        // Farenin tıkladığı yer ile objenin merkezi arasındaki farkı buluyoruz
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        
        offset = transform.position - mouseWorldPosition;

        // Hayalet objeyi kendi kopyası olarak yarat
        ghostObj = Instantiate(gameObject, transform.position, transform.rotation);
        
        // Hayaletin oyunun fiziğini ve kodlarını bozmasını engelle
        Destroy(ghostObj.GetComponent<Rigidbody>());
        Destroy(ghostObj.GetComponent<MeshCollider>());
        Destroy(ghostObj.GetComponent<BlockController>());
        
        // Hayalete yarı saydam materyali ata
        if (ghostObj.GetComponent<MeshRenderer>() != null)
        {
            ghostObj.GetComponent<MeshRenderer>().material = ghostMaterial;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging || hasBeenPlaced) return;

        // Fare hareket ettikçe objeyi o derinlikte takip ettiriyoruz
        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDistanceToCamera);
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        
        transform.position = mouseWorldPosition + offset;
    }

    void OnMouseUp()
    {
        if (!isDragging || hasBeenPlaced) return;

        isDragging = false;
        DropBlock();
    }

    void DropBlock()
    {
        hasBeenPlaced = true;
        rb.isKinematic = false;
        gameObject.tag = "Block"; // GameManager'ın bu objeyi bulabilmesi için tag çok önemli

        // Parça bırakıldı, fizik hesaplaması (Settle) için GameManager'a haber veriyoruz
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.CheckingBalance);
        }

        if (ghostObj != null) Destroy(ghostObj); // Hayaleti yok et
    }

    // 3D Çarpışma Kontrolü (2D değil)
    void OnCollisionEnter(Collision collision)
    {
        // Yere veya platforma sert çarptığında çıkacak ses veya efektleri buraya ekleyeceğiz
        if (hasBeenPlaced && collision.relativeVelocity.magnitude > 1.5f)
        {
            Debug.Log("Çarpışma algılandı! Şiddet: " + collision.relativeVelocity.magnitude);
        }
    }
}