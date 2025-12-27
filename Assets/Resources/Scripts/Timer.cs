using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    private float timePassed;
    private float totalTime; //por hacer aun
    private HorseController controller;
    public TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timePassed = timePassed + Time.deltaTime;
        text.text = timePassed.ToString("F2"); 
       /* if (controller.levelPassed)
        {
            totalTime = timePassed;
        } */
    }
}
