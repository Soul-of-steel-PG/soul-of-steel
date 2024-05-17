using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public void Resume()
    {
        UIManager.Instance.ShowMenuPanel(false);
    }

    public void GoMenu()
    {
        UIManager.Instance.matchView.DisconnectPlayer();
    }
}