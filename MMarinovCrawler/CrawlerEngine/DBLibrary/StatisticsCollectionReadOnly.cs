using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class StatisticsCollectionReadOnly : CSLA.ReadOnlyCollectionBase
    {
        [Serializable]
        public class Statistics
        {
            // This has private members, public properties because
            // ASP.NET can't data bind to public members of a structure
            private int _id;		//**PK
            private long _crawledSuccessfulLinks = 0;
            private long _crawledTotalLinks = 0;
            private long _foundTotalLinks = 0;
            private long _foundValidLinks = 0;
            private SmartDate _startDate = new SmartDate(true);
            private string _duration = "";
            private string _processDescription = "";
            private long _words = 0;

            #region public properties

            public long Words
            {
                get { return _words; }
                set { _words = value; }
            }
            public string ProcessDescription
            {
                get { return _processDescription; }
                set { _processDescription = value; }
            }
            public string Duration
            {
                get { return _duration; }
                set { _duration = value; }
            }
            public long FoundValidLinks
            {
                get { return _foundValidLinks; }
                set { _foundValidLinks = value; }
            }
            public long FoundTotalLinks
            {
                get { return _foundTotalLinks; }
                set { _foundTotalLinks = value; }
            }
            public long CrawledTotalLinks
            {
                get { return _crawledTotalLinks; }
                set { _crawledTotalLinks = value; }
            }
            public long CrawledSuccessfulLinks
            {
                get { return _crawledSuccessfulLinks; }
                set { _crawledSuccessfulLinks = value; }
            }
            public int ID
            {
                get
                {
                    return _id;
                }
                set
                {
                    _id = value;
                }
            }


            public DateTime StartDate
            {
                get
                {
                    return _startDate.Date;
                }
                set
                {
                    _startDate.Date = value;
                }
            }

            public SmartDate StartDateSD
            {
                get
                {
                    return _startDate;
                }
                set
                {
                    _startDate = value;
                }
            }


            public bool Equals(Statistics item)
            {
                return _id.Equals(item.ID);
            }
            #endregion
        }

        #region Business Properties and Methods

        public Statistics this[int index]
        {
            get
            {
                return (Statistics)List[index];
            }
        }

        #endregion //Business Properties and Methods

        #region Contains

        public bool Contains(Statistics item)
        {
            return List.Contains(item);
        }

        public bool Contains(int id)
        {
            foreach (Statistics child in List)
            {
                if (child.ID.Equals(id))
                    return true;
            }
            return false;
        }
        #endregion //Contains

        #region Constructor
        private StatisticsCollectionReadOnly()
        {
            //prevent direct creation
            AllowSort = true;
            AllowFind = true;
        }
        #endregion //Constructor

        #region Criteria (identifies the Individual Object/ Primary Key)
        [Serializable]
        public class Criteria
        {
            public Criteria()
            {
            }

            public DateTime StartDateFrom;

            public Criteria(DateTime startDateFrom)
            {
                StartDateFrom = startDateFrom;
            }
        }
        #endregion //Criteria

        #region Static Methods

        public static StatisticsCollectionReadOnly GetStatisticsCollReadOnly()
        {
            return (StatisticsCollectionReadOnly)DataPortal.Fetch(new Criteria());
        }

        public static StatisticsCollectionReadOnly GetStatisticsCollReadOnly(DateTime startDateFrom)
        {
            return (StatisticsCollectionReadOnly)DataPortal.Fetch(new Criteria(startDateFrom));
        }

        #endregion //Static Methods

        #region Data Access
        //Called by DataPortal to load data from the database
        protected override void DataPortal_Fetch(object criteria)
        {
            //retrieve data from database
            //Locked = false;
            Criteria crit = (Criteria)criteria;
            SqlConnection cn = new SqlConnection(DB("WebCrawler"));
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr;

            cn.Open();

            try
            {
                tr = cn.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandText = "sp_SelectStatisticsAll";

                    if (crit.StartDateFrom > DateTime.MinValue)
                    {
                        cm.CommandText = "sp_SelectStatisticsAfter";//TODO
                    }

                    SafeDataReader dr = new SafeDataReader(cm.ExecuteReader());
                    try
                    {
                        while (dr.Read())
                        {
                            FetchStatistics(dr);
                        }
                    }
                    finally
                    {
                        dr.Close();
                    }
                    tr.Commit();
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    throw (ex);
                }
            }
            finally
            {
                cn.Close();
            }
        }


        private void FetchStatistics(SafeDataReader dr)
        {
            StatisticsCollectionReadOnly.Statistics info = new StatisticsCollectionReadOnly.Statistics();
            info.ID = dr.GetInt32("ID");
            info.CrawledSuccessfulLinks = dr.GetInt64("CrawledSuccessfulLinks");
            info.CrawledTotalLinks = dr.GetInt64("CrawledTotalLinks");
            info.Duration = dr.GetString("Duration");
            info.FoundTotalLinks = dr.GetInt64("FoundTotalLinks");
            info.StartDateSD = dr.GetSmartDate("StartDate", true);
            info.FoundValidLinks = dr.GetInt64("FoundValidLinks");
            info.ProcessDescription = dr.GetString("ProcessDescription");
            info.Words = dr.GetInt64("Words");

            InnerList.Add(info);
        }

        #endregion //Data Access
    }
}

