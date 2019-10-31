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
    public const string SONG_MIME_TYPE = ".ausa";
   
    public const float MIN_AR = 1;
    public const float MAX_AR = 5;
    public const float AR_DURATION = 1000;

    public const float MIN_HAL = 1;
    public const float MAX_HAL = 10;
    public const float HAL_DIFF = 5;
    public const float HAL_DEFAULT_DURATION = 100 + HAL_DIFF;

    public const string XML_AUDIO_FILENAME = "AudioFilename";
    public const string XML_DIFF_TITLE = "DifficultyTitle";
    public const string XML_AR = "ApproachRate";
    public const string XML_HAL = "HitAccuracyLevel";
    public const string XML_TIMING = "TimingList";
    public const string XML_TIMING_ITEM = "TimingListItem";
    public const string XML_MS_POS = "MsPosition";
    public const string XML_MAP_OBJ = "MapObjectList";
    public const string XML_MAP_OBJ_ITEM= "MapObjectItem";
    public const string XML_BPM = "BPM";
    public const string XML_MAP_OBJ_TYPE = "MapObjectType";

    /* general */
    private string audioFilename;

    /* Difficulty */
    private string difficultyTitle;
    private float approachRate;
    private float hitAccuracyLevel;

    /* map specific points */
    private List<(float, float)> timingList = new List<(float, float)>();

    private List<(float, string)> mapObjects = new List<(float, string)>();

    /* Constructs an new Songmap based on the given audio file */
    public Songmap()
    {

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

    /*
     * Attempts to deserialize songmap information from the given file and verifies it.
     */
    public static Songmap load(string filePath)
    {
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Songmap));
            return serializer.Deserialize(stream) as Songmap;
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
            do
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case XML_AUDIO_FILENAME when reader.Read():
                            audioFilename = reader.Value;
                            break;
                        case XML_DIFF_TITLE when reader.Read():
                            difficultyTitle = reader.Value;
                            break;
                        case XML_AR when reader.Read():
                            approachRate = float.Parse(reader.Value);
                            break;
                        case XML_HAL when reader.Read():
                            hitAccuracyLevel = float.Parse(reader.Value);
                            break;
                        case XML_TIMING:
                            if (reader.ReadToFollowing(XML_TIMING_ITEM))
                            {
                                do
                                {
                                    reader.ReadToDescendant(XML_MS_POS);
                                    var ms = reader.ReadElementContentAsFloat();
                                    var bpm = reader.ReadElementContentAsFloat();
                                    timingList.Add((ms, bpm));
                                } while (reader.ReadToNextSibling(XML_TIMING_ITEM));
                            }
                            break;
                        case XML_MAP_OBJ:
                            if (reader.ReadToFollowing(XML_MAP_OBJ_ITEM))
                            {
                                do
                                {
                                    reader.ReadToDescendant(XML_MS_POS);
                                    var ms = reader.ReadElementContentAsFloat();
                                    var type = reader.ReadElementContentAsString();
                                    mapObjects.Add((ms, type));
                                } while (reader.ReadToNextSibling(XML_MAP_OBJ_ITEM));
                            }
                            break;
                    }
                }
            } while (reader.Read());
            /*
            reader.ReadStartElement()
            audioFilename = reader.ReadContentAsString();
            reader.ReadToFollowing(XML_DIFF_TITLE);
            difficultyTitle = reader.ReadContentAsString();
            reader.ReadToFollowing(XML_AR);
            approachRate = reader.ReadContentAsFloat();
            reader.ReadToFollowing(XML_HAL);
            hitAccuracyLevel = reader.ReadContentAsFloat();
            reader.ReadToFollowing(XML_TIMING);
            if (reader.ReadToDescendant(XML_TIMING_ITEM))
            {
                do
                {
                    timingList.Add((reader.ReadElementContentAsFloat(XML_MS_POS, reader.NamespaceURI), reader.ReadElementContentAsFloat(XML_BPM, reader.NamespaceURI)));
                } while (reader.ReadToNextSibling(XML_TIMING_ITEM));
            }
            reader.ReadToFollowing(XML_MAP_OBJ);
            if (reader.ReadToDescendant(XML_MAP_OBJ_ITEM))
            {
                do
                {
                    mapObjects.Add((reader.ReadElementContentAsFloat(XML_MS_POS, reader.NamespaceURI), reader.ReadElementContentAsString(XML_MAP_OBJ_TYPE, reader.NamespaceURI)));
                } while (reader.ReadToNextSibling(XML_MAP_OBJ_ITEM));
            }*/
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElementString(XML_AUDIO_FILENAME, audioFilename);
        writer.WriteElementString(XML_DIFF_TITLE, difficultyTitle);
        writer.WriteElementString(XML_AR, $"{approachRate}");
        writer.WriteElementString(XML_HAL, $"{hitAccuracyLevel}");
        writer.WriteStartElement(XML_TIMING);
        foreach ((float, float) timingItem in timingList) 
        {
            writer.WriteStartElement(XML_TIMING_ITEM);
            writer.WriteElementString(XML_MS_POS, $"{timingItem.Item1}");
            writer.WriteElementString(XML_BPM, $"{timingItem.Item2}");
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteStartElement(XML_MAP_OBJ);
        foreach ((float, string) mapObj in mapObjects)
        {
            writer.WriteStartElement(XML_MAP_OBJ_ITEM);
            writer.WriteElementString(XML_MS_POS, $"{mapObj.Item1}");
            writer.WriteElementString(XML_MAP_OBJ_TYPE, $"{mapObj.Item2}");
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    /* Private methods */

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

