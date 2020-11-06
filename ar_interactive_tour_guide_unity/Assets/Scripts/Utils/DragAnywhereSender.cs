using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// This component will enable you to receive drag events anywhere, ie without any collider
// How to use:
// Make the Monobehaviour you want to receive the events implement IDragAnywhereHandler
// And attach this component as well
// voila!

// Message system code based on
// https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/MessagingSystem.html

public class DragAnywhereSender : MonoBehaviour
{
    // if making this public, make sure to handle changes in mouseButtonIndex correctly
    // ie end drag if currently dragging
    int mouseButtonIndex = 0;

    Vector2 lastPosition;
    bool isDragging;

    void Update()
    {
        Vector2 position = Input.mousePosition;

        if (Input.GetMouseButtonDown(mouseButtonIndex))
        {
            lastPosition = position;
            isDragging = false;
        }

        if (Input.GetMouseButton(mouseButtonIndex) && isDragging == false)
        {
            if(position != lastPosition)
            {
                // Begin Drag
                isDragging = true;
                ExecuteEvent(x => x.OnDragBeginAnywhere(lastPosition));
            }
        }

        if (Input.GetMouseButton(mouseButtonIndex) && isDragging)
        {
            if (position != lastPosition)
            {
                Vector2 drag = position - lastPosition;
                ExecuteEvent(x => x.OnDragAnywhere(position, drag));
            }
        }

        if (Input.GetMouseButtonUp(mouseButtonIndex) && isDragging)
        {
            isDragging = false;
            ExecuteEvent(x => x.OnDragEndAnywhere(position));
        }

        lastPosition = position;
    }

    private void ExecuteEvent(Action<IDragAnywhereHandler> action)
    {
        ExecuteEvents.Execute<IDragAnywhereHandler>(gameObject, null, (x, y) => action(x));
    }
}

public interface IDragAnywhereHandler : IEventSystemHandler
{
    void OnDragBeginAnywhere(Vector2 position);

    void OnDragAnywhere(Vector2 position, Vector2 drag);

    void OnDragEndAnywhere(Vector2 position);
}