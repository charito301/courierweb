using System;
using System.Data.SqlClient;
using CSLA;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class WordsInFileCollection : CSLA.BusinessCollectionBase
    {
        #region Business Properties and Methods

        public WordsInFile this[int index]
        {
            get { return (WordsInFile)List[index]; }
        }

        public void Add(WordsInFile item)
        {
            if (!Contains(item))
            {
                List.Add(item);
            }
            else
                throw new Exception("WordsInFile '" + item.ToString() + "' already exist.");
        }

        /// <summary>
        /// Add new word or increase the count
        /// </summary>
        /// <param name="wordName"></param>
        public void AddOrIncrease(long wordID)
        {
            WordsInFile wif = ContainsWordID(wordID);

            if (wif == null)
            {
                List.Add(WordsInFile.NewWordsInFile(wordID));
            }
            else
            {
                wif.Count++;
            }
        }

        protected override object OnAddNew()
        {
            WordsInFile project_det = WordsInFile.NewWordsInFile();
            List.Add(project_det);
            return project_det;
        }

        public bool IsSaveable
        {
            //Since you cannot bind a control to multiple properties you need to create a property that combines the ones you need
            //In this case, bind the UI Save button Enabled property to IsSaveable. (Why save an object that has not changed?)
            get { return IsValid && IsDirty; }
        }

        internal void Update(SqlTransaction tr, long fileID)
        {
            // loop through each non-deleted child object and call its Update() method
            foreach (WordsInFile child in List)
            {
                child.Update(tr, fileID);
            }
        }

        #endregion //Business Properties and Methods

        #region Contains

        public bool Contains(WordsInFile item)
        {
            return List.Contains(item);
        }

        public bool Contains(int id)
        {
            foreach (WordsInFile child in List)
            {
                if (child.ID.Equals(id))
                    return true;
            }
            return false;
        }

        public WordsInFile ContainsWordID(long wordID)
        {
            foreach (WordsInFile child in List)
            {
                if (child.WordID.Equals(wordID))
                    return child;
            }

            return null;
        }

        #endregion //Contains

        #region Constructor
        private WordsInFileCollection()
        {
            //prevent direct creation
            AllowSort = true;
            AllowFind = true;
            AllowEdit = true;
            AllowNew = true;
        }
        #endregion //Constructor

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
        public static WordsInFileCollection NewWordsInFileCollection()
        {
            return (WordsInFileCollection)DataPortal.Create(new Criteria());
        }

        #endregion //Static Methods

        #region Data Access
        //Called by DataPortal so that we can set defaults as needed
        protected override void DataPortal_Create(object criteria)
        {
        }

        #endregion //Data Access

    }
}

