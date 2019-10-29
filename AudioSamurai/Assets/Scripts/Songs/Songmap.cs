using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/*
 * Songmap is a definition on how a song should be populated and how difficult it is.
 * Each songmap object usually defines how a single difficulty should be played out.
 * Here is some definitions to help out what they mean.
 * 
 * AR (Approach rate): How much time the player has before they see the approaching map object. Calculated with = AR_DURATION / approachRate
 * HAL (Hit accuracy level): How much time does the player have to hit/parry map object on beat. Calculated with = HAL_DEFAULT_DURATION - HAL_DIFF * hitAccuracyLevel)
 */
[Serializable()]
[System.Xml.Serialization.XmlRoot("Songmap")]
public class Songmap
{
    /* Constants */
    public static readonly string SONGS_FOLDER = SongmapController.APPLICATION_FOLDER + "\\Songs";
    public static readonly string SONG_MIME_TYPE = ".ausa";
   
    public static readonly float MIN_AR = 1;
    public static readonly float MAX_AR = 5;
    public static readonly float AR_DURATION = 1000;

    public static readonly float MIN_HAL = 1;
    public static readonly float MAX_HAL = 10;
    public static readonly float HAL_DIFF = 5;
    public static readonly float HAL_DEFAULT_DURATION = 100 + HAL_DIFF;

    /* general */
    [System.Xml.Serialization.XmlElement("AudioFilename")]
    private string audioFilename;

    /* Difficulty */
    [System.Xml.Serialization.XmlElement("ApproachRate")]
    private float approachRate;

    [System.Xml.Serialization.XmlElement("HitAccuracyLevel")]
    private float hitAccuracyLevel;

    /* map specific points */
    [System.Xml.Serialization.XmlArray("Timing")]
    [System.Xml.Serialization.XmlArrayItem("TimingItem", typeof(TimingListItem))]
    private List<(float, float)> timingList = new List<(float, float)>();

    [System.Xml.Serialization.XmlArray("MapObjects")]
    [System.Xml.Serialization.XmlArrayItem("MapObjectItem", typeof(MapObjectItem))]
    private List<(float, string)> mapObjects = new List<(float, string)>();

    /* Constructs an new Songmap based on the given audio file */
    public Songmap()
    {
        audioFilename = "asd";
        approachRate = 1;
        hitAccuracyLevel = 5;
        timingList.Add((500, 220));
        timingList.Add((1000, 110));
        mapObjects.Add((500, "asd"));
        mapObjects.Add((1000, "dasd"));
        serialize();
    }

    /*
     * Deserializes the songmap object based on the '.as' xml file found from the maps folder (Songs folder found at User/AppData/local/AudioSamurai/Songs)
     * songmapFile : path to the songmap file that contains information on how the file should be constructed.
     * Exceptions : 
     */
    public Songmap(string songmapFilePath)
    {
        if (songmapFilePath.Contains(SONG_MIME_TYPE))
        {
            deserialize(songmapFilePath);
        }
        else
        {
            throw new Exception("Invalid songmap file type");
        }
    }

    /* 
     * Places an single map object of given type to the position in milliseconds.
     * pos : position in milliseconds where the object should be placed on map.
     */
    public void placeMapObject(float pos, string objectType)
    {

    }

    /*
     * Exports the given songmap as independent difficulty file with SONG_MIME_TYPE file ending. The class is serialized as XML type.
     */
    public void serialize()
    {

    }

    public float getAR()
    {
        return AR_DURATION / approachRate;
    }

    public float getHALDuration()
    {
        return HAL_DEFAULT_DURATION - hitAccuracyLevel * HAL_DIFF;
    }


    /* Private methods */

    /*
     * Attempts to deserialize songmap information from the given file and verifies it.
     */
    private void deserialize(string filePath)
    {
        FileStream fs = File.OpenRead(filePath);

    }
}

/* XML Helper classes */
public class TimingListItem
{
    [System.Xml.Serialization.XmlAttribute("Position")]
    public float Position { get; set; }
    [System.Xml.Serialization.XmlAttribute("BPM")]
    public float BPM { get; set; }
}

public class MapObjectItem
{
    [System.Xml.Serialization.XmlAttribute("Position")]
    public float Position { get; set; }
    [System.Xml.Serialization.XmlAttribute("ObjectType")]
    public float ObjectType { get; set; }
}

