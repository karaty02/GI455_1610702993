using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnection : MonoBehaviour
    {
        struct SocketEvent
        {
            public string eventName;
            public string data;
            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
            }
        }
        public GameObject popupCereate;
        public GameObject popupJoin;
        public GameObject closeMenu;
        public GameObject rootConnection;
        public GameObject rootMessenger;

        public InputField inputUsername;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;

        public Text C_room;
        public Text J_room;

        string roomNumber;
        string joinNumber;

        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);


            
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:36500/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootMessenger.SetActive(false);
            closeMenu.SetActive(true);


            //CreateRoom("TestRoom01");
        }
        public void buttonCreate()
        {
            roomNumber = C_room.text;

            CreateRoom(roomNumber);
        }

        public void CreateRoom(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
            }
        }
        public void buttonJoin()
        {
            joinNumber = J_room.text;

            JoinRoom(joinNumber);
        }
        public void JoinRoom(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
                Debug.Log(jsonStr);
            }
        }

        public void LeaveRoom(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", roomName);

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);

            }
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            ws.Send(inputText.text);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            Getserver();
        }

        public void Getserver()
        {
            if (tempMessageString != null && tempMessageString != "")
            {
                SocketEvent getserverData = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                Debug.Log(getserverData.eventName);
                Debug.Log(getserverData.data);
                if (getserverData.eventName == "CreateRoom")
                {

                    if (getserverData.data == "Success")
                    {

                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(true);
                        closeMenu.SetActive(false);

                    }
                    else if (getserverData.data == "fail")
                    {
                        popupCereate.SetActive(true);
                    }

                }
                else if(getserverData.eventName == "JoinRoom")
                {
                    if (getserverData.data == "Success")
                    {
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(true);
                        closeMenu.SetActive(false);
                    }
                    else if (getserverData.data == "fail")
                    {
                        popupJoin.SetActive(true);
                    }
                }
                else if (getserverData.eventName == "LeaveRoom")
                {
                    rootConnection.SetActive(false);
                    rootMessenger.SetActive(false);
                    closeMenu.SetActive(true);
                }
                tempMessageString = "";

            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);
            tempMessageString = messageEventArgs.Data;
        }

        
    }

}


