using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SongmapController : MonoBehaviour
{
    public static readonly string APPLICATION_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\AudioSamurai";

    // Start is called before the first frame update
    void Start()
    {
        ensureApplicationFolders();
        Songmap m = new Songmap("S:\\Music\\ROY KNOX - Earthquake [NCS Release].ogg");
        m.DifficultyTitle = "[Normal]";
        m.save();
        Songmap m2 = Songmap.load($"{Songmap.SONGS_FOLDER}\\ROY KNOX - Earthquake [NCS Release]\\ROY KNOX - Earthquake [NCS Release] [Normal]");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Ensures that the required application folders exists in the local application data and creates them if they dont.
     */
    private void ensureApplicationFolders()
    {

        try
        {
            if (!Directory.Exists(APPLICATION_FOLDER))
            {
                Directory.CreateDirectory(APPLICATION_FOLDER);
            }
            if (!Directory.Exists(Songmap.SONGS_FOLDER))
            {
                Directory.CreateDirectory(Songmap.SONGS_FOLDER);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception occurred during application folder creation: " + e.ToString());
        }
    }
}
