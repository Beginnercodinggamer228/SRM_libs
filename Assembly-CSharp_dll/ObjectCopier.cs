using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Token: 0x0200024A RID: 586
public static class ObjectCopier
{
	// Token: 0x06000CA4 RID: 3236 RVA: 0x000342CC File Offset: 0x000324CC
	public static T Clone<T>(T source)
	{
		if (!typeof(T).IsSerializable)
		{
			Debug.Log("The type must be serializable.");
		}
		T result;
		if (source == null)
		{
			result = default(T);
			return result;
		}
		IFormatter formatter = new BinaryFormatter();
		SurrogateSelector surrogateSelector = new SurrogateSelector();
		surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3Surrogate());
		formatter.SurrogateSelector = surrogateSelector;
		Stream stream = new MemoryStream();
		using (stream)
		{
			formatter.Serialize(stream, source);
			stream.Seek(0L, SeekOrigin.Begin);
			result = (T)((object)formatter.Deserialize(stream));
		}
		return result;
	}
}
