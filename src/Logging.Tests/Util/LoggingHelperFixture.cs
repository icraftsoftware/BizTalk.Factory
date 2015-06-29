#region Copyright & License

// Copyright © 2012 François Chabot, Yves Dierick
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Be.Stateless.Logging.Util
{
	[TestFixture]
	public class LoggingHelperFixture
	{
		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_exceptionTypes = GetAllExceptionType();
		}

		#endregion

		[Test]
		public void ComputeHashCodeTest()
		{
			var result = MeasureExceptionHashCollision(true);
			Assert.IsTrue(result.CollisionPercentage <= ACCEPTED_COLLISION_PERCENTAGE, "Computation on the full type name Exceptions.");
			Debug.WriteLine(result);
			result = MeasureExceptionHashCollision(false);
			Assert.IsTrue(result.CollisionPercentage <= ACCEPTED_COLLISION_PERCENTAGE, "Computation on the short type name Exceptions.");
			Debug.WriteLine(result);
		}

		private CollisionResult MeasureExceptionHashCollision(bool withFullName)
		{
			var result = new CollisionResult();
			IDictionary<uint, string> container = new Dictionary<uint, string>();

			foreach (var type in _exceptionTypes)
			{
				var hashInput = withFullName ? type.FullName : type.Name;

				uint hashCode = BitConverter.ToUInt16(LoggingHelper.Compute16BitsHashCode(Encoding.ASCII.GetBytes(hashInput)), 0);

				result.ValueNumber++;
				//Debug.WriteLine(string.Format("{0}: {1} - {2} ({3})", hashCode, assembly.GetName().Name, type.FullName, exceptionCount));
				if (!container.ContainsKey(hashCode)) container.Add(hashCode, type.FullName);
				else result.CollisionNumber++;
			}
			return result;
		}

		private static bool IsException(Type type)
		{
			if (type == typeof(Exception)) return true;
			if (type.BaseType == null) return false;
			return IsException(type.BaseType);
		}

		private static Assembly LoadAssembly(string file)
		{
			try
			{
				return Assembly.LoadFile(file);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
			catch (BadImageFormatException)
			{
				return null;
			}
		}

		private static IEnumerable<Type> GetAllExceptionType()
		{
			IList<Type> exceptionTypes = new List<Type>();
			string gacDirectoryName = string.Format(
				"{0}\\assembly\\GAC",
				Environment.GetEnvironmentVariable("windir"));

			foreach (string directoryName in Directory.GetDirectories(gacDirectoryName))
			{
				foreach (string subDirectoryName in Directory.GetDirectories(directoryName))
				{
					string[] fileNames = Directory.GetFiles(subDirectoryName, "*.dll");
					foreach (string fileName in fileNames)
					{
						Assembly a = LoadAssembly(fileName);
						if (a != null) FillExceptionTypes(a, exceptionTypes);
					}
				}
			}
			return exceptionTypes;
		}

		private static void FillExceptionTypes(Assembly assembly, ICollection<Type> exceptionTypes)
		{
			try
			{
				foreach (var type in assembly.GetTypes())
				{
					if (IsException(type))
					{
						exceptionTypes.Add(type);
					}
				}
			}
			catch (ReflectionTypeLoadException)
			{
				//Debug.WriteLine(string.Format("{0}: {1} )", ex.GetType(), assembly.GetName().Name ));
			}
		}

		private class CollisionResult
		{
			public int CollisionNumber
			{
				get { return _collisionNumber; }
				set { _collisionNumber = value; }
			}

			public int ValueNumber
			{
				get { return _valueNumber; }
				set { _valueNumber = value; }
			}

			public decimal CollisionPercentage
			{
				get { return _valueNumber == 0 ? 0 : Math.Round(decimal.Divide(_collisionNumber, _valueNumber) * 100, 2); }
			}

			public override string ToString()
			{
				return string.Format("Total: {0}\nCollision: {1} ({2}%)", _valueNumber, _collisionNumber, CollisionPercentage);
			}

			private int _collisionNumber;
			private int _valueNumber;
		}

		private const int ACCEPTED_COLLISION_PERCENTAGE = 25;
		private IEnumerable<Type> _exceptionTypes;
	}
}