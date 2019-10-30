using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

/*
 * Songmap is a definition on how a song should be populated and how difficult it is.
 * Each songmap object usually defines how a single difficulty should be played out.
 * A songmaps audioFilename marks the songmaps folder name in %localappdata%\AudioSamurai\Songs and the audio file inside that given folder.
 * 
 * AR (Approach rate): How much time the player has before they see the approaching map object. Calculated with = AR_DURATION / approachRate
 * HAL (Hit accuracy level): How much time does the player have to hit/parry map object on beat. Calculated with = HAL_DEFAULT_DURATION - HAL_DIFF * hitAccuracyLevel)
 */

public class Songmap : IXmlSerializable
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

    public static readonly string XML_AUDIO_FILENAME = "AudioFilename";
    public static readonly string XML_DIFF_TITLE = "DifficultyTitle";
    public static readonly string XML_AR = "ApproachRate";
    public static readonly string XML_HAL = "HitAccuracyLevel";

    /* general */
    private string audioFilename;

    /* Difficulty */
    private string difficultyTitle;
    private float approachRate;
    private float hitAccuracyLevel;

    /* map specific points */
    // [System.Xml.Serialization.XmlArray("Timing")]
    // [System.Xml.Serialization.XmlArrayItem("TimingItem", typeof(TimingListItem))]
    // private List<(float, float)> timingList = new List<(float, float)>();

    // [System.Xml.Serialization.XmlArray("MapObjects")]
    // [System.Xml.Serialization.XmlArrayItem("MapObjectItem", typeof(MapObjectItem))]
    // private List<(float, string)> mapObjects = new List<(float, string)>();

    /* Constructs an new Songmap based on the given audio file */
    public Songmap()
    {
        audioFilename = "Uinuka uinuka lai lai lai";
        difficultyTitle = "[easy]";
        approachRate = 1;
        hitAccuracyLevel = 5;
        /*
        timingList.Add((500, 220));
        timingList.Add((1000, 110));
        mapObjects.Add((500, "asd"));
        mapObjects.Add((1000, "dasd"));*/
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
            load(songmapFilePath);
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
    public void save()
    {
        string folder = getSongmapFolderPath();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        using (var writer = new System.IO.StreamWriter(getSongmapFilePath()))
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
            serializer.Serialize(writer, this);
            writer.Flush();
        }
    }

    public float getAR()
    {
        return AR_DURATION / approachRate;
    }

    public float getHALDuration()
    {
        return HAL_DEFAULT_DURATION - hitAccuracyLevel * HAL_DIFF;
    }

    /* XML Serialization interface */
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
        {
            audioFilename = reader.GetAttribute(XML_AUDIO_FILENAME);
            difficultyTitle = reader.GetAttribute(XML_DIFF_TITLE);
            approachRate = float.Parse(reader.GetAttribute(XML_AR));
            hitAccuracyLevel = float.Parse(reader.GetAttribute(XML_HAL));
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString(XML_AUDIO_FILENAME, audioFilename);
        writer.WriteElementString(XML_DIFF_TITLE, difficultyTitle);
        writer.WriteElementString(XML_AR, $"{approachRate}");
        writer.WriteElementString(XML_HAL, $"{hitAccuracyLevel}");
    }

    /* Private methods */

    /*
     * Attempts to deserialize songmap information from the given file and verifies it.
     */
    private static Songmap load(string filePath)
    {
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Songmap));
            return serializer.Deserialize(stream) as Songmap;
        }
    }

    /* Returns filepath to currently selected audio files folder */
    private string getSongmapFolderPath()
    {
        return $"{SONGS_FOLDER}\\{getSimplifiedAudioName()}"; 
    }

    /* Returns filepath to this songmaps difficulty file */
    private string getSongmapFilePath()
    {
        return $"{getSongmapFolderPath()}\\{getSimplifiedAudioName()} {difficultyTitle}";
    }

    /* Returns the currently selected audioFilename without the extension */
    private string getSimplifiedAudioName()
    {
        return audioFilename.Contains(".mp3") || audioFilename.Contains(".ogg") ? audioFilename.Split(new string[] { ".mp3", ".ogg" }, StringSplitOptions.None)[0] : audioFilename;
    }
}

