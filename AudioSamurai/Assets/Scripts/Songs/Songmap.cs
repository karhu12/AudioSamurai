﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Linq;

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
            string path = GetSongmapFolderPath();
            if (Directory.Exists(path))
            {
                if (!File.Exists(GetSongmapAudioFilePath()))
                {
                    File.Copy(audioFilePath, GetSongmapAudioFilePath());
                }
            } 
            else
            {
                Directory.CreateDirectory(path);
                File.Copy(audioFilePath, GetSongmapAudioFilePath());
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

    /* Returns readonly list from mapObjects. mapObjects can be edited only by using addMapObject, editMapObject and removeMapObject methods */
    public IList<(float, string)> GetMapObjects()
    {
        return mapObjects.AsReadOnly();
    }

    /* 
     * Places an single map object of given type to the position in milliseconds.
     * pos : position in milliseconds where the object should be placed on map.
     * objectType : type of mapObject to be added to the given position.
     * returns boolean whether the object was added or not.
     */
    public bool AddMapObject(float pos, string objectType)
    {
        if (GetPositionDuplicate(ref mapObjects, pos) == DUPLICATE_NOT_FOUND)
        {
            /*
             TODO: if (objectType is valid) { add } 
             */
            mapObjects.Add((pos, objectType));
            mapObjects.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            return true;
        }
        return false;
    }

    /* Replaces the mapObject at given pos by newMapObj. return boolean whether the objectse was replaced or not. */
    public bool EditMapObject(float pos, (float, string) newMapObj)
    {
        int dupePos;
        if ((dupePos = GetPositionDuplicate(ref mapObjects, pos)) != DUPLICATE_NOT_FOUND)
        {
            mapObjects[dupePos] = newMapObj;
            mapObjects.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            return true;
        }
        return false;
    }

    /* Removes an mapObject from given pos. Returns boolean wether an item was removed or not. */
    public bool RemoveMapObject(float pos)
    {
        int dupePos;
        if ((dupePos = GetPositionDuplicate(ref mapObjects, pos)) != DUPLICATE_NOT_FOUND)
        {
            mapObjects.RemoveAt(dupePos);
            return true;
        }
        return false;
    }

    /* Returns an readonly version of timingList. timingList can be edited only by using addTiming, editTiming and removeTiming methods */
    public IList<(float, float, int)> GetTimingList()
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
    public bool AddTiming(float pos, float bpm, int measureDivisor = 4)
    {

        if (GetPositionDuplicate(ref timingList, pos) == -1)
        {
            if (bpm > 0 && measureDivisor <= 8 && measureDivisor >= 1)
            {
                timingList.Add((pos, bpm, measureDivisor));
                timingList.Sort((x, y) => y.Item1.CompareTo(x.Item1));
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
    public bool EditTiming(float pos, (float, float, int) newTiming)
    {
        int dupePos;
        if ((dupePos = GetPositionDuplicate(ref timingList, pos)) != DUPLICATE_NOT_FOUND)
        {
            timingList[dupePos] = newTiming;
            timingList.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            return true;
        }
        return false;
    }

    /*
     * Removes timing item from given ms pos.
     * pos : position in milliseconds where to remove an timing from
     * returns an boolean whether item was removed or not.
     */
    public bool RemoveTiming(float pos)
    {
        int dupePos;
        if ((dupePos = GetPositionDuplicate(ref timingList, pos)) != DUPLICATE_NOT_FOUND)
        {
            timingList.RemoveAt(dupePos);
            return true;
        }
        return false;
    }

    /*
     * Returns an list of floats which indicate that when beats happen based on the start position and timingList. An empty list is returned if position is not valid compared to timingList (eg. before first timing).
     * pos : position in milliseconds from where on to fetch the beat list.
     * count : how many beats to fetch.
     */
    public List<float> getBeatList(float pos, int count = 10)
    {
        List<float> beatList = new List<float>();
        if (timingList.Count > 0)
        {
            float currentBeat = pos;
            int timingLoop = -1;
            do
            {
                timingLoop++;
                (float, float, int) closestTiming = timingList.Aggregate((x, y) => Math.Abs(x.Item1 - currentBeat) < Math.Abs(y.Item1 - currentBeat) ? x : y);

                if (closestTiming.Item1 > currentBeat)
                    break;

                float beatTime = ((60 / closestTiming.Item2) / closestTiming.Item3) * 1000;

                if (timingLoop == 0)
                {
                    currentBeat = (int)((currentBeat - closestTiming.Item1) / beatTime) * beatTime;
                }
                beatList.Add(currentBeat);


                float nextTimingPos = timingList.FirstOrDefault(x => x.Item1 > currentBeat).Item1;
                nextTimingPos = nextTimingPos > currentBeat ? nextTimingPos : -1;

                while (beatList.Count < count)
                {
                    if (nextTimingPos != -1)
                    {
                        if (currentBeat + beatTime >= nextTimingPos)
                        {
                            currentBeat += beatTime;
                            break;
                        }
                    }
                    currentBeat += beatTime;
                    beatList.Add(currentBeat);
                }
            } while (beatList.Count < count);
        }
        return beatList;
    }


    /*
     * Exports the given songmap as independent difficulty file with SONG_MIME_TYPE file ending. The class is serialized as XML type.
     */
    public void Save()
    {
        if (Validate())
        {
            string folder = GetSongmapFolderPath();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            using (var writer = new System.IO.StreamWriter(GetSongmapPath()))
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
    public static Songmap Load(string filePath)
    {
        using (var stream = System.IO.File.OpenRead(filePath))
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Songmap));
            Songmap newSongmap = serializer.Deserialize(stream) as Songmap;
            if (newSongmap.Validate())
            {
                return newSongmap;
            }
            throw new Exception("Loaded songmap was not valid");
        }
    }

    /* Returns the calculated approach rate of map in milliseconds on how fast the objects should become visible at. */
    public float GetApproachRate()
    {
        return AR_DURATION / approachRate;
    }

    /* returns the calculated HAL which tells what is the windows of accurate hitting on mapObjects. */
    public float GetHitAccuracyLevel()
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
    private bool Validate()
    {
        List<bool> checks = new List<bool>();
        checks.Add(HasAudioFile());
        checks.Add(HasValidContent());
        return !checks.Contains(false);
    }

    /* Checks if the audioFile of given songmap is in their respective song folder */
    private bool HasAudioFile()
    {
        return File.Exists($"{GetSongmapFolderPath()}\\{audioFilename}");
    }

    /* Validates that the contents (timings and mapObjects) are valid and usable */
    private bool HasValidContent()
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
    private int GetPositionDuplicate(ref List<(float, float, int)> timingList, float pos)
    {
        foreach ((float, float, int) timingItem in timingList)
        {
            if (timingItem.Item1 == pos)
                return timingList.IndexOf(timingItem);
        }
        return DUPLICATE_NOT_FOUND;
    }

    /* Performs same operation as method above, but to an map object list */
    private int GetPositionDuplicate(ref List<(float, string)> mapObjList, float pos)
    {

        foreach ((float, string) mapObj in mapObjList)
        {
            if (mapObj.Item1 == pos)
                return mapObjList.IndexOf(mapObj);
        }
        return DUPLICATE_NOT_FOUND;
    }

    /* Returns filepath to currently selected audio files folder */
    private string GetSongmapFolderPath()
    {
        return $"{SONGS_FOLDER}\\{Path.GetFileNameWithoutExtension(audioFilename)}";
    }

    /* Returns filepath to this songmaps difficulty file */
    private string GetSongmapPath()
    {
        return $"{GetSongmapFolderPath()}\\{Path.GetFileNameWithoutExtension(audioFilename)} {difficultyTitle}{SONG_MIME_TYPE}";
    }

    /* Returns filepath to this songmaps audio file */
    private string GetSongmapAudioFilePath()
    {
        return $"{GetSongmapFolderPath()}\\{audioFilename}";
    }
}
