using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject modeSelection;
    public Button StartBtn;

    void Start()
    {
        startPanel.SetActive(true);
        modeSelection.SetActive(false);
        StartBtn.onClick.AddListener(SelectMode);
    }
    
    void SelectMode()
    {
        startPanel.SetActive(false);
        modeSelection.SetActive(true);

    }
}
