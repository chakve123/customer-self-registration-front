using System;

namespace EservicesLib.User
{
    [Serializable]
    public class Person
    {
        public string FullName { get; set; }

        public string PersonID { get; set; }

        public int UnID { get; set; }

        /// <summary>
        /// -1 მოქალაქე;
        /// (0,2)- მოქმედი;
        /// სხვა - ლიკვიდირებული;
        /// </summary>
        public int OrgStatusID { get; set; }

        public string OrgStatus { get; set; }

        public bool Active { get; set; }
    }
}
