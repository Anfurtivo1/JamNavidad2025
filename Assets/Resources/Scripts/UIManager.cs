using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelectMenu;
    public GameObject credits;
    public GameObject exitConfirmPopup;
    private GameObject activePanel;

    public AudioClip sonidoBotonJugar;
    public AudioClip sonidoBoton;
    public AudioClip sonidoCreditos;
    public AudioClip sonidoMenuPrincipal;
    public AudioSource srcMusica;
    public AudioSource srcSonidos;

    public void Start()
    {
        srcMusica.clip = sonidoMenuPrincipal;
        srcMusica.Play();
    }

    public void Show(GameObject gameObject)
    {
        if (activePanel != null)
        {
            activePanel.SetActive(false);
        }

        gameObject.SetActive(true);

        activePanel = gameObject;
    }

   public void GoToMainMenu()
    {
        srcSonidos.clip = sonidoBoton;
        srcSonidos.Play();

        srcMusica.clip = sonidoMenuPrincipal;
        srcMusica.Play();

        Show(mainMenu);
    }

    public void GoToFirstLevel()
    {
        srcSonidos.clip = sonidoBotonJugar;
        srcSonidos.Play();

        mainMenu.SetActive(false);

        SceneManager.LoadScene("Nivel1");
    }

    public void GoToCredits()
    {
        srcSonidos.clip = sonidoBoton;
        srcSonidos.Play();

        Show(credits);

        srcMusica.clip = sonidoCreditos;
        srcMusica.Play();
    }

    public void GoToLvlSelect()
    {
        srcSonidos.clip = sonidoBoton;
        srcSonidos.Play();

        Show(levelSelectMenu);
    }

    public void GoToLvl1()
    {
        srcSonidos.clip = sonidoBotonJugar;
        srcSonidos.Play();

        SceneManager.LoadScene("Nivel1");
    }

    public void GoToLvl2()
    {
        srcSonidos.clip = sonidoBotonJugar;
        srcSonidos.Play();

        SceneManager.LoadScene("Nivel2");
    }

    public void OpenExitConfirm()
    {
        srcSonidos.clip = sonidoBoton;
        srcSonidos.Play();

        exitConfirmPopup.SetActive(true);
    }

    public void ConfirmExitGame()
    {
        Application.Quit();
        EditorApplication.isPlaying = false;
    }

    public void CancelExitGame()
    {
        srcSonidos.clip = sonidoBoton;
        srcSonidos.Play();
        
        exitConfirmPopup.SetActive(false);
    }    
    public void GoBack()
    {
        srcSonidos.clip = sonidoBoton;
        srcSonidos.Play();

        GoToMainMenu();
    }
}
