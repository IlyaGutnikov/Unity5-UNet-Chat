using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChatClient : MonoBehaviour {

    private string _ip = "127.0.0.1";
    private int _port = 5555;

    private NetworkClient _client;

    [SerializeField]
    private Text _chatText;
    [SerializeField]
    private InputField _sendTextInput;
    [SerializeField]
    private InputField _nameInput;


    void Start () {

        _chatText.text = "";
        _sendTextInput.text = "";
        _nameInput.text = "Client" + Random.Range(0, int.MaxValue);

        Application.runInBackground = true;

        var config = new ConnectionConfig();
        //что за каналы?
        config.AddChannel(QosType.Unreliable);
        config.AddChannel(QosType.Reliable);

        _client = new NetworkClient();
        _client.Configure(config, 1);

        RegisterHandlers();

        _client.Connect(_ip, _port);
    
    }

    void OnApplicationQuit() {

        if (_client != null)
        {
            _client.Disconnect();
            _client.Shutdown();
            _client = null;

        }

    }

    private void RegisterHandlers() {

        _client.RegisterHandler((short)MessageType.Message, OnMessageRecevied);

    }

    public void SendChatMessage() {

        var mes = new ChatMessageMsg();
        mes.Message = _nameInput.text + ":" + _sendTextInput.text;

        _sendTextInput.text = "";

        _client.Send((short)MessageType.Message, mes);

    }

    private void OnMessageRecevied(NetworkMessage message) {

        var mes = message.ReadMessage<ChatMessageMsg>().Message;
        Debug.Log("message from server id:" + message.conn.connectionId + " is " + mes);
        _chatText.text = mes + "\n" + _chatText.text;

    }
}
