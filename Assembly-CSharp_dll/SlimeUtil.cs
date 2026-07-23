using System;
using UnityEngine;

// Token: 0x02000496 RID: 1174
public class SlimeUtil
{
	// Token: 0x0600187D RID: 6269 RVA: 0x0005EC08 File Offset: 0x0005CE08
	public static Color[] GetColors(GameObject slimeObj, Identifiable.Id identId, bool isGordo = false)
	{
		if (Identifiable.IsTarr(identId))
		{
			return SlimeUtil.GetColors(slimeObj, "prefab_slimeBase/slime_tarr");
		}
		if (!isGordo && identId == Identifiable.Id.GOLD_SLIME)
		{
			return SlimeUtil.GetColors(slimeObj, "prefab_slimeBase/slime_gold");
		}
		string transformRootPath = isGordo ? "Vibrating/slime_gordo" : "prefab_slimeBase/slime_default";
		return SlimeUtil.GetColors(slimeObj, transformRootPath);
	}

	// Token: 0x0600187E RID: 6270 RVA: 0x0005EC54 File Offset: 0x0005CE54
	private static Color[] GetColors(GameObject slimeObj, string transformRootPath)
	{
		Transform transform = slimeObj.transform.Find(transformRootPath);
		if (transform == null)
		{
			Log.Warning("Could not find renderer transform, returning default colors: " + slimeObj.name, Array.Empty<object>());
			return SlimeUtil.DEFAULTS;
		}
		Renderer component = transform.GetComponent<Renderer>();
		if (component == null)
		{
			Log.Warning("Could not get renderer, returning default colors: " + slimeObj.name, Array.Empty<object>());
			return SlimeUtil.DEFAULTS;
		}
		Material material = component.sharedMaterials[0];
		return new Color[]
		{
			material.GetColor(SlimeUtil.TopColorPropertyId),
			material.GetColor(SlimeUtil.MiddleColorPropertyId),
			material.GetColor(SlimeUtil.BottomColorPropertyId)
		};
	}

	// Token: 0x0600187F RID: 6271 RVA: 0x0005ED0C File Offset: 0x0005CF0C
	public static Material SetTarrColors(GameObject slimeObj, Color[] colors)
	{
		Transform transform = slimeObj.transform.Find("prefab_slimeBase/slime_tarr");
		if (transform == null)
		{
			Log.Warning("Could not find renderer transform, returning default colors: " + slimeObj.name, Array.Empty<object>());
			return null;
		}
		Renderer component = transform.GetComponent<Renderer>();
		if (component == null)
		{
			Log.Warning("Could not get renderer, returning default colors: " + slimeObj.name, Array.Empty<object>());
			return null;
		}
		Material material = component.material;
		material.SetColor(SlimeUtil.TopColorPropertyId, colors[0]);
		material.SetColor(SlimeUtil.MiddleColorPropertyId, colors[0]);
		material.SetColor(SlimeUtil.BottomColorPropertyId, colors[0]);
		return material;
	}

	// Token: 0x06001880 RID: 6272 RVA: 0x0005EDB8 File Offset: 0x0005CFB8
	public static Material SetTarrSterile(GameObject slimeObj, Texture rampTex)
	{
		Transform transform = slimeObj.transform.Find("prefab_slimeBase/slime_tarr");
		Transform transform2 = slimeObj.transform.Find("prefab_slimeBase/slime_tarr_bite");
		Transform transform3 = slimeObj.transform.Find("prefab_slimeBase/bone_root/bone_slime/slime_default_LOD1");
		Transform transform4 = slimeObj.transform.Find("prefab_slimeBase/bone_root/bone_slime/slime_default_LOD2");
		Transform transform5 = slimeObj.transform.Find("prefab_slimeBase/bone_root/bone_slime/slime_default_LOD3");
		if (transform == null)
		{
			Log.Warning("Could not find renderer transform, returning default colors: " + slimeObj.name, Array.Empty<object>());
			return null;
		}
		Renderer component = transform.GetComponent<Renderer>();
		if (component == null)
		{
			Log.Warning("Could not get renderer, returning default colors: " + slimeObj.name, Array.Empty<object>());
			return null;
		}
		Material material = component.material;
		material.SetTexture(SlimeUtil.ColorRampPropertyId, rampTex);
		foreach (Transform transform6 in new Transform[]
		{
			transform2,
			transform3,
			transform4,
			transform5
		})
		{
			if (transform6 != null)
			{
				Renderer component2 = transform6.GetComponent<Renderer>();
				if (component2 != null)
				{
					component2.material = material;
				}
			}
		}
		return material;
	}

	// Token: 0x06001881 RID: 6273 RVA: 0x0005EEE0 File Offset: 0x0005D0E0
	public static FixedJoint AttachToMouth(GameObject slimeObj, GameObject target)
	{
		Vector3 vector = Vector3.forward * (PhysicsUtil.RadiusOfObject(slimeObj) + PhysicsUtil.RadiusOfObject(target) * 0.25f) / slimeObj.transform.localScale.z;
		Rigidbody component = target.GetComponent<Rigidbody>();
		target.transform.position = slimeObj.transform.position + slimeObj.transform.localToWorldMatrix.MultiplyVector(vector);
		component.velocity = (component.angularVelocity = Vector3.zero);
		FixedJoint fixedJoint = slimeObj.AddComponent<FixedJoint>();
		SafeJointReference.AttachSafely(target, fixedJoint, true);
		fixedJoint.anchor = vector;
		fixedJoint.breakForce = 1000f;
		fixedJoint.breakTorque = 1000f;
		return fixedJoint;
	}

	// Token: 0x0400180D RID: 6157
	private static Color[] DEFAULTS = new Color[]
	{
		Color.grey,
		Color.grey,
		Color.grey
	};

	// Token: 0x0400180E RID: 6158
	private static int TopColorPropertyId = Shader.PropertyToID("_TopColor");

	// Token: 0x0400180F RID: 6159
	private static int MiddleColorPropertyId = Shader.PropertyToID("_MiddleColor");

	// Token: 0x04001810 RID: 6160
	private static int BottomColorPropertyId = Shader.PropertyToID("_BottomColor");

	// Token: 0x04001811 RID: 6161
	private static int ColorRampPropertyId = Shader.PropertyToID("_ColorRamp");
}
