namespace LABTOOLS.API.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool IsNotNullOrEmpty(this Dictionary<string, int> value)
        {
            return value != null;
        }

        public static bool IsNotNullOrEmpty(this Dictionary<string, string> value)
        {
            return value != null;
        }
    }
}