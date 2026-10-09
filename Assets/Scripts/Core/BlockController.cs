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