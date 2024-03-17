using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAIcount : MonoBehaviour
{
    public List<GameObject> AiOBJ = new List<GameObject>();
    public int PlayerCount;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerCount = 0;
        foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerCount += 1;
        }

        if (PlayerCount > 4)
        {
            int AICount = PlayerCount - 4;
            for (int i = 0; i < AICount; i++)
            {
                AiOBJ[i].SetActive(false);
            }
        }

        // TODO: Check if this breaks scenes besides dodgeball where DynamicAIcount is used.
        for (int i = 0; i < AiOBJ.Count; i++)
            if(AiOBJ[i].activeSelf)
                ManagePlayerHub.Instance.AddPlayer(AiOBJ[i]);
    }
}
