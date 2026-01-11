using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public HorseController horse;
    public void Destroy()
    {
        if(horse != null)
        {
            if (horse.vidas <= 0)
            {
                horse.defeated = true;
            }
        }

        Destroy(gameObject);
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1f;
    }

}
