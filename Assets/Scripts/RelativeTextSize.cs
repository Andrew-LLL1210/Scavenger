using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelativeTextSize : MonoBehaviour {
    public RectTransform parent;
    public float relative_size;

    void Start() {
        Text text = GetComponent<Text>();
        text.fontSize = (int)(parent.rect.height * relative_size / 100f);
    }
}
