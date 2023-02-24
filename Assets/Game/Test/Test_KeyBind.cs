using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test_KeyBind : MonoBehaviour
{
    public TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            KeyCode key;
            for (int i = 0; i < Enum.GetNames(typeof(KeyCode)).Length; i++)
            {
                key = (KeyCode)i;
                if (Input.GetKeyDown(key))
                {
                    txt.text = key.ToString();
                    this.enabled = false;
                }
            }
        }
    }

    public void listen_to_key()
    {
        this.enabled = true;
        txt.text = "Press any key...";
    }
}
