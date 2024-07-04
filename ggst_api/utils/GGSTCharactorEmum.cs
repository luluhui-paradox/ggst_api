namespace ggst_api.utils
{
    public sealed class GGSTCharactorList
    {
        public const string SOL = "SO";
        public const string KY = "KY";
        public const string NA = "NA";
        public const string BR = "BR";
        public const string BA = "BA";

        public static List<string> CharactorList { get; } = ["SO","KY","NA","BR","BA"];
        private GGSTCharactorList() { }
    }
}
