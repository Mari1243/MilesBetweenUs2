using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using UnityEngine.UIElements;
using System.Collections;
using Cursor = UnityEngine.Cursor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class EmptyStealTrigger : MonoBehaviour
{
    private bool isinEmpty;
    public static event Action EmptyStealBehavior;

    private void OnEnable()
    {
        interactable.EmptySteal += transition;
    }
    private void OnDisable()
    {
        interactable.EmptySteal -= transition;
    }


    private void transition()
    {
        if (isinEmpty)
        {
            print("transitioning and moving player");
            EmptyStealBehavior?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("IN");
        ChangeCamera.instance.changeCamera(StealableItemBehavior.instance.camIndex);
        isinEmpty = true;
    }

    private void OnTriggerExit(Collider other)
    {
        print("OIUT");
        ChangeCamera.instance.changeCamera(0);
        isinEmpty = false;

    }
}
