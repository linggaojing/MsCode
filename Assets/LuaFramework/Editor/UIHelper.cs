using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIHelper:Editor {
    
	[MenuItem("GameObject/UI/Image")]
    static void CreatImage()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("image", typeof(Image));
                go.GetComponent<Image>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                Selection.activeGameObject = go;
            }
        }
    }

    
	[MenuItem("GameObject/UI/Text")]
    static void CreatText()
    {
        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.GetComponentInParent<Canvas>())
            {
                GameObject go = new GameObject("text", typeof(Text));
                go.GetComponent<Text>().raycastTarget = false;
                go.transform.SetParent(Selection.activeTransform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                Selection.activeGameObject = go;
            }
        }
    }
}
