using System;
using System.Data;
using System.Data.SqlClient;
using CSLA;
using CSLA.Data;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class WordCollection : CSLA.BusinessCollectionBase
    {
        #region Business Properties and Methods

        public Word this[int index]
        {
            get { return (Word)List[index]; }
        }

        public void Add(Word item)
        {
            if (!Contains(item))
            {
                List.Add(item);
            }
            else
                throw new Exception("Word '" + item.ToString() + "' already exist.");
        }

        /// <summary>
        /// Adds a new word if not exists
        /// </summary>
        /// <param name="wordName"></param>
        /// <returns></returns>
        public Word AddWord(string wordName)
        {
            Word w = GetWord(wordName);

            if (w == null)
            {
                w = Word.NewWord(wordName);
                w.SaveOne();
                List.Add(w);
            }

            return w;
        }

        public Word GetWord(string wordName)
        {
            foreach (Word child in List)
            {
                if (child == null)
                {
                }
                if (child.WordName.Equals(wordName))
                    return child;
            }
            return null;
        }

        protected override object OnAddNew()
        {
            Word project_det = Word.NewWord();
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

        public bool Contains(Word item)
        {
            return List.Contains(item);
        }

        public bool Contains(string wordName)
        {
            foreach (Word child in List)
            {
                if (child.WordName.Equals(wordName))
                    return true;
            }
            return false;
        }


        public bool Contains(int id)
        {
            foreach (Word child in List)
            {
                if (child.ID.Equals(id))
                    return true;
            }
            return false;
        }

        #endregion //Contains

        #region Constructor
        private WordCollection()
        {
            //prevent direct creation
            AllowSort = true;
            AllowFind = true;
            AllowNew = true;
        }
        #endregion //Constructor

        #region Static Methods
        public static WordCollection NewWordCollection()
        {
            return (WordCollection)DataPortal.Create(new Criteria());
        }

        public static WordCollection GetWordCollection()
        {
            return (WordCollection)DataPortal.Fetch(new Criteria());
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

        //Called by DataPortal to load data from the database
        protected override void DataPortal_Fetch(object criteria)
        {
            //retrieve data from database
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

                    cm.CommandText = @"sp_SelectWordsAll";

                    SafeDataReader dr = new SafeDataReader(cm.ExecuteReader());
                    try
                    {
                        while (dr.Read())
                        {
                            List.Add(Word.FetchWord(dr));
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
            catch (Exception ex)
            {
                throw (ex);
            }
            finally
            {
                cn.Close();
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
                    // loop through each non-deleted child object and call its Update() method
                    foreach (Word child in List)
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

        internal void AddRange(WordCollection newWordsColl)
        {
            InnerList.AddRange(newWordsColl);
        }
    }
}

