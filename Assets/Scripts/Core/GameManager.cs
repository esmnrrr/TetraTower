using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    WaitingForP1,
    WaitingForP2,
    Dragging,
    CheckingBalance,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState currentState;

    [Header("Blok ve Spawn Ayarları")]
    public GameObject[] blockPrefabs;
    public Transform spawnPoint;

    public int currentPlayer = 1;

    // OBSERVER PATTERN Sinyalleri
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnScreenShake;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        OnScreenShake = null;
        OnGameStateChanged = null;
    }

    public static void TriggerShake()
    {
        OnScreenShake?.Invoke();
    }

    private void Start()
    {
        // Oyunu P1'in sırasıyla başlatıyoruz
        ChangeState(GameState.WaitingForP1);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // UI ve diğer sistemlere durumun değiştiğini haber ver
        OnGameStateChanged?.Invoke(currentState);

        switch (currentState)
        {
            case GameState.WaitingForP1:
                currentPlayer = 1;
                SpawnBlock();
                break;

            case GameState.WaitingForP2:
                currentPlayer = 2;
                SpawnBlock();
                break;

            case GameState.CheckingBalance:
                // Akıllı Fizik Hakemi devreye girer
                StartCoroutine(SmartPhysicsCheckRoutine());
                break;

            case GameState.GameOver:
                Debug.Log("GAME OVER! Loser: Player " + currentPlayer);
                break;
        }
    }

    // AKILLI FİZİK KONTROLÜ (Denge Dedektörü)
    private IEnumerator SmartPhysicsCheckRoutine()
    {
        // Bloğun düşüp fiziğin başlaması için ufak avans
        yield return new WaitForSeconds(0.5f);

        bool isStable = false;

        // Kule tamamen durana kadar döngü çalışır
        while (!isStable)
        {
            // Kule yıkıldıysa sistemi anında durdur (Cezayı o anki oyuncuya kes)
            if (currentState == GameState.GameOver)
            {
                yield break;
            }

            isStable = true;

            // Tüm blokların hareketini (velocity) kontrol et
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject block in blocks)
            {
                // Rigidbody2D DEĞİL, normal Rigidbody kullanıyoruz
                Rigidbody rb = block.GetComponent<Rigidbody>();

                // Hız 0.05'ten büyükse blok hala hareket ediyor demektir
                if (rb != null && rb.linearVelocity.magnitude > 0.05f)
                {
                    isStable = false;
                    break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Kule yıkılmadı ve tamamen durduysa sırayı diğer oyuncuya geçir
        if (currentPlayer == 1)
        {
            ChangeState(GameState.WaitingForP2);
        }
        else
        {
            ChangeState(GameState.WaitingForP1);
        }
    }

    // KURŞUN GEÇİRMEZ SPAWN MANTIĞI
    void SpawnBlock()
    {
        if (blockPrefabs.Length > 0 && spawnPoint != null)
        {
            // Rastgele bir parça seç
            int randomIndex = UnityEngine.Random.Range(0, blockPrefabs.Length);

            // 3D sistemde kameraya göre hesaplama yapmıyoruz. 
            // Objeyi doğrudan belirlediğimiz SpawnPoint'in tam merkezinde oluşturuyoruz!
            Instantiate(blockPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
