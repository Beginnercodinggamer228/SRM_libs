using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Util.Extensions
{
	// Token: 0x02000A1B RID: 2587
	public static class GameObjectExtensions
	{
		// Token: 0x060045BE RID: 17854 RVA: 0x000CD9ED File Offset: 0x000CBBED
		public static T GetComponentInParent<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
		{
			return gameObject.GetComponentsInParent<T>(includeInactive).FirstOrDefault<T>();
		}

		// Token: 0x060045BF RID: 17855 RVA: 0x000CD9FB File Offset: 0x000CBBFB
		public static T GetRequiredComponent<T>(this GameObject gameObject) where T : Component
		{
			return gameObject.GetComponent<T>();
		}

		// Token: 0x060045C0 RID: 17856 RVA: 0x000CDA03 File Offset: 0x000CBC03
		public static T GetRequiredComponentInParent<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
		{
			return gameObject.GetComponentInParent(includeInactive);
		}

		// Token: 0x060045C1 RID: 17857 RVA: 0x000CDA0C File Offset: 0x000CBC0C
		public static T GetRequiredComponentInChildren<T>(this GameObject gameObject, bool includeInactive = false) where T : Component
		{
			return gameObject.GetComponentInChildren<T>(includeInactive);
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x000CDA15 File Offset: 0x000CBC15
		public static void StartCoroutine(this GameObject gameObject, IEnumerator coroutine)
		{
			GameObjectExtensions.CoroutineRunner coroutineRunner = gameObject.AddComponent<GameObjectExtensions.CoroutineRunner>();
			coroutineRunner.StartCoroutine(coroutineRunner.CoroutineWrapper(coroutine));
		}

		// Token: 0x060045C3 RID: 17859 RVA: 0x000CDA2A File Offset: 0x000CBC2A
		public static void DestroyChildren(this GameObject parent, string source)
		{
			parent.transform.DestroyChildren(source);
		}

		// Token: 0x060045C4 RID: 17860 RVA: 0x000CDA38 File Offset: 0x000CBC38
		public static void DestroyChildren(this Transform parent, string source)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				Destroyer.Destroy(parent.GetChild(i).gameObject, source);
			}
		}

		// Token: 0x02000A1C RID: 2588
		private class CoroutineRunner : MonoBehaviour
		{
			// Token: 0x060045C5 RID: 17861 RVA: 0x000CDA68 File Offset: 0x000CBC68
			public IEnumerator CoroutineWrapper(IEnumerator coroutine)
			{
				yield return coroutine;
				Destroyer.Destroy(this, "CoroutineRunner.CoroutineWrapper");
				yield break;
			}

			// Token: 0x060045C6 RID: 17862 RVA: 0x000CDA7E File Offset: 0x000CBC7E
			public void OnDisable()
			{
				Destroyer.Destroy(this, "CoroutineRunner.OnDisable");
			}
		}
	}
}
