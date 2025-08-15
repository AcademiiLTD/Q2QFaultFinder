using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AgentChatController : Controller
{
    [SerializeField] private string _apiKey, _apiURL, _characterKey, _sessionID;
    [SerializeField] private string _customCharacterName;
    [SerializeField] private AgentChatView _chatView;
    [SerializeField] private TMP_InputField _inputField;

    private bool _waitingForResponse;

    private void Start()
    {
        _sessionID = "-1"; //This starts a new session on Convai if the ID is -1
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) //Eventually want this to be changed to the newer input system, but this works for now
        {
            AttemptSendMessage();
        }
    }

    public void AttemptSendMessage()
    {
        if (!string.IsNullOrEmpty(_inputField.text))
        {
            if (_waitingForResponse) return;
            SendTextMessage();
        }
    }

    protected override void CheckIncomingControllerEvent(ControllerEvent eventType, object data)
    {
        switch (eventType)
        {
    
        }
    }

    private void SendTextMessage()
    {
        string userMessage = _inputField.text;
        _chatView.CreateNewMessage("User", userMessage, MessageType.USER);
        _inputField.text = "";
        //RaiseControllerEvent(ControllerEvent.MESSAGE_SENT, userMessage);
        _waitingForResponse = true;
        _chatView.ToggleChatWindow(false);
        CreateAPIForm(userMessage);
    }

    private void ProcessAgentResponse(object data)
    {
        ResponseData responseData = (ResponseData)data;
        if (_sessionID == "-1")
        {
            //RaiseControllerEvent(ControllerEvent.NEW_SESSION, null);
            _sessionID = responseData.sessionID;
        }

        _chatView.CreateNewMessage(_customCharacterName, responseData.response, MessageType.AGENT);
        _chatView.ToggleChatWindow(true);
        _waitingForResponse = false;
        //RaiseControllerEvent(ControllerEvent.MESSAGE_RECEIVED, responseData);
    }

    public void ResetSession()
    {
        _sessionID = "-1";
        _chatView.ClearMessageList();
        //RaiseControllerEvent(ControllerEvent.RESET_SESSION, null);
    }

    private void CreateAPIForm(string userMessage)
    {
        WWWForm form = new WWWForm();

        if (!string.IsNullOrEmpty(userMessage))
        {
            form.AddField("userText", userMessage);
        }

        form.AddField("charID", _characterKey);
        form.AddField("sessionID", _sessionID);
        form.AddField("voiceResponse", "True");

        UnityWebRequest request = UnityWebRequest.Post(_apiURL + "character/getResponse", form); 
        StartCoroutine(SendAPIRequest(request));
    }

    private IEnumerator SendAPIRequest(UnityWebRequest request)
    {
        request.SetRequestHeader("CONVAI-API-KEY", _apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error with request: " + request.error + "\n" + request.downloadHandler.text);

        }
        else
        {
            Debug.Log("Request success: " + request.downloadHandler.text);
            ResponseData data = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            ProcessAgentResponse(data);
        }
    }
}

[Serializable]
public class ResponseData
{
    public string response;
    public string sessionID;
}
