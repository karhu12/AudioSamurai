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
    public const string XML_MAP_OBJ_ITEM = "MapObjectItem";
    public const string XML_BPM = "BPM";
    public const string XML_MAP_OBJ_TYPE = "MapObjectType";

    /* general */
    private string audioFilename;

    /* Difficulty */
    private string difficultyTitle;
    public string DifficultyTitle { 
        get => difficultyTitle;
        set { if (value.Length <= 12) difficultyTitle = value; }
    }

    private float approachRate;
    public float ApproachRate { 
        get => approachRate;
        set { if (value >= MIN_AR && value <= MAX_AR) approachRate = value; }
    }

    private float hitAccuracyLevel;
    public float HitAccuracyLevel
    {
        get => hitAccuracyLevel;
        set { if (value >= MIN_HAL && value <= MAX_HAL) hitAccuracyLevel = value; }
    }

    /* map specific points */
    private List<(float, float)> timingList = new List<(float, float)>();

    private List<(float, string)> mapObjects = new List<(float, string)>();

    /* Default constructor. Not recommended to use because it does not initialize the map correctly. */
    public Songmap()
    {
        approachRate = MIN_AR;
        hitAccuracyLevel = MIN_HAL;
        difficultyTitle = "";
    }

    /*
     * Constructs an new Songmap based on the given audio file. Creates a new folder to the SONGS_FOLDER based on the given audioFile and copies it there.
     * audioFilePath : path to the original audio file.
     * throws : exception in case file operations fails or passed audio file does not exist.
     */
    public Songmap(string audioFilePath)
    {
        if (File.Exists(audioFilePath))
        {
            audioFilename = Path.GetFileName(audioFilePath);
            string path = getSongmapFolderPath();
            if (Directory.Exists(path))
            {
                if (!File.Exists(getSongmapAudioFilePath()))
                {
                    File.Copy(audioFilePath, getSongmapAudioFilePath());
                }
            } 
            else
            {
                Directory.CreateDirectory(path);
                File.Copy(audioFilePath, getSongmapAudioFilePath());
            }
        } else
        {
            throw new Exception("Given audio file did not exist");
        }
        approachRate = MIN_AR;
        hitAccuracyLevel = MIN_HAL;
        difficultyTitle = "";
    }

    /*
     * Constructs an new Songmap based on the given audio file. Creates a new folder to the SONGS_FOLDER based on the given audioFile and copies it there. Also sets the difficultyTitle to passed value.
     * audioFilePath : path to the original audio file.
     * difficultyTitle : name of this songmaps difficulty
     * throws : exception in case file operations fail
     */
    public Songmap(string audioFilePath, string difficultyTitle) : this(audioFilePath)
    {
        difficultyTitle = difficultyTitle;
    }

    /* 
     * Places an single map object of given type to the position in milliseconds.
     * pos : position in milliseconds where the object should be placed on map.
     * objectType : type of mapObject to be added to the given position.
     */
    public void placeMapObject(float pos, string objectType)
    {
        bool hasDuplicate = false;
        foreach ((float, string) objTuple in mapObjects)
        {
            if (objTuple.Item1 == pos)
                hasDuplicate = true;
                break;
        }
        if (!hasDuplicate)
        {
            /*
             TODO: if (objectType is valid) { add } 
             */
            mapObjects.Add((pos, objectType));
        }
    }

    /*
     * Exports the given songmap as independent difficulty file with SONG_MIME_TYPE file ending. The class is serialized as XML type.
     */
    public void save()
    {
        if (validate())
        {
            string folder = getSongmapFolderPath();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            using (var writer = new System.IO.StreamWriter(getSongmapPath()))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }
        else
        {
            throw new Exception("Songmap failed validation");
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

    /* Validates the songmap attributes if it's an functional songmap */
    private bool validate()
    {
        if (!File.Exists($"{getSongmapFolderPath()}\\{audioFilename}"))
        {
            return false;
        }
        return true;
    }

    /* Returns filepath to currently selected audio files folder */
    private string getSongmapFolderPath()
    {
        return $"{SONGS_FOLDER}\\{Path.GetFileNameWithoutExtension(audioFilename)}";
    }

    /* Returns filepath to this songmaps difficulty file */
    private string getSongmapPath()
    {
        return $"{getSongmapFolderPath()}\\{Path.GetFileNameWithoutExtension(audioFilename)} {difficultyTitle}";
    }

    /* Returns filepath to this songmaps audio file */
    private string getSongmapAudioFilePath()
    {
        return $"{getSongmapFolderPath()}\\{audioFilename}";
    }
}

