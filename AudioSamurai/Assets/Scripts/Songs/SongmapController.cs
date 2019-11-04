﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SongmapController : MonoBehaviour
{
    public static readonly string APPLICATION_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\AudioSamurai";

    private Dictionary<string, List<Songmap>> songmaps = new Dictionary<string, List<Songmap>>();


    // Start is called before the first frame update
    void Start()
    {
        EnsureApplicationFolders();
        LoadSongmaps();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Private methods */

    /*
     * Ensures that the required application folders exists in the local application data and creates them if they dont.
     */
    private void EnsureApplicationFolders()
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
            print("Exception occurred during application folder creation: " + e.ToString());
        }
    }

    private void LoadSongmaps()
    {
        songmaps.Clear();
        
        foreach (var dir in Directory.EnumerateDirectories(Songmap.SONGS_FOLDER))
        {
            string key = dir.Substring(dir.LastIndexOf("\\") + 1);
            songmaps.Add(key, new List<Songmap>());
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                if (file.Contains(Songmap.SONG_MIME_TYPE))
                {
                    try
                    {
                        Songmap songmap = Songmap.Load(file);
                        songmaps[key].Add(songmap);
                    } catch (Exception e)
                    {
                        print($"Exception occurred while loading file {file} as songmap. {e.ToString()}");
                    }
                }
            }
            if (songmaps[key].Count == 0)
                songmaps.Remove(key);
        }
    }
}
