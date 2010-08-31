using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;

namespace MMarinov.WebCrawler.Library
{
    public class WordsInFile : CSLA.BusinessBase
    {
        #region Class Level Private Variables

        private long _id = 0;
        private long _wordID = 0;
        private long _fileID = 0;
        private int _count = 0;

        #endregion

        #region Constructors

        private WordsInFile()
        {
            MarkAsChild();
        }

        #endregion //Constructors

        #region Business Properties and Methods

        public long ID
        {
            get { return _id; }
        }

        public long WordID
        {
            get { return _wordID; }
            set
            {
                if (value != _wordID)
                {
                    _wordID = value;
                    MarkDirty();
                }
            }
        }

        public long FileID
        {
            get { return _fileID; }
            set
            {
                if (value != _fileID)
                {
                    _fileID = value;
                    MarkDirty();
                }
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                if (value != _count)
                {
                    _count = value;
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
            return "WordsInFile" + "/" + _id.ToString();
        }

        public bool Equals(WordsInFile wif)
        {
            return _id.Equals(wif.ID);
        }

        public override int GetHashCode()
        {
            return ("WordsInFile" + "/" + _id.ToString()).GetHashCode();
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

        public static WordsInFile NewWordsInFile(long wordID)
        {
            WordsInFile wif = (WordsInFile)DataPortal.Create(new Criteria());
            wif.WordID = wordID;
            wif.Count = 1;
            return wif;
        }

        public static WordsInFile NewWordsInFile()
        {
            return (WordsInFile)DataPortal.Create(new Criteria());
        }


        #endregion //Static Methods

        #region Data Access

        //Called by DataPortal so that we can set defaults as needed
        protected override void DataPortal_Create(object criteria)
        {

        }

        internal void Update(SqlTransaction tr, long fileID)
        {
            if (!IsDirty)
            {
                return;
            }

            _fileID = fileID;

            // save data into db
            SqlConnection cn = tr.Connection;
            SqlCommand cm = new SqlCommand();

            cm.Connection = cn;
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;

            // is not deleted object, check if this is an update or insert
            if (this.IsNew)
            {
                //perform an insert, object has not been persisted
                cm.CommandText = "sp_InsertWordInFile";
            }
            else
            {
                //check
            }

            cm.Parameters.AddWithValue("@Count", _count);
            cm.Parameters.AddWithValue("@FileID", _fileID);
            cm.Parameters.AddWithValue("@WordID", _wordID);

            if (IsNew)
            {
                _id = Convert.ToInt32(cm.ExecuteScalar());
            }

            // mark the object as old (persisted)
            MarkOld();
        }

        #endregion //Data Access
    }
}
