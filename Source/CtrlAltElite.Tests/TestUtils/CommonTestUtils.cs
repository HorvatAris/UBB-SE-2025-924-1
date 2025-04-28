namespace SteamStore.Tests.TestUtils
{
    using System.Text;

    public static class CommonTestUtils
    {
        private const string CHAR_SET_FOR_RANDOM_PATH = "/123456789abcdefghijklmnopqrstuvwxyz";
        private const string CHAR_SET_FOR_RANDOM_STRING = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string PATH_PREFIX = "/";
        private const int STARTING_INDEX = 0;
        private const int MAX_NUMBER_OF_CHARS = 100;
        private static readonly Random Random = new Random();

        public static string RandomPath()
        {
            return PATH_PREFIX + RandomString(MAX_NUMBER_OF_CHARS, CHAR_SET_FOR_RANDOM_PATH);
        }

        public static string RandomName(int count)
        {
            return RandomString(count);
        }

        public static decimal RandomNumber(int min, int max, int decimals)
        {
            var value = (Random.NextDouble() * (max - min)) + min;
            var rounded = Convert.ToDecimal(Math.Round(value, decimals));
            return rounded;
        }

        public static T RandomElement<T>(T[] array)
        {
            var index = Random.Next(array.Length);
            return array[index];
        }

        private static string RandomString(int i, string chars = CHAR_SET_FOR_RANDOM_STRING)
        {
            var result = new StringBuilder(i);

            for (var j = STARTING_INDEX; j < i; j++)
            {
                result.Append(chars[Random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
