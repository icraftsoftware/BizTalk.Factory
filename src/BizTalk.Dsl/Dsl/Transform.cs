using Microsoft.XLANGs.BaseTypes;

namespace Be.Stateless.BizTalk.Dsl
{
	/// <summary>
	/// Allows to write rules in terms of BizTalk TransformBase artifacts.
	/// </summary>
	/// <remarks>
	/// This class is just syntactic sugar to support the fluent rule DSL. It is only used at install time when
	/// populating the rule store (i.e. when translating a DSL rule into a BRE rule).
	/// </remarks>
	public static class Transform<T> where T : TransformBase
	{
		public static string MapTypeName
		{
			get { return typeof(T).AssemblyQualifiedName; }
		}
	}
}