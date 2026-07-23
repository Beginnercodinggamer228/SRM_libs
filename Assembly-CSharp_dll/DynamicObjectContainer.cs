using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class DynamicObjectContainer : SRSingleton<DynamicObjectContainer>
{
	// Token: 0x060009DB RID: 2523 RVA: 0x0002B929 File Offset: 0x00029B29
	public override void Awake()
	{
		base.Awake();
		Destroyer.Monitor(base.gameObject, delegate(Destroyer.Metadata metadata)
		{
			InvalidOperationException ex = new InvalidOperationException(string.Format("DynamicObjectContainer is being destroyed. [metadata={0}]", metadata));
			Log.Error(ex.ToString(), Array.Empty<object>());
			SentrySdk.CaptureMessage("DynamicObjectContainer is being destroyed!");
			throw ex;
		});
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x0002B95C File Offset: 0x00029B5C
	private List<GameObject> GetChildren()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (Identifiable identifiable in base.GetComponentsInChildren<Identifiable>())
		{
			list.Add(identifiable.gameObject);
		}
		return list;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x0002B998 File Offset: 0x00029B98
	public void RegisterDynamicObjectActors()
	{
		List<GameObject> children = this.GetChildren();
		foreach (GameObject actorObj in children)
		{
			SRSingleton<SceneContext>.Instance.GameModel.RegisterStartingActor(actorObj, RegionRegistry.RegionSetId.HOME);
		}
		foreach (GameObject gameObject in children)
		{
			gameObject.transform.SetParent(null);
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x0002BA38 File Offset: 0x00029C38
	public void DestroyDynamicObjectActors()
	{
		foreach (GameObject instance in this.GetChildren())
		{
			Destroyer.Destroy(instance, 0f, "DynamicObjectContainer.Awake", true, false);
		}
	}
}
