using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObject : MonoBehaviour
{
    public Material[] animationMaterials;
    public GameObject[] objectsToSwap;
    public float swapTime;
    public GameObject objectToAnimate;

    public bool toggleMaterial;
    public bool toggleGameObject;



    private int materialCounter;
    private int objectCounter;
    private float timeCount;
    // Start is called before the first frame update
    void Start()
    {
        if(animationMaterials.Length > 0)
        {
            objectToAnimate.GetComponent<MeshRenderer>().material = animationMaterials[0];
            materialCounter++;
        }

        if(objectsToSwap.Length > 0)
        {
            objectsToSwap[0].SetActive(true);
            for(int i = 1; i < objectsToSwap.Length; i++)
            {
                objectsToSwap[i].SetActive(false);
            }
            objectCounter++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (toggleMaterial)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= swapTime)
            {
                if (animationMaterials.Length <= materialCounter)
                {
                    objectToAnimate.GetComponent<MeshRenderer>().material = animationMaterials[0];
                    materialCounter = 1;
                }
                else
                {
                    objectToAnimate.GetComponent<MeshRenderer>().material = animationMaterials[materialCounter];
                    materialCounter++;
                }
                timeCount = 0;
            }
        }else if (toggleGameObject)
        {
            timeCount += Time.deltaTime;
            if (timeCount >= swapTime)
            {
                if (objectsToSwap.Length <= objectCounter)
                {
                    objectsToSwap[0].SetActive(true);
                    objectsToSwap[objectsToSwap.Length - 1].SetActive(false);
                    objectCounter = 1;
                }
                else
                {
                    objectsToSwap[objectCounter].SetActive(true);
                    objectsToSwap[objectCounter - 1].SetActive(false);
                    objectCounter++;
                }
                timeCount = 0;
            }
        }

    }
}
