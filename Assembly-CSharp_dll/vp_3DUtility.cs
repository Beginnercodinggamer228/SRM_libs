using System;
using UnityEngine;

// Token: 0x02000837 RID: 2103
public static class vp_3DUtility
{
	// Token: 0x06002C1F RID: 11295 RVA: 0x000A6794 File Offset: 0x000A4994
	public static Vector3 HorizontalVector(Vector3 value)
	{
		value.y = 0f;
		return value;
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x000A67A4 File Offset: 0x000A49A4
	public static Vector3 RandomHorizontalDirection()
	{
		return (UnityEngine.Random.rotation * Vector3.up).normalized;
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x000A67C8 File Offset: 0x000A49C8
	public static bool OnScreen(Camera camera, Renderer renderer, Vector3 worldPosition, out Vector3 screenPosition)
	{
		screenPosition = Vector2.zero;
		if (camera == null || renderer == null || !renderer.isVisible)
		{
			return false;
		}
		screenPosition = camera.WorldToScreenPoint(worldPosition);
		return screenPosition.z >= 0f;
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x000A6820 File Offset: 0x000A4A20
	public static bool InLineOfSight(Vector3 from, Transform target, Vector3 targetOffset, int layerMask)
	{
		RaycastHit raycastHit;
		Physics.Linecast(from, target.position + targetOffset, out raycastHit, layerMask);
		return raycastHit.collider == null || raycastHit.collider.transform.root == target;
	}

	// Token: 0x06002C23 RID: 11299 RVA: 0x000A686E File Offset: 0x000A4A6E
	public static bool WithinRange(Vector3 from, Vector3 to, float range, out float distance)
	{
		distance = Vector3.Distance(from, to);
		return distance <= range;
	}

	// Token: 0x06002C24 RID: 11300 RVA: 0x000A6884 File Offset: 0x000A4A84
	public static float DistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}

	// Token: 0x06002C25 RID: 11301 RVA: 0x000A68B4 File Offset: 0x000A4AB4
	public static float LookAtAngle(Vector3 fromPosition, Vector3 fromForward, Vector3 toPosition)
	{
		if (Vector3.Cross(fromForward, (toPosition - fromPosition).normalized).y >= 0f)
		{
			return Vector3.Angle(fromForward, (toPosition - fromPosition).normalized);
		}
		return -Vector3.Angle(fromForward, (toPosition - fromPosition).normalized);
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x000A690E File Offset: 0x000A4B0E
	public static float LookAtAngleHorizontal(Vector3 fromPosition, Vector3 fromForward, Vector3 toPosition)
	{
		return vp_3DUtility.LookAtAngle(vp_3DUtility.HorizontalVector(fromPosition), vp_3DUtility.HorizontalVector(fromForward), vp_3DUtility.HorizontalVector(toPosition));
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x000A6928 File Offset: 0x000A4B28
	public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		return Vector3.Angle(dirA, dirB) * (float)((Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f) ? -1 : 1);
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x000A6974 File Offset: 0x000A4B74
	public static Quaternion GetBoneLookRotationInWorldSpace(Quaternion originalRotation, Quaternion parentRotation, Vector3 worldLookDir, float amount, Vector3 referenceLookDir, Vector3 referenceUpDir, Quaternion relativeWorldSpaceDifference)
	{
		Vector3 vector = Quaternion.Inverse(parentRotation) * worldLookDir.normalized;
		vector = Quaternion.AngleAxis(vp_3DUtility.AngleAroundAxis(referenceLookDir, vector, referenceUpDir), referenceUpDir) * Quaternion.AngleAxis(vp_3DUtility.AngleAroundAxis(vector - Vector3.Project(vector, referenceUpDir), vector, Vector3.Cross(referenceUpDir, vector)), Vector3.Cross(referenceUpDir, referenceLookDir)) * referenceLookDir;
		Vector3 vector2 = referenceUpDir;
		Vector3.OrthoNormalize(ref vector, ref vector2);
		Vector3 forward = vector;
		Vector3 upwards = vector2;
		Vector3.OrthoNormalize(ref forward, ref upwards);
		return Quaternion.Lerp(Quaternion.identity, parentRotation * Quaternion.LookRotation(forward, upwards) * Quaternion.Inverse(parentRotation * Quaternion.LookRotation(referenceLookDir, referenceUpDir)), amount) * originalRotation * relativeWorldSpaceDifference;
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x000A6A34 File Offset: 0x000A4C34
	public static GameObject DebugPrimitive(PrimitiveType primitiveType, Vector3 scale, Color color, Vector3 pivotOffset, Transform parent = null)
	{
		GameObject gameObject = null;
		Material material = new Material(Shader.Find("Transparent/Diffuse"));
		material.color = color;
		GameObject gameObject2 = GameObject.CreatePrimitive(primitiveType);
		gameObject2.GetComponent<Collider>().enabled = false;
		gameObject2.GetComponent<Renderer>().material = material;
		gameObject2.transform.localScale = scale;
		gameObject2.name = "Debug" + gameObject2.name;
		if (pivotOffset != Vector3.zero)
		{
			gameObject = new GameObject(gameObject2.name);
			gameObject2.name = gameObject2.name.Replace("Debug", "");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = pivotOffset;
		}
		if (parent != null)
		{
			if (gameObject == null)
			{
				gameObject2.transform.parent = parent;
				gameObject2.transform.localPosition = Vector3.zero;
			}
			else
			{
				gameObject.transform.parent = parent;
				gameObject.transform.localPosition = Vector3.zero;
			}
		}
		if (!(gameObject != null))
		{
			return gameObject2;
		}
		return gameObject;
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x000A6B47 File Offset: 0x000A4D47
	public static GameObject DebugPointer(Transform parent = null)
	{
		return vp_3DUtility.DebugPrimitive(PrimitiveType.Sphere, new Vector3(0.01f, 0.01f, 3f), new Color(1f, 1f, 0f, 0.75f), Vector3.forward, parent);
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x000A6B82 File Offset: 0x000A4D82
	public static GameObject DebugBall(Transform parent = null)
	{
		return vp_3DUtility.DebugPrimitive(PrimitiveType.Sphere, Vector3.one * 0.25f, new Color(1f, 0f, 0f, 0.5f), Vector3.zero, parent);
	}
}
