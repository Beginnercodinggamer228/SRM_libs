using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class AdjustMusicOnSlimesNearby : MonoBehaviour
{
	// Token: 0x06000D9B RID: 3483 RVA: 0x00037588 File Offset: 0x00035788
	public void Awake()
	{
		this.musicDir = SRSingleton<GameContext>.Instance.MusicDirector;
		this.tarrGroup = new CullingGroup();
		this.tarrGroup.SetBoundingSpheres(TarrBoundingSphere.allSpheres);
		this.tarrGroup.SetBoundingDistances(new float[]
		{
			30f
		});
		this.tarrGroup.SetDistanceReferencePoint(base.transform);
		this.tarrGroup.SetBoundingSphereCount(TarrBoundingSphere.sphereCount);
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000375FA File Offset: 0x000357FA
	public void OnDestroy()
	{
		this.tarrGroup.Dispose();
		TarrBoundingSphere.ResetTarrData();
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x0003760C File Offset: 0x0003580C
	public void Update()
	{
		this.RefreshCullingGroup();
		if (Time.time > this.rethinkTime)
		{
			this.RethinkMusic();
		}
		this.ResetTarrBoundingSphereCount();
	}

	// Token: 0x06000D9E RID: 3486 RVA: 0x0003762D File Offset: 0x0003582D
	private void RefreshCullingGroup()
	{
		if (this.local_nearbyTarrIndices.Length != TarrBoundingSphere.allSpheres.Length)
		{
			Array.Resize<int>(ref this.local_nearbyTarrIndices, TarrBoundingSphere.allSpheres.Length);
		}
		this.tarrGroup.SetBoundingSphereCount(TarrBoundingSphere.sphereCount);
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x00037664 File Offset: 0x00035864
	private void RethinkMusic()
	{
		bool flag = this.tarrGroup.QueryIndices(0, this.local_nearbyTarrIndices, 0) >= 1;
		if (flag != this.tarrMode)
		{
			this.tarrMode = flag;
			this.musicDir.SetTarrMode(this.tarrMode);
			this.rethinkTime = Time.time + (this.tarrMode ? 10f : 5f);
			return;
		}
		this.rethinkTime = Time.time + 0.5f;
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x000376DE File Offset: 0x000358DE
	private void ResetTarrBoundingSphereCount()
	{
		TarrBoundingSphere.sphereCount = 0;
	}

	// Token: 0x04000CF7 RID: 3319
	private MusicDirector musicDir;

	// Token: 0x04000CF8 RID: 3320
	private bool tarrMode;

	// Token: 0x04000CF9 RID: 3321
	private float rethinkTime;

	// Token: 0x04000CFA RID: 3322
	private const float RETHINK_TIME_ON_TARR_START = 10f;

	// Token: 0x04000CFB RID: 3323
	private const float RETHINK_TIME_ON_TARR_END = 5f;

	// Token: 0x04000CFC RID: 3324
	private const float RETHINK_TIME_UNCHANGED = 0.5f;

	// Token: 0x04000CFD RID: 3325
	private const float TARR_DISTANCE_FOR_MUSIC = 30f;

	// Token: 0x04000CFE RID: 3326
	private CullingGroup tarrGroup;

	// Token: 0x04000CFF RID: 3327
	private int[] local_nearbyTarrIndices = new int[100];
}
