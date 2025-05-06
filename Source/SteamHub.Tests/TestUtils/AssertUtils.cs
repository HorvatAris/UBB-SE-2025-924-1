namespace SteamHub.Tests.TestUtils
{
    using System.Collections.ObjectModel;
    using System.Reflection;
    using Xunit;
    using Xunit.Sdk;

    public static class AssertUtils
    {
        private const string? OBJECTS_CANNOT_BE_NULL = "Objects cannot be null";

        public static void AssertAllPropertiesEqual<T>(T expected, T actual)
        {
            if (expected == null || actual == null)
            {
                throw new ArgumentException(OBJECTS_CANNOT_BE_NULL);
            }

            var type = typeof(T);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var expectedValue = property.GetValue(expected);
                var actualValue = property.GetValue(actual);
                try
                {
                    Assert.Equal(expectedValue, actualValue);
                }
                catch (EqualException)
                {
                    throw EqualException.ForMismatchedValues(expectedValue, actualValue, property.ToString());
                }
            }
        }

        public static void AssertContainsSingle<T>(Collection<T> collection, T expectedElement)
        {
            Assert.Single(collection);
            Assert.Equal(expectedElement, collection.First());
        }
        public static void AssertContainsExactly<T>(Collection<T> collection, params T[] expectedElements)
        {
            Assert.Equal(expectedElements.Length, collection.Count);
            for (int i = 0; i < expectedElements.Length; i++)
            {
                Assert.Equal(expectedElements[i], collection.ElementAt(i));
            }
        }

		public static void AssertContainsEquivalent<T>(IList<T> collection, params T[] expectedElements)
		{
			Assert.Equal(expectedElements.Length, collection.Count);
			for (int i = 0; i < expectedElements.Length; i++)
			{
				Assert.Equivalent(expectedElements[i], collection.ElementAt(i));
			}
		}
	}
}
