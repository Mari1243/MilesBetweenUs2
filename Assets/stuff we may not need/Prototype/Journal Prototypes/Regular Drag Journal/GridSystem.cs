using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public GameObject objtoPlace;
    private GameObject ghostObject;
    private HashSet<UnityEngine.Vector3> occupiedPositions = new HashSet<UnityEngine.Vector3>();
    
    public float gridSize = 1f;

    private void Start()
    {
        CreateGhostObject();
    }

    private void Update()
    {
        updateGhostPosition();
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }
    }

    void CreateGhostObject()
    {
        ghostObject = Instantiate(objtoPlace);
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f;

            mat.SetFloat("_Mode", 2);
            mat.SetInt("_ScrBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }

    void updateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            UnityEngine.Vector3 point = hit.point;
            UnityEngine.Vector3 snappedPostion = new UnityEngine.Vector3(Mathf.Round(point.x / gridSize) * gridSize, Mathf.Round(point.y / gridSize) * gridSize, Mathf.Round(point.z / gridSize) * gridSize);
            ghostObject.transform.position = snappedPostion;

            if (occupiedPositions.Contains(snappedPostion))
            {
                SetGhostColor(Color.red);
            }
            else
            {
                SetGhostColor(new Color(1f, 1f, 1f, .5f));
            }
        }
    }

    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    void PlaceObject()
    {
        UnityEngine.Vector3 placementPosition = ghostObject.transform.position;
        if (!occupiedPositions.Contains(placementPosition))
        {
            Instantiate(objtoPlace, placementPosition, UnityEngine.Quaternion.identity);
            //making sure the script knows that place is occupied!
            occupiedPositions.Add(placementPosition);
        }
    }
}
