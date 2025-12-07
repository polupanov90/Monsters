using System;
using UnityEngine;

public class Need {
    public Need(NeedO needO) {
        type = needO.type;
        sprite = needO.sprite;
        guid = Guid.NewGuid();
    }
    public NeedEnum type;
    public Guid guid;
    public Sprite sprite;
}