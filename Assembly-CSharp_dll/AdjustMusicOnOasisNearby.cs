using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class AdjustMusicOnOasisNearby : MonoBehaviour
{
	// Token: 0x06000D94 RID: 3476 RVA: 0x00037408 File Offset: 0x00035608
	public void Awake()
	{
		this.musicDir = SRSingleton<GameContext>.Instance.MusicDirector;
		this.oasisGroup = new CullingGroup();
		this.oasisGroup.SetBoundingSpheres(Oasis.oasisSpheres.Data);
		this.oasisGroup.SetBoundingSphereCount(Oasis.oasisSpheres.GetCount());
		this.oasisGroup.SetDistanceReferencePoint(base.gameObject.transform);
		this.oasisGroup.SetBoundingDistances(new float[]
		{
			50f
		});
	}

	// Token: 0x06000D95 RID: 3477 RVA: 0x00037489 File Offset: 0x00035689
	public void OnEnable()
	{
		this.RefreshCullingGroup();
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00037491 File Offset: 0x00035691
	public void OnDestroy()
	{
		Oasis.oasisSpheres.Clear();
		this.oasisGroup.Dispose();
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x000374A8 File Offset: 0x000356A8
	public void Update()
	{
		if (this.firstFramePassed && Time.time > this.rethinkTime)
		{
			this.RefreshCullingGroup();
			this.RethinkMusic();
			return;
		}
		if (!this.firstFramePassed)
		{
			this.RefreshCullingGroup();
			this.firstFramePassed = true;
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x000374E1 File Offset: 0x000356E1
	private void RefreshCullingGroup()
	{
		this.oasisGroup.SetBoundingSphereCount(Oasis.oasisSpheres.GetCount());
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x000374F8 File Offset: 0x000356F8
	private void RethinkMusic()
	{
		bool flag = this.oasisGroup.QueryIndices(0, this.local_sphereCheck, 0) >= 1;
		if (flag != this.oasisMode)
		{
			this.oasisMode = flag;
			this.musicDir.SetOasisMode(this.oasisMode);
			this.rethinkTime = Time.time + (this.oasisMode ? 10f : 5f);
			return;
		}
		this.rethinkTime = Time.time + 0.5f;
	}

	// Token: 0x04000CEE RID: 3310
	private MusicDirector musicDir;

	// Token: 0x04000CEF RID: 3311
	private bool oasisMode;

	// Token: 0x04000CF0 RID: 3312
	private float rethinkTime;

	// Token: 0x04000CF1 RID: 3313
	private const float RETHINK_TIME_ON_OASIS_START = 10f;

	// Token: 0x04000CF2 RID: 3314
	private const float RETHINK_TIME_ON_OASIS_END = 5f;

	// Token: 0x04000CF3 RID: 3315
	private const float RETHINK_TIME_UNCHANGED = 0.5f;

	// Token: 0x04000CF4 RID: 3316
	private CullingGroup oasisGroup;

	// Token: 0x04000CF5 RID: 3317
	private bool firstFramePassed;

	// Token: 0x04000CF6 RID: 3318
	private int[] local_sphereCheck = new int[20];
}
