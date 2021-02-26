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
            public string idUser;
            public string userName;
            public string passWord;
            public string textUInput;
            public SocketEvent(string eventName, string data, string idUser, string userName, string passWord, string textUInput)
            {
                
                this.eventName = eventName;
                this.data = data;
                this.idUser = idUser;
                this.userName = userName;
                this.passWord = passWord;
                this.textUInput = textUInput;
            }
        }
        public GameObject closeMenu;
        public GameObject rootConnection;
        public GameObject rootMessenger;

        public GameObject regiterInput;
        public GameObject loginInput;
        public GameObject popupOk;
        public Text Oktext;
        public InputField idInput;
        //public InputField passInputOne;
        //public InputField passInputTwo;
        public InputField nameInput;
        //Login Input
        public InputField IDnameLogin;
        public InputField PassLogin;


        //Regiter Input
        public InputField nameIP;
        public InputField IDnameIP;
        public InputField passusernameIP;
        public InputField REpassusernameIP;

        //Get Name Sql
        public Text Getname;

        //send
        string sendM;

        //join Room Name
        string NameroomChat;
        public Text ShowRoomname;

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
            closeMenu.SetActive(false);
            loginInput.SetActive(true);


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
                SocketEvent socketEvent = new SocketEvent("CreateRoom", roomName, "", "", "", "");

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
                SocketEvent socketEvent = new SocketEvent("JoinRoom", roomName, "", "" ,"", "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);
                Debug.Log(jsonStr);
            }
        }

        public void LeaveRoom(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", roomName, "", "", "", "");

                string jsonStr = JsonUtility.ToJson(socketEvent);


                ws.Send(jsonStr);

                sendText.text = "";
                receiveText.text = "";         
            }
        }
        public void LogIn()
        {
            if (IDnameLogin.text == "" || PassLogin.text == "")
            {
                popupOk.SetActive(true);
                Oktext.text = "Please input all field";
                return;
            }


            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("Login", "", IDnameLogin.text, "", PassLogin.text, "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);

            }
        }

        public void Moveregiter()
        {
            regiterInput.SetActive(true);
            loginInput.SetActive(false);
        }

        public void Regiter()
        {
            if (idInput.text == "" || passusernameIP.text == "" || nameInput.text == "")
            {
                popupOk.SetActive(true);
                Oktext.text = "Please input all field";
                return;
            }
            else if (passusernameIP.text != REpassusernameIP.text)
            {
                popupOk.SetActive(true);
                Oktext.text = "Password not match";
                return;
            }
            


            else if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("Regiter", "", IDnameIP.text, nameIP.text, passusernameIP.text, "");

                string jsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(jsonStr);

            }
        }
        public void Closepopup()
        {
            popupOk.SetActive(false);
        }


        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            print(sendM);
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;
            SocketEvent socketEvent = new SocketEvent("SendMessage", "", "", sendM, "", inputText.text);

            //MessageData messageData = new MessageData();
            //messageData.username = inputUsername.text;
            //messageData.message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(toJsonStr);
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
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                

                SocketEvent getserverData = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                Debug.Log(getserverData.eventName);
                Debug.Log(getserverData.data);

                if (getserverData.eventName == "CreateRoom")
                {

                    if (getserverData.data == "Success")
                    {
                        print(getserverData.userName);
                        NameroomChat = getserverData.userName;
                        ShowRoomname.text = NameroomChat;
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(true);
                        closeMenu.SetActive(false);

                    }
                    else if (getserverData.data == "fail")
                    {
                        popupOk.SetActive(true);
                        Oktext.text = "Can't CearteRoom";

                    }

                }
                else if(getserverData.eventName == "JoinRoom")
                {
                    if (getserverData.data == "Success")
                    {

                        NameroomChat = getserverData.userName;
                        ShowRoomname.text = NameroomChat;
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(true);
                        closeMenu.SetActive(false);
                    }
                    else if (getserverData.data == "fail")
                    {
                        popupOk.SetActive(true);
                        Oktext.text = "Can't ConnecRoom";
                    }
                }
                else if (getserverData.eventName == "LeaveRoom")
                {
                    rootConnection.SetActive(false);
                    rootMessenger.SetActive(false);
                    closeMenu.SetActive(true);
                }
                else if (getserverData.eventName == "Login") //Login
                {
                    if (getserverData.data == "Success")
                    {
                        Getname.text = getserverData.userName;
                        sendM = Getname.text;
                        Debug.Log(getserverData.userName);
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        closeMenu.SetActive(true);
                        loginInput.SetActive(false);
                        regiterInput.SetActive(false);
                    }
                    else if (getserverData.data == "fail")
                    {
                        popupOk.SetActive(true);
                        Oktext.text = "Login fail Please try again";
                    }                   
                }
                else if (getserverData.eventName == "Regiter")
                {
                    if (getserverData.data == "Success")
                    {
                        regiterInput.SetActive(false);
                        rootConnection.SetActive(false);
                        rootMessenger.SetActive(false);
                        closeMenu.SetActive(false);
                        loginInput.SetActive(true);
                    }
                    else if (getserverData.data == "fail")
                    {
                        popupOk.SetActive(true);
                        Oktext.text = "This ID cannot be used.";
                    }
                }
                else if (getserverData.eventName == "SendMessage")
                {
                    
                    //print(Getname.text);
                    if (getserverData.userName == sendM)
                    {
                        sendText.text += getserverData.userName + " : " + getserverData.textUInput + "\n";
                        receiveText.text += "\n";
                    }
                    else
                    {
                        sendText.text += "\n";
                        receiveText.text += getserverData.userName + " : " + getserverData.textUInput + "\n";
                    }
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



