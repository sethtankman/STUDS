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
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Penny", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", true);
        if (ManagePlayerHub.Instance)
        {
            var players = ManagePlayerHub.Instance.getPlayers();
            foreach (GameObject player in players)
            {
                /*
                // Drop the players through the floor!
                player.transform.position = new Vector3(0, -10, 0);
                */
                // Destroying the player might be better...
                //Destroy(player);
                //Going to set them to inactive here and will delete them in my script after I have extracted their race positions
                player.SetActive(false);
            }
        }
        else if (NetGameManager.Instance)
        {
            NetGameManager.Instance.DeletePlayers();
        }

    }
}
