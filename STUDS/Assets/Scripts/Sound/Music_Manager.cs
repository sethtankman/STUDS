using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public AK.Wwise.Event menuMusic;
    public AK.Wwise.Event strollerMusic;
    public AK.Wwise.Event shoppingMusic;


    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        PlayMusic("Start_Loop");
    }
    public void PlayMusic(string soundName)
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
