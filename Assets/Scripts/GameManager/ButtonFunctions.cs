using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void resume()
    {
        GameManager.instance.unPauseState();
    }

    public void settings()
    {
        if(SceneManager.GetActiveScene().ToString() == "MainMenu")
        {

        }
        else
        {
           GameManager.instance.OpenSettings();
        }
        
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
