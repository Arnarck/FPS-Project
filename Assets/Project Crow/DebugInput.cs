using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugInput : MonoBehaviour
{
    Player _player;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _player.ModifyHealthAmount(-30);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            _player.ModifyHealthAmount(10);
        }
    }
}
