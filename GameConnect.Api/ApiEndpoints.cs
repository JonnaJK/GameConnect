namespace GameConnect.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Category
    {
        private const string Base = $"{ApiBase}/category";

        public const string Create = Base;
        public const string GetById = $"{Base}/{{id:int}}";
        public const string GetByName = $"{Base}/{{name}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:int}}";
        public const string Delete = $"{Base}/{{id:int}}";
    }

    public static class Tag
    {
        private const string Base = $"{ApiBase}/tag";

        public const string Create = Base;
        public const string GetByName = $"{Base}/{{name}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:int}}";
        public const string Delete = $"{Base}/{{id:int}}";
    }
}
