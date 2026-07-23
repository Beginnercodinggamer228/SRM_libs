using System;
using UnityEngine;

// Token: 0x020000D3 RID: 211
[CreateAssetMenu(menuName = "Ambiance Director/Zone Setting", fileName = "AmbianceDirectorZoneSetting")]
public class AmbianceDirectorZoneSetting : ScriptableObject
{
	// Token: 0x060004BE RID: 1214 RVA: 0x0001E740 File Offset: 0x0001C940
	public AmbianceDirectorZoneSetting Clone()
	{
		AmbianceDirectorZoneSetting ambianceDirectorZoneSetting = ScriptableObject.CreateInstance<AmbianceDirectorZoneSetting>();
		ambianceDirectorZoneSetting.zone = this.zone;
		ambianceDirectorZoneSetting.dayFogColor = this.dayFogColor;
		ambianceDirectorZoneSetting.dayFogDensity = this.dayFogDensity;
		ambianceDirectorZoneSetting.dayAmbientColor = this.dayAmbientColor;
		ambianceDirectorZoneSetting.nightFogColor = this.nightFogColor;
		ambianceDirectorZoneSetting.nightFogDensity = this.nightFogDensity;
		ambianceDirectorZoneSetting.nightAmbientColor = this.nightAmbientColor;
		ambianceDirectorZoneSetting.daySkyColor = this.daySkyColor;
		ambianceDirectorZoneSetting.daySkyHorizon = this.daySkyHorizon;
		ambianceDirectorZoneSetting.nightSkyColor = this.nightSkyColor;
		ambianceDirectorZoneSetting.nightSkyHorizon = this.nightSkyHorizon;
		return ambianceDirectorZoneSetting;
	}

	// Token: 0x040004F7 RID: 1271
	public AmbianceDirector.Zone zone;

	// Token: 0x040004F8 RID: 1272
	public Color dayFogColor;

	// Token: 0x040004F9 RID: 1273
	public float dayFogDensity;

	// Token: 0x040004FA RID: 1274
	public Color dayAmbientColor;

	// Token: 0x040004FB RID: 1275
	public Color nightFogColor;

	// Token: 0x040004FC RID: 1276
	public float nightFogDensity;

	// Token: 0x040004FD RID: 1277
	public Color nightAmbientColor;

	// Token: 0x040004FE RID: 1278
	public Color daySkyColor;

	// Token: 0x040004FF RID: 1279
	public Color daySkyHorizon;

	// Token: 0x04000500 RID: 1280
	public Color nightSkyColor;

	// Token: 0x04000501 RID: 1281
	public Color nightSkyHorizon;
}
