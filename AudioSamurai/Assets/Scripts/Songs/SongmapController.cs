using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Networking;

/*
 * SongmapController is a singleton class thats mission is to ensure that the Songs can be loaded into the game, displayed for the user and played so they can choose which ever one to play.
 */
public class SongmapController : Singleton<SongmapController>
{
    public static readonly string APPLICATION_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\AudioSamurai";

    private Dictionary<string, List<Songmap>> songmaps = new Dictionary<string, List<Songmap>>();

    public AudioSource AudioSource;

    void Start()
    {
        EnsureApplicationFolders();
        LoadSongmaps();
    }

    /* Starts an couroutine that loads the songmaps audio clip and starts playing it. */
    public void PlaySongmapAudio(Songmap songmap)
    {
        StartCoroutine(PlaySongmapAudioCoroutine(songmap));
    }

    /* Starts an coroutine that loads the songmaps audio clip into the audio source. */
    public void LoadSongmapAudio(Songmap songmap)
    {
        StartCoroutine(LoadSongmapAudioCoroutine(songmap));
    }

    /* Returns an readonly songmaps dictionary that contains different songmaps under their parent song. */
    public IReadOnlyDictionary<string, List<Songmap>> GetSongmaps()
    {
        LoadSongmaps();
        return (IReadOnlyDictionary<string, List<Songmap>>) songmaps;
    }

    /*
     * Loads songmaps from the Songs folder into an dictionary where the string key represents the parent song for all the songmaps under it.
     */
    public void LoadSongmaps()
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
                    }
                    catch (Exception e)
                    {
                        print($"Exception occurred while loading file {file} as songmap. {e.ToString()}");
                    }
                }
            }
            if (songmaps[key].Count == 0)
                songmaps.Remove(key);
        }
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

    /*
     * Couroutine that loads the audio clip from given songmap and starts playing it.
     */
    private IEnumerator PlaySongmapAudioCoroutine(Songmap songmap)
    {
        yield return LoadSongmapAudioCoroutine(songmap);
        AudioSource.Play();
    }

    /*
     * Couroutine that loads the audio clip from given songmap.
     */
    private IEnumerator LoadSongmapAudioCoroutine(Songmap songmap)
    {
        using (var www = UnityWebRequestMultimedia.GetAudioClip(songmap.GetSongmapAudioFilePath(), songmap.GetAudioType()))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                AudioSource.volume = .5f;
            }
        }
    }
}
