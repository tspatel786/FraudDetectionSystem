namespace FraudDetectionSystem.ML.Common
{
    public static class MlHelper
    {
        public static float ToPercent(float probability) =>
            (float)Math.Round(probability * 100f, 2);

        public static float NameMismatchScore(string nameA, string nameB)
        {
            if (string.IsNullOrWhiteSpace(nameA) || string.IsNullOrWhiteSpace(nameB))
                return 1f;

            var a = nameA.Trim().ToLowerInvariant();
            var b = nameB.Trim().ToLowerInvariant();

            if (a == b) return 0f;

            var maxLen = Math.Max(a.Length, b.Length);
            if (maxLen == 0) return 0f;

            var distance = LevenshteinDistance(a, b);
            return Math.Min(1f, (float)distance / maxLen);
        }

        private static int LevenshteinDistance(string s, string t)
        {
            var d = new int[s.Length + 1, t.Length + 1];
            for (var i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (var j = 0; j <= t.Length; j++) d[0, j] = j;

            for (var i = 1; i <= s.Length; i++)
            {
                for (var j = 1; j <= t.Length; j++)
                {
                    var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }
    }
}
