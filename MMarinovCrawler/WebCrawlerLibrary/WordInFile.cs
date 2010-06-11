using System;
using System.Linq;
using Csla;
using Csla.Data;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class WordInFile : Csla.BusinessBase<WordInFile>
    {
        #region private & public Fields

        private Int64 _wordID = 0;
        private Int64 _fileID = 0;
        private int _count = 0;

        public Int64 WordID
        {
            get { return _wordID; }
            set { _wordID = value; }
        }

        public Int64 FileID
        {
            get { return _fileID; }
            set { _fileID = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        #endregion

        #region Constructor

        private WordInFile()
        {
            MarkAsChild();
        }

        #endregion

        #region Static Methods

        public static WordInFile NewWordInFile()
        {
            return DataPortal.Create<WordInFile>();
        }

        internal static WordInFile NewWordInFile(long wordID, long fileID, int count)
        {
            WordInFile wif = DataPortal.Create<WordInFile>();
            wif.WordID = wordID;
            wif.FileID = fileID;
            wif.Count = count;
            return wif;
        }

        public static WordInFile GetWordInFile(Int64 wordID, Int64 fileID)
        {
            return DataPortal.Fetch<WordInFile>(new Criteria(wordID, fileID));
        }

        //public static WordInFile GetWordInFile(DALWebCrawler.WordInFile data)
        //{
        //    WordInFile item = new WordInFile();
        //    item.Fetch(data);
        //    return item;
        //}

        #endregion

        #region DataAccess

        #region Criteria
        [Serializable]
        private class Criteria
        {
            public enum FetchTypes
            {
                WordID,
                FileID,
                WordIDAndFileID,
                All
            }

            public FetchTypes FetchType = FetchTypes.WordIDAndFileID;

            public Int64 WordID = 0;
            public Int64 FileID = 0;


            public Criteria(Int64 wordId, Int64 fileID)
            {
                this.WordID = wordId;
                this.FileID = fileID;

                FetchType = FetchTypes.WordIDAndFileID;
            }

            public string URL = "";

            public Criteria()
            {
                FetchType = FetchTypes.All;
            }
        }

        #endregion

        #region DataPortal Create

        protected override void DataPortal_Create()
        {
            ValidationRules.CheckRules();
        }

        #endregion

        //#region DataPortal Fetch

        //protected override void DataPortal_Fetch(object criteria)
        //{
        //    using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
        //    {
        //        Criteria crit = (Criteria)criteria;
        //        var file = mgr.DataContext.sp_SelectWordsInFiles(crit.WordID, crit.FileID);

        //        Fetch((DALWebCrawler.WordsInFile)file);

        //        MarkOld();
        //    }
        //}

        //private void Fetch(DALWebCrawler.WordsInFile data)
        //{
        //    if (data == null)
        //    {
        //        return;
        //    }

        //    _wordID = data.WordID;
        //    _fileID = data.FileID;
        //    _count = data.Count;

        //    MarkOld();
        //}

        //#endregion

        #region DataPortal Insert

        private void Child_Insert()
        {
            if (!IsDirty && !IsNew)
            {
                return;
            }

            DataPortal_Insert();
        }

        private void Child_Update()
        {
            if (!IsDirty)
            {
                return;
            }

            DataPortal_Update();
        }


        protected override void DataPortal_Insert()
        {
            using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
            {
                mgr.DataContext.sp_InsertWordInFile(_wordID, _fileID, _count);

                MarkOld();
            }
        }

        #endregion

        #region DataPortal Update

        protected override void DataPortal_Update()
        {
            using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
            {
                mgr.DataContext.sp_UpdateWordInFile(_wordID, _fileID, _count);

                MarkOld();
            }
        }

        #endregion

        #endregion        
    }
}
