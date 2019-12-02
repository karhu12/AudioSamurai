using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    /* Constants */
    public const float BEAT_DISTANCE = 4f;
    public const float BEAT_DISTANCE_PER_BPM_MULT = 1f;
    public const float BPM_MULTIPLIER = 60f;

    private const float SPAWN_AHEAD_IN_MS = 5000;
    private const float TIME_AFTER_LAST_OBJECT = 4000;
    private const float FADEOUT_STEP_DURATION = 100;
    public static readonly Vector3 START_POSITION = new Vector3(0, 0, 0);

    public Collider hitArea;
    public Player player;
    public Canvas hud;
    public FailMenu failMenu;
    
    public enum GameState
    {
        Idle,
        Ready,
        Playing,
        WaitingForEnd,
        Paused,
        EndScreen,
        FailScreen
    }

    public Songmap SelectedSongmap { get; private set; }
    public (float, float, int) CurrentTiming { get; private set; }
    private List<(float, float, string)> spawnQueue = new List<(float, float, string)>();
    private List<(float, float, int)> timingQueue = new List<(float, float, int)>();

    public GameState State
    {
        get; private set;
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag(Player.COLLIDER_NAME).GetComponent<Player>();

        if (hitArea == null)
            hitArea = GameObject.FindGameObjectWithTag(Player.HIT_COLLIDER_NAME).GetComponent<Collider>();

        if (hud == null)
            hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
        hud.gameObject.SetActive(false);

        if (failMenu == null)
            failMenu = GameObject.FindGameObjectWithTag(FailMenu.TAG).GetComponent<FailMenu>();
    }

    public bool Pause()
    {
        if (State == GameState.Playing)
        {
            SongmapController.Instance.AudioSource.Pause();
            State = GameState.Paused;
            return true;
        }
        return false;
    }

    public bool Unpause()
    {

        if (State == GameState.Paused)
        {
            SongmapController.Instance.AudioSource.UnPause();
            State = GameState.Playing;
            return true;
        }
        return false;
    }

    public void Retry() {
        if (State == GameState.Paused || State == GameState.FailScreen)
        {
            ResetGameState();
            if (LoadGame(SelectedSongmap)) {
                StartGame();
            }
        }
    }

    public void QuitGame()
    {
        if (State == GameState.Paused || State == GameState.Playing || State == GameState.FailScreen)
        {
            CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
            State = GameState.Idle;
            ResetGameState();
        }
    }

    /*
     * Loads the given songmaps resources and sets everything ready for playing.
     * Returns boolean whether it succeeded in loading the map.
     */
    public bool LoadGame(Songmap songmap)
    {
        if (State == GameState.Idle || State == GameState.FailScreen || State == GameState.Paused)
        {
            SelectedSongmap = songmap;
            Calculate();
            State = GameState.Ready;
            return true;
        }
        return false;
    }

    /* Performs the game start coroutine if it is ready. */
    public void StartGame()
    {
        if (State == GameState.Ready)
            StartCoroutine(GameStartCoroutine());
    }

    /* If the game is currently on the result screen, it will move the game back to song selection and set the state back to idling. */
    public void MoveFromEndScreen()
    {
        FindObjectOfType<AudioManager>().Stop("Win");
        FindObjectOfType<AudioManager>().Play("ClickDeny");
        if (State == GameState.EndScreen)
        {
            State = GameState.Idle;
            CameraController.Instance.SetCameraToState(CameraController.CameraState.SongSelection);
        }
    }

    /* Calculates what score it should reward player with based on the hitTiming */
    public int CalculateHitScore(float hitTiming, float originalHitTime) {
        float accHitTime = SelectedSongmap.GetHitAccuracyLevel(GameController.Instance.CurrentTiming);
        float hitTime = Math.Abs(originalHitTime - hitTiming);
        if (hitTime < accHitTime) {
            return (int)ScoreSystem.HitType.Perfect;
        }
        else if (hitTime >= accHitTime && hitTime <= accHitTime * 2) {
            return (int)ScoreSystem.HitType.Normal;
        }
        else if (hitTime >= accHitTime * 2 && hitTime <= accHitTime * 3) {
            return (int)ScoreSystem.HitType.Poor;
        }
        return (int)ScoreSystem.HitType.Miss;
    }

    /* Calculates what the damage multiplier should be based on current mods. */
    public float GetDamageMultiplier()
    {
        float damageMultiplier = SelectedSongmap.HealthDrainlevel;
        switch (ModeManager.Instance.GetMode())
        {
            case ModeManager.SUDDEN_DEATH_MOD:
                damageMultiplier = Player.STARTING_HEALTH / Player.DAMAGE_AMOUNT;
                break;
            case ModeManager.NO_FAIL_MOD:
                if (player.Health <= 0)
                    damageMultiplier = 0;
                break;
        }
        return damageMultiplier;
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
    private void OnPlayingUpdate() {
        if (player.Health <= 0 && ModeManager.Instance.GetMode() != ModeManager.NO_FAIL_MOD) {
            GameFail();
        } else {
            HandleGameSpeed();
            HandleSpawnQueue();
            if (spawnQueue.Count == 0 && !MapObjectManager.Instance.HasActiveObjects()) {
                StartCoroutine(GameEndCoroutine());
                State = GameState.WaitingForEnd;
            }
        }
    }

    
    /* Checks if the items in the queue are within SPAWN_AHEAD_IN_MS from the current time in song and instantiates them in correct place if they are. */
    private void HandleSpawnQueue()
    {
        List<(float, float, string)> removeList = new List<(float, float, string)>();
        /* Note. mapObject ms position can not be less than first beat becaues of map validation. */
        foreach (var obj in spawnQueue)
        {
            float songPosMs = SongmapController.Instance.GetAccuratePlaybackPositionMs();
            if (obj.Item1 <= (songPosMs + SPAWN_AHEAD_IN_MS))
            {
                removeList.Add(obj);
                MapObject mapObject = MapObjectManager.Instance.GetMapObject(obj.Item3);
                mapObject.Timing = obj.Item1;
                mapObject.transform.position = new Vector3(0, mapObject.GetPlacementValue(), obj.Item2);

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
        if (!player.IsRunning)
            player.IsRunning = true;

        List<(float, float, int)> removeList = new List<(float, float, int)>();

        foreach (var timing in timingQueue)
        {
            if (timing.Item1 <= SongmapController.Instance.GetAccuratePlaybackPositionMs())
            {
                player.ChangeSpeed(timing.Item2);
                CurrentTiming = timing;
                removeList.Add(timing);
            }
        }

        foreach (var remove in removeList)
        {
            timingQueue.Remove(remove);
        }
    }

    /* Handles moving the user from game scene to result screen and display the result ui. */
    private void OnGameEnd()
    {
        ScoreSystem.Instance.FinalizeResult();
        FindObjectOfType<ResultUpdater>().UpdateResult();
        State = GameState.EndScreen;
        CameraController.Instance.SetCameraToState(CameraController.CameraState.GameResult);
        FindObjectOfType<AudioManager>().Play("Win");
        ResetGameState();
    }

    private void ResetGameState() {
        MapObjectManager.Instance.Cleanup();
        SongmapController.Instance.AudioSource.Stop();
        spawnQueue.Clear();
        timingQueue.Clear();
        player.ResetPlayerStatus();
        if (hud.gameObject.activeSelf)
            hud.gameObject.SetActive(false);
        ScoreSystem.Instance.ResetCombo();
        ScoreSystem.Instance.ResetScore();
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
    private IEnumerator GameStartCoroutine() {
        yield return InitialCoroutine();
        if (!hud.gameObject.activeSelf)
            hud.gameObject.SetActive(true);
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
        FindObjectOfType<AudioManager>().Play("Countdown");
        for (int second = seconds; second > 0; second--)
        {
            Debug.Log($"Countdown: {second}");
            FloatingTextManager.Instance.PlaceFloatingText(new Vector3(3.3f, 3.75f, .79f), $"{second}", Color.red);
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Go!");
        FloatingTextManager.Instance.PlaceFloatingText(new Vector3(3.3f, 3.75f, .79f), "Go", Color.green);
        yield return new WaitForSeconds(1);
    }

    private void GameFail() {
        if (State == GameState.Playing) {
            CameraController.Instance.SetCameraToState(CameraController.CameraState.FailMenu);
            State = GameState.FailScreen;
            failMenu.ToggleFailMusic(true);
            ResetGameState();
        }
    }
    
    private void Calculate()
    {
        List<float> beats = SelectedSongmap.getBeatList(0, SongmapController.Instance.AudioSource.clip.length * 1000);
        List<(float, float)> beatSpawnPositions = new List<(float, float)>();
        foreach (var beat in beats) {
            var idx = beats.IndexOf(beat);
            var closest = SelectedSongmap.GetClosestTimingAt(beat);
            float spawnPosition = 0;
            if (idx == 0)
            {
                spawnPosition += Player.HIT_AREA_OFFSET;
                spawnPosition += Player.HIT_AREA_DEPTH;
            } else {
                spawnPosition = beatSpawnPositions.Last().Item2 + BEAT_DISTANCE / closest.Item3;
            }
            beatSpawnPositions.Add((beat, spawnPosition));
        }

        foreach (var mapObj in SelectedSongmap.GetMapObjects())
        {
            (float,float) closestBeat = beatSpawnPositions.FindLast(item => Math.Round((double)item.Item1, 0) <= Math.Round((double)mapObj.Item1, 0));
            spawnQueue.Add((mapObj.Item1, closestBeat.Item2, mapObj.Item2));
        }

        foreach (var timing in SelectedSongmap.GetTimingList())
        {
            timingQueue.Add((timing.Item1, timing.Item2, timing.Item3));
        }

        ScoreSystem.Instance.gameResult.MaxCombo = SelectedSongmap.GetMaxCombo();
        HandleSpawnQueue();
    }
}
