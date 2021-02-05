using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginP : MonoBehaviour
{
    public GameObject No;
    public Text Show_connect;
    public Text connect_I_S;
    public Text connect_P_S;
    public Text input;
    string connect_I;
    string connect_P;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void login_S()
    {
        connect_I = connect_I_S.text;
        connect_P = connect_P_S.text;
        if (connect_I == "127.0.0.1" && connect_P == "36500")
        {
            No.SetActive(false);
        }
        else
        {
            print("error");
        }
    }
}
