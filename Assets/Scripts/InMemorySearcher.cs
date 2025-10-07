//using System;
//using System.Collections.Generic;
//using Lucene.Net.Analysis.Standard;
//using Lucene.Net.Documents;
//using Lucene.Net.Index;
//using Lucene.Net.Search;
//using Lucene.Net.Store;
//using Lucene.Net.Util;
//using Lucene.Net.QueryParsers.Classic;

//public class InMemoryFileSearcher : IDisposable
//{
//    private static readonly LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
//    private readonly RAMDirectory _directory;
//    private readonly IndexWriter _writer;
//    private IndexSearcher _searcher;
//    private DirectoryReader _reader;
//    private readonly StandardAnalyzer _analyzer;

//    public InMemoryFileSearcher(IEnumerable<string> filePaths)
//    {
//        _directory = new RAMDirectory();
//        _analyzer = new StandardAnalyzer(AppLuceneVersion);

//        var config = new IndexWriterConfig(AppLuceneVersion, _analyzer);
//        _writer = new IndexWriter(_directory, config);

//        foreach (var path in filePaths)
//        {
//            var doc = new Document
//            {
//                new TextField("path", path, Field.Store.YES)
//            };
//            _writer.AddDocument(doc);
//        }

//        _writer.Commit();

//        _reader = DirectoryReader.Open(_writer, applyAllDeletes: true);
//        _searcher = new IndexSearcher(_reader);
//    }

//    public List<string> Search(string queryText, int maxResults = 50)
//    {
//        var parser = new QueryParser(AppLuceneVersion, "path", _analyzer);
//        var query = parser.Parse(queryText);

//        var hits = _searcher.Search(query, maxResults).ScoreDocs;

//        var results = new List<string>();
//        foreach (var hit in hits)
//        {
//            var foundDoc = _searcher.Doc(hit.Doc);
//            results.Add(foundDoc.Get("path"));
//        }

//        return results;
//    }

//    public void AddFile(string path)
//    {
//        var doc = new Document
//        {
//            new TextField("path", path, Field.Store.YES)
//        };
//        _writer.AddDocument(doc);
//        RefreshSearcher();
//    }

//    public void RemoveFile(string path)
//    {
//        _writer.DeleteDocuments(new Term("path", path));
//        RefreshSearcher();
//    }


//    private void RefreshSearcher()
//    {
//        _writer.Commit(); // ensure pending changes are visible

//        var newReader = DirectoryReader.OpenIfChanged(_reader);
//        if (newReader != null)
//        {
//            _reader.Dispose();
//            _reader = newReader;
//            _searcher = new IndexSearcher(_reader);
//        }
//    }


//    public void Dispose()
//    {
//        _reader?.Dispose();
//        _writer?.Dispose();
//        _directory?.Dispose();
//    }
//}