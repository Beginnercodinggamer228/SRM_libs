using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

// Token: 0x020001B3 RID: 435
[RequireComponent(typeof(Drone))]
[RequireComponent(typeof(DroneSubbehaviourPlexer))]
public abstract class DroneSubbehaviour : CollidableActorBehaviour
{
	// Token: 0x17000124 RID: 292
	// (get) Token: 0x0600092E RID: 2350 RVA: 0x000291B3 File Offset: 0x000273B3
	// (set) Token: 0x0600092F RID: 2351 RVA: 0x000291BB File Offset: 0x000273BB
	public DroneSubbehaviourPlexer plexer { get; private set; }

	// Token: 0x06000930 RID: 2352 RVA: 0x000291C4 File Offset: 0x000273C4
	public override void Awake()
	{
		base.Awake();
		this.timeDirector = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.plexer = base.GetComponent<DroneSubbehaviourPlexer>();
		this.drone = base.GetComponent<Drone>();
	}

	// Token: 0x06000931 RID: 2353
	public abstract bool Relevancy();

	// Token: 0x06000932 RID: 2354
	public abstract void Action();

	// Token: 0x06000933 RID: 2355 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Selected()
	{
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00003296 File Offset: 0x00001496
	public virtual void Deselected()
	{
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x000291F4 File Offset: 0x000273F4
	protected bool OnAction_DumpAmmo(ref double time)
	{
		if (this.timeDirector.HasReached(time))
		{
			GameObject gameObject = SRSingleton<SceneContext>.Instance.GameModel.InstantiateActor(SRSingleton<GameContext>.Instance.LookupDirector.GetPrefab(this.drone.ammo.Pop()), this.drone.region.setId, this.drone.guideDumpSpawn.position, Quaternion.Euler((float)Randoms.SHARED.GetInRange(0, 360), (float)Randoms.SHARED.GetInRange(0, 360), (float)Randoms.SHARED.GetInRange(0, 360)), false);
			PhysicsUtil.RestoreFreezeRotationConstraints(gameObject);
			gameObject.GetComponent<Rigidbody>().velocity = (Quaternion.Euler(0f, (float)Randoms.SHARED.GetInRange(0, 360), 0f) * new Vector3(0f, -0.5f, 0.5f) + Vector3.down).normalized * 5f;
			float fromValue = gameObject.transform.localScale.x * 0.2f;
			gameObject.transform.DOScale(gameObject.transform.localScale, 0.1f).From(fromValue, true).SetEase(Ease.Linear);
			time = this.timeDirector.HoursFromNow(0.0050000004f);
			return true;
		}
		return false;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00029356 File Offset: 0x00027556
	protected IEnumerable<DroneProgram.Orientation> GetTargetOrientations_Gather(GameObject source)
	{
		return this.GetTargetOrientations_Gather(source, DroneSubbehaviour.GatherConfig.DEFAULT);
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00029364 File Offset: 0x00027564
	protected IEnumerable<DroneProgram.Orientation> GetTargetOrientations_Gather(GameObject source, DroneSubbehaviour.GatherConfig config)
	{
		for (float distanceMultiplier = 1f; distanceMultiplier <= 3f; distanceMultiplier += 1f)
		{
			float angle = Randoms.SHARED.GetFloat(6.2831855f);
			float delta = 0.62831855f;
			int ii = 0;
			while (ii < 10)
			{
				Vector3 vector = new Vector3(Mathf.Cos(angle) * config.distanceHorizontal, distanceMultiplier * config.distanceVertical, Mathf.Sin(angle) * config.distanceHorizontal);
				Vector3 vector2 = source.transform.position + vector;
				if (this.SpaceIsClearForDrone(vector2))
				{
					yield return new DroneProgram.Orientation(vector2, Quaternion.LookRotation(-vector));
				}
				int num = ii + 1;
				ii = num;
				angle += delta;
			}
		}
		yield return new DroneProgram.Orientation(source.transform.position + config.fallbackOffset, Quaternion.LookRotation(config.fallbackOffset));
		yield break;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00029384 File Offset: 0x00027584
	protected DroneSubbehaviour.CatcherOrientation GetTargetOrientation_Catcher(GameObject source)
	{
		for (int i = 0; i < DroneSubbehaviour.CATCHER_ORIENTATIONS.Length; i++)
		{
			if (DroneSubbehaviour.CatcherOrientation.IsAvailable(source, i))
			{
				DroneProgram.Orientation orientation = DroneSubbehaviour.CATCHER_ORIENTATIONS[i];
				Vector3 vector = orientation.rot * (source.transform.forward * 2f + orientation.pos);
				Vector3 vector2 = source.transform.position + vector;
				if (this.SpaceIsClearForDrone(vector2))
				{
					return new DroneSubbehaviour.CatcherOrientation(source, i, new DroneProgram.Orientation
					{
						pos = vector2,
						rot = Quaternion.LookRotation(-vector)
					});
				}
			}
		}
		return new DroneSubbehaviour.CatcherOrientation(new DroneProgram.Orientation
		{
			pos = source.transform.position + Vector3.up,
			rot = Quaternion.LookRotation(-Vector3.up)
		});
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00029460 File Offset: 0x00027660
	private bool SpaceIsClearForDrone(Vector3 position)
	{
		if (this.drone.noClip.enabled)
		{
			return true;
		}
		RaycastHit[] array = Physics.SphereCastAll(position + Vector3.up * 1000f, 0.5f, Vector3.down, 1000f, -537968901);
		this.collidersToReset.Clear();
		bool result = false;
		if (array.Length != 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				MeshCollider component = array[i].collider.GetComponent<MeshCollider>();
				if (component != null && !component.convex && array[i].collider.GetComponent<Rigidbody>() == null)
				{
					this.collidersToReset.Add(component);
					try
					{
						component.convex = true;
					}
					catch
					{
						Log.Error("Exception when changing to convex.", new object[]
						{
							"object name",
							component.name
						});
						throw;
					}
				}
			}
			result = !Physics.CheckSphere(position, 0.5f, -537968901);
			using (List<MeshCollider>.Enumerator enumerator = this.collidersToReset.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MeshCollider meshCollider = enumerator.Current;
					meshCollider.convex = false;
				}
				return result;
			}
		}
		result = true;
		return result;
	}

	// Token: 0x040007B6 RID: 1974
	protected Drone drone;

	// Token: 0x040007B7 RID: 1975
	protected TimeDirector timeDirector;

	// Token: 0x040007B8 RID: 1976
	private static readonly DroneProgram.Orientation[] CATCHER_ORIENTATIONS = new DroneProgram.Orientation[]
	{
		new DroneProgram.Orientation(),
		new DroneProgram.Orientation
		{
			rot = Quaternion.Euler(0f, -45f, 0f)
		},
		new DroneProgram.Orientation
		{
			rot = Quaternion.Euler(0f, 45f, 0f)
		},
		new DroneProgram.Orientation
		{
			pos = Vector3.up
		},
		new DroneProgram.Orientation
		{
			rot = Quaternion.Euler(0f, -45f, 0f),
			pos = Vector3.up
		},
		new DroneProgram.Orientation
		{
			rot = Quaternion.Euler(0f, 45f, 0f),
			pos = Vector3.up
		}
	};

	// Token: 0x040007B9 RID: 1977
	private const float POSITION_CHECK_SPHERECAST_START = 1000f;

	// Token: 0x040007BA RID: 1978
	private const float DRONE_RADIUS = 0.5f;

	// Token: 0x040007BB RID: 1979
	private List<MeshCollider> collidersToReset = new List<MeshCollider>();

	// Token: 0x020001B4 RID: 436
	protected class GatherConfig
	{
		// Token: 0x0600093C RID: 2364 RVA: 0x00029695 File Offset: 0x00027895
		public GatherConfig()
		{
			this.fallbackOffset = Vector3.up;
			this.distanceHorizontal = 2f;
			this.distanceVertical = 1f;
		}

		// Token: 0x040007BC RID: 1980
		public static DroneSubbehaviour.GatherConfig DEFAULT = new DroneSubbehaviour.GatherConfig();

		// Token: 0x040007BD RID: 1981
		public const int GATHER_ATTEMPTS = 10;

		// Token: 0x040007BE RID: 1982
		public Vector3 fallbackOffset;

		// Token: 0x040007BF RID: 1983
		public float distanceHorizontal;

		// Token: 0x040007C0 RID: 1984
		public float distanceVertical;
	}

	// Token: 0x020001B5 RID: 437
	protected class CatcherOrientation : IDisposable
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x000296CA File Offset: 0x000278CA
		// (set) Token: 0x0600093F RID: 2367 RVA: 0x000296D2 File Offset: 0x000278D2
		public DroneProgram.Orientation orientation { get; private set; }

		// Token: 0x06000940 RID: 2368 RVA: 0x000296DB File Offset: 0x000278DB
		public static bool IsAvailable(GameObject source, int index)
		{
			return !DroneSubbehaviour.CatcherOrientation.BLACKLIST.ContainsKey(source) || !DroneSubbehaviour.CatcherOrientation.BLACKLIST[source].Contains(index);
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x00029700 File Offset: 0x00027900
		public CatcherOrientation(GameObject source, int index, DroneProgram.Orientation orientation)
		{
			this.orientation = orientation;
			this.source = source;
			this.index = new int?(index);
			if (!DroneSubbehaviour.CatcherOrientation.BLACKLIST.ContainsKey(source))
			{
				DroneSubbehaviour.CatcherOrientation.BLACKLIST[source] = new HashSet<int>();
			}
			DroneSubbehaviour.CatcherOrientation.BLACKLIST[source].Add(index);
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x0002975C File Offset: 0x0002795C
		public CatcherOrientation(DroneProgram.Orientation orientation)
		{
			this.orientation = orientation;
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x0002976C File Offset: 0x0002796C
		public void Dispose()
		{
			if (this.source != null && this.index != null)
			{
				if (DroneSubbehaviour.CatcherOrientation.BLACKLIST.ContainsKey(this.source) && DroneSubbehaviour.CatcherOrientation.BLACKLIST[this.source].Remove(this.index.Value) && DroneSubbehaviour.CatcherOrientation.BLACKLIST[this.source].Count == 0)
				{
					DroneSubbehaviour.CatcherOrientation.BLACKLIST.Remove(this.source);
				}
				this.source = null;
				this.index = null;
			}
		}

		// Token: 0x040007C2 RID: 1986
		private static Dictionary<GameObject, HashSet<int>> BLACKLIST = new Dictionary<GameObject, HashSet<int>>();

		// Token: 0x040007C3 RID: 1987
		private GameObject source;

		// Token: 0x040007C4 RID: 1988
		private int? index;
	}
}
