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
    public const string SONG_MIME_TYPE = ".as";

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
    public const string XML_BPM = "BPM";
    public const string XML_TIMING_DIV = "Divisor";
    public const string XML_MAP_OBJ = "MapObjectList";
    public const string XML_MAP_OBJ_ITEM = "MapObjectItem";
    public const string XML_MAP_OBJ_TYPE = "MapObjectType";

    public const int DUPLICATE_NOT_FOUND = -1;

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
    private List<(float, float, int)> timingList = new List<(float, float, int)>();

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
        this.difficultyTitle = difficultyTitle;
    }

    /* Returns readonly list from mapObjects. mapObjects can be edited only by using addMapObject and editMapObject methods */
    public IList<(float, string)> getMapObjects()
    {
        return mapObjects.AsReadOnly();
    }

    /* 
     * Places an single map object of given type to the position in milliseconds.
     * pos : position in milliseconds where the object should be placed on map.
     * objectType : type of mapObject to be added to the given position.
     * returns boolean whether the object was added or not.
     */
    public bool addMapObject(float pos, string objectType)
    {
        if (getPositionDuplicate(ref mapObjects, pos) == DUPLICATE_NOT_FOUND)
        {
            /*
             TODO: if (objectType is valid) { add } 
             */
            mapObjects.Add((pos, objectType));
            return true;
        }
        return false;
    }

    public bool editMapObject(float pos, string objectType)
    {
        int dupePos;
        if ((dupePos = getPositionDuplicate(ref mapObjects, pos)) != DUPLICATE_NOT_FOUND)
        {
            mapObjects[dupePos] = (pos, objectType);
            return true;
        }
        return false;
    }

    /* Returns an readonly version of timingList. timingList can be edited only by using addTiming and editTiming methods */
    public IList<(float, float, int)> getTimingList()
    {
        return timingList.AsReadOnly();
    }

    /*
     * Adds an new timing item to the list which will indicate on at what bpm does the map run onwards from the given millisecond pos. Also measureDivisor dictates how many notes are per measure.
     * pos : position in milliseconds from whereon the bpm should affect. If there is more timing elements the bpm will affect from pos to next timing item pos.
     * bpm : beat per minute starting from given pos.
     * measureDivisor : how many notes should be per measure starting from pos (default 4 == 4/4)
     * returns boolean whether it was added
     */
    public bool addTiming(float pos, float bpm, int measureDivisor = 4)
    {

        if (getPositionDuplicate(ref timingList, pos) == -1)
        {
            if (bpm > 0 && measureDivisor <= 8 && measureDivisor >= 1)
            {
                timingList.Add((pos, bpm, measureDivisor));
                return true;
            }
        }
        return false;
    }

    /*
     * Edits the timing at given position to new bpm and measure divisor.
     * pos : position of the editable timing
     * newTiming : the new timing item which should replace the old timing
     */
    public bool editTiming(float pos, (float, float, int) newTiming)
    {
        int dupePos;
        if ((dupePos = getPositionDuplicate(ref timingList, pos)) != DUPLICATE_NOT_FOUND)
        {
            timingList[dupePos] = newTiming;
            return true;
        }
        return false;
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
     * Attempts to deserialize songmap information from the given file, verifies it and returns the Songmap.
     * throws : Exception if the loaded songmap is not valid
     */
    public static Songmap load(string filePath)
    {
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Songmap));
            Songmap newSongmap = serializer.Deserialize(stream) as Songmap;
            if (newSongmap.validate())
            {
                return newSongmap;
            }
            throw new Exception("Loaded songmap was not valid");
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
                                    var div = reader.ReadElementContentAsInt();
                                    timingList.Add((ms, bpm, div));
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
        foreach ((float, float, int) timingItem in timingList)
        {
            writer.WriteStartElement(XML_TIMING_ITEM);
            writer.WriteElementString(XML_MS_POS, $"{timingItem.Item1}");
            writer.WriteElementString(XML_BPM, $"{timingItem.Item2}");
            writer.WriteElementString(XML_TIMING_DIV, $"{timingItem.Item3}");
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
        List<bool> checks = new List<bool>();
        checks.Add(hasAudioFile());
        checks.Add(hasValidContent());
        return !checks.Contains(false);
    }

    /* Checks if the audioFile of given songmap is in their respective song folder */
    private bool hasAudioFile()
    {
        return File.Exists($"{getSongmapFolderPath()}\\{audioFilename}");
    }

    /* Validates that the contents (timings and mapObjects) are valid and usable */
    private bool hasValidContent()
    {
        if (timingList.Count > 0)
        {
            if (mapObjects.Count > 0)
            {
                float timingMin = float.MaxValue;

                foreach (var timing in timingList)
                {
                    if (timing.Item1 < timingMin)
                        timingMin = timing.Item1;
                }
                float mapObjMin = float.MaxValue;
                foreach (var mapObj in mapObjects)
                {
                    if (mapObj.Item1 < mapObjMin)
                        mapObjMin = mapObj.Item1;
                }

                if (mapObjMin > timingMin)
                    return true;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    /* 
     * Checks if the given tuple lists first element is the same as pos
     * timingList : the timing list where you want to check for duplicates
     * pos : position in milliseconds
     * returns the position of duplicate in the list if found, -1 else.
     */
    private int getPositionDuplicate(ref List<(float, float, int)> timingList, float pos)
    {
        foreach ((float, float, int) timingItem in timingList)
        {
            if (timingItem.Item1 == pos)
                return timingList.IndexOf(timingItem);
        }
        return DUPLICATE_NOT_FOUND;
    }

    /* Performs same operation as method above, but to an map object list */
    private int getPositionDuplicate(ref List<(float, string)> mapObjList, float pos)
    {

        foreach ((float, string) mapObj in mapObjList)
        {
            if (mapObj.Item1 == pos)
                return mapObjList.IndexOf(mapObj);
        }
        return DUPLICATE_NOT_FOUND;
    }

    /* Returns filepath to currently selected audio files folder */
    private string getSongmapFolderPath()
    {
        return $"{SONGS_FOLDER}\\{Path.GetFileNameWithoutExtension(audioFilename)}";
    }

    /* Returns filepath to this songmaps difficulty file */
    private string getSongmapPath()
    {
        return $"{getSongmapFolderPath()}\\{Path.GetFileNameWithoutExtension(audioFilename)} {difficultyTitle}{SONG_MIME_TYPE}";
    }

    /* Returns filepath to this songmaps audio file */
    private string getSongmapAudioFilePath()
    {
        return $"{getSongmapFolderPath()}\\{audioFilename}";
    }
}

