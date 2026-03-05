using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class StealableItemBehavior : MonoBehaviour
{
    public int camIndex;

    private void OnEnable()
    {
        StealingManager.OnStartedStealing += setCamera;
        Interactor.OnHoldProgress += floatObj;
        Interactor.OnHoldCanceled += droppingObj;
    }
    private void OnDisable()
    {
        StealingManager.OnStartedStealing -= setCamera;
        Interactor.OnHoldProgress -= floatObj;
        Interactor.OnHoldCanceled -= droppingObj;
    }


    void setCamera()
    {
        ChangeCamera.instance.changeCamera(camIndex);
    }
    void floatObj(float progress)
    {
        gameObject.transform.DOMoveY(3, 1f);

    }
    void droppingObj()
    {
        gameObject.transform.DOLocalMoveY(-2, 1f);
    }
}
