using UnityEngine;

public class OpenLink : MonoBehaviour
{
    public string url; // ссылка, которую нужно открыть

    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}