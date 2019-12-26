using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManagerF : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1"))
        {
            SceneManager.LoadScene("Scenef1");
        }
        else if (Input.GetKey("2"))
        {
            SceneManager.LoadScene("Scenef2");
        }
        else if (Input.GetKey("3"))
        {
            SceneManager.LoadScene("Scenef3");
        }
        else if (Input.GetKey("4"))
        {
            SceneManager.LoadScene("Scenef4");
        }
        else if (Input.GetKey("5"))
        {
            SceneManager.LoadScene("infinitePlane");
        }
        else if (Input.GetKey("6"))
        {
            SceneManager.LoadScene("ScenaryWithOutPhysics");
        }
        else if (Input.GetKey("7"))
        {
            SceneManager.LoadScene("ScenaryWithPhysics");
        }        
        else if ( Input.GetKey("escape") )
        {
            Application.Quit();
        }
    }
}