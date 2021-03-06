﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController
{
    // skinwidth is a small part of the player model on the bottom
    public const float skinWidth = .015f;
    const float distanceBetweenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D boxcollider;
    public RaycastOrigins raycastOrigins;

    // TODO: CalculateRaySpacing() only gets called when creating the RaycastController.
    // This means that the rayspacing will NOT be recalculated if the size of the object changes.
    public RaycastController(BoxCollider2D boxcollider)
    {
        this.boxcollider = boxcollider;
        CalculateRaySpacing();
    }

    // Calculate exact position of object
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxcollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = boxcollider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        // makes sure that there are at least 2 rays
        horizontalRayCount = Mathf.RoundToInt(boundsHeight / distanceBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / distanceBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
