using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class StealableItemBehavior : MonoBehaviour
{
    public int camIndex;
    public static StealableItemBehavior instance;

    private void OnEnable()
    {
        Interactor.OnHoldCompleted += defaultCamera;
        Interactor.OnHoldProgress += floatObj;
        Interactor.OnHoldCanceled += droppingObj;
        Interactor.OnStartedStealing += setCamera;

    }
    private void OnDisable()
    {
        Interactor.OnStartedStealing -= setCamera;
        Interactor.OnHoldCompleted -= defaultCamera;
        Interactor.OnHoldProgress -= floatObj;
        Interactor.OnHoldCanceled -= droppingObj;
    }

    void Awake()
    {

        if (instance == null)
            instance = this;

    }
    public void setCamera(int cam)
    {
        cam = camIndex;
        ChangeCamera.instance.changeCamera(camIndex);
        Debug.Log("This is current camera: "+ ChangeCamera.instance.currentCamera.name);
    }
    void defaultCamera()
    {
        ChangeCamera.instance.changeCamera(0);
    }
    void floatObj(float progress)
    {
        //gameObject.transform.DOLocalMove(, 1f);

    }
    void droppingObj()
    {
        gameObject.transform.DOLocalMoveY(-2, 1f);
    }
   
}
