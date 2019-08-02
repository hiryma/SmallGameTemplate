using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointData
{
	public float x;
	public float y;
}

[System.Serializable]
public class LineData
{
	public PointData[] points;
}

[System.Serializable]
public class ShapeData
{
	public LineData[] lines;
}

[System.Serializable]
public class LauncherParams
{
	public float x;
	public float y;
	public float spring;
	public float damper;
	public float drawAccel;
}

[System.Serializable]
public class GlobalParams
{
	public float gravity;
	public int stageCount;
	public float flipperMotorSpeed;
	public float flipperDuration;
	public LauncherParams launcher;
	public ShapeData commonShape;
}
