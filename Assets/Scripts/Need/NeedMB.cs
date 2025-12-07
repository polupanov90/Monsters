using System;
using UnityEngine;
using UnityEngine.UI;

public class NeedMB : MonoBehaviour {
    private RawImage rawImageComponent;
    public NeedO needO;
    
    private void Awake() {
        rawImageComponent = GetComponent<RawImage>();
    }

    public void SetImage(NeedO _needO) {
        needO = _needO;
        rawImageComponent.texture = needO.sprite.texture;
        rawImageComponent.color = new Color(255, 255, 255, 255);
    }
    public void ClearImage() {
        needO = null;
        rawImageComponent.texture = null;
        rawImageComponent.color = new Color(0, 0, 0, 0);
    }

    public bool GetIncludeNeedType(NeedO _needO) {
        return needO != null && _needO.type == needO.type;
    }

    public bool GetIsShow() {
        return needO != null;
    }
}
