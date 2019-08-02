using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kayac;

[Serializable]
public class StageData
{
	public FlipperParams[] flippers;
	public ShapeData shape;
}

[System.Serializable]
public class FlipperParams
{
	public bool isLeft;
	public float x;
	public float y;
	public float rotation;
	public float length;
	public float thickness;
	public float motorSpeed;
	public float angleMin;
	public float angleMax;
}

