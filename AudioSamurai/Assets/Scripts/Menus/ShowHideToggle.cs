using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideToggle : MonoBehaviour
{

    public List<GameObject> hideList;
    public List<GameObject> showList;

    public void Toggle(bool show)
    {
        foreach(var obj in hideList)
        {
            if (show)
                obj.SetActive(false);
            else
                obj.SetActive(true);
        }
        foreach (var obj in showList)
        {
            if (show)
                obj.SetActive(true);
            else
                obj.SetActive(false);
        }
    }
}
