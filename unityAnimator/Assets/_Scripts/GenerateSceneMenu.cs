using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GenerateSceneMenu : MonoBehaviour
{
    struct MenuScene
    {
        string sceneFileName;
        string sceneTitle;
        public MenuScene(string sceneFileName_, string sceneTitle_)
        {
            this.sceneFileName = sceneFileName_;
            this.sceneTitle = sceneTitle_;
        }
        public string getFileName()
        {
            return this.sceneFileName;
        }
        public string getTitle()
        {
            return this.sceneTitle;
        }
        public override string ToString()
        {
            return "{ " + this.sceneFileName + " --- " + this.sceneTitle + " }";
        }
    }

    #region InEditorVariables

    [SerializeField] private List<string> scenesFileNames;
    [SerializeField] private List<string> scenesTitles;
    [SerializeField] private Transform menuGameObject;
    [SerializeField] private GameObject menuButtonPrefab;
    #endregion
    private List<MenuScene> menuScenes = new List<MenuScene>();

    void Awake()
    {
        //Loads the scenes and use the scenes title as file name and viceversa if needed.
        int commonLength = Mathf.Min(this.scenesFileNames.Count, this.scenesTitles.Count);
        for (int i = 0; i < commonLength; ++i)
        {
            this.menuScenes.Add(new MenuScene(this.scenesFileNames[i], this.scenesTitles[i]));
        }
        if (this.scenesFileNames.Count < this.scenesTitles.Count)
        {
            for (int i = this.scenesFileNames.Count; i < this.scenesTitles.Count; ++i)
            {
                this.menuScenes.Add(new MenuScene(this.scenesTitles[i], this.scenesTitles[i]));
            }
        }
        else
        {
            for (int i = scenesTitles.Count; i < scenesFileNames.Count; ++i)
            {
                this.menuScenes.Add(new MenuScene(this.scenesFileNames[i], this.scenesFileNames[i]));
            }
        }
    }

    void Start()
    {
        for (int sceneId = 0; sceneId < this.menuScenes.Count; ++sceneId)
        {
            GameObject sceneButtonObject = Instantiate(menuButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            string sceneFileName = this.menuScenes[sceneId].getFileName();
            sceneButtonObject.GetComponent<SceneMenuButton>().initialize(this.menuScenes[sceneId].getTitle(), sceneFileName, menuGameObject, sceneFileName != SceneManager.GetActiveScene().name);
        }
    }

}