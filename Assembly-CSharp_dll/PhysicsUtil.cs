using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000693 RID: 1683
public static class PhysicsUtil
{
	// Token: 0x06002316 RID: 8982 RVA: 0x0008875C File Offset: 0x0008695C
	public static void Explode(GameObject source, float radius, float power, float minPlayerDamage, float maxPlayerDamage, bool ignites = false)
	{
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		Vector3 position = source.transform.position;
		LayerMask mask = -65537;
		foreach (Collider collider in Physics.OverlapSphere(position, radius, mask))
		{
			if (collider && !collider.isTrigger && collider.GetComponent<Rigidbody>() != null && collider.gameObject != source && !hashSet.Contains(collider.gameObject))
			{
				vp_FPController component = collider.gameObject.GetComponent<vp_FPController>();
				if (component != null)
				{
					Vector3 a = SlimeSubbehaviour.GetGotoPos(component.gameObject) - position;
					float magnitude = a.magnitude;
					a.Normalize();
					float num = (1f - magnitude / radius) * 0.001f;
					component.AddForce(a * (power * num));
					Damageable interfaceComponent = collider.gameObject.GetInterfaceComponent<Damageable>();
					if (interfaceComponent != null)
					{
						int healthLoss = Mathf.RoundToInt(Mathf.Lerp(minPlayerDamage, maxPlayerDamage, 1f - magnitude / radius));
						if (interfaceComponent.Damage(healthLoss, source))
						{
							DeathHandler.Kill(collider.gameObject, DeathHandler.Source.SLIME_EXPLODE, source, "PhysicsUtil.Explode");
						}
					}
					if (ignites)
					{
						Ignitable[] componentsInChildren = collider.gameObject.GetComponentsInChildren<Ignitable>();
						for (int j = 0; j < componentsInChildren.Length; j++)
						{
							componentsInChildren[j].Ignite(source);
						}
					}
				}
				else
				{
					PhysicsUtil.SoftExplosionForce(power, position, radius, collider.GetComponent<Rigidbody>());
				}
				hashSet.Add(collider.gameObject);
			}
		}
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000888FC File Offset: 0x00086AFC
	public static void SoftExplosionForce(float power, Vector3 pos, float radius, Rigidbody body)
	{
		Vector3 a = body.position - pos;
		float magnitude = a.magnitude;
		a.Normalize();
		float num = 1f - Mathf.Max(2f, magnitude) / radius;
		body.AddForce(a * (power * num * num));
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x0008894C File Offset: 0x00086B4C
	public static float RadiusOfObject(GameObject obj)
	{
		float num = 0f;
		foreach (Collider collider in obj.GetComponents<Collider>())
		{
			if (!collider.isTrigger)
			{
				if (obj.activeInHierarchy)
				{
					Bounds bounds = collider.bounds;
					num = Mathf.Max(num, 0.5f * Mathf.Max(new float[]
					{
						bounds.size.x,
						bounds.size.y,
						bounds.size.z
					}));
				}
				else
				{
					num = Mathf.Max(num, PhysicsUtil.CalcRad(collider));
				}
			}
		}
		return num;
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000889E8 File Offset: 0x00086BE8
	private static float CalcRad(Collider col)
	{
		Vector3 lossyScale = col.transform.lossyScale;
		if (col is SphereCollider)
		{
			return ((SphereCollider)col).radius * Mathf.Max(new float[]
			{
				lossyScale.x,
				lossyScale.y,
				lossyScale.z
			});
		}
		if (col is BoxCollider)
		{
			BoxCollider boxCollider = (BoxCollider)col;
			return Mathf.Max(new float[]
			{
				boxCollider.size.x * lossyScale.x,
				boxCollider.size.y * lossyScale.y,
				boxCollider.size.z * lossyScale.z
			}) * 0.5f;
		}
		if (col is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = (CapsuleCollider)col;
			float num = (capsuleCollider.direction == 0) ? (capsuleCollider.height * 0.5f) : capsuleCollider.radius;
			float num2 = (capsuleCollider.direction == 1) ? (capsuleCollider.height * 0.5f) : capsuleCollider.radius;
			float num3 = (capsuleCollider.direction == 2) ? (capsuleCollider.height * 0.5f) : capsuleCollider.radius;
			return Mathf.Max(new float[]
			{
				num * lossyScale.x,
				num2 * lossyScale.y,
				num3 * lossyScale.z
			});
		}
		return 0f;
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x00088B3D File Offset: 0x00086D3D
	public static bool IsPlayerMainCollider(Collider collider)
	{
		return collider.gameObject == SRSingleton<SceneContext>.Instance.Player && collider is CharacterController;
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x00088B64 File Offset: 0x00086D64
	public static void RestoreFreezeRotationConstraints(GameObject gameObject)
	{
		Rigidbody component = gameObject.GetComponent<Rigidbody>();
		if (component != null && component.constraints != RigidbodyConstraints.None)
		{
			Vector3 eulerAngles = component.transform.rotation.eulerAngles;
			if ((component.constraints & RigidbodyConstraints.FreezeRotationX) != RigidbodyConstraints.None)
			{
				eulerAngles.x = 0f;
			}
			if ((component.constraints & RigidbodyConstraints.FreezeRotationY) != RigidbodyConstraints.None)
			{
				eulerAngles.y = 0f;
			}
			if ((component.constraints & RigidbodyConstraints.FreezeRotationZ) != RigidbodyConstraints.None)
			{
				eulerAngles.z = 0f;
			}
			component.transform.rotation = Quaternion.Euler(eulerAngles);
		}
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x00088BF4 File Offset: 0x00086DF4
	public static void IgnoreCollision(GameObject a, GameObject b, bool ignored = true)
	{
		Collider[] componentsInChildren = a.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = b.GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			foreach (Collider collider2 in componentsInChildren2)
			{
				Physics.IgnoreCollision(collider, collider2, ignored);
			}
		}
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x00088C47 File Offset: 0x00086E47
	public static void IgnoreCollision(GameObject a, GameObject b, float enableAfter)
	{
		PhysicsUtil.IgnoreCollision(a, b, true);
		SRSingleton<GameContext>.Instance.StartCoroutine(PhysicsUtil.RestoreCollision(a, b, enableAfter));
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x00088C64 File Offset: 0x00086E64
	private static IEnumerator RestoreCollision(GameObject a, GameObject b, float enableAfter)
	{
		yield return new WaitForSeconds(enableAfter);
		if (a != null && b != null)
		{
			PhysicsUtil.IgnoreCollision(a, b, false);
		}
		yield break;
	}

	// Token: 0x0400228D RID: 8845
	private const float PLAYER_FORCE_FACTOR = 0.001f;
}
