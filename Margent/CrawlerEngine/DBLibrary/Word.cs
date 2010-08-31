using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace MMarinov.WebCrawler.Library
{
    public class Word : CSLA.BusinessBase
    {
        #region Class Level Private Variables

        private long _id = 0;		//**PK
        private string _wordName = "";

        #endregion //Class Level Private Variables

        #region Constructors

        private Word()
        {
            MarkAsChild();
        }

        #endregion //Constructors

        #region Business Properties and Methods

        public long ID
        {
            get { return _id; }
        }

        public string WordName
        {
            get { return _wordName; }
            set
            {
                if (value != _wordName)
                {
                    _wordName = value;
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
            return "Word" + "/" + _id.ToString();
        }

        public bool Equals(Word file)
        {
            return _id.Equals(file.ID);
        }

        public override int GetHashCode()
        {
            return ("Word" + "/" + _id.ToString()).GetHashCode();
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

        public static Word NewWord()
        {
            return (Word)DataPortal.Create(new Criteria());
        }

        public static Word NewWord(string wordName)
        {
            Word w = (Word)DataPortal.Create(new Criteria());
            w.WordName = wordName;
            return w;
        }

        public static Word FetchWord(SafeDataReader dr)
        {
            // Load an Existing Object from Data Reader
            Word child = NewWord();
            child.Fetch(dr);
            return child;
        }

        /// <summary>
        /// Called by DataPortal to load data from the database
        /// </summary>
        /// <param name="dr"></param>
        public void Fetch(SafeDataReader dr)
        {
            // Retrieve the data from the passed in data reader, 
            // which may or may not have a transaction associated with it
            _id = dr.GetInt64("ID");
            _wordName = dr.GetString("WordName");

            MarkOld();
        }

        #endregion //Static Methods


        #region Data Access

        //Called by DataPortal so that we can set defaults as needed
        protected override void DataPortal_Create(object criteria)
        {

        }

        internal void Update(SqlTransaction tr)
        {
            if (!IsDirty)
            {
                return;
            }

            // save data into db
            SqlConnection cn = tr.Connection;
            SqlCommand cm = new SqlCommand();

            try
            {
                cm.Connection = cn;
                cm.Transaction = tr;
                cm.CommandType = CommandType.StoredProcedure;

                // is not deleted object, check if this is an update or insert
                if (this.IsNew)
                {
                    //perform an insert, object has not been persisted
                    cm.CommandText = @"sp_InsertWord";
                }
                else
                {
                    //check
                }

                cm.Parameters.AddWithValue("@WordName", _wordName);

                _id = Convert.ToInt32(cm.ExecuteScalar());

                // mark the object as old (persisted)
                MarkOld();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
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


                    // is not deleted object, check if this is an update or insert
                    if (this.IsNew)
                    {
                        //perform an insert, object has not been persisted
                        cm.CommandText = @"sp_InsertWord";
                    }
                    else
                    {
                        //check
                    }

                    cm.Parameters.AddWithValue("@WordName", _wordName);
                    cm.Parameters.AddWithValue("@ID", _id).Direction = ParameterDirection.Output;

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
