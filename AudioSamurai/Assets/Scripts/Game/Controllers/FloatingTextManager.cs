using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextManager : Singleton<FloatingTextManager>
{
    public FloatingText prefab;
    private ObjectPool floatingTextPool;

    private new void Awake() {
        base.Awake();
        floatingTextPool = new ObjectPool(prefab, transform);
    }

    /* Fetches an inactive floating text from the object pool and places it on the target transform position. Also sets the given text and color if given. */
    public FloatingText PlaceFloatingText(Vector3 position, string text = default, Color color = default) {
        var textObj = (FloatingText)floatingTextPool.Get();
        textObj.transform.position = position;
        textObj.transform.LookAt(Camera.main.transform);
        textObj.transform.Rotate(new Vector3(0, 180, 0));
        var textMeshPro = textObj.GetComponent<TMPro.TextMeshPro>();
        if (text != default) {
            textMeshPro.SetText(text);
        }
        if (color != default) {
            textMeshPro.color = color;
        }
        return textObj;
    }

    public FloatingText PlaceFloatingText(Vector3 position, Vector3 offset, string text = default, Color color = default) {
        FloatingText textObj = PlaceFloatingText(position, text, color);
        textObj.transform.position += offset;
        return textObj;
    }
}
