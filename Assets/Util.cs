using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Util Instance { get; private set; }
    public WaitForEndOfFrame waitForEndOfFrame;

    void Awake()
    {
        Instance = this;
        waitForEndOfFrame = new WaitForEndOfFrame();
    }
}
