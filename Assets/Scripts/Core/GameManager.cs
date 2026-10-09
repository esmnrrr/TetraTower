using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    WaitingForChoice,
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

    [Header("3 Seçenek (UI) Sistemi")]
    public GameObject choicePanel; // Butonların içinde bulunduğu ana panel
    public Button[] choiceButtons; // 3 adet butonumuz
    private int[] currentChoices; // Ekrana gelen 3 parçanın indeksleri

    [Header("Parça İkonları")]
    public Sprite[] blockIcons; // Prefab dizisindeki sırayla aynı olacak resimler

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
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        
        // Oyunu P1'in sırasıyla başlatıyoruz
        ChangeState(GameState.WaitingForChoice);
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
                break;

            case GameState.WaitingForP2:
                currentPlayer = 2;
                break;

            case GameState.CheckingBalance:
                // Akıllı Fizik Hakemi devreye girer
                StartCoroutine(SmartPhysicsCheckRoutine());
                break;

            case GameState.GameOver:
                Debug.Log("KULE YIKILDI! Kaybeden: Oyuncu " + currentPlayer);
                
                // 1. Ekranı sars! (Eski kodundaki Observer tetikleyicisi)
                TriggerShake();

                // 2. Slow Motion (Yavaş Çekim) efekti - GDD'deki hissiyat hedefleri
                Time.timeScale = 0.3f; 
                Time.fixedDeltaTime = 0.02f * Time.timeScale; // Fizik motorunu da yavaşlatır
                
                // UI Paneli açıksa gizle
                if(choicePanel != null) choicePanel.SetActive(false);
                break;

            case GameState.WaitingForChoice:
                PrepareChoices();
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

        // Kule yıkılmadıysa sırayı diğer oyuncuya geçir ve panel aç
        currentPlayer = (currentPlayer == 1) ? 2 : 1; 
        ChangeState(GameState.WaitingForChoice);
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

    // Bu fonksiyon paneli açıp 3 butona rastgele parça atayacak
    public void PrepareChoices()
    {
        choicePanel.SetActive(true); // Paneli görünür yap
        currentChoices = new int[3];

        for (int i = 0; i < 3; i++)
        {
            // 3 rastgele parça seç
            int randomIndex = UnityEngine.Random.Range(0, blockPrefabs.Length);
            currentChoices[i] = randomIndex;
            
            // Butonun eski tıklama görevlerini temizleyip yeni görevi atıyoruz
            int buttonIndex = i; 
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(buttonIndex));
            
            // Ekstra: Eğer butonun içinde Text objesi varsa, parçanın ismini yazdırabilirsin
            Text btnText = choiceButtons[i].GetComponentInChildren<Text>();
            if(btnText != null) btnText.text = blockPrefabs[randomIndex].name;

            // Butonun üzerindeki Image bileşenini bul ve resmi değiştir
            Image btnImage = choiceButtons[i].GetComponent<Image>();
            
            // Eğer blockIcons dizisine resimleri koyduysak, butona o resmi ata
            if(btnImage != null && blockIcons.Length > randomIndex)
            {
                btnImage.sprite = blockIcons[randomIndex];
            }
        }
    }

    // Butona tıklandığında çalışacak fonksiyon
    public void OnChoiceSelected(int buttonIndex)
    {
        int selectedPrefabIndex = currentChoices[buttonIndex];
        
        choicePanel.SetActive(false); // Paneli gizle
        
        // Seçilen parçayı SpawnPoint merkezinde yarat
        if (blockPrefabs.Length > 0 && spawnPoint != null)
        {
            Instantiate(blockPrefabs[selectedPrefabIndex], spawnPoint.position, spawnPoint.rotation);
        }

        // Parça doğduktan sonra oyuncunun tutması (Waiting) durumuna geç
        if (currentPlayer == 1) ChangeState(GameState.WaitingForP1);
        else ChangeState(GameState.WaitingForP2);
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
