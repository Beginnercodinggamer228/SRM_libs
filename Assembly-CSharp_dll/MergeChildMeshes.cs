using System;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class MergeChildMeshes : MonoBehaviour
{
	// Token: 0x06000BF1 RID: 3057 RVA: 0x00032218 File Offset: 0x00030418
	private void Start()
	{
		base.gameObject.AddComponent<MeshRenderer>().material = this.mergedMaterial;
		MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
		int num = componentsInChildren.Length;
		CombineInstance[] array = new CombineInstance[num];
		for (int i = 0; i < num; i++)
		{
			array[i].mesh = componentsInChildren[i].sharedMesh;
			Log.Info("source mesh", new object[]
			{
				"item",
				i,
				"vertexCount",
				componentsInChildren[i].sharedMesh.vertexCount
			});
			array[i].transform = componentsInChildren[i].transform.localToWorldMatrix;
			componentsInChildren[i].gameObject.SetActive(false);
		}
		Log.Info("We wrote some stuff3", new object[]
		{
			"count",
			array.Length
		});
		Mesh mesh = new Mesh();
		MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		meshFilter.mesh.CombineMeshes(array);
		Log.Info("Our combined mesh", new object[]
		{
			"vertexCount",
			mesh.vertexCount
		});
		base.transform.rotation = Quaternion.identity;
		base.transform.position = Vector3.zero;
		base.transform.gameObject.SetActive(true);
	}

	// Token: 0x04000AD5 RID: 2773
	public Material mergedMaterial;
}
