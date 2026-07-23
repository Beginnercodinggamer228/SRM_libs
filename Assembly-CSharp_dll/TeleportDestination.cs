using System;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000793 RID: 1939
public class TeleportDestination : MonoBehaviour
{
	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06002869 RID: 10345 RVA: 0x000995A6 File Offset: 0x000977A6
	// (set) Token: 0x0600286A RID: 10346 RVA: 0x000995AE File Offset: 0x000977AE
	public RegionRegistry.RegionSetId regionSetId { get; private set; }

	// Token: 0x0600286B RID: 10347 RVA: 0x000995B7 File Offset: 0x000977B7
	public virtual void Awake()
	{
		SRSingleton<SceneContext>.Instance.TeleportNetwork.Register(this);
		this.regionSetId = base.GetComponentInParent<Region>().setId;
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000995DC File Offset: 0x000977DC
	public virtual void OnDepart()
	{
		TeleportSource component = base.gameObject.GetComponent<TeleportSource>();
		if (component != null)
		{
			component.waitForTriggerExit = true;
		}
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x00099605 File Offset: 0x00097805
	public void OnArrive()
	{
		if (this.arriveFX != null)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.arriveFX, base.transform.position, base.transform.rotation);
		}
		base.GetComponent<SECTR_AudioSource>().Play();
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x0008E024 File Offset: 0x0008C224
	public Vector3 GetPosition()
	{
		return base.gameObject.transform.position;
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x00099644 File Offset: 0x00097844
	public Vector3? GetEulerAngles()
	{
		if (this.reorient)
		{
			return new Vector3?(base.gameObject.transform.eulerAngles);
		}
		return null;
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x00099678 File Offset: 0x00097878
	public virtual void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			SRSingleton<SceneContext>.Instance.TeleportNetwork.Deregister(this);
		}
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x00099698 File Offset: 0x00097898
	public virtual bool IsLinkActive()
	{
		TeleportSource component = base.gameObject.GetComponent<TeleportSource>();
		return component == null || component.IsLinkActive();
	}

	// Token: 0x04002812 RID: 10258
	public Transform destLoc;

	// Token: 0x04002813 RID: 10259
	public GameObject arriveFX;

	// Token: 0x04002814 RID: 10260
	public string teleportDestinationName;

	// Token: 0x04002815 RID: 10261
	public bool reorient = true;
}
