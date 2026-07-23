using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200024C RID: 588
public abstract class Pather
{
	// Token: 0x06000CA8 RID: 3240 RVA: 0x00034384 File Offset: 0x00032584
	public void RecalculateNodeConnections(PathingNetworkNode[] nodes, List<Pather.NodePair> whitelist, List<Pather.NodePair> blacklist)
	{
		this.nodes = nodes;
		for (int i = 0; i < nodes.Length; i++)
		{
			nodes[i].connections = new List<PathingNetworkNode>();
		}
		HashSet<Pather.NodePair> hashSet = new HashSet<Pather.NodePair>(whitelist);
		HashSet<Pather.NodePair> hashSet2 = new HashSet<Pather.NodePair>(blacklist);
		for (int j = 0; j < nodes.Length; j++)
		{
			PathingNetworkNode pathingNetworkNode = nodes[j];
			for (int k = j + 1; k < nodes.Length; k++)
			{
				PathingNetworkNode pathingNetworkNode2 = nodes[k];
				Pather.NodePair item = new Pather.NodePair(nodes[j], nodes[k]);
				if (!hashSet2.Contains(item) && (hashSet.Contains(item) || this.PathPredicate(pathingNetworkNode.position, pathingNetworkNode2.position)))
				{
					pathingNetworkNode.connections.Add(pathingNetworkNode2);
					pathingNetworkNode2.connections.Add(pathingNetworkNode);
				}
			}
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x00034450 File Offset: 0x00032650
	public List<PathingNetworkNode> GeneratePathNodes(Vector3 start, Vector3 end)
	{
		if (this.PathPredicate(start, end))
		{
			return new List<PathingNetworkNode>();
		}
		PathingNetworkNode pathingNetworkNode = this.NearestAccessibleNode(start);
		if (pathingNetworkNode == null)
		{
			return null;
		}
		PathingNetworkNode pathingNetworkNode2 = this.NearestAccessibleNode(end);
		if (pathingNetworkNode2 == null)
		{
			return null;
		}
		HashSet<PathingNetworkNode> hashSet = new HashSet<PathingNetworkNode>();
		HashSet<PathingNetworkNode> hashSet2 = new HashSet<PathingNetworkNode>();
		hashSet2.Add(pathingNetworkNode);
		Dictionary<PathingNetworkNode, PathingNetworkNode> dictionary = new Dictionary<PathingNetworkNode, PathingNetworkNode>();
		Dictionary<PathingNetworkNode, float> dictionary2 = new Dictionary<PathingNetworkNode, float>();
		dictionary2[pathingNetworkNode] = 0f;
		Dictionary<PathingNetworkNode, float> fScore = new Dictionary<PathingNetworkNode, float>();
		fScore[pathingNetworkNode] = (pathingNetworkNode.position - pathingNetworkNode2.position).magnitude;
		Func<PathingNetworkNode, float> <>9__0;
		while (hashSet2.Count > 0)
		{
			IEnumerable<PathingNetworkNode> source = hashSet2;
			Func<PathingNetworkNode, float> keySelector;
			if ((keySelector = <>9__0) == null)
			{
				keySelector = (<>9__0 = ((PathingNetworkNode node) => fScore[node]));
			}
			PathingNetworkNode pathingNetworkNode3 = source.OrderBy(keySelector).First<PathingNetworkNode>();
			if (pathingNetworkNode3 == pathingNetworkNode2)
			{
				List<PathingNetworkNode> list = this.ConstructPathFromAStarResults(dictionary, pathingNetworkNode3);
				this.TrimPathEnds(list, start, end);
				return list;
			}
			hashSet2.Remove(pathingNetworkNode3);
			hashSet.Add(pathingNetworkNode3);
			foreach (PathingNetworkNode pathingNetworkNode4 in pathingNetworkNode3.connections)
			{
				if (!hashSet.Contains(pathingNetworkNode4))
				{
					if (!hashSet2.Contains(pathingNetworkNode4))
					{
						hashSet2.Add(pathingNetworkNode4);
					}
					float num = this.GetValueOrDefault<PathingNetworkNode, float>(dictionary2, pathingNetworkNode3, float.PositiveInfinity) + (pathingNetworkNode3.position - pathingNetworkNode4.position).magnitude;
					if (num < this.GetValueOrDefault<PathingNetworkNode, float>(dictionary2, pathingNetworkNode4, float.PositiveInfinity))
					{
						dictionary[pathingNetworkNode4] = pathingNetworkNode3;
						dictionary2[pathingNetworkNode4] = num;
						fScore[pathingNetworkNode4] = num + (pathingNetworkNode4.position - pathingNetworkNode2.position).magnitude;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x00034660 File Offset: 0x00032860
	public Queue<Vector3> GeneratePath(Vector3 start, Vector3 end)
	{
		List<PathingNetworkNode> list = this.GeneratePathNodes(start, end);
		if (list == null)
		{
			return null;
		}
		Queue<Vector3> queue = new Queue<Vector3>(from n in list
		select n.position);
		queue.Enqueue(end);
		return queue;
	}

	// Token: 0x06000CAB RID: 3243
	protected abstract bool PathPredicate(Vector3 start, Vector3 end);

	// Token: 0x06000CAC RID: 3244
	protected abstract bool NearestAccessibleNodePredicate(Vector3 start, Vector3 end);

	// Token: 0x06000CAD RID: 3245 RVA: 0x000346AC File Offset: 0x000328AC
	private PathingNetworkNode NearestAccessibleNode(Vector3 pos)
	{
		return (from n in this.nodes
		where this.NearestAccessibleNodePredicate(n.position, pos)
		orderby (n.position - pos).sqrMagnitude
		select n).FirstOrDefault<PathingNetworkNode>();
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x000346FA File Offset: 0x000328FA
	private V GetValueOrDefault<K, V>(Dictionary<K, V> dict, K key, V defVal)
	{
		if (dict.ContainsKey(key))
		{
			return dict[key];
		}
		return defVal;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x00034710 File Offset: 0x00032910
	private List<PathingNetworkNode> ConstructPathFromAStarResults(Dictionary<PathingNetworkNode, PathingNetworkNode> cameFrom, PathingNetworkNode goal)
	{
		List<PathingNetworkNode> list = new List<PathingNetworkNode>();
		list.Add(goal);
		PathingNetworkNode pathingNetworkNode = goal;
		while (cameFrom.ContainsKey(pathingNetworkNode))
		{
			pathingNetworkNode = cameFrom[pathingNetworkNode];
			list.Add(pathingNetworkNode);
		}
		list.Reverse();
		return list;
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x00034750 File Offset: 0x00032950
	private void TrimPathEnds(List<PathingNetworkNode> path, Vector3 start, Vector3 end)
	{
		for (int i = path.Count - 1; i >= 0; i--)
		{
			if (this.PathPredicate(start, path[i].position))
			{
				path.RemoveRange(0, i);
				break;
			}
		}
		for (int j = 0; j < path.Count - 1; j++)
		{
			if (this.PathPredicate(path[j].position, end))
			{
				path.RemoveRange(j + 1, path.Count - (j + 1));
				return;
			}
		}
	}

	// Token: 0x04000B84 RID: 2948
	protected PathingNetworkNode[] nodes = new PathingNetworkNode[0];

	// Token: 0x0200024D RID: 589
	[Serializable]
	public class NodePair : IEquatable<Pather.NodePair>
	{
		// Token: 0x06000CB2 RID: 3250 RVA: 0x000347DF File Offset: 0x000329DF
		public NodePair(PathingNetworkNode node1, PathingNetworkNode node2)
		{
			this.node1 = node1;
			this.node2 = node2;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000347F8 File Offset: 0x000329F8
		public bool Equals(Pather.NodePair other)
		{
			return (this.node1 == other.node1 && this.node2 == other.node2) || (this.node1 == other.node2 && this.node2 == other.node1);
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x00034853 File Offset: 0x00032A53
		public override int GetHashCode()
		{
			if (this.node1 == null || this.node2 == null)
			{
				return 0;
			}
			return this.node1.GetHashCode() ^ this.node2.GetHashCode();
		}

		// Token: 0x04000B85 RID: 2949
		public PathingNetworkNode node1;

		// Token: 0x04000B86 RID: 2950
		public PathingNetworkNode node2;
	}
}
