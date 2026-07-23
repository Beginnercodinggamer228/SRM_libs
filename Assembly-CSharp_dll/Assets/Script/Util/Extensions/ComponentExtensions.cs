using System;
using UnityEngine;

namespace Assets.Script.Util.Extensions
{
	// Token: 0x02000A19 RID: 2585
	public static class ComponentExtensions
	{
		// Token: 0x060045B5 RID: 17845 RVA: 0x000CD925 File Offset: 0x000CBB25
		public static T GetComponentInParent<T>(this Component component, bool includeInactive = false) where T : Component
		{
			return component.gameObject.GetComponentInParent(includeInactive);
		}

		// Token: 0x060045B6 RID: 17846 RVA: 0x000CD933 File Offset: 0x000CBB33
		public static T GetRequiredComponent<T>(this Component component) where T : Component
		{
			return component.gameObject.GetRequiredComponent<T>();
		}

		// Token: 0x060045B7 RID: 17847 RVA: 0x000CD940 File Offset: 0x000CBB40
		public static T GetRequiredComponentInParent<T>(this Component component, bool includeInactive = false) where T : Component
		{
			return component.gameObject.GetRequiredComponentInParent(includeInactive);
		}

		// Token: 0x060045B8 RID: 17848 RVA: 0x000CD94E File Offset: 0x000CBB4E
		public static T GetRequiredComponentInChildren<T>(this Component component, bool includeInactive = false) where T : Component
		{
			return component.gameObject.GetRequiredComponentInChildren(includeInactive);
		}

		// Token: 0x060045B9 RID: 17849 RVA: 0x000CD95C File Offset: 0x000CBB5C
		public static void DestroyChildren(this Component parent, string source)
		{
			parent.DestroyChildren((GameObject go) => true, source);
		}

		// Token: 0x060045BA RID: 17850 RVA: 0x000CD984 File Offset: 0x000CBB84
		public static void DestroyChildren(this Component parent, Predicate<GameObject> predicate, string source)
		{
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				GameObject gameObject = parent.transform.GetChild(i).gameObject;
				if (predicate(gameObject))
				{
					if (gameObject.GetComponent<Identifiable>() != null)
					{
						Destroyer.DestroyActor(gameObject, source, false);
					}
					else
					{
						Destroyer.Destroy(gameObject, source);
					}
				}
			}
		}
	}
}
