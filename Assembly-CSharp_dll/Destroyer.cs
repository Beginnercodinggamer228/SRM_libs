using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// Token: 0x02000121 RID: 289
public static class Destroyer
{
	// Token: 0x0600062E RID: 1582 RVA: 0x00022534 File Offset: 0x00020734
	public static void Monitor(UnityEngine.Object instance, Action<Destroyer.Metadata> action)
	{
		List<Action<Destroyer.Metadata>> list;
		if (!Destroyer.monitorsDict.TryGetValue(instance.GetInstanceID(), out list))
		{
			Destroyer.monitorsDict.Add(instance.GetInstanceID(), list = new List<Action<Destroyer.Metadata>>());
		}
		list.Add(action);
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x00022573 File Offset: 0x00020773
	public static void DestroyActor(GameObject actorObj, string source, bool okIfNonActor = false)
	{
		if (actorObj.GetComponent<Identifiable>() != null)
		{
			SRSingleton<SceneContext>.Instance.GameModel.DestroyActorModel(actorObj);
			Destroyer.Destroy(actorObj, 0f, source, true, false);
			return;
		}
		Destroyer.Destroy(actorObj, 0f, source, false, false);
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x000225B0 File Offset: 0x000207B0
	public static void DestroyGadget(string siteId, GameObject gadgetObj, string source)
	{
		Destroyer.Destroy(gadgetObj, 0f, source, false, true);
		SRSingleton<SceneContext>.Instance.GameModel.DestroyGadgetModel(siteId, gadgetObj);
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x000225D1 File Offset: 0x000207D1
	public static void Destroy(UnityEngine.Object instance, string source)
	{
		Destroyer.Destroy(instance, 0f, source, false, false);
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x000225E1 File Offset: 0x000207E1
	public static void Destroy(UnityEngine.Object instance, float t, string source, bool asActorOk = false, bool asGadgetOk = false)
	{
		if (instance != null)
		{
			Destroyer.Destroy(instance, t, new Destroyer.Metadata
			{
				objectName = instance.name,
				source = source,
				frame = Time.frameCount
			});
		}
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x00022618 File Offset: 0x00020818
	private static void Destroy(UnityEngine.Object instance, float t, Destroyer.Metadata metadata)
	{
		if (instance is GameObject)
		{
			DOTween.Kill(((GameObject)instance).transform, false);
		}
		List<Action<Destroyer.Metadata>> list;
		if (Destroyer.monitorsDict.TryGetValue(instance.GetInstanceID(), out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i](metadata);
			}
			Destroyer.monitorsDict.Remove(instance.GetInstanceID());
		}
		UnityEngine.Object.Destroy(instance, t);
	}

	// Token: 0x040005EE RID: 1518
	private static Dictionary<int, List<Action<Destroyer.Metadata>>> monitorsDict = new Dictionary<int, List<Action<Destroyer.Metadata>>>();

	// Token: 0x02000122 RID: 290
	public class Metadata
	{
		// Token: 0x06000635 RID: 1589 RVA: 0x00022695 File Offset: 0x00020895
		public override string ToString()
		{
			return string.Format("{0} [object={1}, source={2}, frame={3}]", new object[]
			{
				typeof(Destroyer.Metadata),
				this.objectName,
				this.source,
				this.frame
			});
		}

		// Token: 0x040005EF RID: 1519
		public string objectName;

		// Token: 0x040005F0 RID: 1520
		public string source;

		// Token: 0x040005F1 RID: 1521
		public int frame;
	}
}
