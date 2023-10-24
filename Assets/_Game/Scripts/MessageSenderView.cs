using UnityEngine;

public class MessageSenderView : MonoBehaviour
{
    public GameObject messageCanvas;
    public GameObject gameCanvas;

    public void showMessageCanvas(bool mostrar)
    {
        messageCanvas.SetActive(mostrar);
        gameCanvas.SetActive(!mostrar);
    }
}
