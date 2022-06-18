using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstances : MonoBehaviour
{
    public static GameInstances Instance { get; private set; }
    public Player player;

    private void Awake()
    {
        Instance = this;
    }
}
