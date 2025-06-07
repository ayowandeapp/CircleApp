using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Helpers.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";

        public static readonly IReadOnlyList<string> All = [Admin, User];
    }
}