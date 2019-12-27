using UnityEngine.UI;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField] private GameObject hiddenObject;
    [SerializeField] private GameObject showObject;

    void Start()
    {
        Button button = this.GetComponent<Button>();
        button.onClick.AddListener(delegate { btnClicked(); });
    }

    public void btnClicked()
    {
        hiddenObject.SetActive(false);
        showObject.SetActive(true);
    }
}