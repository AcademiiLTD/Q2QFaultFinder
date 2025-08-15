using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _messageText;

    public void PopulateNewChatMessage(string name, string message)
    {
        _nameText.text = name;
        _messageText.text = message;
    }
}
