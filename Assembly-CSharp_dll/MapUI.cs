using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DLCPackage;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005C4 RID: 1476
public class MapUI : BaseUI
{
	// Token: 0x06001EAB RID: 7851 RVA: 0x00073F9C File Offset: 0x0007219C
	public override void Awake()
	{
		base.Awake();
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
		this.PrepareCoefficients();
		this.scrollRect.onZoom = new MapScrollRect.OnZoomEvent(this.ScaleMarkersOnZoom);
		this.mapUIRectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06001EAC RID: 7852 RVA: 0x00073FE8 File Offset: 0x000721E8
	public override void OnBundlesAvailable(MessageDirector msgDir)
	{
		base.OnBundlesAvailable(msgDir);
		this.pediaBundle = msgDir.GetBundle("pedia");
	}

	// Token: 0x06001EAD RID: 7853 RVA: 0x00074002 File Offset: 0x00072202
	public void AddZoneToReveal(ZoneDirector.Zone zoneId)
	{
		this.zonesToReveal.Add(zoneId);
	}

	// Token: 0x06001EAE RID: 7854 RVA: 0x00074011 File Offset: 0x00072211
	public void OpenMap()
	{
		base.Play(this.openCue);
		this.RefreshMap();
	}

	// Token: 0x06001EAF RID: 7855 RVA: 0x00074028 File Offset: 0x00072228
	public override void Update()
	{
		base.Update();
		if (SRInput.PauseActions.menuTabLeft.IsPressed)
		{
			this.scrollRect.ZoomOut();
		}
		else if (SRInput.PauseActions.menuTabRight.IsPressed)
		{
			this.scrollRect.ZoomIn();
		}
		this.scrollRect.Scroll(new Vector2(SRInput.PauseActions.menuLeft.Value - SRInput.PauseActions.menuRight.Value, SRInput.PauseActions.menuUp.Value - SRInput.PauseActions.menuDown.Value) * 240f * Time.unscaledDeltaTime);
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000740D7 File Offset: 0x000722D7
	public override void OnDestroy()
	{
		this.scrollRect.onZoom = null;
		base.OnDestroy();
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000740EB File Offset: 0x000722EB
	public void RegisterObject(DisplayOnMap displayOnMap)
	{
		if (!this.mappableObjects.Contains(displayOnMap))
		{
			this.mappableObjects.Add(displayOnMap);
		}
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x00074107 File Offset: 0x00072307
	public void DeregisterObject(DisplayOnMap displayOnMap)
	{
		this.mappableObjects.Remove(displayOnMap);
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x00074118 File Offset: 0x00072318
	private void RefreshMap()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.mapUIRectTransform);
		float currentZoom = this.scrollRect.GetCurrentZoom();
		this.scrollRect.ResetToDefaultZoom();
		this.UpdateZoneFog();
		MapMarker mapMarker = null;
		foreach (DisplayOnMap displayOnMap in this.mappableObjects)
		{
			displayOnMap.Refresh();
			if (displayOnMap.ShowOnMap())
			{
				MapMarker marker = displayOnMap.GetMarker();
				marker.transform.SetParent(this.mapMarkerSection.gameObject.transform, false);
				marker.transform.localPosition = Vector3.zero;
				marker.gameObject.SetActive(true);
				RegionRegistry.RegionSetId regionSetId = displayOnMap.GetRegionSetId();
				Vector4 coefficients;
				Vector2 minPoint;
				Vector2 maxPoint;
				if (regionSetId == RegionRegistry.RegionSetId.DESERT)
				{
					coefficients = this.desertCoefficients;
					minPoint = this.desertMarkerPositionMin;
					maxPoint = this.desertMarkerPositionMax;
				}
				else
				{
					coefficients = this.mainCoefficients;
					minPoint = this.worldMarkerPositionMin;
					maxPoint = this.worldMarkerPositionMax;
				}
				marker.SetAnchoredPosition(this.GetMapPosClamped(displayOnMap.GetCurrentPosition(), coefficients, minPoint, maxPoint));
				if (displayOnMap is PlayerDisplayOnMap)
				{
					this.UpdatePlayerMarker(displayOnMap as PlayerDisplayOnMap, marker, regionSetId);
					mapMarker = marker;
				}
			}
			else
			{
				displayOnMap.GetMarker().gameObject.SetActive(false);
			}
		}
		this.scrollRect.ZoomTo(currentZoom);
		Canvas.ForceUpdateCanvases();
		this.CenterMapOnMarker(mapMarker.GetLocalPosition());
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x00074290 File Offset: 0x00072490
	private void UpdatePlayerMarker(PlayerDisplayOnMap mappableObject, MapMarker marker, RegionRegistry.RegionSetId regionId)
	{
		ZoneDirector.Zone zoneId = mappableObject.GetZoneId();
		this.UpdateZoneItemCounts(zoneId);
		float num;
		if (regionId == RegionRegistry.RegionSetId.DESERT)
		{
			num = this.desertRotationAdjustment;
		}
		else
		{
			num = this.mainRotationAdjustment;
		}
		if (mappableObject.IsInUnknownArea() || zoneId == ZoneDirector.Zone.NONE || !ZoneDirector.zonePediaIdLookup.ContainsKey(zoneId))
		{
			this.currentLocationNameText.text = this.uiBundle.Xlate("t.unknown_location");
			if (this.disruptionOverlay != null)
			{
				this.disruptionOverlay.SetActive(true);
				this.disruptionCuePlaying = SECTR_AudioSystem.Play(this.disruptionCue, base.transform.position, true);
				return;
			}
		}
		else
		{
			Vector3 eulerAngles = mappableObject.GetCurrentRotation().eulerAngles;
			marker.Rotate(Quaternion.Euler(eulerAngles.x + num, eulerAngles.y, eulerAngles.z));
			this.currentLocationNameText.text = this.pediaBundle.Xlate(string.Format("t.{0}", ZoneDirector.zonePediaIdLookup[zoneId].ToString().ToLowerInvariant()));
			this.disruptionOverlay.SetActive(false);
		}
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000743B0 File Offset: 0x000725B0
	private void UpdateZoneFog()
	{
		foreach (MapUI.ZoneFogEntry zoneFogEntry in this.zoneFogEntries)
		{
			if (this.zonesToReveal.Contains(zoneFogEntry.zoneId))
			{
				this.RevealZone(zoneFogEntry);
			}
			else if (this.playerState.HasUnlockedMap(zoneFogEntry.zoneId))
			{
				zoneFogEntry.fogObject.gameObject.SetActive(false);
			}
			else
			{
				zoneFogEntry.fogObject.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x00074450 File Offset: 0x00072650
	private void RevealZone(MapUI.ZoneFogEntry zoneFogEntry)
	{
		base.Play(this.fogClearCue);
		zoneFogEntry.fogObject.DOFade(0f, 2f).SetUpdate(true);
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x0007447C File Offset: 0x0007267C
	private void UpdateZoneItemCounts(ZoneDirector.Zone zoneId)
	{
		this.treasurePodsInZone = 0;
		this.treasurePodsOpenedInZone = 0;
		this.keysInZone = 0;
		this.keysCollectedInZone = 0;
		bool flag = SRSingleton<GameContext>.Instance.DLCDirector.IsPackageInstalledAndEnabled(Id.SECRET_STYLE);
		foreach (KeyValuePair<string, TreasurePodModel> keyValuePair in SRSingleton<SceneContext>.Instance.GameModel.AllPods())
		{
			if (!DLCDirector.SECRET_STYLE_TREASURE_PODS.Contains(keyValuePair.Key) || flag)
			{
				TreasurePodModel value = keyValuePair.Value;
				if (value.GetZoneId() == zoneId)
				{
					this.treasurePodsInZone++;
					if (value.state == TreasurePod.State.OPEN)
					{
						this.treasurePodsOpenedInZone++;
					}
				}
			}
		}
		foreach (GordoModel gordoModel in SRSingleton<SceneContext>.Instance.GameModel.AllGordos().Values)
		{
			if (gordoModel.GetZoneId() == zoneId && gordoModel.DropsKey())
			{
				this.keysInZone++;
				if (gordoModel.HasPopped())
				{
					this.keysCollectedInZone++;
				}
			}
		}
		using (List<SlimeKey>.Enumerator enumerator3 = SlimeKey.allKeys.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				if (enumerator3.Current.IsKeyInZone(zoneId))
				{
					this.keysCollectedInZone--;
				}
			}
		}
		bool flag2 = this.playerState.HasUnlockedMap(zoneId);
		if (this.keysInZone > 0 && flag2)
		{
			this.keyCountLine.SetActive(true);
			this.keyCount.text = string.Format("{0}/{1}", this.keysCollectedInZone, this.keysInZone);
		}
		else
		{
			this.keyCountLine.SetActive(false);
		}
		if (this.treasurePodsInZone > 0 && flag2)
		{
			this.treasurePodCountLine.SetActive(true);
			this.treasurePodCount.text = string.Format("{0}/{1}", this.treasurePodsOpenedInZone, this.treasurePodsInZone);
			return;
		}
		this.treasurePodCountLine.SetActive(false);
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000746D4 File Offset: 0x000728D4
	private void CenterMapOnMarker(Vector3 position)
	{
		Vector2 v = -position * this.scrollRect.content.localScale.x;
		this.scrollRect.content.localPosition = v;
		this.scrollRect.Scroll(Vector2.zero);
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x0007472D File Offset: 0x0007292D
	private float ApplyCoefficients(Vector2 coefficients, float xVal)
	{
		return coefficients.x * xVal + coefficients.y;
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x0007473E File Offset: 0x0007293E
	public override void Close()
	{
		base.Play(this.closeCue);
		this.zonesToReveal.Clear();
		this.disruptionCuePlaying.Stop(true);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x00074770 File Offset: 0x00072970
	public void ScaleMarkersOnZoom(float zoomLevel)
	{
		float num = zoomLevel * this.markerZoomSlopeOffset.x + this.markerZoomSlopeOffset.y;
		float num2 = zoomLevel * this.playerMarkerZoomSlopeOffset.x + this.playerMarkerZoomSlopeOffset.y;
		foreach (DisplayOnMap displayOnMap in this.mappableObjects)
		{
			if (displayOnMap is PlayerDisplayOnMap)
			{
				displayOnMap.GetMarker().SetSize(num2, num2);
			}
			else
			{
				displayOnMap.GetMarker().SetSize(num, num);
			}
		}
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x00074814 File Offset: 0x00072A14
	private void PrepareCoefficients()
	{
		this.mainCoefficients = this.GetWorldToMapCoefficients(this.mainWorldPoint2, this.mainWorldPoint1, this.mainMapPoint2, this.mainMapPoint1);
		this.desertCoefficients = this.GetWorldToMapCoefficients(this.desertWorldPoint1, this.desertWorldPoint2, this.desertMapPoint1, this.desertMapPoint2);
		this.markerZoomSlopeOffset = this.GetSlopeAndOffset(this.markerZoomInToSizePoint, this.markerZoomOutToSizePoint);
		this.playerMarkerZoomSlopeOffset = this.GetSlopeAndOffset(this.playerMarkerZoomInToSizePoint, this.playerMarkerZoomOutToSizePoint);
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x0007489C File Offset: 0x00072A9C
	private Vector4 GetWorldToMapCoefficients(Vector2 worldPoint1, Vector2 worldPoint2, Vector2 mapPoint1, Vector2 mapPoint2)
	{
		Vector2 slopeAndOffset = this.GetSlopeAndOffset(new Vector2(worldPoint2.x, mapPoint2.x), new Vector2(worldPoint1.x, mapPoint1.x));
		Vector2 slopeAndOffset2 = this.GetSlopeAndOffset(new Vector2(worldPoint2.y, mapPoint2.y), new Vector2(worldPoint1.y, mapPoint1.y));
		return new Vector4(slopeAndOffset.x, slopeAndOffset.y, slopeAndOffset2.x, slopeAndOffset2.y);
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x0007491C File Offset: 0x00072B1C
	private Vector2 GetSlopeAndOffset(Vector2 point1, Vector2 point2)
	{
		float num = (point2.y - point1.y) / (point2.x - point1.x);
		float y = point2.y - point2.x * num;
		return new Vector2(num, y);
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x0007495C File Offset: 0x00072B5C
	private Vector2 GetMapPos(Vector3 playerPosition, Vector4 coefficients)
	{
		return new Vector2(playerPosition.x * coefficients.x + coefficients.y, playerPosition.z * coefficients.z + coefficients.w);
	}

	// Token: 0x06001EC0 RID: 7872 RVA: 0x0007498C File Offset: 0x00072B8C
	private Vector2 GetMapPosClamped(Vector3 playerPosition, Vector4 coefficients, Vector2 minPoint, Vector2 maxPoint)
	{
		Vector2 mapPos = this.GetMapPos(playerPosition, coefficients);
		return new Vector2(Mathf.Clamp(mapPos.x, minPoint.x, maxPoint.x), Mathf.Clamp(mapPos.y, maxPoint.y, minPoint.y));
	}

	// Token: 0x04001DA8 RID: 7592
	public MapScrollRect scrollRect;

	// Token: 0x04001DA9 RID: 7593
	public GameObject mapContentArea;

	// Token: 0x04001DAA RID: 7594
	public GameObject mapMarkerSection;

	// Token: 0x04001DAB RID: 7595
	public GameObject disruptionOverlay;

	// Token: 0x04001DAC RID: 7596
	public List<MapUI.ZoneFogEntry> zoneFogEntries;

	// Token: 0x04001DAD RID: 7597
	public TMP_Text currentLocationNameText;

	// Token: 0x04001DAE RID: 7598
	public TMP_Text keyCount;

	// Token: 0x04001DAF RID: 7599
	public TMP_Text treasurePodCount;

	// Token: 0x04001DB0 RID: 7600
	public GameObject keyCountLine;

	// Token: 0x04001DB1 RID: 7601
	public GameObject treasurePodCountLine;

	// Token: 0x04001DB2 RID: 7602
	public SECTR_AudioCue openCue;

	// Token: 0x04001DB3 RID: 7603
	public SECTR_AudioCue closeCue;

	// Token: 0x04001DB4 RID: 7604
	public SECTR_AudioCue fogClearCue;

	// Token: 0x04001DB5 RID: 7605
	public SECTR_AudioCue disruptionCue;

	// Token: 0x04001DB6 RID: 7606
	private List<DisplayOnMap> mappableObjects = new List<DisplayOnMap>();

	// Token: 0x04001DB7 RID: 7607
	private PlayerState playerState;

	// Token: 0x04001DB8 RID: 7608
	private MessageBundle pediaBundle;

	// Token: 0x04001DB9 RID: 7609
	private RectTransform mapUIRectTransform;

	// Token: 0x04001DBA RID: 7610
	private SECTR_AudioCueInstance disruptionCuePlaying;

	// Token: 0x04001DBB RID: 7611
	private Vector2 mainMapPoint1 = new Vector2(2468f, -2532f);

	// Token: 0x04001DBC RID: 7612
	private Vector2 mainMapPoint2 = new Vector2(2655f, -2741f);

	// Token: 0x04001DBD RID: 7613
	private Vector2 mainWorldPoint1 = new Vector2(89.3f, -144.5f);

	// Token: 0x04001DBE RID: 7614
	private Vector2 mainWorldPoint2 = new Vector2(193.8f, -260.8f);

	// Token: 0x04001DBF RID: 7615
	private Vector2 desertMapPoint1 = new Vector2(4219f, -2497f);

	// Token: 0x04001DC0 RID: 7616
	private Vector2 desertMapPoint2 = new Vector2(4433f, -1685f);

	// Token: 0x04001DC1 RID: 7617
	private Vector2 desertWorldPoint1 = new Vector2(119.4345f, 918.0937f);

	// Token: 0x04001DC2 RID: 7618
	private Vector2 desertWorldPoint2 = new Vector2(-12.69382f, 416.4283f);

	// Token: 0x04001DC3 RID: 7619
	private Vector4 mainCoefficients;

	// Token: 0x04001DC4 RID: 7620
	private Vector4 desertCoefficients;

	// Token: 0x04001DC5 RID: 7621
	private float mainRotationAdjustment;

	// Token: 0x04001DC6 RID: 7622
	private float desertRotationAdjustment = 180f;

	// Token: 0x04001DC7 RID: 7623
	private Vector2 markerZoomInToSizePoint = new Vector2(2f, 30f);

	// Token: 0x04001DC8 RID: 7624
	private Vector2 markerZoomOutToSizePoint = new Vector2(0.55f, 50f);

	// Token: 0x04001DC9 RID: 7625
	private Vector2 markerZoomSlopeOffset;

	// Token: 0x04001DCA RID: 7626
	private Vector2 playerMarkerZoomInToSizePoint = new Vector2(2f, 50f);

	// Token: 0x04001DCB RID: 7627
	private Vector2 playerMarkerZoomOutToSizePoint = new Vector2(0.55f, 70f);

	// Token: 0x04001DCC RID: 7628
	private Vector2 playerMarkerZoomSlopeOffset;

	// Token: 0x04001DCD RID: 7629
	private Vector2 worldMarkerPositionMin = new Vector2(1000f, -180f);

	// Token: 0x04001DCE RID: 7630
	private Vector2 worldMarkerPositionMax = new Vector2(3548f, -3050f);

	// Token: 0x04001DCF RID: 7631
	private Vector2 desertMarkerPositionMin = new Vector2(2236f, -372f);

	// Token: 0x04001DD0 RID: 7632
	private Vector2 desertMarkerPositionMax = new Vector2(4960f, -2680f);

	// Token: 0x04001DD1 RID: 7633
	private int keysInZone;

	// Token: 0x04001DD2 RID: 7634
	private int keysCollectedInZone;

	// Token: 0x04001DD3 RID: 7635
	private int treasurePodsInZone;

	// Token: 0x04001DD4 RID: 7636
	private int treasurePodsOpenedInZone;

	// Token: 0x04001DD5 RID: 7637
	private HashSet<ZoneDirector.Zone> zonesToReveal = new HashSet<ZoneDirector.Zone>();

	// Token: 0x04001DD6 RID: 7638
	private const float ZONE_REVEAL_TIME = 2f;

	// Token: 0x04001DD7 RID: 7639
	private const float SCROLL_SPEED = 240f;

	// Token: 0x020005C5 RID: 1477
	[Serializable]
	public class ZoneFogEntry
	{
		// Token: 0x04001DD8 RID: 7640
		public ZoneDirector.Zone zoneId;

		// Token: 0x04001DD9 RID: 7641
		public CanvasGroup fogObject;
	}
}
