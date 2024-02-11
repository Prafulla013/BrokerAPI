using Application.Common.Interfaces;
using Common.Enumerations;
using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Test.Setup
{
    public static class SetupBroker
    {
        public const long ID1 = 1;
        public const string SUBDOMAIN = "test-subdomain";
        public static string Name = "test-broker";
        public static string ContactName = "test-contact-name";
        public static string Email = "test-broker@testmail.com";

        public const string ADMIN_ID = "2d49397c-bfb0-4e85-9ed0-8f1091485654";

        public const string EMP_ID = "8dbbdac1-1948-42e5-83f8-72fda79e2050";
        public const string EMP_FNAME = "Emp FUserName";
        public const string EMP_LNAME = "Emp LUserName";
        public const string EMP_EMAIL = "emp1user@domain.com";
        public const string EMP_PHONE = "9888888888";
        public const bool EMP_HASSYSTEMACCESS = true;

        public const string COTR_ID = "03bf6679-f607-4b7b-a43a-7465aff0d4e3";
        public const string COTR_FNAME = "C FUserName";
        public const string COTR_LNAME = "C LUserName";
        public const string COTR_EMAIL = "c1user@domain.com";
        public const string COTR_PHONE = "9222222222";
        public const bool COTR_HASSYSTEMACCESS = false;

        /// <summary>
        /// Broker with user
        /// </summary>
        public const long ID2 = 2;
    }
}
