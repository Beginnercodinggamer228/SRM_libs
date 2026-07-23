using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200002D RID: 45
[ExecuteInEditMode]
public class SlicedVolume : MonoBehaviour
{
	// Token: 0x060000AB RID: 171 RVA: 0x00005880 File Offset: 0x00003A80
	private void OnDrawGizmos()
	{
		this.editorUpdate();
		if (this.generateNewSlices)
		{
			if (this.cloudMaterial)
			{
				this.integrityCheck();
				this.settingValuesUp(false);
				if (this.shadowCaster)
				{
					int num = this.sliceAmount;
					this.sliceAmount = 1;
					this.settingValuesUp(true);
					this.sliceAmount = num;
				}
			}
			this.generateNewSlices = false;
		}
	}

	// Token: 0x060000AC RID: 172 RVA: 0x000058E0 File Offset: 0x00003AE0
	private void syncCloudAndShadowCaster()
	{
		this.shadowCasterMat.CopyPropertiesFromMaterial(this.cloudMaterial);
	}

	// Token: 0x060000AD RID: 173 RVA: 0x000058F4 File Offset: 0x00003AF4
	private void editorUpdate()
	{
		this.sliceAmount = ((this.sliceAmount <= 1) ? 1 : this.sliceAmount);
		this.segmentCount = ((this.segmentCount > 2) ? this.segmentCount : 2);
		if (Camera.current.name != "PreRenderCamera" && this.cloudMaterial && !this.curved && this.meshSlices)
		{
			if (Camera.current.transform.position.y > base.transform.position.y && this.cameraCloudRelation == -1)
			{
				this.cameraCloudRelation = 1;
				this.updateCloudDirection = true;
			}
			else if (Camera.current.transform.position.y < base.transform.position.y && this.cameraCloudRelation == 1)
			{
				this.cameraCloudRelation = -1;
				this.updateCloudDirection = true;
			}
			if (this.updateCloudDirection)
			{
				this.meshSlices.transform.localScale = new Vector3(Mathf.Abs(this.meshSlices.transform.localScale.x), Mathf.Abs(this.meshSlices.transform.localScale.y) * (float)this.cameraCloudRelation, Mathf.Abs(this.meshSlices.transform.localScale.z));
				this.cloudMaterial.SetVector("_CloudNormalsDirection", new Vector4(this.normalDirection.x, this.normalDirection.y * (float)this.cameraCloudRelation, this.normalDirection.z * -1f, 0f));
				this.updateCloudDirection = false;
			}
		}
		else if (this.curved && this.cloudMaterial && this.meshSlices)
		{
			this.meshSlices.transform.localScale = new Vector3(Mathf.Abs(this.meshSlices.transform.localScale.x), Mathf.Abs(this.meshSlices.transform.localScale.y) * -1f, Mathf.Abs(this.meshSlices.transform.localScale.z));
			if (this.meshShadowCaster)
			{
				this.meshShadowCaster.transform.localScale = new Vector3(Mathf.Abs(this.meshSlices.transform.localScale.x), Mathf.Abs(this.meshSlices.transform.localScale.y) * -1f, Mathf.Abs(this.meshSlices.transform.localScale.z));
			}
			this.cloudMaterial.SetVector("_CloudNormalsDirection", new Vector4(this.normalDirection.x, this.normalDirection.y * -1f, this.normalDirection.z * -1f, 0f));
		}
		if (this.transferVariables && this.cloudMaterial && this.shadowCasterMat)
		{
			this.syncCloudAndShadowCaster();
		}
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00005C38 File Offset: 0x00003E38
	private void integrityCheck()
	{
		if (!this.meshSlices)
		{
			foreach (object obj in base.transform)
			{
				Transform transform = (Transform)obj;
				if (transform.name == "Clouds")
				{
					this.meshSlices = transform.gameObject;
				}
			}
			if (!this.meshSlices)
			{
				this.meshSlices = new GameObject("Clouds");
				this.meshSlices.transform.parent = base.transform;
				this.meshSlices.transform.localPosition = Vector3.zero;
				this.meshSlices.AddComponent<MeshFilter>();
				this.meshSlices.AddComponent<MeshRenderer>();
				this.meshSlices.GetComponent<Renderer>().material = this.cloudMaterial;
			}
		}
		if (this.shadowCaster && !this.meshShadowCaster)
		{
			foreach (object obj2 in base.transform)
			{
				Transform transform2 = (Transform)obj2;
				if (transform2.name == "Shadow Caster")
				{
					this.meshShadowCaster = transform2.gameObject;
				}
			}
			if (!this.meshShadowCaster)
			{
				this.meshShadowCaster = new GameObject("Shadow Caster");
				this.meshShadowCaster.transform.parent = base.transform;
				this.meshShadowCaster.transform.localPosition = Vector3.zero;
				this.meshShadowCaster.AddComponent<MeshFilter>();
				this.meshShadowCaster.AddComponent<MeshRenderer>();
				this.meshShadowCaster.GetComponent<Renderer>().material = this.shadowCasterMat;
			}
		}
		if (!this.shadowCaster)
		{
			if (this.meshShadowCaster)
			{
				UnityEngine.Object.DestroyImmediate(this.meshShadowCaster);
			}
			else
			{
				foreach (object obj3 in base.transform)
				{
					Transform transform3 = (Transform)obj3;
					if (transform3.name == "Shadow Caster")
					{
						UnityEngine.Object.DestroyImmediate(transform3.gameObject);
					}
				}
			}
		}
		if (this.shadowCaster)
		{
			this.meshShadowCaster.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
		}
		this.meshSlices.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
		if (this.shadowCaster)
		{
			this.meshShadowCaster.GetComponent<MeshRenderer>().receiveShadows = false;
		}
		this.meshSlices.GetComponent<MeshRenderer>().receiveShadows = false;
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005EF8 File Offset: 0x000040F8
	private void settingValuesUp(bool isShadowCaster)
	{
		this.vertices = new Vector3[this.segmentCount * this.segmentCount * this.sliceAmount];
		this.uvMap = new Vector2[this.vertices.Length];
		this.triangleConstructor = new int[(this.segmentCount - 1) * (this.segmentCount - 1) * this.sliceAmount * 2 * 3];
		this.vertexColor = new Color[this.vertices.Length];
		float num = 1f / ((float)this.segmentCount - 1f);
		this.posGainPerVertices = new Vector3(num * this.dimensions.x, 1f / (float)Mathf.Clamp(this.sliceAmount - 1, 1, 999999) * this.dimensions.y, num * this.dimensions.z);
		this.posGainPerUV = num;
		this.trianglesCreation(isShadowCaster);
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00005FE0 File Offset: 0x000041E0
	private void trianglesCreation(bool isShadowCaster)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		float f = 0f;
		for (int i = 0; i < this.sliceAmount; i++)
		{
			float num4 = -1f + (float)i * (2f / (float)this.sliceAmount);
			float w;
			if ((float)i < (float)this.sliceAmount * 0.5f)
			{
				w = 0f + 1f / ((float)this.sliceAmount * 0.5f) * (float)i;
			}
			else
			{
				w = 2f - 1f / ((float)this.sliceAmount * 0.5f) * (float)(i + 1);
			}
			if (this.sliceAmount == 1)
			{
				w = 1f;
			}
			for (int j = 0; j < this.segmentCount; j++)
			{
				int num5 = this.segmentCount * num;
				for (int k = 0; k < this.segmentCount; k++)
				{
					if (this.curved)
					{
						f = Vector3.Distance(new Vector3(this.posGainPerVertices.x * (float)k - this.dimensions.x / 2f, 0f, this.posGainPerVertices.z * (float)j - this.dimensions.z / 2f), Vector3.zero);
					}
					if (this.sliceAmount == 1)
					{
						this.vertices[k + num5] = new Vector3(this.posGainPerVertices.x * (float)k - this.dimensions.x / 2f, 0f + Mathf.Pow(f, this.roundness) * this.intensity, this.posGainPerVertices.z * (float)j - this.dimensions.z / 2f);
					}
					else
					{
						this.vertices[k + num5] = new Vector3(this.posGainPerVertices.x * (float)k - this.dimensions.x / 2f, this.posGainPerVertices.y * (float)i - this.dimensions.y / 2f + Mathf.Pow(f, this.roundness) * this.intensity, this.posGainPerVertices.z * (float)j - this.dimensions.z / 2f);
					}
					this.uvMap[k + num5] = new Vector2(this.posGainPerUV * (float)k, this.posGainPerUV * (float)j);
					this.vertexColor[k + num5] = new Vector4(num4, num4, num4, w);
				}
				num++;
				if (j >= 1)
				{
					for (int l = 0; l < this.segmentCount - 1; l++)
					{
						this.triangleConstructor[num3] = l + num2 + i * this.segmentCount;
						this.triangleConstructor[1 + num3] = this.segmentCount + l + num2 + i * this.segmentCount;
						this.triangleConstructor[2 + num3] = 1 + l + num2 + i * this.segmentCount;
						this.triangleConstructor[3 + num3] = this.segmentCount + 1 + l + num2 + i * this.segmentCount;
						this.triangleConstructor[4 + num3] = 1 + l + num2 + i * this.segmentCount;
						this.triangleConstructor[5 + num3] = this.segmentCount + l + num2 + i * this.segmentCount;
						num3 += 6;
					}
					num2 += this.segmentCount;
				}
			}
		}
		if (!isShadowCaster)
		{
			Mesh mesh = new Mesh();
			mesh.Clear();
			mesh.name = "GeoSlices";
			mesh.vertices = this.vertices;
			mesh.triangles = this.triangleConstructor;
			mesh.uv = this.uvMap;
			mesh.colors = this.vertexColor;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			SlicedVolume.calculateMeshTangents(mesh);
			this.meshSlices.GetComponent<MeshFilter>().mesh = mesh;
			return;
		}
		Mesh mesh2 = new Mesh();
		mesh2.Clear();
		mesh2.name = "GeoSlices";
		mesh2.vertices = this.vertices;
		mesh2.triangles = this.triangleConstructor;
		mesh2.uv = this.uvMap;
		mesh2.colors = this.vertexColor;
		mesh2.RecalculateNormals();
		mesh2.RecalculateBounds();
		SlicedVolume.calculateMeshTangents(mesh2);
		this.meshShadowCaster.GetComponent<MeshFilter>().mesh = mesh2;
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00006460 File Offset: 0x00004660
	public static void calculateMeshTangents(Mesh mesh)
	{
		int[] triangles = mesh.triangles;
		Vector3[] array = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		int num = triangles.Length;
		int num2 = array.Length;
		Vector3[] array2 = new Vector3[num2];
		Vector3[] array3 = new Vector3[num2];
		Vector4[] array4 = new Vector4[num2];
		for (long num3 = 0L; num3 < (long)num; num3 += 3L)
		{
			long num4 = (long)triangles[(int)(checked((IntPtr)num3))];
			long num5 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 1L))))];
			long num6 = (long)triangles[(int)(checked((IntPtr)(unchecked(num3 + 2L))))];
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector2 vector4;
			Vector2 vector5;
			Vector2 vector6;
			checked
			{
				vector = array[(int)((IntPtr)num4)];
				vector2 = array[(int)((IntPtr)num5)];
				vector3 = array[(int)((IntPtr)num6)];
				vector4 = uv[(int)((IntPtr)num4)];
				vector5 = uv[(int)((IntPtr)num5)];
				vector6 = uv[(int)((IntPtr)num6)];
			}
			float num7 = vector2.x - vector.x;
			float num8 = vector3.x - vector.x;
			float num9 = vector2.y - vector.y;
			float num10 = vector3.y - vector.y;
			float num11 = vector2.z - vector.z;
			float num12 = vector3.z - vector.z;
			float num13 = vector5.x - vector4.x;
			float num14 = vector6.x - vector4.x;
			float num15 = vector5.y - vector4.y;
			float num16 = vector6.y - vector4.y;
			float num17 = 1f / (num13 * num16 - num14 * num15);
			Vector3 b = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
			Vector3 b2 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
			checked
			{
				array2[(int)((IntPtr)num4)] += b;
				array2[(int)((IntPtr)num5)] += b;
				array2[(int)((IntPtr)num6)] += b;
				array3[(int)((IntPtr)num4)] += b2;
				array3[(int)((IntPtr)num5)] += b2;
				array3[(int)((IntPtr)num6)] += b2;
			}
		}
		for (long num18 = 0L; num18 < (long)num2; num18 += 1L)
		{
			checked
			{
				Vector3 lhs = normals[(int)((IntPtr)num18)];
				Vector3 vector7 = array2[(int)((IntPtr)num18)];
				Vector3.OrthoNormalize(ref lhs, ref vector7);
				array4[(int)((IntPtr)num18)].x = vector7.x;
				array4[(int)((IntPtr)num18)].y = vector7.y;
				array4[(int)((IntPtr)num18)].z = vector7.z;
				array4[(int)((IntPtr)num18)].w = ((Vector3.Dot(Vector3.Cross(lhs, vector7), array3[(int)((IntPtr)num18)]) < 0f) ? -1f : 1f);
			}
		}
		mesh.tangents = array4;
	}

	// Token: 0x040000E2 RID: 226
	public Material cloudMaterial;

	// Token: 0x040000E3 RID: 227
	public Material shadowCasterMat;

	// Token: 0x040000E4 RID: 228
	public int sliceAmount = 25;

	// Token: 0x040000E5 RID: 229
	public int segmentCount = 3;

	// Token: 0x040000E6 RID: 230
	public Vector3 dimensions = new Vector3(1000f, 50f, 1000f);

	// Token: 0x040000E7 RID: 231
	public Vector3 normalDirection = new Vector3(1f, 1f, 1f);

	// Token: 0x040000E8 RID: 232
	public bool shadowCaster;

	// Token: 0x040000E9 RID: 233
	public bool transferVariables = true;

	// Token: 0x040000EA RID: 234
	public bool unityFive;

	// Token: 0x040000EB RID: 235
	public bool curved;

	// Token: 0x040000EC RID: 236
	public float roundness = 2f;

	// Token: 0x040000ED RID: 237
	public float intensity = 0.001f;

	// Token: 0x040000EE RID: 238
	public bool generateNewSlices;

	// Token: 0x040000EF RID: 239
	private bool updateCloudDirection = true;

	// Token: 0x040000F0 RID: 240
	private int cameraCloudRelation = 1;

	// Token: 0x040000F1 RID: 241
	private Color[] vertexColor;

	// Token: 0x040000F2 RID: 242
	private Vector3[] vertices;

	// Token: 0x040000F3 RID: 243
	private Vector2[] uvMap;

	// Token: 0x040000F4 RID: 244
	private int[] triangleConstructor;

	// Token: 0x040000F5 RID: 245
	private Vector3 posGainPerVertices;

	// Token: 0x040000F6 RID: 246
	private float posGainPerUV;

	// Token: 0x040000F7 RID: 247
	private GameObject meshSlices;

	// Token: 0x040000F8 RID: 248
	private GameObject meshShadowCaster;
}
