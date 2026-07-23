using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200085E RID: 2142
[Serializable]
public class vp_Respawner : MonoBehaviour
{
	// Token: 0x170002D7 RID: 727
	// (get) Token: 0x06002D31 RID: 11569 RVA: 0x000ABA72 File Offset: 0x000A9C72
	public static Dictionary<Collider, vp_Respawner> Instances
	{
		get
		{
			if (vp_Respawner.m_Instances == null)
			{
				vp_Respawner.m_Instances = new Dictionary<Collider, vp_Respawner>(100);
			}
			return vp_Respawner.m_Instances;
		}
	}

	// Token: 0x170002D8 RID: 728
	// (get) Token: 0x06002D32 RID: 11570 RVA: 0x000ABA8C File Offset: 0x000A9C8C
	public Renderer Renderer
	{
		get
		{
			if (this.m_Renderer == null)
			{
				this.m_Renderer = base.GetComponent<Renderer>();
			}
			return this.m_Renderer;
		}
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000ABAB0 File Offset: 0x000A9CB0
	protected virtual void Awake()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
		this.m_Transform = base.transform;
		this.m_Audio = base.GetComponent<AudioSource>();
		this.Placement.Position = (this.m_InitialPosition = this.m_Transform.position);
		this.Placement.Rotation = (this.m_InitialRotation = this.m_Transform.rotation);
		if (this.m_SpawnMode == vp_Respawner.SpawnMode.SamePosition)
		{
			this.SpawnPointTag = "";
		}
		if (this.SpawnOnAwake)
		{
			this.m_IsInitialSpawnOnAwake = true;
			vp_Utility.Activate(base.gameObject, false);
			this.PickSpawnPoint();
		}
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000ABB58 File Offset: 0x000A9D58
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		vp_Respawner.Instances.Clear();
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000ABB64 File Offset: 0x000A9D64
	protected virtual void OnEnable()
	{
		if (base.GetComponent<Collider>() != null && !vp_Respawner.Instances.ContainsValue(this))
		{
			vp_Respawner.Instances.Add(base.GetComponent<Collider>(), this);
		}
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnDisable()
	{
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000ABB94 File Offset: 0x000A9D94
	protected virtual void SpawnFX()
	{
		if (!this.m_IsInitialSpawnOnAwake)
		{
			if (this.m_Audio != null)
			{
				this.m_Audio.pitch = Time.timeScale;
				this.m_Audio.PlayOneShot(this.SpawnSound);
			}
			if (this.SpawnFXPrefabs != null && this.SpawnFXPrefabs.Length != 0)
			{
				foreach (GameObject gameObject in this.SpawnFXPrefabs)
				{
					if (gameObject != null)
					{
						vp_Utility.Instantiate(gameObject, this.m_Transform.position, this.m_Transform.rotation);
					}
				}
			}
		}
		this.m_IsInitialSpawnOnAwake = false;
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000ABC2F File Offset: 0x000A9E2F
	protected virtual void Die()
	{
		vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.PickSpawnPoint), this.m_RespawnTimer);
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000ABC5C File Offset: 0x000A9E5C
	public virtual void PickSpawnPoint()
	{
		if (this == null)
		{
			return;
		}
		if (this.m_SpawnMode == vp_Respawner.SpawnMode.SamePosition || vp_SpawnPoint.SpawnPoints.Count < 1)
		{
			this.Placement.Position = this.m_InitialPosition;
			this.Placement.Rotation = this.m_InitialRotation;
			if (this.Placement.IsObstructed(this.ObstructionRadius))
			{
				vp_Respawner.ObstructionSolver obstructionSolver = this.m_ObstructionSolver;
				if (obstructionSolver == vp_Respawner.ObstructionSolver.Wait)
				{
					vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.PickSpawnPoint), this.m_RespawnTimer);
					return;
				}
				if (obstructionSolver == vp_Respawner.ObstructionSolver.AdjustPlacement)
				{
					if (!vp_Placement.AdjustPosition(this.Placement, this.ObstructionRadius, 1000))
					{
						vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.PickSpawnPoint), this.m_RespawnTimer);
						return;
					}
				}
			}
		}
		else
		{
			vp_Respawner.ObstructionSolver obstructionSolver = this.m_ObstructionSolver;
			if (obstructionSolver != vp_Respawner.ObstructionSolver.Wait)
			{
				if (obstructionSolver == vp_Respawner.ObstructionSolver.AdjustPlacement)
				{
					this.Placement = vp_SpawnPoint.GetRandomPlacement(this.ObstructionRadius, this.SpawnPointTag);
					if (this.Placement == null)
					{
						vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.PickSpawnPoint), this.m_RespawnTimer);
						return;
					}
				}
			}
			else
			{
				this.Placement = vp_SpawnPoint.GetRandomPlacement(0f, this.SpawnPointTag);
				if (this.Placement == null)
				{
					this.Placement = new vp_Placement();
					this.m_SpawnMode = vp_Respawner.SpawnMode.SamePosition;
					this.PickSpawnPoint();
				}
				if (this.Placement.IsObstructed(this.ObstructionRadius))
				{
					vp_Timer.In(UnityEngine.Random.Range(this.MinRespawnTime, this.MaxRespawnTime), new vp_Timer.Callback(this.PickSpawnPoint), this.m_RespawnTimer);
					return;
				}
			}
		}
		this.Respawn();
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000ABE1A File Offset: 0x000AA01A
	public virtual void PickSpawnPoint(Vector3 position, Quaternion rotation)
	{
		this.Placement.Position = position;
		this.Placement.Rotation = rotation;
		this.Respawn();
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000ABE3C File Offset: 0x000AA03C
	public virtual void Respawn()
	{
		this.LastRespawnTime = Time.time;
		vp_Utility.Activate(base.gameObject, true);
		this.SpawnFX();
		if (vp_Gameplay.isMultiplayer && vp_Gameplay.isMaster)
		{
			vp_GlobalEvent<Transform, vp_Placement>.Send("TransmitRespawn", base.transform.root, this.Placement);
		}
		base.SendMessage("Reset");
		this.Placement.Position = this.m_InitialPosition;
		this.Placement.Rotation = this.m_InitialRotation;
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000ABEBC File Offset: 0x000AA0BC
	public virtual void Reset()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.m_Transform.position = this.Placement.Position;
		this.m_Transform.rotation = this.Placement.Rotation;
		if (base.GetComponent<Rigidbody>() != null && !base.GetComponent<Rigidbody>().isKinematic)
		{
			base.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			base.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000ABF38 File Offset: 0x000AA138
	public static void ResetAll(bool reInitTimers = false)
	{
		foreach (vp_Respawner vp_Respawner in vp_Respawner.Instances.Values)
		{
			if (!(vp_Respawner == null) && (!vp_Utility.IsActive(vp_Respawner.gameObject) || (vp_Respawner is vp_PlayerRespawner && (vp_Respawner as vp_PlayerRespawner).Player.Dead.Active)))
			{
				if (reInitTimers)
				{
					vp_Respawner.Die();
				}
				else
				{
					vp_Respawner.PickSpawnPoint();
				}
			}
		}
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000ABFD0 File Offset: 0x000AA1D0
	public static vp_Respawner GetByCollider(Collider col)
	{
		if (!vp_Respawner.Instances.TryGetValue(col, out vp_Respawner.m_GetInstanceResult))
		{
			vp_Respawner.m_GetInstanceResult = col.transform.root.GetComponentInChildren<vp_Respawner>();
			vp_Respawner.Instances.Add(col, vp_Respawner.m_GetInstanceResult);
		}
		return vp_Respawner.m_GetInstanceResult;
	}

	// Token: 0x04002B3F RID: 11071
	public vp_Respawner.SpawnMode m_SpawnMode;

	// Token: 0x04002B40 RID: 11072
	public string SpawnPointTag = "";

	// Token: 0x04002B41 RID: 11073
	public vp_Respawner.ObstructionSolver m_ObstructionSolver;

	// Token: 0x04002B42 RID: 11074
	public float ObstructionRadius = 1f;

	// Token: 0x04002B43 RID: 11075
	public float MinRespawnTime = 3f;

	// Token: 0x04002B44 RID: 11076
	public float MaxRespawnTime = 3f;

	// Token: 0x04002B45 RID: 11077
	public float LastRespawnTime;

	// Token: 0x04002B46 RID: 11078
	public bool SpawnOnAwake;

	// Token: 0x04002B47 RID: 11079
	public AudioClip SpawnSound;

	// Token: 0x04002B48 RID: 11080
	public GameObject[] SpawnFXPrefabs;

	// Token: 0x04002B49 RID: 11081
	protected Vector3 m_InitialPosition = Vector3.zero;

	// Token: 0x04002B4A RID: 11082
	protected Quaternion m_InitialRotation;

	// Token: 0x04002B4B RID: 11083
	protected vp_Placement Placement = new vp_Placement();

	// Token: 0x04002B4C RID: 11084
	protected Transform m_Transform;

	// Token: 0x04002B4D RID: 11085
	protected AudioSource m_Audio;

	// Token: 0x04002B4E RID: 11086
	protected bool m_IsInitialSpawnOnAwake;

	// Token: 0x04002B4F RID: 11087
	protected vp_Timer.Handle m_RespawnTimer = new vp_Timer.Handle();

	// Token: 0x04002B50 RID: 11088
	protected static Dictionary<Collider, vp_Respawner> m_Instances;

	// Token: 0x04002B51 RID: 11089
	protected static vp_Respawner m_GetInstanceResult;

	// Token: 0x04002B52 RID: 11090
	protected Renderer m_Renderer;

	// Token: 0x0200085F RID: 2143
	public enum SpawnMode
	{
		// Token: 0x04002B54 RID: 11092
		SamePosition,
		// Token: 0x04002B55 RID: 11093
		SpawnPoint
	}

	// Token: 0x02000860 RID: 2144
	public enum ObstructionSolver
	{
		// Token: 0x04002B57 RID: 11095
		Wait,
		// Token: 0x04002B58 RID: 11096
		AdjustPlacement
	}
}
