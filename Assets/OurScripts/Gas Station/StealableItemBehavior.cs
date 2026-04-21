using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class StealableItemBehavior : MonoBehaviour
{
    public int camIndex;
    public int defaultCamIndex = 0;
    public static StealableItemBehavior instance;

    private void OnTriggerEnter(Collider other)
    {
        instance = this;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
}