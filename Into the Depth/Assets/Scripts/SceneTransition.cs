﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public int scene_number;

    public void Transition()
    {
        SceneManager.LoadScene(scene_number);
    }
}