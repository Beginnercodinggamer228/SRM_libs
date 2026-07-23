using System;
using System.Collections.Generic;
using System.Linq;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class AmbianceDirector : SRBehaviour, WorldModel.Participant, AmbianceDirector.TimeOfDay
{
	// Token: 0x06000492 RID: 1170 RVA: 0x0001D914 File Offset: 0x0001BB14
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.timeOfDay.Insert(0, this);
		foreach (AmbianceDirector.WeatherEntry weatherEntry in this.weatherEntries)
		{
			this.weatherPrefabs[weatherEntry.weather] = weatherEntry.prefab;
		}
		if (AmbianceDirector.onAwakeDelegate != null)
		{
			AmbianceDirector.onAwakeDelegate(this);
		}
		if (this.weatherAttach == null && SRSingleton<SceneContext>.Instance.Player != null)
		{
			this.weatherAttach = SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeatherEffectAttachment>();
		}
		foreach (Light light in this.duskedLightDefaultColors.Keys)
		{
			this.duskedLightDefaultColors[light] = light.color;
		}
		this.zoneDict = this.zoneSettings.ToDictionary((AmbianceDirectorZoneSetting m) => m.zone, (AmbianceDirectorZoneSetting m) => m);
		this.skyboxBlendId = Shader.PropertyToID("_Blend");
		this.skyboxDaynessId = Shader.PropertyToID("_Dayness");
		this.skyboxDayColorId = Shader.PropertyToID("_SkyColor");
		this.skyboxDayHorizonId = Shader.PropertyToID("_HorizonColor");
		this.skyboxNightColorId = Shader.PropertyToID("_SkyColorNight");
		this.skyboxNightHorizonId = Shader.PropertyToID("_HorizonNight");
		this.InitSkybox();
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0001DAC4 File Offset: 0x0001BCC4
	public void InitModel(WorldModel worldModel)
	{
		worldModel.currWeather = AmbianceDirector.Weather.NONE;
		worldModel.weatherUntil = this.timeDir.HoursFromNowOrStart(Randoms.SHARED.GetInRange(this.minWeatherCycleHours, this.maxWeatherCycleHours));
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0001DAF4 File Offset: 0x0001BCF4
	public void SetModel(WorldModel worldModel)
	{
		this.worldModel = worldModel;
		if (SRSingleton<SceneContext>.Instance.Player != null)
		{
			this.weatherAttach = SRSingleton<SceneContext>.Instance.Player.GetComponentInChildren<WeatherEffectAttachment>();
			this.weatherAttach.SetWeather(this.weatherPrefabs[worldModel.currWeather]);
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0001DB4C File Offset: 0x0001BD4C
	public void InitForLevel()
	{
		SRSingleton<SceneContext>.Instance.GameModel.RegisterWorldParticipant(this);
		this.InitSkybox();
		this.caveTypeCounts.Clear();
		this.waterCount = 0;
		this.seaCount = 0;
		this.caveDarkness = 0f;
		this.currZoneSetting = null;
		this.UpdateZoneSetting();
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0001DBA0 File Offset: 0x0001BDA0
	public void Update()
	{
		this.UpdateZoneSetting();
		AmbianceDirector.TimeOfDay timeOfDay = this.timeOfDay.Last<AmbianceDirector.TimeOfDay>();
		float currentDayFraction_Position = timeOfDay.GetCurrentDayFraction_Position();
		float currentDayFraction_Color = timeOfDay.GetCurrentDayFraction_Color();
		float num;
		float num2;
		if (currentDayFraction_Color < 0.25f)
		{
			num = 0f;
			num2 = Mathf.Clamp((0.25f - currentDayFraction_Color) * 288f, 0.01f, 1f);
		}
		else if (currentDayFraction_Color > 0.75f)
		{
			num = 0f;
			num2 = Mathf.Clamp((currentDayFraction_Color - 0.75f) * 288f, 0.01f, 1f);
		}
		else
		{
			num2 = 0f;
			num = Mathf.Clamp(Mathf.Min(currentDayFraction_Color - 0.25f, 0.75f - currentDayFraction_Color) * 288f, 0.01f, 1f);
		}
		if (this.caveTypeCounts.Count > 0 && this.caveDarkness < 1f)
		{
			this.caveDarkness = Mathf.Min(1f, this.caveDarkness + Time.deltaTime / this.zoneSettingTransitionTime);
		}
		else if (this.caveTypeCounts.Count <= 0 && this.caveDarkness > 0f)
		{
			this.caveDarkness = Mathf.Max(0f, this.caveDarkness - Time.deltaTime / this.zoneSettingTransitionTime);
		}
		float num3 = Mathf.Clamp((Mathf.Abs(0.5f - currentDayFraction_Color) - 0.15f) * 5f, 0f, 1f);
		float num4 = 1f - num3;
		float num5 = Mathf.Pow(num3, 6f);
		float num6 = 1f - num5;
		if (this.seaCount + this.waterCount > 0)
		{
			RenderSettings.fogColor = this.waterFogColor;
			RenderSettings.fogDensity = ((this.seaCount > 0) ? this.seaFogDensity : this.waterFogDensity);
		}
		else
		{
			RenderSettings.fogColor = Color.Lerp(this.currZoneSetting.dayFogColor, this.currZoneSetting.nightFogColor, num3);
			RenderSettings.fogDensity = num6 * this.currZoneSetting.dayFogDensity + num5 * this.currZoneSetting.nightFogDensity;
		}
		RenderSettings.ambientLight = Color.Lerp(this.currZoneSetting.dayAmbientColor, this.currZoneSetting.nightAmbientColor, num3);
		float x = 360f * (currentDayFraction_Position - 0.5f);
		foreach (AmbianceDirector.Rotator rotator in this.rotators)
		{
			rotator.transform.rotation = rotator.defaultRot * Quaternion.Euler(x, 0f, 0f);
			foreach (Light light in rotator.childLights)
			{
				float num7 = rotator.isNightLight ? num2 : num;
				num7 *= 1f - this.caveDarkness;
				this.UpdateLightIntensity(light, num7, rotator.defaultIntensities[light]);
			}
		}
		foreach (Light light2 in this.duskedLightDefaultColors.Keys)
		{
			light2.color = Color.Lerp(this.duskLightColor, this.duskedLightDefaultColors[light2], Mathf.Clamp((num4 - 0.5f) * 2f, 0f, 1f));
		}
		RenderSettings.skybox.SetFloat(this.skyboxBlendId, num3);
		RenderSettings.skybox.SetFloat(this.skyboxDaynessId, num4);
		RenderSettings.skybox.SetColor(this.skyboxDayColorId, this.currZoneSetting.daySkyColor);
		RenderSettings.skybox.SetColor(this.skyboxDayHorizonId, this.currZoneSetting.daySkyHorizon);
		RenderSettings.skybox.SetColor(this.skyboxNightColorId, this.currZoneSetting.nightSkyColor);
		RenderSettings.skybox.SetColor(this.skyboxNightHorizonId, this.currZoneSetting.nightSkyHorizon);
		foreach (AmbianceDirector.DaynessListener daynessListener in this.daynessListeners)
		{
			daynessListener.SetDayness(num4);
		}
		if (this.weatherAttach != null && this.timeDir.HasReached(this.worldModel.weatherUntil) && this.weatherEnabled)
		{
			this.worldModel.currWeather = this.RandomWeather();
			this.weatherAttach.SetWeather(this.weatherPrefabs[this.worldModel.currWeather]);
			this.worldModel.weatherUntil = this.timeDir.HoursFromNowOrStart(Randoms.SHARED.GetInRange(this.minWeatherCycleHours, this.maxWeatherCycleHours));
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0001E090 File Offset: 0x0001C290
	private void UpdateLightIntensity(Light light, float intensityMod, float defaultIntensity)
	{
		float value = defaultIntensity * intensityMod;
		light.intensity = Mathf.Clamp(value, (intensityMod == 0f) ? 0f : 0.001f, 1f);
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001E0C8 File Offset: 0x0001C2C8
	private void UpdateZoneSetting()
	{
		if (SRSingleton<GameContext>.Instance == null || SRSingleton<SceneContext>.Instance.Player == null)
		{
			if (this.currZoneSetting == null)
			{
				this.currZoneSetting = this.zoneDict[AmbianceDirector.Zone.DEFAULT].Clone();
			}
			return;
		}
		AmbianceDirector.Zone zone = AmbianceDirector.Zone.DEFAULT;
		if (this.caveTypeCounts.Count > 0)
		{
			using (Dictionary<AmbianceDirector.Zone, int>.KeyCollection.Enumerator enumerator = this.caveTypeCounts.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AmbianceDirector.Zone zone2 = enumerator.Current;
					if (zone2 > zone)
					{
						zone = zone2;
					}
				}
				goto IL_F0;
			}
		}
		if (this.firestormActive)
		{
			zone = AmbianceDirector.Zone.FIRESTORM;
		}
		else
		{
			foreach (Region region in SRSingleton<SceneContext>.Instance.Player.GetComponent<RegionMember>().regions)
			{
				AmbianceDirector.Zone ambianceZone = region.cellDir.ambianceZone;
				if (ambianceZone > zone)
				{
					zone = ambianceZone;
				}
			}
		}
		IL_F0:
		AmbianceDirectorZoneSetting ambianceDirectorZoneSetting = this.zoneDict[zone];
		if (this.currZoneSetting == null)
		{
			this.currZoneSetting = ambianceDirectorZoneSetting.Clone();
			this.transitionToZone = zone;
			this.transitionToZoneTime = 1f;
			return;
		}
		if (zone != this.transitionToZone)
		{
			this.transitionToZoneTime = Mathf.Min(1f, Time.deltaTime / this.zoneSettingTransitionTime);
			this.transitionFromZoneSetting = this.currZoneSetting.Clone();
			this.transitionToZone = zone;
			this.AdjustZoneSettings(ambianceDirectorZoneSetting);
			return;
		}
		if (this.transitionToZoneTime < 1f)
		{
			this.transitionToZoneTime = Mathf.Min(1f, this.transitionToZoneTime + Time.deltaTime / this.zoneSettingTransitionTime);
			this.AdjustZoneSettings(ambianceDirectorZoneSetting);
		}
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001E298 File Offset: 0x0001C498
	private void AdjustZoneSettings(AmbianceDirectorZoneSetting targetZoneSetting)
	{
		if (this.transitionFromZoneSetting != null)
		{
			this.currZoneSetting.dayFogColor = this.AdjustZoneSetting(this.transitionFromZoneSetting.dayFogColor, targetZoneSetting.dayFogColor, this.transitionToZoneTime);
			this.currZoneSetting.dayFogDensity = this.AdjustZoneSetting(this.transitionFromZoneSetting.dayFogDensity, targetZoneSetting.dayFogDensity, this.transitionToZoneTime);
			this.currZoneSetting.dayAmbientColor = this.AdjustZoneSetting(this.transitionFromZoneSetting.dayAmbientColor, targetZoneSetting.dayAmbientColor, this.transitionToZoneTime);
			this.currZoneSetting.nightFogColor = this.AdjustZoneSetting(this.transitionFromZoneSetting.nightFogColor, targetZoneSetting.nightFogColor, this.transitionToZoneTime);
			this.currZoneSetting.nightFogDensity = this.AdjustZoneSetting(this.transitionFromZoneSetting.nightFogDensity, targetZoneSetting.nightFogDensity, this.transitionToZoneTime);
			this.currZoneSetting.nightAmbientColor = this.AdjustZoneSetting(this.transitionFromZoneSetting.nightAmbientColor, targetZoneSetting.nightAmbientColor, this.transitionToZoneTime);
			this.currZoneSetting.daySkyColor = this.AdjustZoneSetting(this.transitionFromZoneSetting.daySkyColor, targetZoneSetting.daySkyColor, this.transitionToZoneTime);
			this.currZoneSetting.daySkyHorizon = this.AdjustZoneSetting(this.transitionFromZoneSetting.daySkyHorizon, targetZoneSetting.daySkyHorizon, this.transitionToZoneTime);
			this.currZoneSetting.nightSkyColor = this.AdjustZoneSetting(this.transitionFromZoneSetting.nightSkyColor, targetZoneSetting.nightSkyColor, this.transitionToZoneTime);
			this.currZoneSetting.nightSkyHorizon = this.AdjustZoneSetting(this.transitionFromZoneSetting.nightSkyHorizon, targetZoneSetting.nightSkyHorizon, this.transitionToZoneTime);
		}
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001E446 File Offset: 0x0001C646
	private Color AdjustZoneSetting(Color origColor, Color targetColor, float t)
	{
		return Color.Lerp(origColor, targetColor, t);
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001E450 File Offset: 0x0001C650
	private float AdjustZoneSetting(float origVal, float targetVal, float t)
	{
		return Mathf.Lerp(origVal, targetVal, t);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001E45A File Offset: 0x0001C65A
	public void RegisterDaynessListener(AmbianceDirector.DaynessListener listener)
	{
		this.daynessListeners.Add(listener);
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001E468 File Offset: 0x0001C668
	public void UnregisterDaynessListener(AmbianceDirector.DaynessListener listener)
	{
		this.daynessListeners.Remove(listener);
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0001E477 File Offset: 0x0001C677
	public void RegisterTimeOfDayRotator(GameObject rotator, bool isNightLight)
	{
		this.rotators.Add(new AmbianceDirector.Rotator(rotator, isNightLight));
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0001E48B File Offset: 0x0001C68B
	public void RegisterDuskedLight(Light light)
	{
		this.duskedLightDefaultColors[light] = light.color;
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0001E49F File Offset: 0x0001C69F
	public void EnterCave(AmbianceDirector.Zone caveZone)
	{
		this.caveTypeCounts[caveZone] = this.caveTypeCounts.Get(caveZone) + 1;
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0001E4BB File Offset: 0x0001C6BB
	public void ExitCave(AmbianceDirector.Zone caveZone)
	{
		this.caveTypeCounts[caveZone] = this.caveTypeCounts.Get(caveZone) - 1;
		if (this.caveTypeCounts[caveZone] <= 0)
		{
			this.caveTypeCounts.Remove(caveZone);
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001E4F3 File Offset: 0x0001C6F3
	public void SetFirestormActive(bool active)
	{
		this.firestormActive = active;
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001E4FC File Offset: 0x0001C6FC
	public void EnterWater()
	{
		this.waterCount++;
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001E50C File Offset: 0x0001C70C
	public void ExitWater()
	{
		this.waterCount--;
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001E51C File Offset: 0x0001C71C
	public void EnterSea()
	{
		this.seaCount++;
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001E52C File Offset: 0x0001C72C
	public void ExitSea()
	{
		this.seaCount--;
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001E53C File Offset: 0x0001C73C
	public bool IsInWater()
	{
		return this.waterCount > 0;
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001E547 File Offset: 0x0001C747
	public void ExitAllLiquid()
	{
		this.waterCount = 0;
		this.seaCount = 0;
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001E558 File Offset: 0x0001C758
	public float PrecipitationRate()
	{
		AmbianceDirector.Weather currWeather = this.worldModel.currWeather;
		if (currWeather == AmbianceDirector.Weather.RAIN)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0001E580 File Offset: 0x0001C780
	private void InitSkybox()
	{
		this.rotators.Clear();
		if (RenderSettings.skybox != null)
		{
			RenderSettings.skybox = new Material(RenderSettings.skybox);
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x0001E5A9 File Offset: 0x0001C7A9
	private AmbianceDirector.Weather RandomWeather()
	{
		return Randoms.SHARED.Pick<AmbianceDirector.Weather>((AmbianceDirector.Weather[])Enum.GetValues(typeof(AmbianceDirector.Weather)));
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x0001E5C9 File Offset: 0x0001C7C9
	public void Register(AmbianceDirector.TimeOfDay instance)
	{
		this.timeOfDay.Add(instance);
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0001E5D7 File Offset: 0x0001C7D7
	public void Deregister(AmbianceDirector.TimeOfDay instance)
	{
		this.timeOfDay.Remove(instance);
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x0001E5E6 File Offset: 0x0001C7E6
	public float GetCurrentDayFraction_Position()
	{
		return this.timeDir.CurrDayFraction();
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0001E5E6 File Offset: 0x0001C7E6
	public float GetCurrentDayFraction_Color()
	{
		return this.timeDir.CurrDayFraction();
	}

	// Token: 0x040004B3 RID: 1203
	public static AmbianceDirector.OnAwakeDelegate onAwakeDelegate;

	// Token: 0x040004B4 RID: 1204
	public AmbianceDirectorZoneSetting[] zoneSettings;

	// Token: 0x040004B5 RID: 1205
	public float zoneSettingTransitionTime = 1f;

	// Token: 0x040004B6 RID: 1206
	[Tooltip("The fog density while the camera is in the water.")]
	public float waterFogDensity = 0.24f;

	// Token: 0x040004B7 RID: 1207
	[Tooltip("The fog density while the camera is in the jelly sea.")]
	public float seaFogDensity = 0.36f;

	// Token: 0x040004B8 RID: 1208
	[Tooltip("The fog color while the camera is in the water or sea.")]
	public Color waterFogColor = Color.black;

	// Token: 0x040004B9 RID: 1209
	[Tooltip("The color to shift lights to at dusk time.")]
	public Color duskLightColor;

	// Token: 0x040004BA RID: 1210
	public AmbianceDirector.WeatherEntry[] weatherEntries;

	// Token: 0x040004BB RID: 1211
	[Tooltip("Whether weather is active.")]
	public bool weatherEnabled = true;

	// Token: 0x040004BC RID: 1212
	[Tooltip("Min hours before reselecting a weather type.")]
	public float minWeatherCycleHours = 3f;

	// Token: 0x040004BD RID: 1213
	[Tooltip("Max hours before reselecting a weather type.")]
	public float maxWeatherCycleHours = 12f;

	// Token: 0x040004BE RID: 1214
	private float caveDarkness;

	// Token: 0x040004BF RID: 1215
	private Dictionary<AmbianceDirector.Zone, int> caveTypeCounts = new Dictionary<AmbianceDirector.Zone, int>();

	// Token: 0x040004C0 RID: 1216
	private bool firestormActive;

	// Token: 0x040004C1 RID: 1217
	private int waterCount;

	// Token: 0x040004C2 RID: 1218
	private int seaCount;

	// Token: 0x040004C3 RID: 1219
	private TimeDirector timeDir;

	// Token: 0x040004C4 RID: 1220
	private Dictionary<AmbianceDirector.Weather, GameObject> weatherPrefabs = new Dictionary<AmbianceDirector.Weather, GameObject>();

	// Token: 0x040004C5 RID: 1221
	private WeatherEffectAttachment weatherAttach;

	// Token: 0x040004C6 RID: 1222
	private Dictionary<Light, Color> duskedLightDefaultColors = new Dictionary<Light, Color>();

	// Token: 0x040004C7 RID: 1223
	private Dictionary<AmbianceDirector.Zone, AmbianceDirectorZoneSetting> zoneDict;

	// Token: 0x040004C8 RID: 1224
	private List<AmbianceDirector.DaynessListener> daynessListeners = new List<AmbianceDirector.DaynessListener>();

	// Token: 0x040004C9 RID: 1225
	private List<AmbianceDirector.TimeOfDay> timeOfDay = new List<AmbianceDirector.TimeOfDay>();

	// Token: 0x040004CA RID: 1226
	private AmbianceDirectorZoneSetting currZoneSetting;

	// Token: 0x040004CB RID: 1227
	private AmbianceDirectorZoneSetting transitionFromZoneSetting;

	// Token: 0x040004CC RID: 1228
	private AmbianceDirector.Zone transitionToZone;

	// Token: 0x040004CD RID: 1229
	private float transitionToZoneTime;

	// Token: 0x040004CE RID: 1230
	private const float MINS_PER_FADE = 5f;

	// Token: 0x040004CF RID: 1231
	private const float LIGHT_FADE_SPEED = 288f;

	// Token: 0x040004D0 RID: 1232
	private int skyboxBlendId;

	// Token: 0x040004D1 RID: 1233
	private int skyboxDaynessId;

	// Token: 0x040004D2 RID: 1234
	private int skyboxDayColorId;

	// Token: 0x040004D3 RID: 1235
	private int skyboxDayHorizonId;

	// Token: 0x040004D4 RID: 1236
	private int skyboxNightColorId;

	// Token: 0x040004D5 RID: 1237
	private int skyboxNightHorizonId;

	// Token: 0x040004D6 RID: 1238
	private List<AmbianceDirector.Rotator> rotators = new List<AmbianceDirector.Rotator>();

	// Token: 0x040004D7 RID: 1239
	private WorldModel worldModel;

	// Token: 0x020000CB RID: 203
	// (Invoke) Token: 0x060004B2 RID: 1202
	public delegate void OnAwakeDelegate(AmbianceDirector ambianceDir);

	// Token: 0x020000CC RID: 204
	public interface DaynessListener
	{
		// Token: 0x060004B5 RID: 1205
		void SetDayness(float dayness);
	}

	// Token: 0x020000CD RID: 205
	public interface TimeOfDay
	{
		// Token: 0x060004B6 RID: 1206
		float GetCurrentDayFraction_Position();

		// Token: 0x060004B7 RID: 1207
		float GetCurrentDayFraction_Color();
	}

	// Token: 0x020000CE RID: 206
	public enum Weather
	{
		// Token: 0x040004D9 RID: 1241
		NONE,
		// Token: 0x040004DA RID: 1242
		RAIN
	}

	// Token: 0x020000CF RID: 207
	public enum Zone
	{
		// Token: 0x040004DC RID: 1244
		DEFAULT,
		// Token: 0x040004DD RID: 1245
		QUARRY,
		// Token: 0x040004DE RID: 1246
		MOSS,
		// Token: 0x040004DF RID: 1247
		DESERT,
		// Token: 0x040004E0 RID: 1248
		RUINS,
		// Token: 0x040004E1 RID: 1249
		WILDS,
		// Token: 0x040004E2 RID: 1250
		OGDEN_RANCH,
		// Token: 0x040004E3 RID: 1251
		VALLEY,
		// Token: 0x040004E4 RID: 1252
		MOCHI_RANCH,
		// Token: 0x040004E5 RID: 1253
		SLIMULATIONS,
		// Token: 0x040004E6 RID: 1254
		VIKTOR_LAB,
		// Token: 0x040004E7 RID: 1255
		AUX1 = 1000,
		// Token: 0x040004E8 RID: 1256
		AUX2,
		// Token: 0x040004E9 RID: 1257
		FIRESTORM = 2000,
		// Token: 0x040004EA RID: 1258
		CAVE = 10000,
		// Token: 0x040004EB RID: 1259
		CAVE_VOLCANIC
	}

	// Token: 0x020000D0 RID: 208
	[Serializable]
	public class WeatherEntry
	{
		// Token: 0x040004EC RID: 1260
		public AmbianceDirector.Weather weather;

		// Token: 0x040004ED RID: 1261
		public GameObject prefab;
	}

	// Token: 0x020000D1 RID: 209
	private class Rotator
	{
		// Token: 0x060004B9 RID: 1209 RVA: 0x0001E694 File Offset: 0x0001C894
		public Rotator(GameObject gameObject, bool isNightLight)
		{
			this.gameObject = gameObject;
			this.defaultRot = gameObject.transform.rotation;
			this.isNightLight = isNightLight;
			this.transform = gameObject.transform;
			this.childLights = new List<Light>();
			foreach (Light light in gameObject.GetComponentsInChildren<Light>(true))
			{
				if (light != null)
				{
					this.childLights.Add(light);
					this.defaultIntensities[light] = light.intensity;
				}
			}
		}

		// Token: 0x040004EE RID: 1262
		public GameObject gameObject;

		// Token: 0x040004EF RID: 1263
		public Quaternion defaultRot;

		// Token: 0x040004F0 RID: 1264
		public Dictionary<Light, float> defaultIntensities = new Dictionary<Light, float>();

		// Token: 0x040004F1 RID: 1265
		public bool isNightLight;

		// Token: 0x040004F2 RID: 1266
		public Transform transform;

		// Token: 0x040004F3 RID: 1267
		public List<Light> childLights;
	}
}
