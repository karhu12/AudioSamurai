using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    /* Constants */
    public const float BEAT_DISTANCE = 2f;

    private const float SPAWN_AHEAD_IN_MS = 1000;
    private const float TIME_AFTER_LAST_OBJECT = 4000;
    private const float FADEOUT_STEP_DURATION = 100;
    public static readonly Vector3 START_POSITION = new Vector3(0, 0, 0);

    public Collider hitArea;
    public Player player;

    public enum GameState
    {
        Idle,
        Ready,
        Playing,
        WaitingForEnd,
        Paused,
        EndScreen
    }

    private Songmap selectedSongmap;

    private List<(float, string)> spawnQueue = new List<(float, string)>();
    private (float, float, int) currentTiming;

    public GameState State
    {
        get; private set;
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        if (hitArea == null)
        {
            hitArea = GameObject.FindGameObjectWithTag("HitArea").GetComponent<Collider>();
        }
    }

    /*
     * Loads the given songmaps resources and sets everything ready for playing.
     * Returns boolean whether it succeeded in loading the map.
     */
    public bool LoadGame(Songmap songmap)
    {
        if (State == GameState.Idle)
        {
            foreach (var mapObj in songmap.GetMapObjects())
            {
                spawnQueue.Add(mapObj);
            }
            selectedSongmap = songmap;
            State = GameState.Ready;
            return true;
        }
        return false;
    }

    public void StartGame()
    {
        if (State == GameState.Ready)
            StartCoroutine(GameStartCoroutine());
    }

    public void MoveFromEndScreen()
    {
        if (State == GameState.EndScreen)
        {
            CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
            State = GameState.Idle;
        }
    }

    /* Private methods */

    /* Check internal state on update and do necessary stuff based on it. */
    private void Update()
    {
        switch (State)
        {
            case GameState.Playing:
                OnPlayingUpdate();
                break;
        }
    }

    /* Called for every frame while playing. Controls the map object spawning and settings the correct pace. */
    private void OnPlayingUpdate()
    {
        HandleGameSpeed();
        HandleSpawnQueue();
        if (spawnQueue.Count == 0 && !MapObjectManager.Instance.HasActiveObjects())
        {
            StartCoroutine(GameEndCoroutine());
            State = GameState.WaitingForEnd;
        }
    }

    /* Checks if the items in the queue are within SPAWN_AHEAD_IN_MS from the current time in song and instantiates them in correct place if they are. */
    private void HandleSpawnQueue()
    {
        List<(float, string)> removeList = new List<(float, string)>();
        /* Note. mapObject ms position can not be less than first beat becaues of map validation. */
        foreach (var obj in spawnQueue)
        {
            float songPosMs = SongmapController.Instance.AudioSource.time * 1000;
            if (obj.Item1 <= (songPosMs + SPAWN_AHEAD_IN_MS))
            {
                removeList.Add(obj);
                MapObject mapObject = MapObjectManager.Instance.GetMapObject(obj.Item2);
                float beatTime = (60 / currentTiming.Item2 / currentTiming.Item3) * 1000;

                float spawnPosOffset = ((obj.Item1 - songPosMs) / beatTime) * GameController.BEAT_DISTANCE;

                mapObject.transform.position = new Vector3(0, mapObject.GetPlacementValue(), hitArea.transform.position.z + spawnPosOffset);
            }
        }
        foreach (var removeItem in removeList)
        {
            spawnQueue.Remove(removeItem);
        }
    }

    /* Sets the speed of entities affected by the game based on the BPM of current section. */
    private void HandleGameSpeed()
    {
        (float, float, int) timingItem = selectedSongmap.GetClosestTimingAt(SongmapController.Instance.AudioSource.time);
        if (timingItem.Item1 <= SongmapController.Instance.AudioSource.time)
        {
            currentTiming = timingItem;
        }

        if (!player.IsRunning)
            player.IsRunning = true;
        player.ChangeSpeed(currentTiming.Item2);
    }

    /* Handles moving the user from game scene to result screen and display the result ui. */
    private void OnGameEnd()
    {
        player.IsRunning = false;
        player.transform.position = START_POSITION;
        State = GameState.EndScreen;
        SongmapController.Instance.AudioSource.Stop();
        CameraController.Instance.SetCameraToState(CameraController.CameraState.GameResult);
    }

    private IEnumerator GameEndCoroutine()
    {
        float decrementPerStep = SongmapController.Instance.AudioSource.volume / (TIME_AFTER_LAST_OBJECT / FADEOUT_STEP_DURATION);
        for (float timer = 0; timer < TIME_AFTER_LAST_OBJECT; timer += FADEOUT_STEP_DURATION)
        {
            yield return new WaitForSeconds(FADEOUT_STEP_DURATION / 1000);
            if (SongmapController.Instance.AudioSource.volume - decrementPerStep > 0)
                SongmapController.Instance.AudioSource.volume = SongmapController.Instance.AudioSource.volume - decrementPerStep;
        }
        SongmapController.Instance.AudioSource.volume = 0;
        OnGameEnd();
    }

    /* Coroutine that ensures everything is setup for playing. */
    private IEnumerator GameStartCoroutine()
    {
        yield return InitialCoroutine();
        yield return CountdownCoroutine();
        SongmapController.Instance.AudioSource.Play();
        State = GameState.Playing;
        yield return null;
    }

    /* Ensures that the song starts from the start and the camera is set to the correct state. */
    private IEnumerator InitialCoroutine()
    {
        SongmapController.Instance.AudioSource.Pause();
        SongmapController.Instance.AudioSource.time = 0;
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Game);
        yield return new WaitForSeconds(1); // Wait for the camera to get in place.
    }

    /* Initiates countdown lasting for given amount of seconds. */
    private IEnumerator CountdownCoroutine(int seconds = 3)
    {
        for (int second = seconds; second > 0; second--)
        {
            Debug.Log($"Countdown: {second}");
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Go!");
    }
}
