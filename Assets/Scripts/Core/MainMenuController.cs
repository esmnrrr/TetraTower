using UnityEngine;
using UnityEngine.SceneManagement; // Sahne ge�i�i i�in

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // Kendi oyun sahnenin ad�n� buraya tam olarak yaz (�rn: "SampleScene")
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        // Oyundan ��k�� yapar (Unity Editor i�inde �al��maz, build al�nca �al���r)
        Debug.Log("Oyundan ��k�l�yor...");
        Application.Quit();
    }
}