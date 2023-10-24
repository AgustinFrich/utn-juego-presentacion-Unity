using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageView : MonoBehaviour
{
    public TMP_Text messageText;
    public TMP_Text senderText;

    public void setMessage(string mensaje, string sender)
    {
        messageText.text = mensaje;
        senderText.text = sender;
    }
}
