using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1f;
    }

}
