using System.Collections.Generic;

/// <summary>
/// Floyd结果
/// </summary>
public class FloydResult {
	/// <summary>
	/// 终点城镇id
	/// </summary>
	public string Id;
	/// <summary>
	/// 终点城镇名
	/// </summary>
	public string Name;
	/// <summary>
	/// 起点索引
	/// </summary>
	public int FromIndex;
	/// <summary>
	/// 终点索引
	/// </summary>
	public int ToIndex;
	/// <summary>
	/// 总距离(一两银子一点距离)
	/// </summary>
	public float Distance;
	/// <summary>
	/// 结果路径索引
	/// </summary>
	public List<int> PathStack;

	public FloydResult(float distance, List<int> pathStack) {
		Id = "";
		Name = "";
		FromIndex = 0;
		ToIndex = 0;
		Distance = distance;
		PathStack = pathStack;
	}
}

/// <summary>
/// Floyd算法主体类
/// </summary>
public class Floyd {
	/// <summary>
	/// 临接矩阵
	/// </summary>
	List<List<float>> _dis;

	/// <summary>
	/// 计算单位数
	/// </summary>
	public int UnionCount;

	/// <summary>
	/// 最终计算结果节点集合
	/// </summary>
	Dictionary<string, FloydResult> resultsMapping;

	public Floyd(List<List<float>> dis, int count) {
		_dis = dis;
		UnionCount = count;
		resultsMapping = new Dictionary<string, FloydResult>();
		List<List<int>> path = new List<List<int>>();
		//初始化路径
		for (int i = 0; i < UnionCount; ++i) {
			path.Add(new List<int>());
			for (int j = 0; j < UnionCount; ++j) {
				path[i].Add(i);
			}
		}
		//计算数据池
		for (int k = 0; k < UnionCount; ++k) {
			for (int i = 0; i < UnionCount; ++i) {
				for (int j = 0; j < UnionCount; ++j) {
					if (_dis[i][k] + _dis[k][j] < _dis[i][j]) {
						_dis[i][j] = _dis[i][k] + _dis[k][j];
						path[i][j] = path[k][j];
					}
				}
			}
		}
		//路径归档
		int _k; 
		List<int> _pathStack;
		for (int i = 0; i < UnionCount; ++i) {
			for (int j = 0; j < UnionCount; ++j) {
				if (i != j) {
					_pathStack = new List<int>();
					_k = j;
					do {
						_k = path[i][_k];
						_pathStack.Add(_k);
					}
					while (_k != i);
					_pathStack.Reverse();
					_pathStack.Add(j);
					resultsMapping.Add(string.Format("_{0}_{1}", i, j), new FloydResult(_dis[i][j], _pathStack));
				}
			}
		}
	}

	/// <summary>
	/// 查询计算结果
	/// </summary>
	/// <returns>The result.</returns>
	/// <param name="fromIndex">From index.</param>
	/// <param name="toIndex">To index.</param>
	public FloydResult GetResult(int fromIndex, int toIndex) {
		string key = string.Format("_{0}_{1}", fromIndex, toIndex);
		return resultsMapping.ContainsKey(key) ? resultsMapping[key] : null;
	}
}