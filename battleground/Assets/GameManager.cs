﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }
}