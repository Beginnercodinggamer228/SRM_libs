using System;
using UnityEngine;

// Token: 0x02000518 RID: 1304
public class SplashOnTrigger : SRBehaviour
{
	// Token: 0x06001B32 RID: 6962 RVA: 0x000687DC File Offset: 0x000669DC
	public void Awake()
	{
		this.splashColliders = base.GetComponents<Collider>();
	}

	// Token: 0x06001B33 RID: 6963 RVA: 0x000687EC File Offset: 0x000669EC
	private void OnTriggerEnter(Collider collider)
	{
		if (PhysicsUtil.IsPlayerMainCollider(collider))
		{
			this.SpawnAndPlayFX(this.playerSplashFX, collider);
			return;
		}
		if (!collider.isTrigger)
		{
			Identifiable component = collider.gameObject.GetComponent<Identifiable>();
			if (component == null || component.isPhysicsInitialized)
			{
				Rigidbody component2 = collider.GetComponent<Rigidbody>();
				if (component2 != null && !component2.isKinematic && Mathf.Abs(component2.velocity.y) >= 4f)
				{
					this.SpawnAndPlayFX(this.splashFX, collider);
				}
			}
		}
	}

	// Token: 0x06001B34 RID: 6964 RVA: 0x00068870 File Offset: 0x00066A70
	private void SpawnAndPlayFX(GameObject prefab, Collider collider)
	{
		Ray ray = new Ray(collider.gameObject.transform.position, Vector3.down);
		float num = float.PositiveInfinity;
		Vector3 position = collider.gameObject.transform.position;
		Collider[] array = this.splashColliders;
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit raycastHit;
			if (array[i].Raycast(ray, out raycastHit, 2f) && raycastHit.distance < num)
			{
				num = raycastHit.distance;
				position = raycastHit.point;
			}
		}
		SRBehaviour.SpawnAndPlayFX(prefab, position, Quaternion.identity);
	}

	// Token: 0x04001A9C RID: 6812
	public GameObject splashFX;

	// Token: 0x04001A9D RID: 6813
	public GameObject playerSplashFX;

	// Token: 0x04001A9E RID: 6814
	private Collider[] splashColliders;

	// Token: 0x04001A9F RID: 6815
	private const float SPLASH_THRESHOLD = 4f;
}
