using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AgentChatView : MonoBehaviour
{
    [SerializeField] private GameObject _playerMessagePrefab, _avatarMessagePrefab, _errorMessagePrefab; //Prefabs for showing player and avatar messages
    [SerializeField] private RectTransform _messageListTransform; //List which will be the parent for messages
    [SerializeField] private GameObject _containerObject;
    [SerializeField] private Image _recordingIndicator;
    [SerializeField] private CanvasGroup _chatControlsCanvasGroup;
    [SerializeField] private List<GameObject> _conversationListItems; //This is going to be an object pool

    public void CreateNewMessage(string name, string message, MessageType messageType)
    {
        GameObject newMessage = null;

        switch (messageType)
        {
            case MessageType.USER:
                newMessage = Instantiate(_playerMessagePrefab, _messageListTransform);
                break;
            case MessageType.AGENT:
                newMessage = Instantiate(_avatarMessagePrefab, _messageListTransform);
                break;
            case MessageType.ERROR:
                newMessage = Instantiate(_errorMessagePrefab, _messageListTransform);
                break;
        }

        ChatMessage chatMessage = newMessage.GetComponent<ChatMessage>();
        chatMessage.PopulateNewChatMessage(name, message);
        newMessage.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_messageListTransform);
    }

    [ContextMenu("Player Message")]
    public void MakePlayerMessage()
    {
        CreateNewMessage("User", "Player test message", MessageType.USER);
    }

    [ContextMenu("Avatar Message")]

    public void MakeAvatarMessage()
    {
        CreateNewMessage("Agent", "Avatar test message", MessageType.AGENT);
    }

    [ContextMenu("Error Message")]

    public void MakeErrorMessage()
    {
        CreateNewMessage("ERROR", "An issue has occured", MessageType.ERROR);
    }

    public void ToggleChatWindow(bool state)
    {
        _chatControlsCanvasGroup.interactable = state;
    }

    public void ClearMessageList()
    {
        for (int i = 0; i < _messageListTransform.childCount; i++)
        {
            Destroy(_messageListTransform.GetChild(i).gameObject);
        }

    }

    public void ToggleRecordingIcon(bool state)
    {
        _recordingIndicator.enabled = state;
    }
}

public enum MessageType
{
    USER,
    AGENT,
    ERROR
}
