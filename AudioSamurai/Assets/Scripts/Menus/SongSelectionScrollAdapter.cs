using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectionScrollAdapter : MonoBehaviour
{
    public SongmapController songmapController;
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

    public void Refresh()
    {
        views.Clear();
        playSongButton.gameObject.SetActive(false);
        maps = songmapController.GetSongmaps();

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
                view.AddSongmapChildView(songmapChildPrefab);
                SongmapChildView child = view.songmapChildViews[view.songmapChildViews.Count - 1];
                child.title.text = map.DifficultyTitle;
                child.hitAccuracyLevel.text = $"HAL: {map.HitAccuracyLevel}";
                child.approachRate.text = $"AR: {map.ApproachRate}";
                child.difficulty.text = $"Difficulty: {map.GetDifficulty()}";
            }
            views.Add(view);
            view.ToggleChildren();
        }
    }

    public void OnSongmapParentClick(Text title)
    {
        foreach (var view in views)
        {
            if (view.parentSongmapView.title.text == title.text)
            {
                if (view.ToggleChildren())
                {
                    selectedView = view;
                    songmapController.PlaySongmapAudio(maps[title.text][0]);
                }
                else
                {
                    songmapController.AudioSource.Stop();
                    playSongButton.gameObject.SetActive(false);
                    selectedView = null;
                    selectedChildView = null;
                }
                break;
            }
        }
    }

    public void OnSongmapClick(Text title)
    {
        foreach (var child in selectedView.songmapChildViews)
        {
            if (child.title.text == title.text)
            {
                selectedChildView = child;
                playSongButton.gameObject.SetActive(true);
            }
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

    public void AddSongmapChildView(GameObject childPrefab)
    {
        GameObject obj = MonoBehaviour.Instantiate(childPrefab);
        SongmapChildView childView = new SongmapChildView(obj);
        childView.gameObject.transform.SetParent(gameObject.transform, false);
        childView.gameObject.SetActive(true);
        songmapChildViews.Add(childView);
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
    public Text approachRate;
    public Text hitAccuracyLevel;

    public SongmapChildView(GameObject gameObject) : base(gameObject)
    {
        title = this.gameObject.transform.Find("ItemTitle").GetComponent<Text>();
        difficulty = this.gameObject.transform.Find("ItemDifficulty").GetComponent<Text>();
        approachRate = this.gameObject.transform.Find("ItemAR").GetComponent<Text>();
        hitAccuracyLevel = this.gameObject.transform.Find("ItemHAL").GetComponent<Text>();
    }
}