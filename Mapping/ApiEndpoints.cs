﻿namespace Movies.Api.Mapping
{
    public class ApiEndpoints
    {
        private const string ApiBase = "api";

        public static class Default
        { 
            public static class Movies
            {

                private const string Base = $"{ApiBase}/movies";

                public const string GetAll = Base;
                public const string Get = $"{Base}/{{idOrSlug}}";
                public const string Create = Base;
                public const string Update = $"{Base}/{{id:guid}}";
                public const string Delete = $"{Base}/{{id:guid}}";

                public const string Rate = $"{Base}/{{id:guid}}/ratings";
                public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";
            }

            public static class Ratings
            {
                private const string Base = $"{ApiBase}/ratings";

                public const string GetUserRatings = $"{Base}/me";

            }
        }

        public static class V1
        {
            private const string VersionBase = $"{ApiBase}/v1";
            public static class Movies
            {
                private const string Base = $"{VersionBase}/movies";

                public const string GetAll = Base;
                public const string Get = $"{Base}/{{idOrSlug}}";
                public const string Create = Base;
                public const string Update = $"{Base}/{{id:guid}}";
                public const string Delete = $"{Base}/{{id:guid}}";

                public const string Rate = $"{Base}/{{id:guid}}/ratings";
                public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";
            }

            public static class Ratings
            {
                private const string Base = $"{VersionBase}/ratings";

                public const string GetUserRatings = $"{Base}/me";

            }
        }

        public static class V2
        {
            private const string VersionBase = $"{ApiBase}/v2";
            public static class Movies
            {
                private const string Base = $"{VersionBase}/movies";

                public const string GetAll = Base;
                public const string Get = $"{Base}/{{idOrSlug}}";
                public const string Create = Base;
                public const string Update = $"{Base}/{{id:guid}}";
                public const string Delete = $"{Base}/{{id:guid}}";

                public const string Rate = $"{Base}/{{id:guid}}/ratings";
                public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";
            }

            public static class Ratings
            {
                private const string Base = $"{VersionBase}/ratings";

                public const string GetUserRatings = $"{Base}/me";

            }
        }
    }
}
