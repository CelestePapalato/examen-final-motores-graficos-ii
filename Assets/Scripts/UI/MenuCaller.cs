using UnityEngine;

public class MenuCaller : MonoBehaviour
{
    string menuName;

    public void Open()
    {
        MenuManager.Instance?.OpenMenu(menuName);
    }

    public void Close()
    {
        MenuManager.Instance?.CloseMenu(menuName);
    }
}
