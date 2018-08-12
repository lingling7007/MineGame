using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TPCameraState
{
	public string Name;
	public float forward;
	public float right;
    public float mouseX;
    public float mouseY;
    public float distance;

    public float fieldOfView;
	public float maxDistance;
	public float minDistance;
	public float Height;

    public Vector2 startZoomAngle;
    public Vector2 endZoomAngle;
    public float zoomAngleAateX;
    public float zoomAngleAateY;
    public float startZoomDistance;
    public float endZoomDistance;
    public float zoomDistanceAate;

    public TPCameraState(string name)
    {
        this.Name = name;
        this.forward = -1f;
        this.right = 0.35f;
        this.mouseX = 45f;
        this.mouseY = 60f;
        this.distance = 10f;
        this.maxDistance = 15f;
        this.minDistance = 3.7f;
        this.Height = 1.5f;

        this.startZoomAngle = new Vector2(80, 45);
        this.endZoomAngle = new Vector2(75, 13);
        this.zoomAngleAateX = 1.55f;
        this.zoomAngleAateY = 0.25f;
        this.startZoomDistance = 10f;
        this.endZoomDistance = 6f;
        this.zoomDistanceAate = 0.2f;
    }
}
