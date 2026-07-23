using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class SimpleGPUInstancingExample : MonoBehaviour
{
	// Token: 0x06000036 RID: 54 RVA: 0x00002D30 File Offset: 0x00000F30
	private void Awake()
	{
		this.InstancedMaterial.enableInstancing = true;
		float num = 4f;
		for (int i = 0; i < 1000; i++)
		{
			Component component = UnityEngine.Object.Instantiate<Transform>(this.Prefab, new Vector3(UnityEngine.Random.Range(-num, num), num + UnityEngine.Random.Range(-num, num), UnityEngine.Random.Range(-num, num)), Quaternion.identity);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			Color value = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
			materialPropertyBlock.SetColor("_Color", value);
			component.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
		}
	}

	// Token: 0x0400001E RID: 30
	public Transform Prefab;

	// Token: 0x0400001F RID: 31
	public Material InstancedMaterial;
}
