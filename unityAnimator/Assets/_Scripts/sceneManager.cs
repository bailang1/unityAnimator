using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("1"))
        {
            SceneManager.LoadScene("Scene1");
        }
        else if (Input.GetKey("2"))
        {
            SceneManager.LoadScene("Scene2");
        }
        else if (Input.GetKey("3"))
        {
            SceneManager.LoadScene("Scene3");
        }
        else if (Input.GetKey("4"))
        {
            SceneManager.LoadScene("Scene4");
        }
        else if (Input.GetKey("5"))
        {
            SceneManager.LoadScene("Scene5");
        }
        else if (Input.GetKey("6"))
        {
            SceneManager.LoadScene("Scene6");
        }
        else if (Input.GetKey("7"))
        {
            SceneManager.LoadScene("Scene7");
        }
        else if (Input.GetKey("8"))
        {
            SceneManager.LoadScene("Scene8");
        }
        else if (Input.GetKey(KeyCode.Q) || Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}