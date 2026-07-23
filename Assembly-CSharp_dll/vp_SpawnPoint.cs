using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000861 RID: 2145
[Serializable]
public class vp_SpawnPoint : MonoBehaviour
{
	// Token: 0x170002D9 RID: 729
	// (get) Token: 0x06002D41 RID: 11585 RVA: 0x000AC070 File Offset: 0x000AA270
	public static List<vp_SpawnPoint> SpawnPoints
	{
		get
		{
			if (vp_SpawnPoint.m_SpawnPoints == null)
			{
				vp_SpawnPoint.m_SpawnPoints = new List<vp_SpawnPoint>(UnityEngine.Object.FindObjectsOfType(typeof(vp_SpawnPoint)) as vp_SpawnPoint[]);
			}
			return vp_SpawnPoint.m_SpawnPoints;
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000AC09C File Offset: 0x000AA29C
	public static vp_Placement GetRandomPlacement()
	{
		return vp_SpawnPoint.GetRandomPlacement(0f, null);
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000AC0A9 File Offset: 0x000AA2A9
	public static vp_Placement GetRandomPlacement(float physicsCheckRadius)
	{
		return vp_SpawnPoint.GetRandomPlacement(physicsCheckRadius, null);
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000AC0B2 File Offset: 0x000AA2B2
	public static vp_Placement GetRandomPlacement(string tag)
	{
		return vp_SpawnPoint.GetRandomPlacement(0f, tag);
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000AC0C0 File Offset: 0x000AA2C0
	public static vp_Placement GetRandomPlacement(float physicsCheckRadius, string tag)
	{
		if (vp_SpawnPoint.SpawnPoints == null || vp_SpawnPoint.SpawnPoints.Count < 1)
		{
			return null;
		}
		vp_SpawnPoint randomSpawnPoint;
		if (string.IsNullOrEmpty(tag))
		{
			randomSpawnPoint = vp_SpawnPoint.GetRandomSpawnPoint();
		}
		else
		{
			randomSpawnPoint = vp_SpawnPoint.GetRandomSpawnPoint(tag);
			if (randomSpawnPoint == null)
			{
				randomSpawnPoint = vp_SpawnPoint.GetRandomSpawnPoint();
				Debug.LogWarning("Warning (vp_SpawnPoint --> GetRandomPlacement) Could not find a spawnpoint tagged '" + tag + "'. Falling back to 'any random spawnpoint'.");
			}
		}
		if (randomSpawnPoint == null)
		{
			Debug.LogError("Error (vp_SpawnPoint --> GetRandomPlacement) Could not find a spawnpoint" + ((!string.IsNullOrEmpty(tag)) ? (" tagged '" + tag + "'") : ".") + " Reverting to world origin.");
			return null;
		}
		vp_Placement vp_Placement = new vp_Placement();
		vp_Placement.Position = randomSpawnPoint.transform.position;
		if (randomSpawnPoint.Radius > 0f)
		{
			Vector3 vector = UnityEngine.Random.insideUnitSphere * randomSpawnPoint.Radius;
			vp_Placement vp_Placement2 = vp_Placement;
			vp_Placement2.Position.x = vp_Placement2.Position.x + vector.x;
			vp_Placement vp_Placement3 = vp_Placement;
			vp_Placement3.Position.z = vp_Placement3.Position.z + vector.z;
		}
		if (physicsCheckRadius != 0f)
		{
			if (!vp_Placement.AdjustPosition(vp_Placement, physicsCheckRadius, 1000))
			{
				return null;
			}
			vp_Placement.SnapToGround(vp_Placement, physicsCheckRadius, randomSpawnPoint.GroundSnapThreshold);
		}
		if (randomSpawnPoint.RandomDirection)
		{
			vp_Placement.Rotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
		}
		else
		{
			vp_Placement.Rotation = randomSpawnPoint.transform.rotation;
		}
		return vp_Placement;
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000AC222 File Offset: 0x000AA422
	public static vp_SpawnPoint GetRandomSpawnPoint()
	{
		if (vp_SpawnPoint.SpawnPoints.Count < 1)
		{
			return null;
		}
		return vp_SpawnPoint.SpawnPoints[UnityEngine.Random.Range(0, vp_SpawnPoint.SpawnPoints.Count)];
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000AC250 File Offset: 0x000AA450
	public static vp_SpawnPoint GetRandomSpawnPoint(string tag)
	{
		vp_SpawnPoint.m_MatchingSpawnPoints.Clear();
		for (int i = 0; i < vp_SpawnPoint.SpawnPoints.Count; i++)
		{
			if (vp_SpawnPoint.m_SpawnPoints[i].tag == tag)
			{
				vp_SpawnPoint.m_MatchingSpawnPoints.Add(vp_SpawnPoint.m_SpawnPoints[i]);
			}
		}
		if (vp_SpawnPoint.m_MatchingSpawnPoints.Count < 1)
		{
			return null;
		}
		if (vp_SpawnPoint.m_MatchingSpawnPoints.Count == 1)
		{
			return vp_SpawnPoint.m_MatchingSpawnPoints[0];
		}
		return vp_SpawnPoint.m_MatchingSpawnPoints[UnityEngine.Random.Range(0, vp_SpawnPoint.m_MatchingSpawnPoints.Count)];
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000AC2EB File Offset: 0x000AA4EB
	private void Awake()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000AC2FE File Offset: 0x000AA4FE
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		vp_SpawnPoint.m_SpawnPoints = null;
	}

	// Token: 0x04002B59 RID: 11097
	public bool RandomDirection;

	// Token: 0x04002B5A RID: 11098
	public float Radius;

	// Token: 0x04002B5B RID: 11099
	public float GroundSnapThreshold = 2.5f;

	// Token: 0x04002B5C RID: 11100
	public bool LockGroundSnapToRadius = true;

	// Token: 0x04002B5D RID: 11101
	protected static List<vp_SpawnPoint> m_MatchingSpawnPoints = new List<vp_SpawnPoint>(50);

	// Token: 0x04002B5E RID: 11102
	protected static List<vp_SpawnPoint> m_SpawnPoints = null;
}
