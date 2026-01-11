using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("MenuWin")]
    public GameObject menuWin;
    public GameObject[] listaEstrellas;

    [Header("MenuLose")]
    public GameObject menuLose;

    //public HorseController player;

    public void RestarLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
        

}
