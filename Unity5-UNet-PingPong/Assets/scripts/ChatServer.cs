using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public enum MessageType {

    Message = MsgType.Highest + 1,
    Connected = MsgType.Connect,
    Disconected = MsgType.Disconnect,
}

public class ChatServer : MonoBehaviour {

    private string _ip = "127.0.0.1";
    private int _port = 5555;
    private int _maxConnections = 1000;

    [SerializeField]
    private Text _chatText;
    [SerializeField]
    private InputField _sendTextInput;


    void Start () {
    
        _chatText.text = "";
        _sendTextInput.text = "";

        Application.runInBackground = true;

        RegisterHandlers();

        var config = new ConnectionConfig();

        //unreliable - без гарантии доставки
        //realible - с гарантией
        config.AddChannel(QosType.Unreliable);
        config.AddChannel(QosType.Reliable);

        //задача - получить IP хоста
        var ht = new HostTopology(config, _maxConnections);

        if ((!NetworkServer.Configure(ht)) || (!NetworkServer.Listen(_port)))
        {

            Debug.LogError("no server created");
            return;

        }

    }

    void OnApplicationQuit() {
    
        NetworkServer.Shutdown();
    
    }

    private void RegisterHandlers() {

        RegisterHandler(MessageType.Message, OnMessageRecevied);
        RegisterHandler(MessageType.Connected, OnMessageRecevied);
        RegisterHandler(MessageType.Disconected, OnMessageRecevied);
   
    }

    private void RegisterHandler(MessageType t, NetworkMessageDelegate handler) {

        NetworkServer.RegisterHandler((short) t, handler);
    
    }

    private void AddMessageToChat(string message) {
    
        _chatText.text = message + "\n" + _chatText.text;
    
    }

    private void OnMessageRecevied(NetworkMessage netMes) {
    
        var packet = netMes.ReadMessage<ChatMessageMsg>();
        AddMessageToChat(packet.Message);
        NetworkServer.SendToAll((short)MessageType.Message, packet);
    
    }

    private void OnMessageConnected(NetworkMessage netMes) {

        var mes = new ChatMessageMsg();
        mes.Message = "Player " + netMes.conn.connectionId + " connected";

        AddMessageToChat(mes.Message);
        NetworkServer.SendToAll((short)MessageType.Message, mes);

    }

    private void OnMessageDisconnected(NetworkMessage netMes) {

        var mes = new ChatMessageMsg();
        mes.Message = "Player " + netMes.conn.connectionId + " connected";

        AddMessageToChat(mes.Message);
        NetworkServer.SendToAll((short)MessageType.Message, mes);
    
    }

    public void SendChatMessage() {

        var mes = new ChatMessageMsg();
        mes.Message = "[SERVER]: " + _sendTextInput.text;
        AddMessageToChat(mes.Message);
        _sendTextInput.text = "";

        NetworkServer.SendToAll((short)MessageType.Message, mes);

    }
        
}
