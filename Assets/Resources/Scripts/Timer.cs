using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class Timer : MonoBehaviour
{
    private float timePassed;
    public float totalTime;
    public HorseController controller;
    public TextMeshProUGUI text;
    public bool completed = false;
    public GameObject stopWatch;
    public Animator animator;
    public CameraFollow2D musica;
    public AudioClip musicaLoop;
    public float tiempoPreLoop;

    public void Start()
    {
        StartCoroutine(ChangeMusic());
    }

    IEnumerator ChangeMusic()
    {
        yield return new WaitForSeconds(tiempoPreLoop);
        musica.srcMusica.clip = musicaLoop;
        musica.srcMusica.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!completed && !controller.defeated)
        {
            timePassed = timePassed + Time.deltaTime;
            text.text = timePassed.ToString("F2");

            int seconds = Mathf.FloorToInt(timePassed);

            if(seconds % 10 == 0)
            {
                animator.SetTrigger("Moverse");
            }
        }
    }
}
