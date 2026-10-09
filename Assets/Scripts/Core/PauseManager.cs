using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject pausePanel;

    // Oyunun durdurulup durdurulmad���n� takip eden anahtar
    private bool isPaused = false;

    void Update()
    {
        // Klavyeden ESC tu�una bas�ld���nda tetiklenir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // A��ksa kapat
            }
            else
            {
                PauseGame(); // Kapal�ysa a�
            }
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Oyunun zaman ak���n� 0'a indirir (Fizik donar)
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Zaman�n ak���n� tekrar normale �evirir
        isPaused = false;
    }

    public void LoadMainMenu()
    {
        //Zamanin ak���n� normale �evirir ve ana men� sahnesine ge�i� yapar
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan ��k�l�yor...");
        Application.Quit();
    }
}