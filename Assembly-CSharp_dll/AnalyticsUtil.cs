using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000679 RID: 1657
public static class AnalyticsUtil
{
	// Token: 0x0600224E RID: 8782 RVA: 0x00084AA8 File Offset: 0x00082CA8
	public static void CustomEvent(string eventName, IDictionary<string, object> customEventData = null, bool includeDefaultEventData = true)
	{
		Dictionary<string, object> eventData = (customEventData != null) ? new Dictionary<string, object>(customEventData) : new Dictionary<string, object>();
		if (includeDefaultEventData)
		{
			foreach (KeyValuePair<string, object> keyValuePair in AnalyticsUtil.GetDefaultEventData())
			{
				if (!eventData.ContainsKey(keyValuePair.Key))
				{
					eventData[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}
		AnalyticsUtil.Listeners.ForEach(delegate(AnalyticsUtil.IListener instance)
		{
			instance.CustomEvent(eventName, eventData);
		});
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x00084B5C File Offset: 0x00082D5C
	private static Dictionary<string, object> GetDefaultEventData()
	{
		try
		{
			return new Dictionary<string, object>
			{
				{
					"Game.Id",
					SRSingleton<GameContext>.Instance.AutoSaveDirector.SavedGame.GetName()
				},
				{
					"Game.Mode",
					SRSingleton<SceneContext>.Instance.GameModel.currGameMode
				},
				{
					"Player.Position",
					AnalyticsUtil.GetEventData(SRSingleton<SceneContext>.Instance.Player.transform.position)
				},
				{
					"Player.Region",
					SRSingleton<SceneContext>.Instance.RegionRegistry.GetCurrentRegionSetId()
				},
				{
					"Player.Zone",
					SRSingleton<SceneContext>.Instance.Player.GetComponent<PlayerZoneTracker>().GetCurrentZone()
				},
				{
					"Time.WorldTime",
					AnalyticsUtil.GetEventData(SRSingleton<SceneContext>.Instance.TimeDirector.WorldTime(), 0)
				}
			};
		}
		catch (Exception ex)
		{
			Log.Warning("Failed to get default analytics event metadata.", new object[]
			{
				"exception",
				ex
			});
		}
		return new Dictionary<string, object>();
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x00084C6C File Offset: 0x00082E6C
	public static string GetEventData(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return "null";
		}
		Identifiable componentInParent = gameObject.GetComponentInParent<Identifiable>();
		if (componentInParent != null)
		{
			return componentInParent.id.ToString();
		}
		return gameObject.name;
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x00084CB0 File Offset: 0x00082EB0
	public static string GetEventData(Vector3 vector3)
	{
		return string.Format("{{\"x\":{0},\"y\":{1},\"z\":{2}}}", AnalyticsUtil.GetEventData(vector3.x, 2), AnalyticsUtil.GetEventData(vector3.y, 2), AnalyticsUtil.GetEventData(vector3.z, 2));
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x00084CE0 File Offset: 0x00082EE0
	public static string GetEventData(double value, int decimals = 2)
	{
		return value.ToString(string.Format("F{0}", decimals));
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x00084CF9 File Offset: 0x00082EF9
	public static string GetEventData(float value, int decimals = 2)
	{
		return value.ToString(string.Format("F{0}", decimals));
	}

	// Token: 0x0400222D RID: 8749
	public const string EVENT_SESSION_ENDED = "SessionEnded";

	// Token: 0x0400222E RID: 8750
	public const string NULL = "null";

	// Token: 0x0400222F RID: 8751
	private static List<AnalyticsUtil.IListener> Listeners = new List<AnalyticsUtil.IListener>();

	// Token: 0x0200067A RID: 1658
	private interface IListener
	{
		// Token: 0x06002255 RID: 8789
		void CustomEvent(string eventName, IDictionary<string, object> eventData);
	}
}
