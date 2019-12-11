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

    /* 
     * Returns an inactive floating text from the object pool and places it on the target position. Also sets the given text and color if given. 
     * position : vector3 position where the text should appear at.
     * text : text that should be set to the given text.
     * color : color that should be set to the given text.
     * lookAtCamera : whether the text should aim at camera when instantiated.
     */
    public FloatingText PlaceFloatingText(Vector3 position, string text = default, Color color = default, bool lookAtCamera = true)
    {
        var textObj = (FloatingText)floatingTextPool.Get();
        textObj.transform.position = position;
        if (lookAtCamera)
        {
            textObj.transform.LookAt(Camera.main.transform);
            textObj.transform.Rotate(new Vector3(0, 180, 0));
        }
        var textMeshPro = textObj.GetComponent<TMPro.TextMeshPro>();
        if (text != default)
        {
            textMeshPro.SetText(text);
        }
        if (color != default)
        {
            textMeshPro.color = color;
        }
        return textObj;
    }

    /* Same as above except position offset is applied after creation. */
    public FloatingText PlaceFloatingText(Vector3 position, Vector3 offset, string text = default, Color color = default, bool lookAtCamera = true)
    {
        position += offset;
        FloatingText textObj = PlaceFloatingText(position, text, color, lookAtCamera);
        return textObj;
    }

    /* Same as above except text will not look at camera but rotate to given quaternion. */
    public FloatingText PlaceFloatingText(Vector3 position, Vector3 eulerAngles, string text = default, Color color = default)
    {
        FloatingText textObj = PlaceFloatingText(position, text, color, false);
        textObj.gameObject.transform.eulerAngles = eulerAngles;
        return textObj;
    }

    /* Same as above but with offset. */
    public FloatingText PlaceFloatingText(Vector3 position, Vector3 offset, Vector3 eulerAngles, string text = default, Color color = default)
    {
        position += offset;
        FloatingText textObj = PlaceFloatingText(position, eulerAngles, text, color);
        return textObj;
    }
}
