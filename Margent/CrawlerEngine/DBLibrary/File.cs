using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;

namespace MMarinov.WebCrawler.Library
{
    public class File : CSLA.BusinessBase
    {
        #region Class Level Private Variables

        private long _id = 0;		//**PK
        private string _url = "";
        private string _description = "";
        private string _keywords = "";
        private string _title = "";
        private byte _fileType = 0;

        //Any child collection should be declared here As Private....
        private WordsInFileCollection _wordsInFileColl = WordsInFileCollection.NewWordsInFileCollection();

        #endregion //Class Level Private Variables

        #region Constructors

        private File()
        {
            MarkAsChild();
        }

        #endregion //Constructors

        #region Business Properties and Methods

        public WordsInFileCollection WordsInFileColl
        {
            get
            {
                return _wordsInFileColl;
            }
        }

        public long ID
        {
            get { return _id; }
        }

        public byte FileType
        {
            get { return _fileType; }
            set
            {
                if (value != _fileType)
                {
                    _fileType = value;
                    MarkDirty();
                }
            }
        }

        public string Keywords
        {
            get { return _keywords; }
            set
            {
                if (value != _keywords)
                {
                    _keywords = value;
                    MarkDirty();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    MarkDirty();
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    MarkDirty();
                }
            }
        }

        public string URL
        {
            get { return _url; }
            set
            {
                if (value != _url)
                {
                    _url = value;
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
                return IsValid && IsDirty && _wordsInFileColl.IsValid;
            }
        }

        #endregion //Business Properties and Methods

        #region System.Object Overrides
        public override string ToString()
        {
            return "File" + "/" + _id.ToString();
        }

        public bool Equals(File file)
        {
            return _id.Equals(file.ID);
        }

        public override int GetHashCode()
        {
            return ("File" + "/" + _id.ToString()).GetHashCode();
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

        public static File NewFile()
        {
            return (File)DataPortal.Create(new Criteria());
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

            cm.Connection = cn;
            cm.Transaction = tr;
            cm.CommandType = CommandType.StoredProcedure;

            // is not deleted object, check if this is an update or insert
            if (this.IsNew)
            {
                //perform an insert, object has not been persisted
                cm.CommandText = @"sp_InsertFile";
            }
            else
            {
                //check
            }

            cm.Parameters.AddWithValue("@URL", _url);
            cm.Parameters.AddWithValue("@Description", _description);
            cm.Parameters.AddWithValue("@Keywords", _keywords);
            cm.Parameters.AddWithValue("@Title", _title);
            cm.Parameters.AddWithValue("@FileType", _fileType);

            _id = Convert.ToInt32(cm.ExecuteScalar());

            // update child object, passing the transaction
            _wordsInFileColl.Update(tr, _id);

            // mark the object as old (persisted)
            MarkOld();
        }

        #endregion //Data Access
    }
}
