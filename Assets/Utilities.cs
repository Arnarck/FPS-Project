using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    public static Utilities Instance { get; private set; }
    public WaitForEndOfFrame waitForEndOfFrame;

    void Awake()
    {
        Instance = this;
        waitForEndOfFrame = new WaitForEndOfFrame();
    }
}
