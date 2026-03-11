using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class StealableItemBehavior : MonoBehaviour
{
    public int camIndex;
    public static StealableItemBehavior instance;
    private bool thisItem;
    private float holdProgress;
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
    private void OnTriggerEnter(Collider other)
    {
        thisItem = true;
    }
    private void OnTriggerExit(Collider other)
    {
        thisItem = false;
    }
    void Awake()
    {

        if (instance == null)
            instance = this;

    }
    private void Update()
    {
    }

    public void setCamera(int cam)
    {
        if (thisItem)
        {

            cam = camIndex;
            ChangeCamera.instance.changeCamera(camIndex);
            Debug.Log("This is current camera: " + ChangeCamera.instance.currentCamera.name);
        }
    }
    void defaultCamera()
    {
        ChangeCamera.instance.changeCamera(0);
    }
    void floatObj(float progress)
    {
        Debug.Log("Floating");
        //gameObject.transform.DOLocalMoveY(progress, 1f);

    }
    void droppingObj()
    {
        //gameObject.transform.DOLocalMoveY(-2, 1f);
        Debug.Log("Dropping");
    }
   
}
