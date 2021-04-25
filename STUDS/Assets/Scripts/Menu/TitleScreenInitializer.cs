using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Shopping", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", true);

        var players = ManagePlayerHub.Instance.getPlayers();
        foreach(GameObject player in players)
        {
            /*
            // Drop the players through the floor!
            player.transform.position = new Vector3(0, -10, 0);
            */
            // Destroying the player might be better...
            Destroy(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
