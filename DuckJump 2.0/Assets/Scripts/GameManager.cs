using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;   
    }

    public void StartGameTimer()
    {

    }

    public void StopGameTimer()
    {

    }

    public void RestartGame()
    {

    }

    public void StartGame()
    {

    }

    public void PauseGame()
    {

    }
}
