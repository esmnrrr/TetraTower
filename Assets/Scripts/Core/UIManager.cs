// using UnityEngine;
// using UnityEngine.UI; // Panellerin arka plan rengini de�i�tirmek i�in bu �art
// using TMPro;

// public class UIManager : MonoBehaviour
// {
//     [Header("HUD Elemanlar�")]
//     public TextMeshProUGUI turnText;    // Oyuncu s�ras�n� g�stermek i�in

//     [Header("B�l�nm�� Game Over Ekran�")]
//     public GameObject gameOverPanel;    // Game Over panelini g�stermek i�in

//     // Sol Taraf (Oyuncu 1)
//     public Image p1Background;          // Arka plan rengini de�i�tirmek i�in
//     public TextMeshProUGUI p1ResultText;// Sonu� metnini g�stermek i�in

//     // Sa� Taraf (Oyuncu 2)
//     public Image p2Background;
//     public TextMeshProUGUI p2ResultText;

//     private void OnEnable()
//     {
//         GameManager.OnGameStateChanged += UpdateUI;
//     }

//     private void OnDisable()
//     {
//         GameManager.OnGameStateChanged -= UpdateUI;
//     }

//     private void UpdateUI(GameState newState)
//     {
//         switch (newState)
//         {
//             case GameState.WaitingForP1:
//                 turnText.text = "PLAYER 1 TURN";
//                 turnText.color = Color.red;
//                 gameOverPanel.SetActive(false);
//                 break;

//             case GameState.WaitingForP2:
//                 turnText.text = "PLAYER 2 TURN";
//                 turnText.color = Color.blue;
//                 gameOverPanel.SetActive(false);
//                 break;

//             case GameState.CheckingBalance:
//                 turnText.text = "BALANCE CHECKING...";
//                 turnText.color = Color.yellow;
//                 break;

//             case GameState.GameOver:
//                 turnText.gameObject.SetActive(false);
//                 gameOverPanel.SetActive(true);

//                 int kuleyiYikan = GameManager.Instance.currentPlayer;
//                 Color redColor = new Color(1f, 0.2f, 0.2f, 0.7f);
//                 Color greenColor = new Color(0.2f, 1f, 0.2f, 0.7f);

//                 // GameOverUI bile�enlerini yaz�lardan �ekiyoruz
//                 GameOverUI p1Juice = p1ResultText.GetComponent<GameOverUI>();
//                 GameOverUI p2Juice = p2ResultText.GetComponent<GameOverUI>();

//                 if (kuleyiYikan == 1)
//                 {
//                     // Oyuncu 1 kuleyi y�kt� ve KAYBETT�
//                     p1Background.color = redColor;
//                     p1ResultText.text = "LOSE!";
//                     if (p1Juice != null) p1Juice.PlayLoseAnimation(); // P1 Titreyip d��ecek

//                     p2Background.color = greenColor;
//                     p2ResultText.text = "YOU WIN!";
//                     if (p2Juice != null) p2Juice.PlayWinAnimation(); // P2 Yaylanarak b�y�yecek
//                 }
//                 else
//                 {
//                     // Oyuncu 2 kuleyi y�kt� ve KAYBETT�
//                     p2Background.color = redColor;
//                     p2ResultText.text = "LOSE!";
//                     if (p2Juice != null) p2Juice.PlayLoseAnimation();

//                     p1Background.color = greenColor;
//                     p1ResultText.text = "YOU WIN!";
//                     if (p1Juice != null) p1Juice.PlayWinAnimation();
//                 }
//                 break;
//         }
//     }
// }