using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Searching : MonoBehaviour
{
    public Text Show_text;
    string Friend;
    public Text input;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Work()
    {
        Friend = input.text;
        if (Friend == "Dome" || Friend == "Joe" || Friend == "Tong" || Friend == "Nea" || Friend == "Aun" || Friend == "Jean")
        {
            Show_text.text = Friend+" it Found";
        }
        else 
        {
            Show_text.text = Friend+" it Not Found";
        }
    }  

}
