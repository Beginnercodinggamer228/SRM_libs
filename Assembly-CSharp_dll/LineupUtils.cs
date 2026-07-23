using System;
using UnityEngine;

// Token: 0x020004C9 RID: 1225
public static class LineupUtils
{
	// Token: 0x060019A6 RID: 6566 RVA: 0x00063F8C File Offset: 0x0006218C
	public static SlimeAppearanceApplicator GenerateAppearancePreview(SlimeAppearanceApplicator prefab, SlimeDefinition slimeDefinition, SlimeAppearance appearance)
	{
		SlimeAppearanceApplicator slimeAppearanceApplicator = UnityEngine.Object.Instantiate<SlimeAppearanceApplicator>(prefab, Vector3.zero, Quaternion.identity);
		slimeAppearanceApplicator.enabled = false;
		slimeAppearanceApplicator.SlimeDefinition = null;
		slimeAppearanceApplicator.Appearance = appearance;
		try
		{
			slimeAppearanceApplicator.ApplyAppearance();
		}
		catch (Exception ex)
		{
			Log.Error("An issue occurred while trying to apply the appearance: " + appearance.name, new object[]
			{
				ex
			});
		}
		foreach (EnableBasedOnGrounded enableBasedOnGrounded in slimeAppearanceApplicator.GetComponentsInChildren<EnableBasedOnGrounded>())
		{
			enableBasedOnGrounded.gameObject.SetActive(!enableBasedOnGrounded.enableOnGrounded);
		}
		DeactivateOnHeld[] componentsInChildren2 = slimeAppearanceApplicator.GetComponentsInChildren<DeactivateOnHeld>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = false;
		}
		NotifyBiteComplete[] componentsInChildren3 = slimeAppearanceApplicator.GetComponentsInChildren<NotifyBiteComplete>();
		for (int i = 0; i < componentsInChildren3.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren3[i]);
		}
		slimeAppearanceApplicator.transform.localScale = new Vector3(slimeDefinition.PrefabScale, slimeDefinition.PrefabScale, slimeDefinition.PrefabScale);
		return slimeAppearanceApplicator;
	}
}
