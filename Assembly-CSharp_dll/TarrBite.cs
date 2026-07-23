using System;
using Assets.Script.Util.Extensions;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public class TarrBite : MonoBehaviour
{
	// Token: 0x060018DB RID: 6363 RVA: 0x00060E10 File Offset: 0x0005F010
	public void Awake()
	{
		this.aggregator = base.gameObject.GetRequiredComponentInChildren(false);
		this.appearanceApplicator = base.gameObject.GetRequiredComponentInChildren(false);
	}

	// Token: 0x060018DC RID: 6364 RVA: 0x00060E38 File Offset: 0x0005F038
	public void Start()
	{
		this.aggregator.OnEnableBite += this.ShowBite;
		this.aggregator.OnDisableBite += this.HideBite;
		this.WireBodyAndBiteComponents();
		this.appearanceApplicator.OnAppearanceChanged += this.OnAppearanceChanged;
	}

	// Token: 0x060018DD RID: 6365 RVA: 0x00060E90 File Offset: 0x0005F090
	private void OnAppearanceChanged(SlimeAppearance appearance)
	{
		this.WireBodyAndBiteComponents();
	}

	// Token: 0x060018DE RID: 6366 RVA: 0x00060E98 File Offset: 0x0005F098
	private void WireBodyAndBiteComponents()
	{
		this.bodyMeshes = base.GetComponentsInChildren<BodyMeshMarker>();
		this.biteMeshes = base.GetComponentsInChildren<BiteMeshMarker>();
		if (this.aggregator.IsBiteAnimationStateActive())
		{
			this.ShowBite();
			return;
		}
		this.HideBite();
	}

	// Token: 0x060018DF RID: 6367 RVA: 0x00060ECC File Offset: 0x0005F0CC
	private void ShowBite()
	{
		BodyMeshMarker[] array = this.bodyMeshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(false);
		}
		BiteMeshMarker[] array2 = this.biteMeshes;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x00060F20 File Offset: 0x0005F120
	private void HideBite()
	{
		BodyMeshMarker[] array = this.bodyMeshes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(true);
		}
		BiteMeshMarker[] array2 = this.biteMeshes;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x00060F74 File Offset: 0x0005F174
	public void OnDestroy()
	{
		if (this.aggregator != null)
		{
			this.aggregator.OnEnableBite -= this.ShowBite;
			this.aggregator.OnDisableBite -= this.HideBite;
		}
		if (this.appearanceApplicator != null)
		{
			this.appearanceApplicator.OnAppearanceChanged -= this.OnAppearanceChanged;
		}
	}

	// Token: 0x04001899 RID: 6297
	private BodyMeshMarker[] bodyMeshes;

	// Token: 0x0400189A RID: 6298
	private BiteMeshMarker[] biteMeshes;

	// Token: 0x0400189B RID: 6299
	private BiteEventAggregator aggregator;

	// Token: 0x0400189C RID: 6300
	private SlimeAppearanceApplicator appearanceApplicator;
}
