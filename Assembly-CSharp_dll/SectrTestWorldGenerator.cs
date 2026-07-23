using System;
using UnityEngine;

// Token: 0x0200077D RID: 1917
public class SectrTestWorldGenerator : MonoBehaviour
{
	// Token: 0x0600281C RID: 10268 RVA: 0x00097FAC File Offset: 0x000961AC
	private void Awake()
	{
		for (int i = -30; i <= 30; i += 10)
		{
			for (int j = 10; j <= 50; j += 10)
			{
				this.CreateRandomSector((float)i, (float)j, 10f, 10f);
			}
		}
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x00097FEC File Offset: 0x000961EC
	private void CreateRandomSector(float x, float z, float width, float height)
	{
		Vector3 vector = new Vector3(x, 0f, z);
		Vector3 size = new Vector3(width, 20f, height);
		GameObject gameObject = new GameObject(string.Concat(new object[]
		{
			"TerrainBlock(",
			x,
			",",
			z,
			")"
		}));
		gameObject.transform.position = vector;
		SECTR_Sector sectr_Sector = gameObject.AddComponent<SECTR_Sector>();
		sectr_Sector.OverrideBounds = true;
		sectr_Sector.BoundsOverride = new Bounds(vector, size);
		sectr_Sector.Frozen = true;
		SECTR_Chunk sectr_Chunk = gameObject.AddComponent<SECTR_Chunk>();
		string text = this.PickSubSceneName();
		sectr_Chunk.ScenePath = text;
		sectr_Chunk.NodeName = "Assets/Scene/test/SectrTestDynamic/Chunks/" + text + ".unity";
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000980A4 File Offset: 0x000962A4
	private string PickSubSceneName()
	{
		return this.PATHS[UnityEngine.Random.Range(0, this.PATHS.Length)];
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x00003296 File Offset: 0x00001496
	private void Update()
	{
	}

	// Token: 0x040027A6 RID: 10150
	private readonly string[] PATHS = new string[]
	{
		"SectrTestDynamic_OtherChunk",
		"SectrTestDynamic_OtherChunk2"
	};
}
