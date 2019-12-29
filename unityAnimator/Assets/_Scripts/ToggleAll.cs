using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class ToggleAll : MonoBehaviour
{
    [SerializeField] private List<GameObject> hiddenObjects;
    [SerializeField] private List<GameObject> showObjects;

    void Start()
    {
        Button button = this.GetComponent<Button>();
        button.onClick.AddListener(delegate { btnClicked(); });
    }

    public void btnClicked()
    {
        foreach (GameObject hiddenObject in hiddenObjects)
        {
            hiddenObject.SetActive(false);
        }
        foreach (GameObject showObject in showObjects)
        {
            showObject.SetActive(true);
        }
    }
}