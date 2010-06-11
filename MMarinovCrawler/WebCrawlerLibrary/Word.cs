using System;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Csla;
using Csla.Data;

namespace MMarinov.WebCrawler.Library
{
    [Serializable]
    public class Word : Csla.BusinessBase<Word>
    {
        #region Private fields

        private Int64 _id = 0;
        private string _wordName = "";

        /// <summary>Collection of files the word appears in</summary>
        private WordInFileCollection _wordInFileColl = WordInFileCollection.NewWordInFileCollection();

        #endregion

        /// <summary>
        /// The cataloged word
        /// </summary>
        public string WordName
        {
            get { return _wordName; }
            set
            {
                if (_wordName != value)
                {
                    _wordName = value;
                }
            }
        }

        public WordInFileCollection WordInFileColl
        {
            get
            {
                return _wordInFileColl;
            }
        }

        ///// <summary>Constructor with first file reference</summary>
        //public Word(string text, File infile, int position)
        //{
        //    _wordName = text;
        //    //WordInFile thefile = new WordInFile(filename, position);
        //    _wordInFileColl.Add(infile, 1);
        //}

        /// <summary>Add a file referencing this word</summary>
        public void AddWordInFile(File inFile)
        {
            WordInFile wif = _wordInFileColl.GetWordInFile(_id, inFile.ID);

            if (wif != null)
            {
                wif.Count++; //thefile.Add (position);
            }
            else
            {
                _wordInFileColl.Add(WordInFile.NewWordInFile(_id, inFile.ID, 1));
            }
        }

        #region Constructor

        private Word()
        {
            MarkAsChild();
        }

        #endregion

        #region Static Methods

        public static Word NewWord()
        {
            return DataPortal.Create<Word>();
        }

        public static Word NewWord(string wordName)
        {
            Word word = DataPortal.Create<Word>();
            word.WordName = wordName;
            return word;
        }

        //public static Word GetWord(int id)
        //{
        //    return DataPortal.Fetch<Word>(new Criteria(id));
        //}

        //public static Word GetWord(string wordName)
        //{
        //    return DataPortal.Fetch<Word>(new Criteria(wordName));
        //}

        //public static Word GetFile(DALWebCrawler.Word data)
        //{
        //    Word item = new Word();
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
                ID,
                WordName
            }

            public FetchTypes FetchType = FetchTypes.ID;

            public int ID = 0;
            public string WordName = "";

            public Criteria(int id)
            {
                this.ID = id;
                FetchType = FetchTypes.ID;
            }

            public Criteria(string wordName)
            {
                this.WordName = wordName;
                FetchType = FetchTypes.WordName;
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
        //        var word = mgr.DataContext.sp_SelectWord(((Criteria)criteria).ID);

        //        Fetch((DALWebCrawler.Word)word);

        //        MarkOld();
        //    }
        //}

        //private void Fetch(DALWebCrawler.Word data)
        //{
        //    if (data == null)
        //    {
        //        return;
        //    }

        //    _id = data.ID;
        //    _wordName = data.WordName;

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

        protected override void DataPortal_Insert()
        {
            using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
            {
                long? newID = 0;

                mgr.DataContext.sp_InsertWord(_wordName, ref newID);

                _id = newID ?? 0;

                MarkOld();
            }
        }

        #endregion

        #region DataPortal Update

        private void Child_Update()
        {
            if (!IsDirty)
            {
                return;
            }

            DataPortal_Update();
        }

        protected override void DataPortal_Update()
        {
            using (var mgr = ContextManager<DALWebCrawler.WebCrawlerDataContext>.GetManager(WebCrawler.Preferences.ConnectionString, false))
            {
                mgr.DataContext.sp_UpdateWord(_id, _wordName);

                MarkOld();
            }
        }

        #endregion

        #endregion
    }
}
