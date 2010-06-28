﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DALWebCrawler
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[System.Data.Linq.Mapping.DatabaseAttribute(Name="WebCrawler")]
	public partial class WebCrawlerDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertFile(File instance);
    partial void UpdateFile(File instance);
    partial void DeleteFile(File instance);
    partial void InsertWord(Word instance);
    partial void UpdateWord(Word instance);
    partial void DeleteWord(Word instance);
    partial void InsertWordsInFile(WordsInFile instance);
    partial void UpdateWordsInFile(WordsInFile instance);
    partial void DeleteWordsInFile(WordsInFile instance);
    partial void InsertStatistic(Statistic instance);
    partial void UpdateStatistic(Statistic instance);
    partial void DeleteStatistic(Statistic instance);
    #endregion
		
		public WebCrawlerDataContext() : 
				base(global::DALWebCrawler.Properties.Settings.Default.WebCrawlerConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public WebCrawlerDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WebCrawlerDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WebCrawlerDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public WebCrawlerDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<File> Files
		{
			get
			{
				return this.GetTable<File>();
			}
		}
		
		public System.Data.Linq.Table<Word> Words
		{
			get
			{
				return this.GetTable<Word>();
			}
		}
		
		public System.Data.Linq.Table<WordsInFile> WordsInFiles
		{
			get
			{
				return this.GetTable<WordsInFile>();
			}
		}
		
		public System.Data.Linq.Table<Statistic> Statistics
		{
			get
			{
				return this.GetTable<Statistic>();
			}
		}
		
		[Function(Name="dbo.sp_SelectFile")]
		public ISingleResult<sp_SelectFileResult> sp_SelectFile([Parameter(Name="ID", DbType="BigInt")] System.Nullable<long> iD)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), iD);
			return ((ISingleResult<sp_SelectFileResult>)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectFilesAll")]
		public ISingleResult<sp_SelectFilesAllResult> sp_SelectFilesAll()
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
			return ((ISingleResult<sp_SelectFilesAllResult>)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectFilesDynamic")]
		public int sp_SelectFilesDynamic([Parameter(Name="WhereCondition", DbType="NVarChar(500)")] string whereCondition, [Parameter(Name="OrderByExpression", DbType="NVarChar(250)")] string orderByExpression)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), whereCondition, orderByExpression);
			return ((int)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectWord")]
		public ISingleResult<sp_SelectWordResult> sp_SelectWord([Parameter(Name="ID", DbType="BigInt")] System.Nullable<long> iD)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), iD);
			return ((ISingleResult<sp_SelectWordResult>)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectWordsAll")]
		public ISingleResult<sp_SelectWordsAllResult> sp_SelectWordsAll()
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
			return ((ISingleResult<sp_SelectWordsAllResult>)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectWordsDynamic")]
		public int sp_SelectWordsDynamic([Parameter(Name="WhereCondition", DbType="NVarChar(500)")] string whereCondition, [Parameter(Name="OrderByExpression", DbType="NVarChar(250)")] string orderByExpression)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), whereCondition, orderByExpression);
			return ((int)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectWordsInFiles")]
		public ISingleResult<sp_SelectWordsInFilesResult> sp_SelectWordsInFiles([Parameter(Name="WordID", DbType="BigInt")] System.Nullable<long> wordID, [Parameter(Name="FileID", DbType="BigInt")] System.Nullable<long> fileID)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), wordID, fileID);
			return ((ISingleResult<sp_SelectWordsInFilesResult>)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_SelectWordsInFilesByWordID")]
		public ISingleResult<sp_SelectWordsInFilesByWordIDResult> sp_SelectWordsInFilesByWordID([Parameter(Name="WordID", DbType="BigInt")] System.Nullable<long> wordID)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), wordID);
			return ((ISingleResult<sp_SelectWordsInFilesByWordIDResult>)(result.ReturnValue));
		}
		
		[Function(Name="dbo.sp_TruncateAllTables")]
		public int sp_TruncateAllTables()
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
			return ((int)(result.ReturnValue));
		}
	}
	
	[Table(Name="dbo.Files")]
	public partial class File : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private long _ID;
		
		private string _URL;
		
		private string _ImportantWords;
		
		private string _WeightedWords;
		
		private byte _FileType;
		
		private EntitySet<WordsInFile> _WordsInFiles;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(long value);
    partial void OnIDChanged();
    partial void OnURLChanging(string value);
    partial void OnURLChanged();
    partial void OnImportantWordsChanging(string value);
    partial void OnImportantWordsChanged();
    partial void OnWeightedWordsChanging(string value);
    partial void OnWeightedWordsChanged();
    partial void OnFileTypeChanging(byte value);
    partial void OnFileTypeChanged();
    #endregion
		
		public File()
		{
			this._WordsInFiles = new EntitySet<WordsInFile>(new Action<WordsInFile>(this.attach_WordsInFiles), new Action<WordsInFile>(this.detach_WordsInFiles));
			OnCreated();
		}
		
		[Column(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="BigInt NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_URL", DbType="NVarChar(2500) NOT NULL", CanBeNull=false)]
		public string URL
		{
			get
			{
				return this._URL;
			}
			set
			{
				if ((this._URL != value))
				{
					this.OnURLChanging(value);
					this.SendPropertyChanging();
					this._URL = value;
					this.SendPropertyChanged("URL");
					this.OnURLChanged();
				}
			}
		}
		
		[Column(Storage="_ImportantWords", DbType="NVarChar(2500) NOT NULL", CanBeNull=false)]
		public string ImportantWords
		{
			get
			{
				return this._ImportantWords;
			}
			set
			{
				if ((this._ImportantWords != value))
				{
					this.OnImportantWordsChanging(value);
					this.SendPropertyChanging();
					this._ImportantWords = value;
					this.SendPropertyChanged("ImportantWords");
					this.OnImportantWordsChanged();
				}
			}
		}
		
		[Column(Storage="_WeightedWords", DbType="NVarChar(2500)")]
		public string WeightedWords
		{
			get
			{
				return this._WeightedWords;
			}
			set
			{
				if ((this._WeightedWords != value))
				{
					this.OnWeightedWordsChanging(value);
					this.SendPropertyChanging();
					this._WeightedWords = value;
					this.SendPropertyChanged("WeightedWords");
					this.OnWeightedWordsChanged();
				}
			}
		}
		
		[Column(Storage="_FileType", DbType="TinyInt NOT NULL")]
		public byte FileType
		{
			get
			{
				return this._FileType;
			}
			set
			{
				if ((this._FileType != value))
				{
					this.OnFileTypeChanging(value);
					this.SendPropertyChanging();
					this._FileType = value;
					this.SendPropertyChanged("FileType");
					this.OnFileTypeChanged();
				}
			}
		}
		
		[Association(Name="File_WordsInFile", Storage="_WordsInFiles", ThisKey="ID", OtherKey="FileID")]
		public EntitySet<WordsInFile> WordsInFiles
		{
			get
			{
				return this._WordsInFiles;
			}
			set
			{
				this._WordsInFiles.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_WordsInFiles(WordsInFile entity)
		{
			this.SendPropertyChanging();
			entity.File = this;
		}
		
		private void detach_WordsInFiles(WordsInFile entity)
		{
			this.SendPropertyChanging();
			entity.File = null;
		}
	}
	
	[Table(Name="dbo.Words")]
	public partial class Word : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private long _ID;
		
		private string _WordName;
		
		private EntitySet<WordsInFile> _WordsInFiles;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(long value);
    partial void OnIDChanged();
    partial void OnWordNameChanging(string value);
    partial void OnWordNameChanged();
    #endregion
		
		public Word()
		{
			this._WordsInFiles = new EntitySet<WordsInFile>(new Action<WordsInFile>(this.attach_WordsInFiles), new Action<WordsInFile>(this.detach_WordsInFiles));
			OnCreated();
		}
		
		[Column(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="BigInt NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_WordName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string WordName
		{
			get
			{
				return this._WordName;
			}
			set
			{
				if ((this._WordName != value))
				{
					this.OnWordNameChanging(value);
					this.SendPropertyChanging();
					this._WordName = value;
					this.SendPropertyChanged("WordName");
					this.OnWordNameChanged();
				}
			}
		}
		
		[Association(Name="Word_WordsInFile", Storage="_WordsInFiles", ThisKey="ID", OtherKey="WordID")]
		public EntitySet<WordsInFile> WordsInFiles
		{
			get
			{
				return this._WordsInFiles;
			}
			set
			{
				this._WordsInFiles.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_WordsInFiles(WordsInFile entity)
		{
			this.SendPropertyChanging();
			entity.Word = this;
		}
		
		private void detach_WordsInFiles(WordsInFile entity)
		{
			this.SendPropertyChanging();
			entity.Word = null;
		}
	}
	
	[Table(Name="dbo.WordsInFiles")]
	public partial class WordsInFile : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private long _WordID;
		
		private long _FileID;
		
		private int _Count;
		
		private EntityRef<File> _File;
		
		private EntityRef<Word> _Word;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnWordIDChanging(long value);
    partial void OnWordIDChanged();
    partial void OnFileIDChanging(long value);
    partial void OnFileIDChanged();
    partial void OnCountChanging(int value);
    partial void OnCountChanged();
    #endregion
		
		public WordsInFile()
		{
			this._File = default(EntityRef<File>);
			this._Word = default(EntityRef<Word>);
			OnCreated();
		}
		
		[Column(Storage="_WordID", DbType="BigInt NOT NULL", IsPrimaryKey=true)]
		public long WordID
		{
			get
			{
				return this._WordID;
			}
			set
			{
				if ((this._WordID != value))
				{
					if (this._Word.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnWordIDChanging(value);
					this.SendPropertyChanging();
					this._WordID = value;
					this.SendPropertyChanged("WordID");
					this.OnWordIDChanged();
				}
			}
		}
		
		[Column(Storage="_FileID", DbType="BigInt NOT NULL", IsPrimaryKey=true)]
		public long FileID
		{
			get
			{
				return this._FileID;
			}
			set
			{
				if ((this._FileID != value))
				{
					if (this._File.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnFileIDChanging(value);
					this.SendPropertyChanging();
					this._FileID = value;
					this.SendPropertyChanged("FileID");
					this.OnFileIDChanged();
				}
			}
		}
		
		[Column(Storage="_Count", DbType="Int NOT NULL")]
		public int Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				if ((this._Count != value))
				{
					this.OnCountChanging(value);
					this.SendPropertyChanging();
					this._Count = value;
					this.SendPropertyChanged("Count");
					this.OnCountChanged();
				}
			}
		}
		
		[Association(Name="File_WordsInFile", Storage="_File", ThisKey="FileID", OtherKey="ID", IsForeignKey=true)]
		public File File
		{
			get
			{
				return this._File.Entity;
			}
			set
			{
				File previousValue = this._File.Entity;
				if (((previousValue != value) 
							|| (this._File.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._File.Entity = null;
						previousValue.WordsInFiles.Remove(this);
					}
					this._File.Entity = value;
					if ((value != null))
					{
						value.WordsInFiles.Add(this);
						this._FileID = value.ID;
					}
					else
					{
						this._FileID = default(long);
					}
					this.SendPropertyChanged("File");
				}
			}
		}
		
		[Association(Name="Word_WordsInFile", Storage="_Word", ThisKey="WordID", OtherKey="ID", IsForeignKey=true)]
		public Word Word
		{
			get
			{
				return this._Word.Entity;
			}
			set
			{
				Word previousValue = this._Word.Entity;
				if (((previousValue != value) 
							|| (this._Word.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Word.Entity = null;
						previousValue.WordsInFiles.Remove(this);
					}
					this._Word.Entity = value;
					if ((value != null))
					{
						value.WordsInFiles.Add(this);
						this._WordID = value.ID;
					}
					else
					{
						this._WordID = default(long);
					}
					this.SendPropertyChanged("Word");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="dbo.[Statistics]")]
	public partial class Statistic : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.DateTime _StartDate;
		
		private string _Duration;
		
		private long _Words;
		
		private long _FoundTotalLinks;
		
		private long _FoundValidLinks;
		
		private long _CrawledTotalLinks;
		
		private long _CrawledSuccessfulLinks;
		
		private string _ProcessDescription;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnStartDateChanging(System.DateTime value);
    partial void OnStartDateChanged();
    partial void OnDurationChanging(string value);
    partial void OnDurationChanged();
    partial void OnWordsChanging(long value);
    partial void OnWordsChanged();
    partial void OnFoundTotalLinksChanging(long value);
    partial void OnFoundTotalLinksChanged();
    partial void OnFoundValidLinksChanging(long value);
    partial void OnFoundValidLinksChanged();
    partial void OnCrawledTotalLinksChanging(long value);
    partial void OnCrawledTotalLinksChanged();
    partial void OnCrawledSuccessfulLinksChanging(long value);
    partial void OnCrawledSuccessfulLinksChanged();
    partial void OnProcessDescriptionChanging(string value);
    partial void OnProcessDescriptionChanged();
    #endregion
		
		public Statistic()
		{
			OnCreated();
		}
		
		[Column(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[Column(Storage="_StartDate", DbType="DateTime NOT NULL")]
		public System.DateTime StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				if ((this._StartDate != value))
				{
					this.OnStartDateChanging(value);
					this.SendPropertyChanging();
					this._StartDate = value;
					this.SendPropertyChanged("StartDate");
					this.OnStartDateChanged();
				}
			}
		}
		
		[Column(Storage="_Duration", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Duration
		{
			get
			{
				return this._Duration;
			}
			set
			{
				if ((this._Duration != value))
				{
					this.OnDurationChanging(value);
					this.SendPropertyChanging();
					this._Duration = value;
					this.SendPropertyChanged("Duration");
					this.OnDurationChanged();
				}
			}
		}
		
		[Column(Storage="_Words", DbType="BigInt NOT NULL")]
		public long Words
		{
			get
			{
				return this._Words;
			}
			set
			{
				if ((this._Words != value))
				{
					this.OnWordsChanging(value);
					this.SendPropertyChanging();
					this._Words = value;
					this.SendPropertyChanged("Words");
					this.OnWordsChanged();
				}
			}
		}
		
		[Column(Storage="_FoundTotalLinks", DbType="BigInt NOT NULL")]
		public long FoundTotalLinks
		{
			get
			{
				return this._FoundTotalLinks;
			}
			set
			{
				if ((this._FoundTotalLinks != value))
				{
					this.OnFoundTotalLinksChanging(value);
					this.SendPropertyChanging();
					this._FoundTotalLinks = value;
					this.SendPropertyChanged("FoundTotalLinks");
					this.OnFoundTotalLinksChanged();
				}
			}
		}
		
		[Column(Storage="_FoundValidLinks", DbType="BigInt NOT NULL")]
		public long FoundValidLinks
		{
			get
			{
				return this._FoundValidLinks;
			}
			set
			{
				if ((this._FoundValidLinks != value))
				{
					this.OnFoundValidLinksChanging(value);
					this.SendPropertyChanging();
					this._FoundValidLinks = value;
					this.SendPropertyChanged("FoundValidLinks");
					this.OnFoundValidLinksChanged();
				}
			}
		}
		
		[Column(Storage="_CrawledTotalLinks", DbType="BigInt NOT NULL")]
		public long CrawledTotalLinks
		{
			get
			{
				return this._CrawledTotalLinks;
			}
			set
			{
				if ((this._CrawledTotalLinks != value))
				{
					this.OnCrawledTotalLinksChanging(value);
					this.SendPropertyChanging();
					this._CrawledTotalLinks = value;
					this.SendPropertyChanged("CrawledTotalLinks");
					this.OnCrawledTotalLinksChanged();
				}
			}
		}
		
		[Column(Storage="_CrawledSuccessfulLinks", DbType="BigInt NOT NULL")]
		public long CrawledSuccessfulLinks
		{
			get
			{
				return this._CrawledSuccessfulLinks;
			}
			set
			{
				if ((this._CrawledSuccessfulLinks != value))
				{
					this.OnCrawledSuccessfulLinksChanging(value);
					this.SendPropertyChanging();
					this._CrawledSuccessfulLinks = value;
					this.SendPropertyChanged("CrawledSuccessfulLinks");
					this.OnCrawledSuccessfulLinksChanged();
				}
			}
		}
		
		[Column(Storage="_ProcessDescription", DbType="VarChar(250)")]
		public string ProcessDescription
		{
			get
			{
				return this._ProcessDescription;
			}
			set
			{
				if ((this._ProcessDescription != value))
				{
					this.OnProcessDescriptionChanging(value);
					this.SendPropertyChanging();
					this._ProcessDescription = value;
					this.SendPropertyChanged("ProcessDescription");
					this.OnProcessDescriptionChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	public partial class sp_SelectFileResult
	{
		
		private long _ID;
		
		private string _URL;
		
		private string _ImportantWords;
		
		private string _WeightedWords;
		
		private byte _FileType;
		
		public sp_SelectFileResult()
		{
		}
		
		[Column(Storage="_ID", DbType="BigInt NOT NULL")]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[Column(Storage="_URL", DbType="NVarChar(2500) NOT NULL", CanBeNull=false)]
		public string URL
		{
			get
			{
				return this._URL;
			}
			set
			{
				if ((this._URL != value))
				{
					this._URL = value;
				}
			}
		}
		
		[Column(Storage="_ImportantWords", DbType="NVarChar(2500) NOT NULL", CanBeNull=false)]
		public string ImportantWords
		{
			get
			{
				return this._ImportantWords;
			}
			set
			{
				if ((this._ImportantWords != value))
				{
					this._ImportantWords = value;
				}
			}
		}
		
		[Column(Storage="_WeightedWords", DbType="NVarChar(2500)")]
		public string WeightedWords
		{
			get
			{
				return this._WeightedWords;
			}
			set
			{
				if ((this._WeightedWords != value))
				{
					this._WeightedWords = value;
				}
			}
		}
		
		[Column(Storage="_FileType", DbType="TinyInt NOT NULL")]
		public byte FileType
		{
			get
			{
				return this._FileType;
			}
			set
			{
				if ((this._FileType != value))
				{
					this._FileType = value;
				}
			}
		}
	}
	
	public partial class sp_SelectFilesAllResult
	{
		
		private long _ID;
		
		private string _URL;
		
		private string _ImportantWords;
		
		private string _WeightedWords;
		
		private byte _FileType;
		
		public sp_SelectFilesAllResult()
		{
		}
		
		[Column(Storage="_ID", DbType="BigInt NOT NULL")]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[Column(Storage="_URL", DbType="NVarChar(2500) NOT NULL", CanBeNull=false)]
		public string URL
		{
			get
			{
				return this._URL;
			}
			set
			{
				if ((this._URL != value))
				{
					this._URL = value;
				}
			}
		}
		
		[Column(Storage="_ImportantWords", DbType="NVarChar(2500) NOT NULL", CanBeNull=false)]
		public string ImportantWords
		{
			get
			{
				return this._ImportantWords;
			}
			set
			{
				if ((this._ImportantWords != value))
				{
					this._ImportantWords = value;
				}
			}
		}
		
		[Column(Storage="_WeightedWords", DbType="NVarChar(2500)")]
		public string WeightedWords
		{
			get
			{
				return this._WeightedWords;
			}
			set
			{
				if ((this._WeightedWords != value))
				{
					this._WeightedWords = value;
				}
			}
		}
		
		[Column(Storage="_FileType", DbType="TinyInt NOT NULL")]
		public byte FileType
		{
			get
			{
				return this._FileType;
			}
			set
			{
				if ((this._FileType != value))
				{
					this._FileType = value;
				}
			}
		}
	}
	
	public partial class sp_SelectWordResult
	{
		
		private long _ID;
		
		private string _WordName;
		
		public sp_SelectWordResult()
		{
		}
		
		[Column(Storage="_ID", DbType="BigInt NOT NULL")]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[Column(Storage="_WordName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string WordName
		{
			get
			{
				return this._WordName;
			}
			set
			{
				if ((this._WordName != value))
				{
					this._WordName = value;
				}
			}
		}
	}
	
	public partial class sp_SelectWordsAllResult
	{
		
		private long _ID;
		
		private string _WordName;
		
		public sp_SelectWordsAllResult()
		{
		}
		
		[Column(Storage="_ID", DbType="BigInt NOT NULL")]
		public long ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this._ID = value;
				}
			}
		}
		
		[Column(Storage="_WordName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string WordName
		{
			get
			{
				return this._WordName;
			}
			set
			{
				if ((this._WordName != value))
				{
					this._WordName = value;
				}
			}
		}
	}
	
	public partial class sp_SelectWordsInFilesResult
	{
		
		private long _WordID;
		
		private long _FileID;
		
		private int _Count;
		
		public sp_SelectWordsInFilesResult()
		{
		}
		
		[Column(Storage="_WordID", DbType="BigInt NOT NULL")]
		public long WordID
		{
			get
			{
				return this._WordID;
			}
			set
			{
				if ((this._WordID != value))
				{
					this._WordID = value;
				}
			}
		}
		
		[Column(Storage="_FileID", DbType="BigInt NOT NULL")]
		public long FileID
		{
			get
			{
				return this._FileID;
			}
			set
			{
				if ((this._FileID != value))
				{
					this._FileID = value;
				}
			}
		}
		
		[Column(Storage="_Count", DbType="Int NOT NULL")]
		public int Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				if ((this._Count != value))
				{
					this._Count = value;
				}
			}
		}
	}
	
	public partial class sp_SelectWordsInFilesByWordIDResult
	{
		
		private long _WordID;
		
		private long _FileID;
		
		private int _Count;
		
		public sp_SelectWordsInFilesByWordIDResult()
		{
		}
		
		[Column(Storage="_WordID", DbType="BigInt NOT NULL")]
		public long WordID
		{
			get
			{
				return this._WordID;
			}
			set
			{
				if ((this._WordID != value))
				{
					this._WordID = value;
				}
			}
		}
		
		[Column(Storage="_FileID", DbType="BigInt NOT NULL")]
		public long FileID
		{
			get
			{
				return this._FileID;
			}
			set
			{
				if ((this._FileID != value))
				{
					this._FileID = value;
				}
			}
		}
		
		[Column(Storage="_Count", DbType="Int NOT NULL")]
		public int Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				if ((this._Count != value))
				{
					this._Count = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
