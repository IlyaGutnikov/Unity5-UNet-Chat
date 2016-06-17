using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

    [SerializeField]
    private GameObject _server, _client;

    void OnGUI() {
    
        if (GUI.Button (new Rect(25,25,200,50), " Server ")) {

            _server.SetActive(true);
            enabled = false;

        }

        if (GUI.Button (new Rect(25,100,200,50), " Client ")) {

            _client.SetActive(true);
            enabled = false;

        }
    
    }
}
