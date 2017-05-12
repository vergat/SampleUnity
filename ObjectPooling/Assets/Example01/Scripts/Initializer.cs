﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        ObjectPool.InitializeMain();

        ObjectPoolController poolController = FindObjectOfType<ObjectPoolController>();
        if (poolController != null)
        {
            poolController.LoadAll();
        }
    }
}