using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    private float timePassed;
    public float totalTime;
    private HorseController controller;
    public TextMeshProUGUI text;
    public bool completed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!completed)
        {
            timePassed = timePassed + Time.deltaTime;
            text.text = timePassed.ToString("F2"); 
        }
        
       /* if (controller.levelPassed)
        {
            totalTime = timePassed;
        } */
    }
}
