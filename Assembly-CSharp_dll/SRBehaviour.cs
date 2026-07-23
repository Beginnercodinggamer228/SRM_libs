using System;
using Assets.Script.Util.Extensions;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000368 RID: 872
public class SRBehaviour : MonoBehaviour
{
	// Token: 0x0600120A RID: 4618 RVA: 0x00047AA8 File Offset: 0x00045CA8
	public I GetInterfaceComponent<I>() where I : class
	{
		return base.GetComponent(typeof(I)) as I;
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x00047AC4 File Offset: 0x00045CC4
	public static GameObject InstantiateDynamic(GameObject original)
	{
		return UnityEngine.Object.Instantiate<GameObject>(original);
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x00047ACC File Offset: 0x00045CCC
	public static GameObject InstantiateDynamic(GameObject original, Vector3 position, Quaternion rotation, bool asActor = false)
	{
		return UnityEngine.Object.Instantiate<GameObject>(original, position, rotation);
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x00047AD6 File Offset: 0x00045CD6
	public static GameObject InstantiateActor(GameObject original, RegionRegistry.RegionSetId regionSetId, bool nonActorOk = false)
	{
		return SRBehaviour.InstantiateActor(original, regionSetId, Vector3.zero, Quaternion.identity, nonActorOk);
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x00047AEA File Offset: 0x00045CEA
	public static GameObject InstantiateActor(GameObject original, RegionRegistry.RegionSetId regionSetId, Vector3 position, Quaternion rotation, bool nonActorOk = false)
	{
		return SRSingleton<SceneContext>.Instance.GameModel.InstantiateActor(original, regionSetId, position, rotation, nonActorOk);
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x00047B01 File Offset: 0x00045D01
	public static GameObject InstantiatePooledDynamic(GameObject original, Vector3 position, Quaternion rotation)
	{
		return SRSingleton<SceneContext>.Instance.fxPool.Spawn(original, null, position, rotation);
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x00047B16 File Offset: 0x00045D16
	public static GameObject SpawnAndPlayFX(GameObject prefab)
	{
		return SRBehaviour.SpawnAndPlayFX(prefab, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x00047B28 File Offset: 0x00045D28
	public static GameObject SpawnAndPlayFX(GameObject prefab, GameObject parentObject)
	{
		return SRBehaviour.SpawnAndPlayFX(prefab, parentObject, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x00047B3B File Offset: 0x00045D3B
	public static GameObject SpawnAndPlayFX(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return SRBehaviour.SpawnAndPlayFX(prefab, null, position, rotation);
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x00047B48 File Offset: 0x00045D48
	public static GameObject SpawnAndPlayFX(GameObject prefab, GameObject parentObject, Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = null;
		if (SRSingleton<SceneContext>.Instance != null)
		{
			gameObject = SRSingleton<SceneContext>.Instance.fxPool.Spawn(prefab, (parentObject != null) ? parentObject.transform : null, position, rotation);
			SRBehaviour.PlayFX(gameObject);
		}
		return gameObject;
	}

	// Token: 0x06001214 RID: 4628 RVA: 0x00047B90 File Offset: 0x00045D90
	public static void RecycleAndStopFX(GameObject obj)
	{
		SRBehaviour.StopFX(obj);
		SRSingleton<SceneContext>.Instance.fxPool.Recycle(obj);
	}

	// Token: 0x06001215 RID: 4629 RVA: 0x00047BA8 File Offset: 0x00045DA8
	public static void PlayFX(GameObject fxObject)
	{
		if (fxObject != null)
		{
			ParticleSystem particleSystem = fxObject.GetComponent<ParticleSystem>();
			if (particleSystem == null)
			{
				particleSystem = fxObject.GetComponentInChildren<ParticleSystem>();
			}
			if (particleSystem != null)
			{
				particleSystem.Play();
				if (particleSystem.gameObject != null)
				{
					SECTR_PointSource[] components = particleSystem.gameObject.GetComponents<SECTR_PointSource>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i] != null)
						{
							components[i].Play();
						}
					}
				}
			}
		}
	}

	// Token: 0x06001216 RID: 4630 RVA: 0x00047C20 File Offset: 0x00045E20
	public static void StopFX(GameObject fxObject)
	{
		if (fxObject != null)
		{
			ParticleSystem particleSystem = fxObject.GetComponent<ParticleSystem>();
			if (particleSystem == null)
			{
				particleSystem = fxObject.GetComponentInChildren<ParticleSystem>();
			}
			if (particleSystem != null)
			{
				particleSystem.Stop();
				if (particleSystem.gameObject != null)
				{
					SECTR_PointSource[] components = particleSystem.gameObject.GetComponents<SECTR_PointSource>();
					for (int i = 0; i < components.Length; i++)
					{
						if (components[i] != null)
						{
							components[i].Stop(true);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001217 RID: 4631 RVA: 0x00047C98 File Offset: 0x00045E98
	private DestroyRequestHandler GetDestroyRequestHandler()
	{
		if (this.destroyRequestHandler == null)
		{
			this.destroyRequestHandler = base.GetComponent<DestroyRequestHandler>();
		}
		return this.destroyRequestHandler;
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x00047CB4 File Offset: 0x00045EB4
	public void RequestDestroy(string source)
	{
		DestroyRequestHandler destroyRequestHandler = this.GetDestroyRequestHandler();
		if (destroyRequestHandler != null)
		{
			destroyRequestHandler.OnDestroyRequest(base.gameObject);
			return;
		}
		Destroyer.Destroy(base.gameObject, source);
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x00047CE4 File Offset: 0x00045EE4
	public static void RequestDestroy(GameObject obj, string source)
	{
		obj.GetComponent<SRBehaviour>().RequestDestroy(source);
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x00047CF4 File Offset: 0x00045EF4
	public static void LinkNavigation(Selectable source, Selectable target, SRBehaviour.NavigationDirection direction)
	{
		switch (direction)
		{
		case SRBehaviour.NavigationDirection.UP:
			if (source != null)
			{
				Navigation navigation = source.navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnUp = ((target != null && target.interactable) ? target : null);
				source.navigation = navigation;
				return;
			}
			break;
		case SRBehaviour.NavigationDirection.DOWN:
			if (source != null)
			{
				Navigation navigation2 = source.navigation;
				navigation2.mode = Navigation.Mode.Explicit;
				navigation2.selectOnDown = ((target != null && target.interactable) ? target : null);
				source.navigation = navigation2;
				return;
			}
			break;
		case SRBehaviour.NavigationDirection.RIGHT:
			if (source != null)
			{
				Navigation navigation3 = source.navigation;
				navigation3.mode = Navigation.Mode.Explicit;
				navigation3.selectOnRight = ((target != null && target.interactable) ? target : null);
				source.navigation = navigation3;
				return;
			}
			break;
		case SRBehaviour.NavigationDirection.LEFT:
			if (source != null)
			{
				Navigation navigation4 = source.navigation;
				navigation4.mode = Navigation.Mode.Explicit;
				navigation4.selectOnLeft = ((target != null && target.interactable) ? target : null);
				source.navigation = navigation4;
				return;
			}
			break;
		case SRBehaviour.NavigationDirection.RIGHT_LEFT:
			SRBehaviour.LinkNavigation(source, target, SRBehaviour.NavigationDirection.RIGHT);
			SRBehaviour.LinkNavigation(target, source, SRBehaviour.NavigationDirection.LEFT);
			break;
		case SRBehaviour.NavigationDirection.DOWN_UP:
			SRBehaviour.LinkNavigation(source, target, SRBehaviour.NavigationDirection.DOWN);
			SRBehaviour.LinkNavigation(target, source, SRBehaviour.NavigationDirection.UP);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x00047E3A File Offset: 0x0004603A
	public T GetComponentInParent<T>(bool includeInactive) where T : Component
	{
		return this.GetComponentInParent(includeInactive);
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x00047E43 File Offset: 0x00046043
	public T GetRequiredComponent<T>() where T : Component
	{
		return this.GetRequiredComponent<T>();
	}

	// Token: 0x0600121D RID: 4637 RVA: 0x00047E4B File Offset: 0x0004604B
	public T GetRequiredComponentInParent<T>(bool includeInactive = false) where T : Component
	{
		return this.GetRequiredComponentInParent(includeInactive);
	}

	// Token: 0x0600121E RID: 4638 RVA: 0x00047E54 File Offset: 0x00046054
	public T GetRequiredComponentInChildren<T>(bool includeInactive = false) where T : Component
	{
		return this.GetRequiredComponentInChildren(includeInactive);
	}

	// Token: 0x04001120 RID: 4384
	private DestroyRequestHandler destroyRequestHandler;

	// Token: 0x02000369 RID: 873
	public enum NavigationDirection
	{
		// Token: 0x04001122 RID: 4386
		UP,
		// Token: 0x04001123 RID: 4387
		DOWN,
		// Token: 0x04001124 RID: 4388
		RIGHT,
		// Token: 0x04001125 RID: 4389
		LEFT,
		// Token: 0x04001126 RID: 4390
		RIGHT_LEFT,
		// Token: 0x04001127 RID: 4391
		DOWN_UP
	}
}
