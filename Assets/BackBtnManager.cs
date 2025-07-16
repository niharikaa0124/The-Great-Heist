using UnityEngine;
using UnityEngine.UI;
public class BackBtnManager : MonoBehaviour
{
    public GameObject startScreen;
    public GameObject modeScreen;
    public Button back;

    void Start()
    {
        startScreen.SetActive(false);
        modeScreen.SetActive(true);
        back.onClick.AddListener(Return);
    }

    void Return()
    {
        modeScreen.SetActive(false);
        startScreen.SetActive(true);
    }
}
