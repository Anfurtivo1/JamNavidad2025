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
    private HorseController controller;
    public TextMeshProUGUI text;
    public bool completed = false;
    public GameObject stopWatch;
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (!completed)
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
