using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000862 RID: 2146
public class vp_FootstepManager : MonoBehaviour
{
	// Token: 0x170002DA RID: 730
	// (get) Token: 0x06002D4C RID: 11596 RVA: 0x000AC334 File Offset: 0x000AA534
	public static vp_FootstepManager[] FootstepManagers
	{
		get
		{
			if (vp_FootstepManager.mIsDirty)
			{
				vp_FootstepManager.mIsDirty = false;
				vp_FootstepManager.m_FootstepManagers = (UnityEngine.Object.FindObjectsOfType(typeof(vp_FootstepManager)) as vp_FootstepManager[]);
				if (vp_FootstepManager.m_FootstepManagers == null)
				{
					vp_FootstepManager.m_FootstepManagers = (Resources.FindObjectsOfTypeAll(typeof(vp_FootstepManager)) as vp_FootstepManager[]);
				}
			}
			return vp_FootstepManager.m_FootstepManagers;
		}
	}

	// Token: 0x170002DB RID: 731
	// (get) Token: 0x06002D4D RID: 11597 RVA: 0x000AC38C File Offset: 0x000AA58C
	public bool IsDirty
	{
		get
		{
			return vp_FootstepManager.mIsDirty;
		}
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x000AC394 File Offset: 0x000AA594
	protected virtual void Awake()
	{
		this.m_Player = base.transform.root.GetComponentInChildren<vp_FPPlayerEventHandler>();
		this.m_Camera = base.transform.root.GetComponentInChildren<vp_FPCamera>();
		this.m_Controller = base.transform.root.GetComponentInChildren<vp_FPController>();
		this.m_Audio = base.gameObject.AddComponent<AudioSource>();
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x000AC3F4 File Offset: 0x000AA5F4
	public virtual void SetDirty(bool dirty)
	{
		vp_FootstepManager.mIsDirty = dirty;
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x000AC3FC File Offset: 0x000AA5FC
	private void Update()
	{
		if (this.m_Camera.BobStepCallback == null)
		{
			vp_FPCamera camera = this.m_Camera;
			camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
		}
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x000AC433 File Offset: 0x000AA633
	protected virtual void OnEnable()
	{
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x000AC45D File Offset: 0x000AA65D
	protected virtual void OnDisable()
	{
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x000AC488 File Offset: 0x000AA688
	protected virtual void Footstep()
	{
		if (this.m_Player.Dead.Active)
		{
			return;
		}
		if (!this.m_Controller.Grounded)
		{
			return;
		}
		if (this.m_Player.GroundTexture.Get() == null && this.m_Player.SurfaceType.Get() == null)
		{
			return;
		}
		if (this.m_Player.SurfaceType.Get() != null)
		{
			this.PlaySound(this.SurfaceTypes[this.m_Player.SurfaceType.Get().SurfaceID]);
			return;
		}
		foreach (vp_FootstepManager.vp_SurfaceTypes vp_SurfaceTypes in this.SurfaceTypes)
		{
			using (List<Texture>.Enumerator enumerator2 = vp_SurfaceTypes.Textures.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == this.m_Player.GroundTexture.Get())
					{
						this.PlaySound(vp_SurfaceTypes);
						break;
					}
				}
			}
		}
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x000AC5D8 File Offset: 0x000AA7D8
	public virtual void PlaySound(vp_FootstepManager.vp_SurfaceTypes st)
	{
		if (st.Sounds == null || st.Sounds.Count == 0)
		{
			return;
		}
		for (;;)
		{
			this.m_SoundToPlay = st.Sounds[UnityEngine.Random.Range(0, st.Sounds.Count)];
			if (this.m_SoundToPlay == null)
			{
				break;
			}
			if (!(this.m_SoundToPlay == this.m_LastPlayedSound) || st.Sounds.Count <= 1)
			{
				goto IL_68;
			}
		}
		return;
		IL_68:
		this.m_Audio.pitch = UnityEngine.Random.Range(st.RandomPitch.x, st.RandomPitch.y) * Time.timeScale;
		this.m_Audio.clip = this.m_SoundToPlay;
		this.m_Audio.Play();
		this.m_LastPlayedSound = this.m_SoundToPlay;
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x000AC6A4 File Offset: 0x000AA8A4
	public static int GetMainTerrainTexture(Vector3 worldPos, Terrain terrain)
	{
		TerrainData terrainData = terrain.terrainData;
		Vector3 position = terrain.transform.position;
		int x = (int)((worldPos.x - position.x) / terrainData.size.x * (float)terrainData.alphamapWidth);
		int y = (int)((worldPos.z - position.z) / terrainData.size.z * (float)terrainData.alphamapHeight);
		float[,,] alphamaps = terrainData.GetAlphamaps(x, y, 1, 1);
		float[] array = new float[alphamaps.GetUpperBound(2) + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = alphamaps[0, 0, i];
		}
		float num = 0f;
		int result = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] > num)
			{
				result = j;
				num = array[j];
			}
		}
		return result;
	}

	// Token: 0x04002B5F RID: 11103
	private static vp_FootstepManager[] m_FootstepManagers;

	// Token: 0x04002B60 RID: 11104
	public static bool mIsDirty = true;

	// Token: 0x04002B61 RID: 11105
	public List<vp_FootstepManager.vp_SurfaceTypes> SurfaceTypes = new List<vp_FootstepManager.vp_SurfaceTypes>();

	// Token: 0x04002B62 RID: 11106
	protected vp_FPPlayerEventHandler m_Player;

	// Token: 0x04002B63 RID: 11107
	protected vp_FPCamera m_Camera;

	// Token: 0x04002B64 RID: 11108
	protected vp_FPController m_Controller;

	// Token: 0x04002B65 RID: 11109
	protected AudioSource m_Audio;

	// Token: 0x04002B66 RID: 11110
	protected AudioClip m_SoundToPlay;

	// Token: 0x04002B67 RID: 11111
	protected AudioClip m_LastPlayedSound;

	// Token: 0x02000863 RID: 2147
	[Serializable]
	public class vp_SurfaceTypes
	{
		// Token: 0x04002B68 RID: 11112
		public Vector2 RandomPitch = new Vector2(1f, 1.5f);

		// Token: 0x04002B69 RID: 11113
		public bool Foldout = true;

		// Token: 0x04002B6A RID: 11114
		public bool SoundsFoldout = true;

		// Token: 0x04002B6B RID: 11115
		public bool TexturesFoldout = true;

		// Token: 0x04002B6C RID: 11116
		public string SurfaceName = "";

		// Token: 0x04002B6D RID: 11117
		public List<AudioClip> Sounds = new List<AudioClip>();

		// Token: 0x04002B6E RID: 11118
		public List<Texture> Textures = new List<Texture>();
	}
}
