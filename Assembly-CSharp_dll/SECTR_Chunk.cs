using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000A8 RID: 168
[RequireComponent(typeof(SECTR_Sector))]
[AddComponentMenu("SECTR/Stream/SECTR Chunk")]
public class SECTR_Chunk : MonoBehaviour
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x060003CF RID: 975 RVA: 0x00017981 File Offset: 0x00015B81
	public SECTR_Sector Sector
	{
		get
		{
			return this.cachedSector;
		}
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x0001798C File Offset: 0x00015B8C
	public void SetCanProxy(bool canProxy)
	{
		this.canProxy = canProxy;
		if (!canProxy && this.proxy != null)
		{
			Destroyer.Destroy(this.proxy, "SECTR_Chunk.SetCanProxy");
			this.proxy = null;
			return;
		}
		if (this.proxy == null && this.ProxyMesh && canProxy)
		{
			this._CreateProxy();
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x000179F0 File Offset: 0x00015BF0
	public void AddReference()
	{
		if (this.refCount == 0)
		{
			this._Load();
			if (this.Changed != null)
			{
				this.Changed(this, true);
			}
		}
		this.refCount++;
		if (this.ReferenceChange != null)
		{
			this.ReferenceChange(this, true);
		}
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x00017A44 File Offset: 0x00015C44
	public void RemoveReference()
	{
		if (this.ReferenceChange != null)
		{
			this.ReferenceChange(this, false);
		}
		this.refCount--;
		if (this.refCount <= 0)
		{
			if (this.Changed != null)
			{
				this.Changed(this, false);
			}
			this._Unload();
			this.refCount = 0;
		}
	}

	// Token: 0x060003D3 RID: 979 RVA: 0x00017A9F File Offset: 0x00015C9F
	public void AddWakeReference()
	{
		if (this.wakeRefCount == 0 && this.cachedSector != null)
		{
			this.cachedSector.Hibernate = false;
		}
		this.wakeRefCount++;
	}

	// Token: 0x060003D4 RID: 980 RVA: 0x00017AD1 File Offset: 0x00015CD1
	public void RemoveWakeReference()
	{
		this.wakeRefCount--;
		if (this.wakeRefCount <= 0)
		{
			if (this.cachedSector != null)
			{
				this.cachedSector.Hibernate = true;
			}
			this.wakeRefCount = 0;
		}
	}

	// Token: 0x060003D5 RID: 981 RVA: 0x00017B0C File Offset: 0x00015D0C
	public void CheckReferences()
	{
		if (this.refCount <= 0)
		{
			this._Unload();
			this.refCount = 0;
		}
		if (this.wakeRefCount <= 0)
		{
			if (this.cachedSector != null)
			{
				this.cachedSector.Hibernate = true;
			}
			this.wakeRefCount = 0;
		}
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00017B59 File Offset: 0x00015D59
	public bool IsLoaded()
	{
		return this.loadState == SECTR_Chunk.LoadState.Active;
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x00017B64 File Offset: 0x00015D64
	public bool IsUnloaded()
	{
		return this.loadState == SECTR_Chunk.LoadState.Unloaded;
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x00017B70 File Offset: 0x00015D70
	public float LoadProgress()
	{
		switch (this.loadState)
		{
		case SECTR_Chunk.LoadState.Loading:
			if (this.asyncLoadOp == null)
			{
				return 0.5f;
			}
			return this.asyncLoadOp.progress * 0.8f;
		case SECTR_Chunk.LoadState.Loaded:
			return 0.9f;
		case SECTR_Chunk.LoadState.Active:
			return 1f;
		}
		return 0f;
	}

	// Token: 0x14000002 RID: 2
	// (add) Token: 0x060003D9 RID: 985 RVA: 0x00017BD4 File Offset: 0x00015DD4
	// (remove) Token: 0x060003DA RID: 986 RVA: 0x00017C0C File Offset: 0x00015E0C
	public event SECTR_Chunk.LoadCallback Changed;

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x060003DB RID: 987 RVA: 0x00017C44 File Offset: 0x00015E44
	// (remove) Token: 0x060003DC RID: 988 RVA: 0x00017C7C File Offset: 0x00015E7C
	public event SECTR_Chunk.LoadCallback ReferenceChange;

	// Token: 0x060003DD RID: 989 RVA: 0x00017CB1 File Offset: 0x00015EB1
	private void Awake()
	{
		SECTR_LightmapRef.InitRefCounts();
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00017CB8 File Offset: 0x00015EB8
	private void OnEnable()
	{
		this.cachedSector = base.GetComponent<SECTR_Sector>();
		this.cachedSector.Hibernate = (this.wakeRefCount <= 0);
		if (this.cachedSector.Frozen && this.canProxy)
		{
			this._CreateProxy();
		}
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00017CF8 File Offset: 0x00015EF8
	private void OnDisable()
	{
		if (!this.quitting && this.asyncLoadOp != null && !this.asyncLoadOp.isDone)
		{
			Debug.LogError("Chunk unloaded with async operation active. Do not disable chunks until async operations are complete or Unity will likely crash.");
		}
		if (this.loadState != SECTR_Chunk.LoadState.Unloaded)
		{
			this._FindChunkRoot();
			if (this.chunkRoot)
			{
				this._DestoryChunk(false);
			}
		}
		this.cachedSector = null;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00017D55 File Offset: 0x00015F55
	private void OnApplicationQuit()
	{
		this.quitting = true;
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00017D60 File Offset: 0x00015F60
	private void FixedUpdate()
	{
		switch (this.loadState)
		{
		case SECTR_Chunk.LoadState.Loading:
			this._TrySceneActivation();
			if (this.asyncLoadOp == null || this.asyncLoadOp.isDone)
			{
				SECTR_Chunk.sceneActivating = false;
				this.asyncLoadOp = null;
				this.loadState = SECTR_Chunk.LoadState.Loaded;
				this.FixedUpdate();
				return;
			}
			break;
		case SECTR_Chunk.LoadState.Loaded:
			this._SetupChunk();
			return;
		case SECTR_Chunk.LoadState.Unloading:
			this._TrySceneActivation();
			this._FindChunkRoot();
			if (this.chunkRoot)
			{
				this._DestoryChunk(true);
			}
			break;
		case SECTR_Chunk.LoadState.Active:
			break;
		default:
			return;
		}
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00017DEA File Offset: 0x00015FEA
	private void _Load()
	{
		if (base.enabled && (this.loadState == SECTR_Chunk.LoadState.Unloaded || this.loadState == SECTR_Chunk.LoadState.Unloading))
		{
			this.chunkRoot.SetActive(true);
			this.loadState = SECTR_Chunk.LoadState.Loaded;
		}
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00017E18 File Offset: 0x00016018
	public void SetRoot(GameObject root)
	{
		this.chunkRoot = root;
		this.chunkSector = root;
		this.loadState = SECTR_Chunk.LoadState.Active;
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00017E2F File Offset: 0x0001602F
	private void _Unload()
	{
		if (base.enabled && this.loadState != SECTR_Chunk.LoadState.Unloaded)
		{
			this.cachedSector.Frozen = true;
			if (this.chunkRoot)
			{
				this._DestoryChunk(true);
				return;
			}
			this.loadState = SECTR_Chunk.LoadState.Unloading;
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00017E6C File Offset: 0x0001606C
	private void _DestoryChunk(bool createProxy)
	{
		if (this.cachedSector.TopTerrain || this.cachedSector.BottomTerrain || this.cachedSector.RightTerrain || this.cachedSector.LeftTerrain)
		{
			this.cachedSector.DisonnectTerrainNeighbors();
		}
		this.chunkRoot.SetActive(false);
		this.loadState = SECTR_Chunk.LoadState.Unloaded;
		if (createProxy && this.ProxyMesh && this.canProxy)
		{
			this._CreateProxy();
		}
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00017F00 File Offset: 0x00016100
	private void _FindChunkRoot()
	{
		if (this.chunkRoot == null)
		{
			SECTR_ChunkRef sectr_ChunkRef = SECTR_ChunkRef.FindChunkRef(this.NodeName);
			if (sectr_ChunkRef && sectr_ChunkRef.RealSector)
			{
				this.recenterChunk = sectr_ChunkRef.Recentered;
				if (this.recenterChunk)
				{
					sectr_ChunkRef.RealSector.parent = base.transform;
					this.chunkRoot = sectr_ChunkRef.RealSector.gameObject;
					this.chunkSector = this.chunkRoot;
					Destroyer.Destroy(sectr_ChunkRef.gameObject, "SECTR_Chunk._FindChunkRoot#1");
					return;
				}
				this.chunkRoot = sectr_ChunkRef.gameObject;
				this.chunkSector = sectr_ChunkRef.RealSector.gameObject;
				Destroyer.Destroy(sectr_ChunkRef, "SECTR_Chunk._FindChunkRoot#2");
				return;
			}
			else if (!this.quitting)
			{
				this.chunkRoot = GameObject.Find(this.NodeName);
				this.chunkSector = this.chunkRoot;
				this.recenterChunk = false;
			}
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x00017FEC File Offset: 0x000161EC
	private void _SetupChunk()
	{
		this._FindChunkRoot();
		if (this.chunkRoot)
		{
			if (!this.chunkRoot.activeSelf)
			{
				this.chunkRoot.SetActive(true);
			}
			if (this.recenterChunk)
			{
				Transform transform = this.chunkRoot.transform;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
			}
			SECTR_Member sectr_Member = this.chunkSector.GetComponent<SECTR_Member>();
			if (!sectr_Member)
			{
				sectr_Member = this.chunkSector.gameObject.AddComponent<SECTR_Member>();
				sectr_Member.BoundsUpdateMode = SECTR_Member.BoundsUpdateModes.Static;
				sectr_Member.ForceUpdate(true, false);
			}
			else if (this.recenterChunk)
			{
				sectr_Member.ForceUpdate(true, false);
			}
			this.cachedSector.ChildProxy = sectr_Member;
			this.cachedSector.Frozen = false;
			if (this.cachedSector.TopTerrain || this.cachedSector.BottomTerrain || this.cachedSector.LeftTerrain || this.cachedSector.RightTerrain)
			{
				this.cachedSector.ConnectTerrainNeighbors();
				if (this.cachedSector.TopTerrain)
				{
					this.cachedSector.TopTerrain.ConnectTerrainNeighbors();
				}
				if (this.cachedSector.BottomTerrain)
				{
					this.cachedSector.BottomTerrain.ConnectTerrainNeighbors();
				}
				if (this.cachedSector.LeftTerrain)
				{
					this.cachedSector.LeftTerrain.ConnectTerrainNeighbors();
				}
				if (this.cachedSector.RightTerrain)
				{
					this.cachedSector.RightTerrain.ConnectTerrainNeighbors();
				}
			}
			if (this.proxy)
			{
				Destroyer.Destroy(this.proxy, "SECTR_Chunk._SetupChunk");
				this.proxy = null;
			}
			this.loadState = SECTR_Chunk.LoadState.Active;
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000181C4 File Offset: 0x000163C4
	private void _CreateProxy()
	{
		if (this.proxy == null && this.ProxyMesh && !this.quitting)
		{
			this.proxy = new GameObject(base.name + " Proxy");
			this.proxy.AddComponent<MeshFilter>().sharedMesh = this.ProxyMesh;
			MeshRenderer meshRenderer = this.proxy.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			meshRenderer.sharedMaterials = this.ProxyMaterials;
			this.proxy.transform.position = base.transform.position;
			this.proxy.transform.rotation = base.transform.rotation;
			this.proxy.transform.localScale = base.transform.lossyScale;
			this.proxy.transform.SetParent(base.transform, true);
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000182B4 File Offset: 0x000164B4
	private void _TrySceneActivation()
	{
		if (this.asyncLoadOp != null && !this.asyncLoadOp.allowSceneActivation && !SECTR_Chunk.sceneActivating && this.asyncLoadOp.progress >= 0.9f)
		{
			SECTR_Chunk.sceneActivating = true;
			this.asyncLoadOp.allowSceneActivation = true;
		}
	}

	// Token: 0x040003CD RID: 973
	private AsyncOperation asyncLoadOp;

	// Token: 0x040003CE RID: 974
	private SECTR_Chunk.LoadState loadState;

	// Token: 0x040003CF RID: 975
	private int refCount;

	// Token: 0x040003D0 RID: 976
	private int wakeRefCount;

	// Token: 0x040003D1 RID: 977
	private GameObject chunkRoot;

	// Token: 0x040003D2 RID: 978
	private GameObject chunkSector;

	// Token: 0x040003D3 RID: 979
	private bool recenterChunk;

	// Token: 0x040003D4 RID: 980
	private SECTR_Sector cachedSector;

	// Token: 0x040003D5 RID: 981
	private GameObject proxy;

	// Token: 0x040003D6 RID: 982
	private bool quitting;

	// Token: 0x040003D7 RID: 983
	private static bool sceneActivating;

	// Token: 0x040003D8 RID: 984
	[SECTR_ToolTip("The path of the scene to load")]
	public string ScenePath;

	// Token: 0x040003D9 RID: 985
	[SECTR_ToolTip("The unique name of the root object in the exported Sector.")]
	public string NodeName;

	// Token: 0x040003DA RID: 986
	[SECTR_ToolTip("Exports the Chunk in a way that allows it to be shared by multiple Sectors, but may take more CPU to load.")]
	public bool ExportForReuse;

	// Token: 0x040003DB RID: 987
	[SECTR_ToolTip("A mesh to display when this Chunk is unloaded. Will be hidden when loaded.")]
	public Mesh ProxyMesh;

	// Token: 0x040003DC RID: 988
	[SECTR_ToolTip("The per-submesh materials for the proxy.")]
	public Material[] ProxyMaterials;

	// Token: 0x040003DD RID: 989
	private bool canProxy;

	// Token: 0x020000A9 RID: 169
	private enum LoadState
	{
		// Token: 0x040003E1 RID: 993
		Unloaded,
		// Token: 0x040003E2 RID: 994
		Loading,
		// Token: 0x040003E3 RID: 995
		Loaded,
		// Token: 0x040003E4 RID: 996
		Unloading,
		// Token: 0x040003E5 RID: 997
		Active
	}

	// Token: 0x020000AA RID: 170
	// (Invoke) Token: 0x060003ED RID: 1005
	public delegate void LoadCallback(SECTR_Chunk source, bool loaded);
}
