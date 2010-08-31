using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;

namespace MMarinov.WebCrawler.Library
{
    public class Statistics : CSLA.BusinessBase
    {
        private int _id;		//**PK
        private long _crawledSuccessfulLinks = 0;
        private long _crawledTotalLinks = 0;
        private long _foundTotalLinks = 0;
        private long _foundValidLinks = 0;
        private SmartDate _startDate = new SmartDate(true);
        private string _duration = "";
        private string _processDescription = "";
        private long _words = 0;


        #region Constructors

        private Statistics()
        {
            MarkAsChild();
        }

        #endregion //Constructors

        #region Business Properties and Methods

        public long ID
        {
            get { return _id; }
        }

        public long Words
        {
            get { return _words; }
            set
            {
                if (_words != value)
                {
                    _words = value;
                    MarkDirty();
                }
            }
        }
        public string ProcessDescription
        {
            get { return _processDescription; }
            set
            {
                if (_processDescription != value)
                {
                    _processDescription = value;
                    MarkDirty();
                }
            }
        }
        public string Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    MarkDirty();
                }
            }
        }
        public long FoundValidLinks
        {
            get { return _foundValidLinks; }
            set
            {
                if (_foundValidLinks != value)
                {
                    _foundValidLinks = value;
                    MarkDirty();
                }
            }
        }
        public long FoundTotalLinks
        {
            get { return _foundTotalLinks; }
            set
            {
                if (_foundTotalLinks != value)
                {
                    _foundTotalLinks = value;
                    MarkDirty();
                }
            }
        }
        public long CrawledTotalLinks
        {
            get { return _crawledTotalLinks; }
            set
            {
                if (_crawledTotalLinks != value)
                {
                    _crawledTotalLinks = value;
                    MarkDirty();
                }
            }
        }
        public long CrawledSuccessfulLinks
        {
            get { return _crawledSuccessfulLinks; }
            set
            {
                if (_crawledSuccessfulLinks != value)
                {
                    _crawledSuccessfulLinks = value;
                    MarkDirty();
                }
            }
        }
        public DateTime StartDateDT
        {
            get
            {
                return _startDate.Date;
            }
            set
            {
                if (_startDate != value)
                {
                    _startDate.Date = value;
                    MarkDirty();
                }
            }
        }

        public bool IsSaveable
        {
            //Since you cannot bind a control to multiple properties you need to create a property that combines the ones you need
            //In this case, bind the UI Save button Enabled property to IsSaveable. (Why save an object that has not changed?)
            get
            {
                return IsValid && IsDirty;
            }
        }

        #endregion //Business Properties and Methods

        #region System.Object Overrides
        public override string ToString()
        {
            return "Statistics" + "/" + _id.ToString();
        }

        public bool Equals(Statistics file)
        {
            return _id.Equals(file.ID);
        }

        public override int GetHashCode()
        {
            return ("Statistics" + "/" + _id.ToString()).GetHashCode();
        }
        #endregion //System.Object Overrides

        #region Criteria (identifies the Individual Object/ Primary Key)
        [Serializable]
        private class Criteria
        {
            public int ID = 0;

            public Criteria()
            {
            }

            public Criteria(int id)
            {
                this.ID = id;
            }
        }
        #endregion //Criteria

        #region Static Methods

        public static Statistics NewStatistics()
        {
            return (Statistics)DataPortal.Create(new Criteria());
        }

        #endregion //Static Methods


        #region Data Access

        //Called by DataPortal so that we can set defaults as needed
        protected override void DataPortal_Create(object criteria)
        {

        }

        protected override void DataPortal_Update()
        {
            // save data into db
            SqlConnection cn = new SqlConnection(DB("WebCrawler"));
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr;

            cn.Open();

            try
            {
                tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    cm.Connection = cn;
                    cm.Transaction = tr;
                    cm.CommandType = CommandType.StoredProcedure;

                    //perform an insert, object has not been persisted
                    cm.CommandText = @"sp_InsertStatistics";

                    cm.Parameters.AddWithValue("@CrawledSuccessfulLinks", _crawledSuccessfulLinks);
                    cm.Parameters.AddWithValue("@CrawledTotalLinks", _crawledTotalLinks);
                    cm.Parameters.AddWithValue("@Duration", _duration);
                    cm.Parameters.AddWithValue("@FoundTotalLinks", _foundTotalLinks);
                    cm.Parameters.AddWithValue("@FoundValidLinks", _foundValidLinks);
                    cm.Parameters.AddWithValue("@ProcessDescription", _processDescription);
                    cm.Parameters.AddWithValue("@StartDate", _startDate.DBValue);
                    cm.Parameters.AddWithValue("@Words", _words);

                    _id = Convert.ToInt32(cm.ExecuteScalar());

                    // mark the object as old (persisted)
                    MarkOld();

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

        #endregion //Data Access

        internal void SaveOne()
        {
            DataPortal_Update();
        }
    }
}
