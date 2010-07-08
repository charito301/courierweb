using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class FileCollection : CSLA.BusinessCollectionBase
    {
        #region Business Properties and Methods

        public File this[int index]
        {
            get { return (File)List[index]; }
        }

        public void Add(File item)
        {
            if (!Contains(item))
            {
                List.Add(item);
            }
            else
                throw new Exception("File '" + item.ToString() + "' already exist.");
        }

        protected override object OnAddNew()
        {
            File project_det = File.NewFile();
            InnerList.Add(project_det);
            return project_det;
        }

        public bool IsSaveable
        {
            //Since you cannot bind a control to multiple properties you need to create a property that combines the ones you need
            //In this case, bind the UI Save button Enabled property to IsSaveable. (Why save an object that has not changed?)
            get { return IsValid && IsDirty; }
        }

        #endregion //Business Properties and Methods

        #region Contains

        public bool Contains(File item)
        {
            return List.Contains(item);
        }

        public bool Contains(int id)
        {
            foreach (File child in List)
            {
                if (child.ID.Equals(id))
                    return true;
            }
            return false;
        }

        #endregion //Contains

        #region Constructor
        private FileCollection()
        {
            //prevent direct creation
            AllowSort = true;
            AllowFind = true;
            AllowEdit = true;
            AllowNew = true;
        }
        #endregion //Constructor

        #region Static Methods
        public static FileCollection NewFileCollection()
        {
            return (FileCollection)DataPortal.Create(new Criteria());
        }

        #endregion //Static Methods

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

        #region Data Access
        //Called by DataPortal so that we can set defaults as needed
        protected override void DataPortal_Create(object criteria)
        {
        }

        protected override void DataPortal_Update()
        {
            // save data into db
            SqlConnection cn = new SqlConnection(Preferences.ConnectionString);
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr;

            cn.Open();

            try
            {
                tr = cn.BeginTransaction(IsolationLevel.Serializable);
                try
                {
                    // loop through each non-deleted child object and call its Update() method
                    foreach (File child in List)
                        child.Update(tr);

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
    }
}

