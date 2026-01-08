using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject levelSelectMenu;
    public GameObject credits;
    public GameObject exitConfirmPopup;
    private GameObject activePanel;

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
        Show(mainMenu);
    }

    public void GoToCredits()
    {
        Show(credits);
    }

    public void GoToLvlSelect()
    {
        Show(levelSelectMenu);
    }

    public void OpenExitConfirm()
    {
        exitConfirmPopup.SetActive(true);
    }

    public void ConfirmExitGame()
    {
        //Application.Quit();
        EditorApplication.isPlaying = false;
    }

    public void CancelExitGame()
    {
        exitConfirmPopup.SetActive(false);
    }    
    public void GoBack()
    {
        GoToMainMenu();
    }
}
