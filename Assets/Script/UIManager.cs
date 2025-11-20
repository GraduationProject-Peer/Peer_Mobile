using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject mainPanel;

    public void OnStartButtonClicked()
    {
        startPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
}

