using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        public Text Show_text_in;

        public Text Show_text_out;

        public Text input;

        string Chat_send;

        string output_;

        private WebSocket websocket;

        public GameObject pref_;

        public GameObject Canvas;


        // Start is called before the first frame update
        void Start()
        {
            websocket = new WebSocket("ws://127.0.0.1:36500/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

          
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                websocket.Send("Number : " + Random.Range(0, 99999));
            }

        }

        public void chat_Work_in()
        {
            Chat_send = input.text;

            Show_text_in.text = Chat_send;

            websocket.Send(Chat_send);

            GameObject Textsp = Instantiate(pref_, new Vector3(193, -22,0), Quaternion.identity);

            Textsp.transform.SetParent( Canvas.transform);


        }

        public void OnDestroy()
        {
            if(websocket != null)
            {
                websocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log("Message from server : " + messageEventArgs.Data);
            output_ = messageEventArgs.Data;
            Show_text_out.text = output_;
        }
    }

}

