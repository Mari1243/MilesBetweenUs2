using NUnit.Framework.Constraints;
using UnityEngine;
using DG.Tweening;

public class MapManager : MonoBehaviour
{
    public Transform[] carlocations;
    public GameObject car;

    private void OnEnable()
    {
        SceneTrackerSingleton.CurrentSceneEvent += thing;
       
        transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal, true);
        //GetComponent<DOTweenAnimation>().tween?.SetUpdate(UpdateType.Normal, true);
    }

    private void OnDisable()
    {
        SceneTrackerSingleton.CurrentSceneEvent -= thing;
    }

    private void thing(string scenename, int carnum)
    {
        if(scenename == "car")
        {
            if(carnum == 0)
            {
                progressCar(1);
            }
            else if(carnum == 1)
            {
                progressCar(3);
            }
            else if(carnum == 2)
            {
                progressCar(5);
            }
        }
        else if(scenename == "GasStation")
        {
            progressCar(2);
        }
        else if(scenename == "DragonLand")
        {
            progressCar(4);
        }
        else if (scenename == "School")
        {
            progressCar(6);
        }
    }

    private void progressCar(int i)
    {
        print(i-1);
        car.transform.position = carlocations[i-1].position;
    }
}
