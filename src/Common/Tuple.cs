// File provided for Reference Use Only by Microsoft Corporation (c) 2007.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Be.Stateless
{
	/// <summary>
	/// Supports the structural comparison of collection objects.
	/// </summary>
	public interface IStructuralComparable
	{
		/// <summary>
		/// Determines whether the current collection object precedes, occurs in the same position as, or follows another object in the sort order.
		/// </summary>
		/// <returns>
		/// An integer that indicates the relationship of the current collection object to <paramref name="other"/>, as shown in the following table.Return valueDescription-1The current instance precedes <paramref name="other"/>.0The current instance and <paramref name="other"/> are equal.1The current instance follows <paramref name="other"/>.
		/// </returns>
		/// <param name="other">The object to compare with the current instance.</param><param name="comparer">An object that compares members of the current collection object with the corresponding members of <paramref name="other"/>.</param>
		int CompareTo(object other, IComparer comparer);
	}

	/// <summary>
	/// Defines methods to support the comparison of objects for structural equality.
	/// </summary>
	public interface IStructuralEquatable
	{
		/// <summary>
		/// Determines whether an object is structurally equal to the current instance.
		/// </summary>
		/// <returns>
		/// true if the two objects are equal; otherwise, false.
		/// </returns>
		/// <param name="other">The object to compare with the current instance.</param><param name="comparer">An object that determines whether the current instance and <paramref name="other"/> are equal. </param>
		bool Equals(object other, IEqualityComparer comparer);

		/// <summary>
		/// Returns a hash code for the current instance.
		/// </summary>
		/// <returns>
		/// The hash code for the current instance.
		/// </returns>
		/// <param name="comparer">An object that computes the hash code of the current object.</param>
		int GetHashCode(IEqualityComparer comparer);
	}

	/// <summary>
	/// Helper so we can call some tuple methods recursively without knowing the underlying types. 
	/// </summary>
	internal interface ITuple
	{
		int Size { get; }

		string ToString(StringBuilder sb);

		int GetHashCode(IEqualityComparer comparer);
	}

	public static class Tuple
	{
		public static Tuple<T1> Create<T1>(T1 item1)
		{
			return new Tuple<T1>(item1);
		}

		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
		{
			return new Tuple<T1, T2, T3>(item1, item2, item3);
		}

		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
		}

		public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
		}

		public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
		{
			return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8));
		}

		// From System.Web.Util.HashCodeCombiner 
		internal static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2), h3);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6));
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7));
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
		{
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8));
		}
	}

	[Serializable]
	public class Tuple<T1> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1)
		{
			_item1 = item1;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			return comparer.Compare(_item1, objTuple._item1);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return comparer.GetHashCode(_item1);
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 1; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		private readonly T1 _item1;
	}

	[Serializable]
	public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2)
		{
			_item1 = item1;
			_item2 = item2;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			var c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			return comparer.Compare(_item2, objTuple._item2);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(comparer.GetHashCode(_item1), comparer.GetHashCode(_item2));
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 2; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
	}

	[Serializable]
	public class Tuple<T1, T2, T3> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2, T3 item3)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2, T3>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			var c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			c = comparer.Compare(_item2, objTuple._item2);

			if (c != 0) return c;

			return comparer.Compare(_item3, objTuple._item3);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2, T3>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2) && comparer.Equals(_item3, objTuple._item3);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(comparer.GetHashCode(_item1), comparer.GetHashCode(_item2), comparer.GetHashCode(_item3));
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(", ");
			sb.Append(_item3);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 3; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		public T3 Item3
		{
			get { return _item3; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
		private readonly T3 _item3;
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
			_item4 = item4;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2, T3, T4>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			var c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			c = comparer.Compare(_item2, objTuple._item2);

			if (c != 0) return c;

			c = comparer.Compare(_item3, objTuple._item3);

			if (c != 0) return c;

			return comparer.Compare(_item4, objTuple._item4);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2, T3, T4>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2) && comparer.Equals(_item3, objTuple._item3)
				&& comparer.Equals(_item4, objTuple._item4);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(comparer.GetHashCode(_item1), comparer.GetHashCode(_item2), comparer.GetHashCode(_item3), comparer.GetHashCode(_item4));
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(", ");
			sb.Append(_item3);
			sb.Append(", ");
			sb.Append(_item4);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 4; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		public T3 Item3
		{
			get { return _item3; }
		}

		public T4 Item4
		{
			get { return _item4; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
		private readonly T3 _item3;
		private readonly T4 _item4;
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
			_item4 = item4;
			_item5 = item5;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			int c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			c = comparer.Compare(_item2, objTuple._item2);

			if (c != 0) return c;

			c = comparer.Compare(_item3, objTuple._item3);

			if (c != 0) return c;

			c = comparer.Compare(_item4, objTuple._item4);

			if (c != 0) return c;

			return comparer.Compare(_item5, objTuple._item5);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2) && comparer.Equals(_item3, objTuple._item3)
				&& comparer.Equals(_item4, objTuple._item4) && comparer.Equals(_item5, objTuple._item5);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(
				comparer.GetHashCode(_item1), comparer.GetHashCode(_item2), comparer.GetHashCode(_item3), comparer.GetHashCode(_item4), comparer.GetHashCode(_item5));
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(", ");
			sb.Append(_item3);
			sb.Append(", ");
			sb.Append(_item4);
			sb.Append(", ");
			sb.Append(_item5);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 5; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		public T3 Item3
		{
			get { return _item3; }
		}

		public T4 Item4
		{
			get { return _item4; }
		}

		public T5 Item5
		{
			get { return _item5; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
		private readonly T3 _item3;
		private readonly T4 _item4;
		private readonly T5 _item5;
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
			_item4 = item4;
			_item5 = item5;
			_item6 = item6;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5, T6>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			var c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			c = comparer.Compare(_item2, objTuple._item2);

			if (c != 0) return c;

			c = comparer.Compare(_item3, objTuple._item3);

			if (c != 0) return c;

			c = comparer.Compare(_item4, objTuple._item4);

			if (c != 0) return c;

			c = comparer.Compare(_item5, objTuple._item5);

			if (c != 0) return c;

			return comparer.Compare(_item6, objTuple._item6);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5, T6>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2) && comparer.Equals(_item3, objTuple._item3)
				&& comparer.Equals(_item4, objTuple._item4) && comparer.Equals(_item5, objTuple._item5) && comparer.Equals(_item6, objTuple._item6);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(
				comparer.GetHashCode(_item1),
				comparer.GetHashCode(_item2),
				comparer.GetHashCode(_item3),
				comparer.GetHashCode(_item4),
				comparer.GetHashCode(_item5),
				comparer.GetHashCode(_item6));
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(", ");
			sb.Append(_item3);
			sb.Append(", ");
			sb.Append(_item4);
			sb.Append(", ");
			sb.Append(_item5);
			sb.Append(", ");
			sb.Append(_item6);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 6; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		public T3 Item3
		{
			get { return _item3; }
		}

		public T4 Item4
		{
			get { return _item4; }
		}

		public T5 Item5
		{
			get { return _item5; }
		}

		public T6 Item6
		{
			get { return _item6; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
		private readonly T3 _item3;
		private readonly T4 _item4;
		private readonly T5 _item5;
		private readonly T6 _item6;
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
			_item4 = item4;
			_item5 = item5;
			_item6 = item6;
			_item7 = item7;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			var c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			c = comparer.Compare(_item2, objTuple._item2);

			if (c != 0) return c;

			c = comparer.Compare(_item3, objTuple._item3);

			if (c != 0) return c;

			c = comparer.Compare(_item4, objTuple._item4);

			if (c != 0) return c;

			c = comparer.Compare(_item5, objTuple._item5);

			if (c != 0) return c;

			c = comparer.Compare(_item6, objTuple._item6);

			if (c != 0) return c;

			return comparer.Compare(_item7, objTuple._item7);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2) && comparer.Equals(_item3, objTuple._item3)
				&& comparer.Equals(_item4, objTuple._item4) && comparer.Equals(_item5, objTuple._item5) && comparer.Equals(_item6, objTuple._item6)
				&& comparer.Equals(_item7, objTuple._item7);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(
				comparer.GetHashCode(_item1),
				comparer.GetHashCode(_item2),
				comparer.GetHashCode(_item3),
				comparer.GetHashCode(_item4),
				comparer.GetHashCode(_item5),
				comparer.GetHashCode(_item6),
				comparer.GetHashCode(_item7));
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(", ");
			sb.Append(_item3);
			sb.Append(", ");
			sb.Append(_item4);
			sb.Append(", ");
			sb.Append(_item5);
			sb.Append(", ");
			sb.Append(_item6);
			sb.Append(", ");
			sb.Append(_item7);
			sb.Append(")");
			return sb.ToString();
		}

		int ITuple.Size
		{
			get { return 7; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		public T3 Item3
		{
			get { return _item3; }
		}

		public T4 Item4
		{
			get { return _item4; }
		}

		public T5 Item5
		{
			get { return _item5; }
		}

		public T6 Item6
		{
			get { return _item6; }
		}

		public T7 Item7
		{
			get { return _item7; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
		private readonly T3 _item3;
		private readonly T4 _item4;
		private readonly T5 _item5;
		private readonly T6 _item6;
		private readonly T7 _item7;
	}

	[Serializable]
	public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
		{
			if (!(rest is ITuple))
			{
				throw new ArgumentException("The last element of an eight element tuple must be a Tuple.");
			}

			_item1 = item1;
			_item2 = item2;
			_item3 = item3;
			_item4 = item4;
			_item5 = item5;
			_item6 = item6;
			_item7 = item7;
			_rest = rest;
		}

		#region IComparable Members

		Int32 IComparable.CompareTo(Object obj)
		{
			return ((IStructuralComparable) this).CompareTo(obj, Comparer<Object>.Default);
		}

		#endregion

		#region IStructuralComparable Members

		Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
		{
			if (other == null) return 1;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;

			if (objTuple == null)
			{
				throw new ArgumentException(string.Format("Argument must be of type {0}.", GetType()), "other");
			}

			var c = comparer.Compare(_item1, objTuple._item1);

			if (c != 0) return c;

			c = comparer.Compare(_item2, objTuple._item2);

			if (c != 0) return c;

			c = comparer.Compare(_item3, objTuple._item3);

			if (c != 0) return c;

			c = comparer.Compare(_item4, objTuple._item4);

			if (c != 0) return c;

			c = comparer.Compare(_item5, objTuple._item5);

			if (c != 0) return c;

			c = comparer.Compare(_item6, objTuple._item6);

			if (c != 0) return c;

			c = comparer.Compare(_item7, objTuple._item7);

			if (c != 0) return c;

			return comparer.Compare(_rest, objTuple._rest);
		}

		#endregion

		#region IStructuralEquatable Members

		Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
		{
			if (other == null) return false;

			var objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;

			if (objTuple == null)
			{
				return false;
			}

			return comparer.Equals(_item1, objTuple._item1) && comparer.Equals(_item2, objTuple._item2) && comparer.Equals(_item3, objTuple._item3)
				&& comparer.Equals(_item4, objTuple._item4) && comparer.Equals(_item5, objTuple._item5) && comparer.Equals(_item6, objTuple._item6)
				&& comparer.Equals(_item7, objTuple._item7) && comparer.Equals(_rest, objTuple._rest);
		}

		Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			// We want to have a limited hash in this case.  We'll use the last 8 elements of the tuple
			var t = (ITuple) _rest;
			if (t.Size >= 8)
			{
				return t.GetHashCode(comparer);
			}

			// In this case, the rest memeber has less than 8 elements so we need to combine some our elements with the elements in rest 
			int k = 8 - t.Size;
			switch (k)
			{
				case 1:
					return Tuple.CombineHashCodes(comparer.GetHashCode(_item7), t.GetHashCode(comparer));
				case 2:
					return Tuple.CombineHashCodes(comparer.GetHashCode(_item6), comparer.GetHashCode(_item7), t.GetHashCode(comparer));
				case 3:
					return Tuple.CombineHashCodes(comparer.GetHashCode(_item5), comparer.GetHashCode(_item6), comparer.GetHashCode(_item7), t.GetHashCode(comparer));
				case 4:
					return Tuple.CombineHashCodes(
						comparer.GetHashCode(_item4), comparer.GetHashCode(_item5), comparer.GetHashCode(_item6), comparer.GetHashCode(_item7), t.GetHashCode(comparer));
				case 5:
					return Tuple.CombineHashCodes(
						comparer.GetHashCode(_item3),
						comparer.GetHashCode(_item4),
						comparer.GetHashCode(_item5),
						comparer.GetHashCode(_item6),
						comparer.GetHashCode(_item7),
						t.GetHashCode(comparer));
				case 6:
					return Tuple.CombineHashCodes(
						comparer.GetHashCode(_item2),
						comparer.GetHashCode(_item3),
						comparer.GetHashCode(_item4),
						comparer.GetHashCode(_item5),
						comparer.GetHashCode(_item6),
						comparer.GetHashCode(_item7),
						t.GetHashCode(comparer));
				case 7:
					return Tuple.CombineHashCodes(
						comparer.GetHashCode(_item1),
						comparer.GetHashCode(_item2),
						comparer.GetHashCode(_item3),
						comparer.GetHashCode(_item4),
						comparer.GetHashCode(_item5),
						comparer.GetHashCode(_item6),
						comparer.GetHashCode(_item7),
						t.GetHashCode(comparer));
			}
			Debug.Assert(false, "Missed all cases for computing Tuple hash code");
			return -1;
		}

		#endregion

		#region ITuple Members

		Int32 ITuple.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable) this).GetHashCode(comparer);
		}

		string ITuple.ToString(StringBuilder sb)
		{
			sb.Append(_item1);
			sb.Append(", ");
			sb.Append(_item2);
			sb.Append(", ");
			sb.Append(_item3);
			sb.Append(", ");
			sb.Append(_item4);
			sb.Append(", ");
			sb.Append(_item5);
			sb.Append(", ");
			sb.Append(_item6);
			sb.Append(", ");
			sb.Append(_item7);
			sb.Append(", ");
			return ((ITuple) _rest).ToString(sb);
		}

		int ITuple.Size
		{
			get { return 7 + ((ITuple) _rest).Size; }
		}

		#endregion

		#region Base Class Member Overrides

		public override Boolean Equals(Object obj)
		{
			return ((IStructuralEquatable) this).Equals(obj, EqualityComparer<Object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable) this).GetHashCode(EqualityComparer<Object>.Default);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("(");
			return ((ITuple) this).ToString(sb);
		}

		#endregion

		public T1 Item1
		{
			get { return _item1; }
		}

		public T2 Item2
		{
			get { return _item2; }
		}

		public T3 Item3
		{
			get { return _item3; }
		}

		public T4 Item4
		{
			get { return _item4; }
		}

		public T5 Item5
		{
			get { return _item5; }
		}

		public T6 Item6
		{
			get { return _item6; }
		}

		public T7 Item7
		{
			get { return _item7; }
		}

		public TRest Rest
		{
			get { return _rest; }
		}

		private readonly T1 _item1;
		private readonly T2 _item2;
		private readonly T3 _item3;
		private readonly T4 _item4;
		private readonly T5 _item5;
		private readonly T6 _item6;
		private readonly T7 _item7;
		private readonly TRest _rest;
	}
}
