using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LuaInterface;

public class LuaEventListener: EventTrigger
{
    private static LuaFunction onUp;
    private LuaFunction onDown;
    private LuaFunction onEnter;
    private LuaFunction onExit;
    private LuaFunction onDragBegin;
    private LuaFunction onDragEnd;
    private LuaFunction onDrag;
    public static LuaEventListener AddUpEvent(GameObject go,LuaFunction luafunc = null)
    {
        LuaEventListener listen = go.GetComponent<LuaEventListener>();
        if (listen == null)
        {
            listen = go.AddComponent<LuaEventListener>();
        }
        onUp = luafunc;
        return listen;
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null)
        {
            onUp.Call(eventData.pointerEnter, eventData.position.x, eventData.position.y);
        }
    }
}
