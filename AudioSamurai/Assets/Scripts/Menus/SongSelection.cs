using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongSelection : MonoBehaviour
{
    /* Constants */
    readonly Color32 SELECTED_COLOR = new Color32(0xB9, 0x34, 0x88, 0xff);
    readonly Color32 UNSELECTED_COLOR = new Color32(0xB0, 0x15, 0xCF, 0xff);

    public GameObject songmapParentPrefab;
    public GameObject songmapChildPrefab;
    public GameObject songmapPrefab;
    public Button playSongButton;
    public ScrollRect scrollView;
    public RectTransform content;

    List<SongmapView> views = new List<SongmapView>();


    IReadOnlyDictionary<string, List<Songmap>> maps;
    SongmapView selectedView;
    SongmapChildView selectedChildView;

    private void Start()
    {
        Refresh();
    }

    /*
     * Updates the song list using songmapControllers songmaps.
     */
    public void Refresh()
    {
        if (SongmapController.Instance.AudioSource.isPlaying)
        {
            SongmapController.Instance.AudioSource.Stop();
        }
        views.Clear();
        playSongButton.gameObject.SetActive(false);
        maps = SongmapController.Instance.GetSongmaps(SongmapController.SongmapSortType.DIFF, SongmapController.SongmapSortDirection.ASC);

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var key in maps.Keys)
        {
            SongmapView view = new SongmapView(ref content, songmapPrefab);
            view.AddParentSongmapView(songmapParentPrefab);
            view.parentSongmapView.title.text = key.ToString();

            foreach (var map in maps[key])
            {
                view.AddSongmapChildView(songmapChildPrefab, map);
            }
            views.Add(view);
            view.ToggleChildren();
        }
    }

    public void RefreshButton()
    {
        SongmapController.Instance.AudioSource.Stop();
        views.Clear();
        playSongButton.gameObject.SetActive(false);
        maps = SongmapController.Instance.GetSongmaps(SongmapController.SongmapSortType.DIFF, SongmapController.SongmapSortDirection.ASC);

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
            FindObjectOfType<AudioManager>().Play("Refresh");
        }

        foreach (var key in maps.Keys)
        {
            SongmapView view = new SongmapView(ref content, songmapPrefab);
            view.AddParentSongmapView(songmapParentPrefab);
            view.parentSongmapView.title.text = key.ToString();

            foreach (var map in maps[key])
            {
                view.AddSongmapChildView(songmapChildPrefab, map);
            }
            views.Add(view);
            view.ToggleChildren();
        }
    }

    /*
     * Fired when the back button is pressed. Takes the used back to the main menu.
     */
    public void OnBackPress()
    {
        FindObjectOfType<AudioManager>().Play("ClickDeny");
        FindObjectOfType<AudioManager>().Pause("MenuMusic");
        FindObjectOfType<AudioManager>().Play("MenuMusic");
        CameraController.Instance.SetCameraToState(CameraController.CameraState.Menu);
        ResetSongSelectionView();
    }

    /*
     * Fired when one of the songmap titles has been clicked. Expands its actual songmaps if not open, else hides them.
     * Also plays the maps audio while children are expanded
     */
    public void OnSongmapParentClick(Text title)
    {
        playSongButton.gameObject.SetActive(false);
        if (selectedChildView != null && selectedChildView.gameObject != null)
            selectedChildView.gameObject.GetComponent<Image>().color = UNSELECTED_COLOR;

        foreach (var view in views)
        {
            if (view.parentSongmapView.title.text == title.text)
            {
                if (view.ToggleChildren())
                {
                    selectedView = view;
                    FindObjectOfType<AudioManager>().Play("Click");
                    FindObjectOfType<AudioManager>().Pause("MenuMusic");
                    SongmapController.Instance.PlaySongmapAudio(maps[title.text][0]);
                }
                else
                {
                    FindObjectOfType<AudioManager>().Play("Click");
                    SongmapController.Instance.AudioSource.Stop();
                    selectedView = null;
                    selectedChildView = null;
                }
            } else if (view.HasExpandedChildren())
            {
                view.ToggleChildren();
            }
        }
        
    }

    /*
     * Fired when songmap title has been expanded and its child has been pressed. Will selected the given children, set play button active and high light the selection.
     */
    public void OnSongmapClick(Text difficulty)
    {
        foreach (var child in selectedView.songmapChildViews)
        {
            if (child.difficulty.text == difficulty.text)
            {
                if (selectedChildView != null && selectedChildView.gameObject != null)
                    selectedChildView.gameObject.GetComponent<Image>().color = UNSELECTED_COLOR;

                selectedChildView = child;
                child.gameObject.GetComponent<Image>().color = SELECTED_COLOR;
                SetLeaderBoards(child);
                FindObjectOfType<AudioManager>().Play("Click");
                playSongButton.gameObject.SetActive(true);
            }
        }
    }

    public async void SetLeaderBoards(SongmapChildView child)
    {
        var x = await Mongo.Instance.GetLeaderBoards(child.songmap.GetSongmapName());
        for(int i = 0; i < x.Count; i++)
            Debug.Log(x[i].Score + " " + x[i].Name);
    }

    public void OnPlayClick()
    {
        if (GameController.Instance.LoadGame(selectedChildView.songmap))
        {
            ResetSongSelectionView();
            FindObjectOfType<AudioManager>().Play("Click");
            GameController.Instance.StartGame();
        }
    }

    private void ResetSongSelectionView()
    {
        foreach (var view in views)
        {
            view.ToggleChildren(false);
        }
        selectedView = null;
        if (selectedChildView != null && selectedChildView.gameObject != null)
            selectedChildView.gameObject.GetComponent<Image>().color = UNSELECTED_COLOR;
        selectedChildView = null;
        playSongButton.gameObject.SetActive(false);

        if (SongmapController.Instance.AudioSource.isPlaying)
        {
            SongmapController.Instance.AudioSource.Stop();
        }
    }
}

public class SongmapView
{
    public ParentSongmapView parentSongmapView;
    public List<SongmapChildView> songmapChildViews = new List<SongmapChildView>();
    public GameObject gameObject;

    bool childrenExpanded = true;

    public SongmapView(ref RectTransform content, GameObject prefab)
    {
        gameObject = MonoBehaviour.Instantiate(prefab);
        gameObject.transform.SetParent(content, false);
    }

    public void AddParentSongmapView(GameObject parentPrefab)
    {
        if (parentSongmapView != null)
        {
            if (parentSongmapView.gameObject != null)
                MonoBehaviour.Destroy(parentSongmapView.gameObject);
        }

        GameObject obj = MonoBehaviour.Instantiate(parentPrefab);
        parentSongmapView = new ParentSongmapView(obj);
        parentSongmapView.gameObject.transform.SetParent(gameObject.transform, false);
        parentSongmapView.gameObject.SetActive(true);
    }

    public void AddSongmapChildView(GameObject childPrefab, Songmap map)
    {
        GameObject obj = MonoBehaviour.Instantiate(childPrefab);
        SongmapChildView childView = new SongmapChildView(obj, map);
        childView.gameObject.transform.SetParent(gameObject.transform, false);
        childView.gameObject.SetActive(true);
        songmapChildViews.Add(childView);
    }

    public bool HasExpandedChildren()
    {
        return songmapChildViews[0].gameObject.activeSelf;
    }

    public bool ToggleChildren()
    {
        childrenExpanded = !childrenExpanded;
        foreach (var view in songmapChildViews)
        {

            view.gameObject.SetActive(childrenExpanded);
        }
        return childrenExpanded;
    }

    public bool ToggleChildren(bool setOn)
    {
        childrenExpanded = setOn;
        foreach (var view in songmapChildViews)
        {
            view.gameObject.SetActive(setOn);
        }
        return childrenExpanded;
    }
}

public class View
{
    public GameObject gameObject;

    public View(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }
}

public class ParentSongmapView : View
{
    public Text title;

    public ParentSongmapView(GameObject gameObject) : base(gameObject)
    {
        title = this.gameObject.transform.Find("ParentTitle").GetComponent<Text>();
    }
}

public class SongmapChildView : View
{
    public Text title;
    public Text difficulty;
    public Text highScore;
    public Text accuracy;
    public Text gradeTitle;
    public RawImage grade;
    public RectTransform difficultyForeground;
    public RectTransform difficultyMask;
    public Songmap songmap;
    public GameResult gameResult;

    public SongmapChildView(GameObject gameObject, Songmap map) : base(gameObject)
    {
        songmap = map;
        Transform infoPanel = this.gameObject.transform.Find("HorizontalPanel").Find("InfoPanel").Find("InfoPanel (1)");
        title = infoPanel.Find("ItemTitle").GetComponent<Text>();
        difficulty = infoPanel.Find("ItemDifficultyName").GetComponent<Text>();
        Transform highScorePanel = this.gameObject.transform.Find("HorizontalPanel").Find("HiscorePanel");
        highScore = highScorePanel.Find("ItemHiscore").GetComponent<Text>();
        accuracy = highScorePanel.Find("ItemAccuracy").GetComponent<Text>();
        Transform gradePanel = this.gameObject.transform.Find("HorizontalPanel").Find("GradePanel");
        gradeTitle = gradePanel.Find("ItemGradeTitle").GetComponent<Text>();
        grade = gradePanel.Find("ItemGrade").GetComponent<RawImage>();
        difficultyMask = this.gameObject.transform.Find("HorizontalPanel").Find("InfoPanel").Find("DifficultyPanel").Find("ForegroundMask").GetComponent<RectTransform>();
        difficultyForeground = difficultyMask.transform.Find("DifficultyForeground").GetComponent<RectTransform>();

        title.text = map.GetSongmapName(false);
        difficulty.text = map.DifficultyTitle;
        gameResult = HighScoreManager.Instance.GetGameResult(map.GetSongmapName());
        
        if (gameResult.Score > 0)
        {
            highScore.text = HighScoreManager.GetFormattedHighscore(gameResult);
            accuracy.text = $"{gameResult.RoundedHitPercentage} %";
        } else
        {
            highScorePanel.gameObject.SetActive(false);
        }

        if (gameResult.perfects + gameResult.normals + gameResult.poors + gameResult.misses > 0)
        {
            grade.texture = ScoreSystem.Instance.GetResultGradeTexture(gameResult.ResultGrade);
            gradeTitle.text = gameResult.ResultGrade.ToString();
        } else
        {
            gradePanel.gameObject.SetActive(false);
        }

        float difficultyStep = (difficultyForeground.sizeDelta.x / Songmap.MAX_DIFFICULTY);
        float maskOffset = Songmap.MAX_DIFFICULTY * difficultyStep - difficultyStep * map.GetDifficulty();
        difficultyMask.offsetMin = new Vector2(0, 0);
        difficultyMask.offsetMax = new Vector2(-maskOffset, 0);
    }
}