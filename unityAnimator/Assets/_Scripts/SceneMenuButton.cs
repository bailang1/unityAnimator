using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneMenuButton : MonoBehaviour
{

    private string sceneName;

    // Start is called before the first frame update
    public void initialize(string sceneTitle_, string sceneName_, Transform menuTransform, bool isActive)
    {
        this.sceneName = sceneName_;
        this.GetComponentInChildren<Text>().text = sceneTitle_;
        Button sceneButton = this.GetComponent<Button>();
        sceneButton.onClick.AddListener(() => { btnClicked(); });
        sceneButton.enabled = isActive;
        this.transform.SetParent(menuTransform.transform);
        this.transform.localScale = Vector3.one;
    }

    public void btnClicked()
    {
        SceneManager.LoadScene(this.sceneName);
    }
}