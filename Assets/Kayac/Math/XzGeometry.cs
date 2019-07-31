using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kayac
{
	// Y無視してXZ平面で交差判定する。見掛け3Dだが中身2Dなゲーム用
	public static class XzGeometry
	{
		public static bool TestIntersectionSphereVsHalfLine(
			Vector3 sphereCenter,
			float radius,
			Vector3 lineBegin,
			Vector3 lineVector)
		{
			// まず直線と球の判定
			// 当たっている場合、垂線の足が線分の間で当たるならオーケー、
			// そうでない場合、端点が円の内部にあるか判定。
			float t;
			if (!GetIntersectionSphereVsLine(out t, sphereCenter, radius, lineBegin, lineVector))
			{
				return false;
			}
			// tが0以上なら最近点が半直線範囲内にあるので問題なく当たり
			if (t >= 0f)
			{
				return true;
			}
			// そうでない場合、端点と円の判定になる
			if (TestIntersectionSphereVsPoint(sphereCenter, radius, lineBegin))
			{
				return true;
			}
			return false;
		}

		public static bool TestIntersectionSphereVsLineSegment(
			Vector3 sphereCenter,
			float radius,
			Vector3 lineBegin,
			Vector3 lineEnd)
		{
			// まず直線と球の判定
			// 当たっている場合、垂線の足が線分の間で当たるならオーケー、
			// そうでない場合、端点が円の内部にあるか判定。
			float t;
			Vector3 lineVector = lineEnd - lineBegin;
			if (!GetIntersectionSphereVsLine(out t, sphereCenter, radius, lineBegin, lineVector))
			{
				return false;
			}
			// tが0-1なら最近点が線分内にあるので問題なく当たり
			if ((t >= 0f) && (t <= 1f))
			{
				return true;
			}
			// そうでない場合、端点と円の判定になる
			if (TestIntersectionSphereVsPoint(sphereCenter, radius, lineBegin)
				|| TestIntersectionSphereVsPoint(sphereCenter, radius, lineEnd))
			{
				return true;
			}
			return false;
		}

		public static bool TestIntersectionSphereVsPoint(
			Vector3 sphereCenter,
			float radius,
			Vector3 point)
		{
			float dx = sphereCenter.x - point.x;
			float dz = sphereCenter.z - point.z;
			float sqDistance = (dx * dx) + (dz * dz);
			return (sqDistance < (radius * radius));
		}

		public static bool TestIntersectionSphereVsSphere(
			Vector3 sphereCenter0,
			float radius0,
			Vector3 sphereCenter1,
			float radius1)
		{
			// 半径合計して球と点するだけ
			return TestIntersectionSphereVsPoint(sphereCenter0, radius0 + radius1, sphereCenter1);
		}

		public static bool GetIntersectionSphereVsLine(
			out float t,
			Vector3 sphereCenter,
			float radius,
			Vector3 lineBegin,
			Vector3 lineVector)
		{
			/*
			球中心をs、lineBeginをa、lineVectorをbとする。
			|a + b*t - s| < r
			ならば当たる。
			la-s=cとする。
			|c + b*t| < r
			同ベクトルの内積の平方根が長さなので、
			sqrt(dot((c + b*t), (c + b*t))) = |c + b*t|
			二乗して、
			dot((c + b*t), (c + b*t)) = |c + b*t|^2 < r^2
			左辺整理すると、
			dot(c,c) + dot(b,b)*t^2 + 2*dot(c,b)*t
			この値が極値を取るtが最短距離になる時のtなので、微分。
			2*dot(b,b)*t + 2*dot(c,b) = 0

			以上からt = -dot(c,b) / dot(b,b)

			この時の最短距離が半径未満になるか
			*/
			float cx = lineBegin.x - sphereCenter.x;
			float cz = lineBegin.z - sphereCenter.z;
			float bb = (lineVector.x * lineVector.x) + (lineVector.z * lineVector.z);
			if (bb == 0f)
			{
				t = float.NaN;
				return false;
			}
			float cb = (cx * lineVector.x) + (cz * lineVector.z);
			t = -cb / bb;

			// このtにおいて、c + b*tの長さがr未満になるか
			float dx = cx + (lineVector.x * t);
			float dz = cz + (lineVector.z * t);
			float sqDistance = (dx * dx) + (dz * dz);
			return (sqDistance < (radius * radius));
		}
	}
}