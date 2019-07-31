using UnityEngine;

public class CameraController
{
	Vector3 position;
	Vector3 target; // ロールしないのでzはいらない
	public Vector3 PositionGoal{ get; set; }
	public Vector3 TargetGoal{ get; set; }
	public float Stiffness { get; set; }
	public Camera Camera { get; private set; }
	float shakeMagnitude;
	public float ShakeDecline{ get; set; }

	public CameraController(Camera camera)
	{
		Camera = camera;
		position = PositionGoal = camera.transform.position;
		target = TargetGoal = position + camera.transform.forward;
	}

	/// 画角、回転固定で位置調整
	public void FitByMove(Vector3 center, float radius)
	{
		var fov = GetFovMinRadian();
		// d*tan(fov/2)=radiusとなるdを求める
		var d = radius / Mathf.Tan(fov * 0.5f);
		// 目標点からforwardベクタにdを乗じて引く
		var forward = Camera.transform.forward; // 前ベクトル
		var position = center - (forward.normalized * d);
		PositionGoal = position;
		TargetGoal = center;
	}

	// 画角、回転固定で位置調整して箱が入るようにする
	Vector3[] tmpPoints = new Vector3[8];
	public void FitByMove(Vector3 min, Vector3 max)
	{
		// 8点を取得
		var v = tmpPoints;
		v[0] = new Vector3(min.x, min.y, min.z);
		v[1] = new Vector3(min.x, min.y, max.z);
		v[2] = new Vector3(min.x, max.y, min.z);
		v[3] = new Vector3(min.x, max.y, max.z);
		v[4] = new Vector3(max.x, min.y, min.z);
		v[5] = new Vector3(max.x, min.y, max.z);
		v[6] = new Vector3(max.x, max.y, min.z);
		v[7] = new Vector3(max.x, max.y, max.z);
		FitByMove(v);
	}

	public void FitByMove(Vector3[] points)
	{
		Debug.Assert(points.Length <= tmpPoints.Length);
		// 全点をビュー空間に移動
		var toView = Camera.transform.worldToLocalMatrix;
		for (int i = 0; i < points.Length; i++)
		{
			tmpPoints[i] = toView.MultiplyPoint3x4(points[i]);
		}
		// X-,X+,Y-,Y+の各平面と平面距離を求める。
		// それぞれビュー座標で直交するので容易に求まる。
		// Y+平面 tan(θ_y/2) = y/1
		float tanHalfFovY = Mathf.Tan(Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float ay0 = -tanHalfFovY;
		float ay1 = -ay0;
		float ax0 = -tanHalfFovY * Camera.aspect;
		float ax1 = -ax0;
		// y=ay0*z, y=ay1*z, x=ax0*z, x=ax1*z
		// の4本の式が立つが、これを平面の式に直す。
		// y-ay0*z=by0, y-ay1*z=by1, x-ax0*z=bx0, x-ax1*z=bx1
		// by0,by1,bx0,bx1の最小最大を計算する。
		var by0Min = float.MaxValue;
		var by1Max = -float.MaxValue;
		var bx0Min = float.MaxValue;
		var bx1Max = -float.MaxValue;

		for (int i = 0; i < tmpPoints.Length; i++)
		{
			var p = tmpPoints[i];
			var by0 = p.y - (ay0 * p.z);
			var by1 = p.y - (ay1 * p.z);
			var bx0 = p.x - (ax0 * p.z);
			var bx1 = p.x - (ax1 * p.z);
			by0Min = Mathf.Min(by0Min, by0);
			by1Max = Mathf.Max(by1Max, by1);
			bx0Min = Mathf.Min(bx0Min, bx0);
			bx1Max = Mathf.Max(bx1Max, bx1);
		}
		// X,Y別に交点を算出する。
		// y-ay0*z=by0 とy-ay1*z=by1の交点は、連立方程式を解けば良く、いきなりyが消せ、
		// by0+ay0*z=by1+ay1*z
		// (ay0-ay1)z = by1-by0
		// z = (by1-by0)/(ay0-ay1)
		float zy = (by1Max - by0Min) / (ay0 - ay1);
		float y = by0Min + (ay0 * zy);
		float zx = (bx1Max - bx0Min) / (ax0 - ax1);
		float x = bx0Min + (ax0 * zx);
		var posInView = new Vector3(x, y, Mathf.Min(zy, zx));

		// ワールドに戻す
		PositionGoal = Camera.transform.localToWorldMatrix.MultiplyPoint3x4(posInView);

		var forward = Camera.transform.forward; // 前ベクトル
		var d = (TargetGoal - PositionGoal).magnitude;
		TargetGoal = PositionGoal + (forward.normalized * d);
	}

	/// 画角固定で、newForwardVectorが新しい視線ベクタになるように回転しつつ、距離で位置調整
	public void FitByRotateAndMove(Vector3 newForwardVector, Vector3 center, float radius)
	{
		var fov = GetFovMinRadian();
		// d*tan(fov/2)=radiusとなるdを求める
		var d = radius / Mathf.Tan(fov * 0.5f);
		// 目標点からforwardベクタにdを乗じて引く
		PositionGoal = center - (newForwardVector * d);
		TargetGoal = center;
	}

	/// 位置、回転固定で角度調整
	public void LookAt(Vector3 center)
	{
		TargetGoal = center;
	}

	Vector2 CalcAngle(Vector3 v)
	{
		// 仰角を計算
		var xzLength = (v.x * v.x) + (v.z * v.z);
		Vector2 ret;
		ret.x = Mathf.Atan(-v.y / xzLength) * Mathf.Rad2Deg;
		// 方位角を計算
		ret.y = Mathf.Atan2(-v.x, v.z) * Mathf.Rad2Deg;
		return ret;
	}

	float GetFovMinRadian()
	{
		var fov = Camera.fieldOfView * Mathf.Deg2Rad;
		if (Camera.aspect < 1f) // 縦長の場合横で合わせるので修正
		{
			var tanFov = Mathf.Tan(fov * 0.5f);
			tanFov *= Camera.aspect;
			fov = Mathf.Atan(tanFov) * 2f;
		}
		return fov;
	}

	public void Converge()
	{
		position = PositionGoal;
		target = TargetGoal;
		ManualUpdate(0f);
	}

	public void Shake(float magnitude)
	{
		shakeMagnitude = magnitude;
	}

	public void ManualUpdate(float deltaTime)
	{
		position += (PositionGoal - position) * deltaTime * Stiffness;
		target += (TargetGoal - target) * deltaTime * Stiffness;
		shakeMagnitude -= shakeMagnitude * deltaTime * ShakeDecline;
		if (shakeMagnitude > 0f)
		{
			var shake = new Vector3(
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f),
					Random.Range(-1f, 1f)) * shakeMagnitude;
			position += shake;
			target += shake;
		}
		Camera.transform.position = position;
		Camera.transform.LookAt(target);
	}
}
