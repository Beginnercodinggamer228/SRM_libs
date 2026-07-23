using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000035 RID: 53
[ExecuteInEditMode]
[AddComponentMenu("Terrain/Ramp Brush")]
public class RampBrush : MonoBehaviour
{
	// Token: 0x060000D2 RID: 210 RVA: 0x0000808C File Offset: 0x0000628C
	public void OnDrawGizmos()
	{
		if (this.turnBrushOnVar)
		{
			if ((Terrain)base.GetComponent(typeof(Terrain)) == null)
			{
				return;
			}
			Gizmos.color = Color.cyan;
			float num = this.brushSize / 4f;
			Gizmos.DrawLine(this.brushPosition + new Vector3(-num, 0f, 0f), this.brushPosition + new Vector3(num, 0f, 0f));
			Gizmos.DrawLine(this.brushPosition + new Vector3(0f, -num, 0f), this.brushPosition + new Vector3(0f, num, 0f));
			Gizmos.DrawLine(this.brushPosition + new Vector3(0f, 0f, -num), this.brushPosition + new Vector3(0f, 0f, num));
			Gizmos.DrawWireCube(this.brushPosition, new Vector3(this.brushSize, 0f, this.brushSize));
			Gizmos.DrawWireSphere(this.brushPosition, this.brushSize / 2f);
			if (!this.multiPoint)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(this.beginRamp, this.brushSize / 2f);
				return;
			}
			Gizmos.color = Color.magenta;
			for (int i = 0; i < this.controlPoints.Count; i++)
			{
				Gizmos.DrawWireSphere(this.controlPoints[i], this.brushSize / 2f);
			}
			if (this.controlPoints.Count > 2)
			{
				double num2 = 1.0 / (((double)this.controlPoints.Count - 1.0) * 8.0) - 1E-14;
				this.calculateDistBetweenPoints(this.controlPoints);
				Ray ray = this.parameterizedLine(0f, this.controlPoints, null);
				double num3 = num2;
				int num4 = 0;
				while (num3 <= 1.0 && num4 < 1000)
				{
					Ray ray2 = this.parameterizedLine((float)num3, this.controlPoints, null);
					Gizmos.DrawLine(ray.origin, ray2.origin);
					ray = ray2;
					num3 += num2;
					num4++;
				}
			}
		}
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x000082E4 File Offset: 0x000064E4
	public int[] terrainCordsToBitmap(TerrainData terData, Vector3 v)
	{
		float num = (float)terData.heightmapResolution;
		float num2 = (float)terData.heightmapResolution;
		Vector3 size = terData.size;
		int num3 = (int)Mathf.Floor(num / size.x * v.x);
		int num4 = (int)Mathf.Floor(num2 / size.z * v.z);
		return new int[]
		{
			num4,
			num3
		};
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00008340 File Offset: 0x00006540
	public float[] bitmapCordsToTerrain(TerrainData terData, int x, int y)
	{
		int heightmapResolution = terData.heightmapResolution;
		int heightmapResolution2 = terData.heightmapResolution;
		Vector3 size = terData.size;
		float num = (float)x * (size.z / (float)heightmapResolution2);
		float num2 = (float)y * (size.x / (float)heightmapResolution);
		return new float[]
		{
			num2,
			num
		};
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x0000838C File Offset: 0x0000658C
	public void toggleBrushOn()
	{
		if (this.turnBrushOnVar)
		{
			this.turnBrushOnVar = false;
			return;
		}
		this.turnBrushOnVar = true;
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x000083A8 File Offset: 0x000065A8
	public void rampBrush()
	{
		Terrain terrain = (Terrain)base.GetComponent(typeof(Terrain));
		if (terrain == null)
		{
			Debug.LogError("No terrain component on this GameObject");
			return;
		}
		try
		{
			TerrainData terrainData = terrain.terrainData;
			int heightmapResolution = terrainData.heightmapResolution;
			int heightmapResolution2 = terrainData.heightmapResolution;
			Vector3 size = terrainData.size;
			if (this.VERBOSE)
			{
				Debug.Log(string.Concat(new object[]
				{
					"terrainData heightmapHeight/heightmapWidth:",
					heightmapResolution,
					" ",
					heightmapResolution
				}));
			}
			if (this.VERBOSE)
			{
				Debug.Log("terrainData heightMapResolution:" + terrainData.heightmapResolution);
			}
			if (this.VERBOSE)
			{
				Debug.Log("terrainData size:" + terrainData.size);
			}
			Vector3 localScale = base.transform.localScale;
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			Vector3 vector = base.transform.InverseTransformPoint(this.beginRamp);
			Vector3 vector2 = base.transform.InverseTransformPoint(this.endRamp);
			base.transform.localScale = localScale;
			int num = (int)Mathf.Floor((float)heightmapResolution / size.z * this.brushSize);
			int num2 = (int)Mathf.Floor((float)heightmapResolution2 / size.x * this.brushSize);
			int[] array = this.terrainCordsToBitmap(terrainData, vector);
			int[] array2 = this.terrainCordsToBitmap(terrainData, vector2);
			if (array[0] < 0 || array2[0] < 0 || array[1] < 0 || array2[1] < 0 || array[0] >= heightmapResolution || array2[0] >= heightmapResolution || array[1] >= heightmapResolution2 || array2[1] >= heightmapResolution2)
			{
				Debug.LogError("The start point or the end point was out of bounds. Make sure the gizmo is over the terrain before setting the start and end points.Note: that sometimes Unity does not update the collider after changing settings in the 'Set Resolution' dialog. Entering play mode should reset the collider.");
			}
			else
			{
				double num3 = Math.Sqrt((double)((array2[0] - array[0]) * (array2[0] - array[0]) + (array2[1] - array[1]) * (array2[1] - array[1])));
				float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution2);
				vector2.y = heights[array2[0], array2[1]];
				vector.y = heights[array[0], array[1]];
				Vector3 vector3 = vector2 - vector;
				Vector3 rhs = new Vector3(-vector3.z, 0f, vector3.x);
				Vector3 vector4 = Vector3.Cross(vector3, rhs);
				vector4.Normalize();
				Vector3 vector5 = new Vector3(vector3.x, 0f, vector3.z);
				float num4;
				if (this.brushSize < 15f)
				{
					num4 = this.brushSize / 6f / vector3.magnitude;
				}
				else
				{
					num4 = (float)(1.0 / num3 * (double)this.brushSampleDensity);
				}
				if (this.VERBOSE)
				{
					float[] array3 = this.bitmapCordsToTerrain(terrainData, array[0], array[1]);
					Debug.Log("Local Begin Pos:" + vector);
					Debug.Log(string.Concat(new object[]
					{
						"pixel begin coord:",
						array[0],
						" ",
						array[0]
					}));
					Debug.Log(string.Concat(new object[]
					{
						"Local begin Pos Rev Transformed:",
						array3[0],
						" ",
						array3[1]
					}));
					array3 = this.bitmapCordsToTerrain(terrainData, array2[0], array2[1]);
					Debug.Log("Local End Pos:" + vector2);
					Debug.Log(string.Concat(new object[]
					{
						"pixel End coord:",
						array2[0],
						" ",
						array2[1]
					}));
					Debug.Log(string.Concat(new object[]
					{
						"Local End Pos Rev Transformed:",
						array3[0],
						" ",
						array3[1]
					}));
					Debug.Log(string.Concat(new object[]
					{
						"Sample Width/height: ",
						num,
						" ",
						num2
					}));
					Debug.Log("Brush Width: " + num4);
				}
				for (float num5 = 0f; num5 <= 1f; num5 += num4)
				{
					Vector3 v = vector + num5 * vector3;
					int[] array4 = this.terrainCordsToBitmap(terrainData, v);
					int num6 = array4[0] - num / 2;
					int num7 = array4[1] - num2 / 2;
					float[,] array5 = new float[num, num2];
					for (int i = 0; i < num; i++)
					{
						for (int j = 0; j < num2; j++)
						{
							if (num6 + i >= 0 && num7 + j >= 0 && num6 + i < heightmapResolution && num7 + j < heightmapResolution2)
							{
								array5[i, j] = heights[num6 + i, num7 + j];
							}
							else
							{
								array5[i, j] = 0f;
							}
						}
					}
					num = array5.GetLength(0);
					num2 = array5.GetLength(1);
					float[,] array6 = (float[,])array5.Clone();
					for (int k = 0; k < num; k++)
					{
						for (int l = 0; l < num2; l++)
						{
							float[] array7 = this.bitmapCordsToTerrain(terrainData, num6 + k, num7 + l);
							bool flag = false;
							if (vector5.x * (array7[0] - vector.x) + vector5.z * (array7[1] - vector.z) < 0f)
							{
								flag = true;
							}
							else if (-vector5.x * (array7[0] - vector2.x) - vector5.z * (array7[1] - vector2.z) < 0f)
							{
								flag = true;
							}
							if (!flag)
							{
								array6[k, l] = vector.y - (vector4.x * (array7[0] - vector.x) + vector4.z * (array7[1] - vector.z)) / vector4.y;
							}
						}
					}
					float num8 = (float)num / 2f;
					for (int m = 0; m < num; m++)
					{
						for (int n = 0; n < num2; n++)
						{
							float num9 = array6[m, n];
							float num10 = array5[m, n];
							float num11 = Vector2.Distance(new Vector2((float)m, (float)n), new Vector2(num8, num8));
							float num12 = 1f - (num11 - (num8 - num8 * this.brushSoftness)) / (num8 * this.brushSoftness);
							if (num12 < 0f)
							{
								num12 = 0f;
							}
							else if (num12 > 1f)
							{
								num12 = 1f;
							}
							num12 *= this.brushOpacity;
							float num13 = num9 * num12 + num10 * (1f - num12);
							array5[m, n] = num13;
						}
					}
					for (int num14 = 0; num14 < num; num14++)
					{
						for (int num15 = 0; num15 < num2; num15++)
						{
							if (num6 + num14 >= 0 && num7 + num15 >= 0 && num6 + num14 < heightmapResolution && num7 + num15 < heightmapResolution2)
							{
								heights[num6 + num14, num7 + num15] = array5[num14, num15];
							}
						}
					}
				}
				terrainData.SetHeights(0, 0, heights);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError("A brush error occurred: " + arg);
		}
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00008B38 File Offset: 0x00006D38
	public void StrokePath()
	{
		this._StrokePath();
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00008B40 File Offset: 0x00006D40
	public void _StrokePath()
	{
		Terrain terrain = (Terrain)base.GetComponent(typeof(Terrain));
		if (terrain == null)
		{
			Debug.LogError("No terrain component on this GameObject");
			return;
		}
		try
		{
			TerrainData terrainData = terrain.terrainData;
			int heightmapResolution = terrainData.heightmapResolution;
			int heightmapResolution2 = terrainData.heightmapResolution;
			Vector3 size = terrainData.size;
			if (this.VERBOSE)
			{
				Debug.Log(string.Concat(new object[]
				{
					"terrainData heightmapHeight/heightmapWidth:",
					heightmapResolution,
					" ",
					heightmapResolution
				}));
			}
			if (this.VERBOSE)
			{
				Debug.Log("terrainData heightMapResolution:" + terrainData.heightmapResolution);
			}
			if (this.VERBOSE)
			{
				Debug.Log("terrainData size:" + terrainData.size);
			}
			Vector3 localScale = base.transform.localScale;
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < this.controlPoints.Count; i++)
			{
				list.Add(base.transform.InverseTransformPoint(this.controlPoints[i]));
			}
			base.transform.localScale = localScale;
			for (int j = 0; j < list.Count; j++)
			{
				int[] array = this.terrainCordsToBitmap(terrainData, list[j]);
				if (array[0] < 0 || array[1] < 0 || array[0] >= heightmapResolution || array[1] >= heightmapResolution2)
				{
					Debug.LogError("The start point or the end point was out of bounds. Make sure the gizmo is over the terrain before setting the start and end points.Note: that sometimes Unity does not update the collider after changing settings in the 'Set Resolution' dialog. Entering play mode should reset the collider.");
					return;
				}
			}
			int num = (int)Mathf.Floor((float)heightmapResolution / size.z * this.brushSize);
			int num2 = (int)Mathf.Floor((float)heightmapResolution2 / size.x * this.brushSize);
			float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution2);
			for (int k = 0; k < list.Count; k++)
			{
				int[] array2 = this.terrainCordsToBitmap(terrainData, list[k]);
				Vector3 value = list[k];
				value.y = heights[array2[0], array2[1]];
				list[k] = value;
			}
			this.calculateDistBetweenPoints(list);
			this.calculateDistBetweenPointsInPixels(list, terrainData);
			float num3 = this.brushSampleDensity / this._totalLengthPixels;
			float num4 = this.brushSize / this._totalLengthPixels;
			Debug.Log(string.Concat(new object[]
			{
				"Sample w ",
				num,
				" h ",
				num2
			}));
			Debug.Log("parameterized brush width " + num4);
			if (num4 > 0.5f)
			{
				num4 = 0.5f;
			}
			if (this.VERBOSE)
			{
				for (int l = 0; l < list.Count; l++)
				{
					int[] array3 = this.terrainCordsToBitmap(terrainData, list[l]);
					float[] array4 = this.bitmapCordsToTerrain(terrainData, array3[0], array3[1]);
					Debug.Log(l + " Local control Pos:" + list[l]);
					Debug.Log(string.Concat(new object[]
					{
						l,
						" pixel begin coord:",
						array3[0],
						" ",
						array3[0]
					}));
					Debug.Log(string.Concat(new object[]
					{
						l,
						" Local begin Pos Rev Transformed:",
						array4[0],
						" ",
						array4[1]
					}));
				}
				Debug.Log("parameterized brush width " + num4);
			}
			StringBuilder message = new StringBuilder();
			for (float num5 = 0f; num5 <= 1f; num5 += num3)
			{
				Ray ray = this.parameterizedLine(num5, list, null);
				Vector3 vector = Vector3.Cross(new Vector3(-ray.direction.z, 0f, ray.direction.x), ray.direction);
				vector.Normalize();
				if (this.spacingJitter > 0f)
				{
					float f = 6.2831855f * UnityEngine.Random.value;
					float num6 = UnityEngine.Random.value + UnityEngine.Random.value;
					float num7;
					if (num6 > 1f)
					{
						num7 = 2f - num6;
					}
					else
					{
						num7 = num6;
					}
					num7 *= this.spacingJitter * this.brushSize;
					Vector3 vector2 = new Vector3(num7 * Mathf.Cos(f), 0f, num7 * Mathf.Sin(f));
					if (this.VERBOSE)
					{
						Debug.Log(string.Concat(new object[]
						{
							"jittering by ",
							vector2,
							" dir ",
							ray.direction,
							" n ",
							vector
						}));
					}
					Plane plane = new Plane(vector, ray.origin);
					Ray ray2 = new Ray(ray.origin + vector2, Vector3.up);
					float d;
					if (plane.Raycast(ray2, out d))
					{
						Vector3 origin = ray2.origin + ray2.direction * d;
						ray.origin = origin;
					}
				}
				Plane plane2 = new Plane((list[0] - list[1]).normalized, list[0]);
				Plane plane3 = new Plane((list[list.Count - 1] - list[list.Count - 2]).normalized, list[list.Count - 1]);
				int[] array5 = this.terrainCordsToBitmap(terrainData, ray.origin);
				int num8 = array5[0] - num / 2;
				int num9 = array5[1] - num2 / 2;
				float[,] array6 = new float[num, num2];
				for (int m = 0; m < num; m++)
				{
					for (int n = 0; n < num2; n++)
					{
						if (num8 + m >= 0 && num9 + n >= 0 && num8 + m < heightmapResolution && num9 + n < heightmapResolution2)
						{
							array6[m, n] = heights[num8 + m, num9 + n];
						}
						else
						{
							array6[m, n] = 0f;
						}
					}
				}
				float[,] array7 = (float[,])array6.Clone();
				for (int num10 = 0; num10 < num; num10++)
				{
					for (int num11 = 0; num11 < num2; num11++)
					{
						float[] array8 = this.bitmapCordsToTerrain(terrainData, num8 + num10, num9 + num11);
						Vector3 vector3 = new Vector3(array8[0], 0f, array8[1]);
						bool flag = false;
						if (plane2.GetSide(vector3) && num5 < num4 / 2f)
						{
							flag = true;
						}
						else if (plane3.GetSide(vector3) && num5 > 1f - num4 / 2f)
						{
							flag = true;
						}
						if (!flag)
						{
							Plane plane4 = new Plane(vector, ray.origin);
							Ray ray3 = new Ray(vector3, Vector3.up);
							float num12;
							if (plane4.Raycast(ray3, out num12))
							{
								array7[num10, num11] = ray3.origin.y + ray3.direction.y * num12;
							}
						}
					}
				}
				float num13 = Mathf.Min((float)num2 / 2f, (float)num / 2f);
				for (int num14 = 0; num14 < num; num14++)
				{
					for (int num15 = 0; num15 < num2; num15++)
					{
						float num16 = array7[num14, num15];
						float num17 = array6[num14, num15];
						float num18 = Vector2.Distance(new Vector2((float)num14, (float)num15), new Vector2(num13, num13));
						float num19 = (1f - num18 / num13) / this.brushSoftness;
						if (num19 < 0f)
						{
							num19 = 0f;
						}
						else if (num19 > 1f)
						{
							num19 = 1f;
						}
						num19 *= this.brushOpacity;
						float num20 = num16 * num19 + num17 * (1f - num19);
						array6[num14, num15] = num20;
					}
				}
				for (int num21 = 0; num21 < num; num21++)
				{
					for (int num22 = 0; num22 < num2; num22++)
					{
						if (num8 + num21 >= 0 && num9 + num22 >= 0 && num8 + num21 < heightmapResolution && num9 + num22 < heightmapResolution2)
						{
							heights[num8 + num21, num9 + num22] = array6[num21, num22];
						}
					}
				}
			}
			Debug.Log(message);
			terrainData.SetHeights(0, 0, heights);
		}
		catch (Exception arg)
		{
			Debug.LogError("A brush error occurred: " + arg);
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00009418 File Offset: 0x00007618
	private void calculateDistBetweenPoints(List<Vector3> cps)
	{
		this._distBetweenPoints.Clear();
		this._totalLength = 0f;
		for (int i = 1; i < cps.Count; i++)
		{
			this._distBetweenPoints.Add((cps[i] - cps[i - 1]).magnitude);
			this._totalLength += this._distBetweenPoints[this._distBetweenPoints.Count - 1];
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000949C File Offset: 0x0000769C
	private void calculateDistBetweenPointsInPixels(List<Vector3> cps, TerrainData terData)
	{
		this._totalLengthPixels = 0f;
		int[] array = this.terrainCordsToBitmap(terData, cps[0]);
		for (int i = 1; i < cps.Count; i++)
		{
			int[] array2 = this.terrainCordsToBitmap(terData, cps[i]);
			this._totalLengthPixels += Mathf.Sqrt((float)((array2[0] - array[0]) * (array2[0] - array[0]) + (array2[1] - array[1]) * (array2[1] - array[1])));
			array = array2;
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00009518 File Offset: 0x00007718
	private Ray parameterizedLine(float t, List<Vector3> cps, StringBuilder sb = null)
	{
		if (cps.Count < 2)
		{
			Debug.LogError("Less than two control points.");
			return default(Ray);
		}
		if (t < 0f)
		{
			t = 0f;
		}
		if (t >= 1f)
		{
			t = 1f;
		}
		Vector3[] array = new Vector3[cps.Count + 2];
		for (int i = 0; i < cps.Count; i++)
		{
			array[i + 1] = cps[i];
		}
		array[0] = 2f * cps[0] - cps[1];
		array[array.Length - 1] = 2f * cps[cps.Count - 1] - cps[cps.Count - 2];
		float num = t * this._totalLength;
		int num2 = 0;
		float num3 = 0f;
		bool flag = false;
		float num4 = 0f;
		while (!flag)
		{
			if (num4 + this._distBetweenPoints[num2] < num)
			{
				num4 += this._distBetweenPoints[num2];
				if (num2 < this.controlPoints.Count - 2)
				{
					num2++;
				}
				else
				{
					flag = true;
					num3 = 1f;
				}
			}
			else
			{
				flag = true;
				num3 = (num - num4) / this._distBetweenPoints[num2];
			}
		}
		if (num2 >= this.controlPoints.Count - 1)
		{
			num2--;
		}
		if (num3 > 1f)
		{
			num3 = 1f;
		}
		num2++;
		if (num2 + 2 > array.Length - 1)
		{
			Debug.LogError("Off end=" + t);
		}
		if (sb != null)
		{
			sb.AppendFormat("t={0} cpIdx={1} nt={2}\n", t, num2, num3);
		}
		Vector3 a = array[num2 - 1];
		Vector3 a2 = array[num2];
		Vector3 vector = array[num2 + 1];
		Vector3 vector2 = array[num2 + 2];
		float d = num3 * num3;
		float d2 = num3 * num3 * num3;
		Vector3 origin = 0.5f * (2f * a2 + (-a + vector) * num3 + (2f * a - 5f * a2 + 4f * vector - vector2) * d + (-a + 3f * a2 - 3f * vector + vector2) * d2);
		return new Ray(origin, (0.5f * (-a + vector + num3 * (4f * a - 10f * a2 + 8f * vector - 2f * vector2) + d * (-3f * a + 9f * a2 - 9f * vector + 3f * vector2))).normalized);
	}

	// Token: 0x04000134 RID: 308
	private bool VERBOSE;

	// Token: 0x04000135 RID: 309
	public bool brushOn;

	// Token: 0x04000136 RID: 310
	public bool turnBrushOnVar;

	// Token: 0x04000137 RID: 311
	public bool isBrushHidden;

	// Token: 0x04000138 RID: 312
	public Vector3 brushPosition;

	// Token: 0x04000139 RID: 313
	public Vector3 beginRamp;

	// Token: 0x0400013A RID: 314
	public Vector3 endRamp;

	// Token: 0x0400013B RID: 315
	public float brushSize = 50f;

	// Token: 0x0400013C RID: 316
	public float brushOpacity = 1f;

	// Token: 0x0400013D RID: 317
	public float brushSoftness = 0.5f;

	// Token: 0x0400013E RID: 318
	public float brushSampleDensity = 4f;

	// Token: 0x0400013F RID: 319
	public bool shiftProcessed = true;

	// Token: 0x04000140 RID: 320
	public Vector3 backupVector;

	// Token: 0x04000141 RID: 321
	public int numSubDivPerSeg = 10;

	// Token: 0x04000142 RID: 322
	public float spacingJitter;

	// Token: 0x04000143 RID: 323
	public float sizeJitter;

	// Token: 0x04000144 RID: 324
	public bool multiPoint;

	// Token: 0x04000145 RID: 325
	public List<Vector3> controlPoints = new List<Vector3>();

	// Token: 0x04000146 RID: 326
	private List<float> _distBetweenPoints = new List<float>();

	// Token: 0x04000147 RID: 327
	private float _totalLength;

	// Token: 0x04000148 RID: 328
	private float _totalLengthPixels;
}
