#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IElasticContext.cs </Name>
//         <Created> 17/10/17 8:39:55 PM </Created>
//         <Key> ac1501cf-4312-4888-a5c1-3149812c3dcd </Key>
//     </File>
//     <Summary>
//         IElasticContext.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Elastic.ContextAddDeleteUpdate.IndexModel;
using Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel;
using Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using Puppy.Elastic.ContextAlias.AliasModel;
using Puppy.Elastic.ContextSearch;
using Puppy.Elastic.ContextSearch.SearchModel;
using Puppy.Elastic.ContextWarmers;
using Puppy.Elastic.Model;
using System;
using System.Threading.Tasks;

namespace Puppy.Elastic
{
    public interface IElasticContext
    {
        /// <summary>
        ///     Adds a document to the pending changes list. Nor HTTP request is sent with this
        ///     method. If the save changes is not called, the document is not added or updated in
        ///     the search engine
        /// </summary>
        /// <param name="document">          Document to be added or updated </param>
        /// <param name="id">                document id </param>
        /// <param name="routingDefinition">
        ///     parent id of the document. This is only used if the
        ///     ElasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex property is
        ///     set to true. The document is then saved with the parent routing. It will also be
        ///     saved even if the parent does not exist.
        /// </param>
        void UpsertDocument(object document, object id, RoutingDefinition routingDefinition = null);

        /// <summary>
        ///     Adds a document to the pending changes list to be deleted. Nor HTTP request is sent
        ///     with this method. If the save changes is not called, the document is not added or
        ///     updated in the search engine
        /// </summary>
        /// <typeparam name="T"> This type is used to get the index and type of the document </typeparam>
        /// <param name="id">                id of the document which will be deleted. </param>
        /// <param name="routingDefinition"></param>
        void DeleteDocument<T>(object id, RoutingDefinition routingDefinition = null);

        /// <summary>
        ///     Saves all the changes in the pending list of changes, add, update and delete. It also
        ///     creates mappings and indexes if the child documents are saved as separate index
        ///     types. ElasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex= true
        /// </summary>
        /// <returns> Returns HTTP response information </returns>
        ResultDetails<string> SaveChangesAndInitMappings();

        /// <summary>
        ///     Saves all the changes in the pending list of changes, add, update and delete. No
        ///     mappings are created here for child document types.
        /// </summary>
        /// <returns> Returns HTTP response information </returns>
        ResultDetails<string> SaveChanges();

        /// <summary>
        ///     async Saves all the changes in the pending list of changes, add, update and delete.
        ///     No mappings are created here for child document types.
        /// </summary>
        /// <returns> Returns HTTP response information </returns>
        Task<ResultDetails<string>> SaveChangesAsync();

        /// <summary>
        ///     The optimize API allows to optimize one or more indices through an API. The optimize
        ///     process basically optimizes the index for faster search operations (and relates to
        ///     the number of segments a Lucene index holds within each shard). The optimize
        ///     operation allows to reduce the number of segments by merging them.
        /// </summary>
        /// <param name="index">              index to optimize </param>
        /// <param name="optimizeParameters"> all the possible parameters </param>
        /// <returns> number of successfully optimized </returns>
        ResultDetails<OptimizeResult> IndexOptimize(string index = null,
            OptimizeParameters optimizeParameters = null);

        /// <summary>
        ///     Async The optimize API allows to optimize one or more indices through an API. The
        ///     optimize process basically optimizes the index for faster search operations (and
        ///     relates to the number of segments a Lucene index holds within each shard). The
        ///     optimize operation allows to reduce the number of segments by merging them.
        /// </summary>
        /// <param name="index">              index to optimize </param>
        /// <param name="optimizeParameters"> all the possible parameters </param>
        /// <returns> number of successfully optimized </returns>
        Task<ResultDetails<OptimizeResult>> IndexOptimizeAsync(string index = null,
            OptimizeParameters optimizeParameters = null);

        /// <summary>
        ///     The open and close index APIs allow to close an index, and later on opening it. A
        ///     closed index has almost no overhead on the cluster (except for maintaining its
        ///     metadata), and is blocked for read/write operations. A closed index can be opened
        ///     which will then go through the normal recovery process.
        /// </summary>
        /// <param name="index"> index to be closed </param>
        /// <returns> true ids successfully </returns>
        ResultDetails<bool> IndexClose(string index);

        /// <summary>
        ///     Async The open and close index APIs allow to close an index, and later on opening it.
        ///     A closed index has almost no overhead on the cluster (except for maintaining its
        ///     metadata), and is blocked for read/write operations. A closed index can be opened
        ///     which will then go through the normal recovery process.
        /// </summary>
        /// <param name="index"> index to be closed </param>
        /// <returns> true ids successfully </returns>
        Task<ResultDetails<bool>> IndexCloseAsync(string index);

        /// <summary>
        ///     The open and close index APIs allow to close an index, and later on opening it. A
        ///     closed index has almost no overhead on the cluster (except for maintaining its
        ///     metadata), and is blocked for read/write operations. A closed index can be opened
        ///     which will then go through the normal recovery process.
        /// </summary>
        /// <param name="index"> index to be opened </param>
        /// <returns> true ids successfully </returns>
        ResultDetails<bool> IndexOpen(string index);

        /// <summary>
        ///     Async The open and close index APIs allow to close an index, and later on opening it.
        ///     A closed index has almost no overhead on the cluster (except for maintaining its
        ///     metadata), and is blocked for read/write operations. A closed index can be opened
        ///     which will then go through the normal recovery process.
        /// </summary>
        /// <param name="index"> index to be opened </param>
        /// <returns> true ids successfully </returns>
        Task<ResultDetails<bool>> IndexOpenAsync(string index);

        /// <summary>
        ///     Change specific index level settings in real time Can change a single index or global changes
        /// </summary>
        /// <param name="indexUpdateSettings"> index settings, see properties doc for details </param>
        /// <param name="index">              
        ///     index to be updated, if null, update all indices
        /// </param>
        /// <returns> string with details </returns>
        ResultDetails<string> IndexUpdateSettings(IndexUpdateSettings indexUpdateSettings, string index = null);

        /// <summary>
        ///     Async Change specific index level settings in real time Can change a single index or
        ///     global changes
        /// </summary>
        /// <param name="indexUpdateSettings"> index settings, see properties doc for details </param>
        /// <param name="index">              
        ///     index to be updated, if null, update all indexes
        /// </param>
        /// <returns> string with details </returns>
        Task<ResultDetails<string>> IndexUpdateSettingsAsync(IndexUpdateSettings indexUpdateSettings,
            string index = null);

        /// <summary>
        ///     Creates a new index 
        /// </summary>
        /// <param name="index">         index name to lower string! </param>
        /// <param name="indexSettings"> settings for the new index </param>
        /// <param name="indexAliases">  Define aliases for the index at creation time </param>
        /// <param name="indexWarmers">  Warmers for index or type </param>
        /// <returns> details </returns>
        ResultDetails<string> IndexCreate(string index, IndexSettings indexSettings = null,
            IndexAliases indexAliases = null, IndexWarmers indexWarmers = null);

        /// <summary>
        ///     Async Creates a new index 
        /// </summary>
        /// <param name="index">         index name to lower string! </param>
        /// <param name="indexSettings"> settings for the new index </param>
        /// <param name="indexAliases">  Define aliases for the index at creation time </param>
        /// <param name="indexWarmers">  Warmers for index or type </param>
        /// <returns> details </returns>
        Task<ResultDetails<string>> IndexCreateAsync(string index, IndexSettings indexSettings = null,
            IndexAliases indexAliases = null, IndexWarmers indexWarmers = null);

        /// <summary>
        ///     Creates a new index from a Type and also all the property mappings and index definitions
        ///     Note: Child objects cannot be an interface if the mapping should be created first.
        /// </summary>
        /// <param name="indexDefinition"> settings for the new index </param>
        /// <returns> details </returns>
        ResultDetails<string> IndexCreate<T>(IndexDefinition indexDefinition = null);

        /// <summary>
        ///     Async Creates a new index from a Type and also all the property mappings and index definitions
        /// </summary>
        /// <param name="indexDefinition"> settings for the new index </param>
        /// <returns> details </returns>
        Task<ResultDetails<string>> IndexCreateAsync<T>(IndexDefinition indexDefinition = null);

        /// <summary>
        ///     Creates property mappings for an existing index 
        /// </summary>
        /// <typeparam name="T"> Type for the mapping </typeparam>
        /// <param name="mappingDefinition"> Routing index definitions </param>
        /// <returns> details of the request </returns>
        ResultDetails<string> IndexCreateTypeMapping<T>(MappingDefinition mappingDefinition);

        /// <summary>
        ///     Async Creates property mappings for an existing index 
        /// </summary>
        /// <typeparam name="T"> Type for the mapping </typeparam>
        /// <param name="mappingDefinition"> Routing index definitions </param>
        /// <returns> details of the request </returns>
        Task<ResultDetails<string>> IndexCreateTypeMappingAsync<T>(MappingDefinition mappingDefinition);

        /// <summary>
        ///     Gets a document by id. Elastic GET API 
        /// </summary>
        /// <typeparam name="T"> type used for the document index and type definition </typeparam>
        /// <param name="documentId">        document id </param>
        /// <param name="routingDefinition">
        ///     Parent Id of the document if document is a child document Id the Id is incorrect, you
        ///     may still receive the child document (parentId might be saved to the same shard.) If
        ///     the child is a child document and no parent id is set, no document will be found.
        /// </param>
        /// <returns> Document type T </returns>
        T GetDocument<T>(object documentId, RoutingDefinition routingDefinition = null);

        /// <summary>
        ///     Uses Elastic search API to get the document per id 
        /// </summary>
        /// <typeparam name="T"> type T used to get index anf the type of the document. </typeparam>
        /// <param name="documentId">         </param>
        /// <param name="searchUrlParameters"> add routing or pretty parameters if required </param>
        /// <returns> Returns the document of type T </returns>
        T SearchById<T>(object documentId, SearchUrlParameters searchUrlParameters = null);

        GetResult Get(Uri uri);

        /// <summary>
        ///     async Uses Elastic search API to get the document per id 
        /// </summary>
        /// <typeparam name="T"> type T used to get index anf the type of the document. </typeparam>
        /// <param name="documentId">         </param>
        /// <param name="searchUrlParameters"> add routing or pretty parameters if required </param>
        /// <returns> Returns the document of type T in a Task with result details </returns>
        Task<ResultDetails<T>> SearchByIdAsync<T>(object documentId,
            SearchUrlParameters searchUrlParameters = null);

        /// <summary>
        ///     Search API method to search for anything. Any json string which matches the Elastic
        ///     Search API can be used. Only single index and type search
        /// </summary>
        /// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
        /// <param name="searchJsonParameters">
        ///     JSON string which matches the Elastic Search API
        /// </param>
        /// <param name="searchUrlParameters">  add routing or pretty parameters if required </param>
        /// <returns> A collection of documents of type T </returns>
        ResultDetails<SearchResult<T>> Search<T>(string searchJsonParameters,
            SearchUrlParameters searchUrlParameters = null);

        /// <summary>
        ///     Search API method to search for anything. Any json string which matches the Elastic
        ///     Search API can be used. Only single index and type search
        /// </summary>
        /// <typeparam name="T"> Type T used for the index and type used in the search </typeparam>
        /// <param name="search">              search body for Elastic Search API </param>
        /// <param name="searchUrlParameters"> add routing or pretty parameters if required </param>
        /// <returns> A collection of documents of type T </returns>
        ResultDetails<SearchResult<T>> Search<T>(Model.SearchModel.Search search,
            SearchUrlParameters searchUrlParameters = null);

        /// <summary>
        ///     async Search API method to search for anything. Any json string which matches the
        ///     Elastic Search API can be used. Only single index and type search
        /// </summary>
        /// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
        /// <param name="searchJsonParameters">
        ///     JSON string which matches the Elastic Search API
        /// </param>
        /// <param name="searchUrlParameters">  add routing or pretty parameters if required </param>
        /// <returns> A collection of documents of type T in a Task </returns>
        Task<ResultDetails<SearchResult<T>>> SearchAsync<T>(string searchJsonParameters,
            SearchUrlParameters searchUrlParameters = null);

        Task<ResultDetails<SearchResult<T>>> SearchAsync<T>(Model.SearchModel.Search search,
            SearchUrlParameters searchUrlParameters = null);

        /// <summary>
        ///     Search API method to search for anything. Any json string which matches the Elastic
        ///     Search API can be used. Only single index and type search
        /// </summary>
        /// <typeparam name="T"> Type T used for the index and type used in the search </typeparam>
        /// <param name="scrollId">                  
        ///     If this search is part of a scan and scroll, you can add the scrollId to open the context
        /// </param>
        /// <param name="scanAndScrollConfiguration"> Required scroll configuration </param>
        /// <returns> A collection of documents of type T </returns>
        ResultDetails<SearchResult<T>> SearchScanAndScroll<T>(string scrollId,
            ScanAndScrollConfiguration scanAndScrollConfiguration);

        /// <summary>
        ///     async Search API method to search for anything. Any json string which matches the
        ///     Elastic Search API can be used. Only single index and type search
        /// </summary>
        /// <typeparam name="T"> Type T used for the index and type used in the search </typeparam>
        /// <param name="scrollId">                  
        ///     If this search is part of a scan and scroll, you can add the scrollId to open the context
        /// </param>
        /// <param name="scanAndScrollConfiguration"> Required scroll configuration </param>
        /// <returns> A collection of documents of type T in a Task </returns>
        Task<ResultDetails<SearchResult<T>>> SearchScanAndScrollAsync<T>(string scrollId,
            ScanAndScrollConfiguration scanAndScrollConfiguration);

        /// <summary>
        ///     executes a post request to checks if at least one document exists for the search query.
        /// </summary>
        /// <typeparam name="T"> Type used to define the type and index in elastic search </typeparam>
        /// <param name="searchJsonParameters"> json query for elastic </param>
        /// <param name="routing">              routing used for the search </param>
        /// <returns> true if one document exists for the search query </returns>
        bool SearchExists<T>(string searchJsonParameters, string routing = null);

        /// <summary>
        ///     executes a post request to checks if at least one document exists for the search query.
        /// </summary>
        /// <typeparam name="T"> Type used to define the type and index in elastic search </typeparam>
        /// <param name="search">  search body for Elastic Search API </param>
        /// <param name="routing"> routing used for the search </param>
        /// <returns> true if one document exists for the search query </returns>
        bool SearchExists<T>(Model.SearchModel.Search search, string routing = null);

        /// <summary>
        ///     async executes a post request to checks if at least one document exists for the
        ///     search query.
        /// </summary>
        /// <typeparam name="T"> Type used to define the type and index in elastic search </typeparam>
        /// <param name="searchJsonParameters"> json query for elastic </param>
        /// <param name="routing">              routing used for the search </param>
        /// <returns> true if one document exists for the search query </returns>
        Task<ResultDetails<bool>> SearchExistsAsync<T>(string searchJsonParameters, string routing = null);

        /// <summary>
        ///     async executes a post request to checks if at least one document exists for the
        ///     search query.
        /// </summary>
        /// <typeparam name="T"> Type used to define the type and index in elastic search </typeparam>
        /// <param name="search">  search body for Elastic Search API </param>
        /// <param name="routing"> routing used for the search </param>
        /// <returns> true if one document exists for the search query </returns>
        Task<ResultDetails<bool>> SearchExistsAsync<T>(Model.SearchModel.Search search,
            string routing = null);

        /// <summary>
        ///     Creates a new scan and scroll search. Takes the query json content and returns a
        ///     _scroll_id in the payload for the following searches. If your doing a live
        ///     re-indexing, you should use a timestamp in the json content query.
        /// </summary>
        /// <typeparam name="T"> index and type form search scan and scroll </typeparam>
        /// <param name="jsonContent">                query which will be saved. </param>
        /// <param name="scanAndScrollConfiguration">
        ///     The scan and scroll configuration, for example scroll in time units
        /// </param>
        /// <returns>
        ///     Returns the _scroll_id in the Payload property and the total number of hits.
        /// </returns>
        ResultDetails<SearchResult<T>> SearchCreateScanAndScroll<T>(string jsonContent,
            ScanAndScrollConfiguration scanAndScrollConfiguration);

        /// <summary>
        ///     Creates a new scan and scroll search. Takes the query json content and returns a
        ///     _scroll_id in the payload for the following searches. If your doing a live
        ///     re-indexing, you should use a timestamp in the json content query.
        /// </summary>
        /// <typeparam name="T"> index and type form search scan and scroll </typeparam>
        /// <param name="search">                     search body for Elastic Search API </param>
        /// <param name="scanAndScrollConfiguration">
        ///     The scan and scroll configuration, for example scroll in time units
        /// </param>
        /// <returns>
        ///     Returns the _scroll_id in the Payload property and the total number of hits.
        /// </returns>
        ResultDetails<SearchResult<T>> SearchCreateScanAndScroll<T>(Model.SearchModel.Search search,
            ScanAndScrollConfiguration scanAndScrollConfiguration);

        /// <summary>
        ///     Async Creates a new scan and scroll search. Takes the query json content and returns
        ///     a _scroll_id in the payload for the following searches. If your doing a live
        ///     re-indexing, you should use a timestamp in the json content query.
        /// </summary>
        /// <typeparam name="T"> index and type form search scan and scroll </typeparam>
        /// <param name="jsonContent">                query which will be saved. </param>
        /// <param name="scanAndScrollConfiguration">
        ///     The scan and scroll configuration, for example scroll in time units
        /// </param>
        /// <returns>
        ///     Returns the _scroll_id in the Payload property and the total number of hits.
        /// </returns>
        Task<ResultDetails<SearchResult<T>>> SearchCreateScanAndScrollAsync<T>(string jsonContent,
            ScanAndScrollConfiguration scanAndScrollConfiguration);

        /// <summary>
        ///     Async Creates a new scan and scroll search. Takes the query json content and returns
        ///     a _scroll_id in the payload for the following searches. If your doing a live
        ///     re-indexing, you should use a timestamp in the json content query.
        /// </summary>
        /// <typeparam name="T"> index and type form search scan and scroll </typeparam>
        /// <param name="search">                     search body for Elastic Search API </param>
        /// <param name="scanAndScrollConfiguration">
        ///     The scan and scroll configuration, for example scroll in time units
        /// </param>
        /// <returns>
        ///     Returns the _scroll_id in the Payload property and the total number of hits.
        /// </returns>
        Task<ResultDetails<SearchResult<T>>> SearchCreateScanAndScrollAsync<T>(
            Model.SearchModel.Search search, ScanAndScrollConfiguration scanAndScrollConfiguration);

        /// <summary>
        ///     ElasticContextCount to amount of hits for a index, type and query. 
        /// </summary>
        /// <typeparam name="T"> Type to find </typeparam>
        /// <param name="jsonContent"> json query data, returns all in empty </param>
        /// <returns> Result amount of document found </returns>
        long Count<T>(string jsonContent = "");

        /// <summary>
        ///     ElasticContextCount to amount of hits for a index, type and query. 
        /// </summary>
        /// <typeparam name="T"> Type to find </typeparam>
        /// <param name="search"> search body for Elastic Search API </param>
        /// <returns> Result amount of document found </returns>
        long Count<T>(Model.SearchModel.Search search);

        /// <summary>
        ///     ElasticContextCount to amount of hits for a index, type and query. 
        /// </summary>
        /// <typeparam name="T"> Type to find </typeparam>
        /// <param name="jsonContent"> json query data, returns all in empty </param>
        /// <returns> Result amount of document found in a result details task </returns>
        Task<ResultDetails<long>> CountAsync<T>(string jsonContent = "");

        /// <summary>
        ///     ElasticContextCount to amount of hits for a index, type and query. 
        /// </summary>
        /// <typeparam name="T"> Type to find </typeparam>
        /// <param name="search"> search body for Elastic Search API </param>
        /// <returns> Result amount of document found in a result details task </returns>
        Task<ResultDetails<long>> CountAsync<T>(Model.SearchModel.Search search);

        /// <summary>
        ///     Async Deletes all documents found using the query in the body. 
        /// </summary>
        /// <typeparam name="T"> Type used to define the index and the type in Elastic </typeparam>
        /// <param name="jsonContent"> json string using directly in Elastic API. </param>
        /// <returns> Returns true if ok </returns>
        Task<ResultDetails<bool>> DeleteByQueryAsync<T>(string jsonContent);

        /// <summary>
        ///     Deletes all documents found using the query in the body. 
        /// </summary>
        /// <typeparam name="T"> Type used to define the index and the type in Elastic </typeparam>
        /// <param name="jsonContent"> json string using directly in Elastic API. </param>
        /// <returns> Returns true if ok </returns>
        ResultDetails<bool> DeleteByQuery<T>(string jsonContent);

        /// <summary>
        ///     Deletes all documents found using the query in the body. 
        /// </summary>
        /// <typeparam name="T"> Type used to define the index and the type in Elastic </typeparam>
        /// <param name="search"> search body for Elastic Search API </param>
        /// <returns> Returns true if ok </returns>
        ResultDetails<bool> DeleteByQuery<T>(Model.SearchModel.Search search);

        /// <summary>
        ///     Gets a document by id. Elastic GET API 
        /// </summary>
        /// <typeparam name="T"> type used for the document index and type definition </typeparam>
        /// <param name="documentId">        document id </param>
        /// <param name="routingDefinition">
        ///     Parent Id of the document if document is a child document Id the Id is incorrect, you
        ///     may still receive the child document (parentId might be saved to the same shard.) If
        ///     the child is a child document and no parent id is set, no document will be found.
        /// </param>
        /// <returns> Document type T in a Task with result details </returns>
        Task<ResultDetails<T>> GetDocumentAsync<T>(object documentId,
            RoutingDefinition routingDefinition = null);

        /// <summary>
        ///     Checks if a document exists with a head request 
        /// </summary>
        /// <typeparam name="T"> Type of document to find </typeparam>
        /// <param name="documentId">        Id of the document </param>
        /// <param name="routingDefinition">
        ///     parent Id, required if the document is a child document and routing is required.
        ///     NOTE: if the parent Id is incorrect but save on the same shard as the correct
        ///           parentId, the document will be found!
        /// </param>
        /// <returns> true or false </returns>
        bool DocumentExists<T>(object documentId, RoutingDefinition routingDefinition = null);

        /// <summary>
        ///     Async Checks if a document exists with a head request 
        /// </summary>
        /// <typeparam name="T"> Type of document to find </typeparam>
        /// <param name="documentId">        Id of the document </param>
        /// <param name="routingDefinition">
        ///     parent Id, required if the document is a child document and routing is required.
        ///     NOTE: if the parent Id is incorrect but save on the same shard as the correct
        ///           parentId, the document will be found!
        /// </param>
        /// <returns> true or false </returns>
        Task<ResultDetails<bool>> DocumentExistsAsync<T>(object documentId,
            RoutingDefinition routingDefinition = null);

        /// <summary>
        ///     Send a HEAD request to Elastic search to find out if the item defined in the URL exists
        /// </summary>
        /// <param name="uri"> Full URI of Elastic search plus item </param>
        /// <returns> true if it exists false for 404 </returns>
        bool Exists(Uri uri);

        /// <summary>
        ///     Async Send a HEAD request to Elastic search to find out if the item defined in the
        ///     URL exists
        /// </summary>
        /// <param name="uri"> Full URI of Elastic search plus item </param>
        /// <returns> true if it exists false for 404 </returns>
        Task<ResultDetails<bool>> ExistsAsync(Uri uri);

        /// <summary>
        ///     async Checks if a index exists for the type T 
        /// </summary>
        /// <typeparam name="T">
        ///     Type used for the index exists HEAD request. Gets the index using the mapping
        /// </typeparam>
        /// <returns> true if it exists false for 404 </returns>
        bool IndexExists<T>();

        /// <summary>
        ///     async Checks if a index exists for the type T 
        /// </summary>
        /// <typeparam name="T">
        ///     Type used for the index exists HEAD request. Gets the index using the mapping
        /// </typeparam>
        /// <returns> true if it exists false for 404 </returns>
        Task<ResultDetails<bool>> IndexExistsAsync<T>();

        /// <summary>
        ///     Checks if a type exists for an index for the type T 
        /// </summary>
        /// <typeparam name="T">
        ///     Type used for the index + plus exists HEAD request. Gets the index using the mapping
        /// </typeparam>
        /// <returns> true if it exists false for 404 </returns>
        bool IndexTypeExists<T>();

        /// <summary>
        ///     Checks if a type exists for an index for the type T 
        /// </summary>
        /// <typeparam name="T">
        ///     Type used for the index + plus exists HEAD request. Gets the index using the mapping
        /// </typeparam>
        /// <returns> true if it exists false for 404 </returns>
        Task<ResultDetails<bool>> IndexTypeExistsAsync<T>();

        /// <summary>
        ///     Checks if an alias exists for an index for the type T 
        /// </summary>
        /// <typeparam name="T">
        ///     Type used for the index + plus exists HEAD request. Gets the index using the mapping
        /// </typeparam>
        /// <returns> true if the alias exists false for 404 </returns>
        bool AliasExistsForIndex<T>(string alias);

        /// <summary>
        ///     async Checks if an alias exists for an index for the type T 
        /// </summary>
        /// <typeparam name="T">
        ///     Type used for the index + plus exists HEAD request. Gets the index using the mapping
        /// </typeparam>
        /// <returns> true if the alias exists false for 404 </returns>
        Task<ResultDetails<bool>> AliasExistsForIndexAsync<T>(string alias);

        /// <summary>
        ///     Checks if an alias exists 
        /// </summary>
        /// <returns> true if the alias exists false for 404 </returns>
        bool AliasExists(string alias);

        /// <summary>
        ///     async Checks if an alias exists 
        /// </summary>
        /// <returns> true if the alias exists false for 404 </returns>
        Task<ResultDetails<bool>> AliasExistsAsync(string alias);

        /// <summary>
        ///     Clears the cache for the index. The index is defined using the Type 
        /// </summary>
        /// <typeparam name="T"> Type used to get the index name </typeparam>
        /// <returns> returns true if cache has been cleared </returns>
        bool IndexClearCache<T>();

        /// <summary>
        ///     Clears the cache for the index. The index is defined using the Type 
        /// </summary>
        /// <returns> returns true if cache has been cleared </returns>
        bool IndexClearCache(string index);

        /// <summary>
        ///     Async Clears the cache for the index. The index is defined using the Type 
        /// </summary>
        /// <typeparam name="T"> Type used to get the index name </typeparam>
        /// <returns> returns true if cache has been cleared </returns>
        Task<ResultDetails<bool>> IndexClearCacheAsync<T>();

        /// <summary>
        ///     Creates a new alias for the index parameter. 
        /// </summary>
        /// <param name="alias"> name of the alias </param>
        /// <param name="index"> index for the alias </param>
        /// <returns> true if the alias was created </returns>
        bool AliasCreateForIndex(string alias, string index);

        /// <summary>
        ///     Async Creates a new alias for the index parameter. 
        /// </summary>
        /// <param name="alias"> name of the alias </param>
        /// <param name="index"> index for the alias </param>
        /// <returns> true if the alias was created </returns>
        Task<ResultDetails<bool>> AliasCreateForIndexAsync(string alias, string index);

        /// <summary>
        ///     Creates any alias command depending on the json content 
        /// </summary>
        /// <param name="jsonContent"> content for the _aliases, see Elastic documentation </param>
        /// <returns> returns true if the alias command was completed successfully </returns>
        bool Alias(string jsonContent);

        /// <summary>
        ///     Creates any alias command depending on the json content var aliasParameters = new
        ///     AliasParameters { Actions = new List AliasBaseParameters { new
        ///     AliasAddParameters("test2", "indexaliasdtotests"), new AliasAddParameters("test3",
        ///     "indexaliasdtotests") } };
        /// </summary>
        /// <param name="aliasParameters"> content for the _aliases, see Elastic documentation </param>
        /// <returns> returns true if the alias command was completed successfully </returns>
        bool Alias(AliasParameters aliasParameters);

        /// <summary>
        ///     Async Creates any alias command depending on the json content 
        /// </summary>
        /// <param name="jsonContent"> content for the _aliases, see Elastic documentation </param>
        /// <returns> returns true if the alias command was completed successfully </returns>
        Task<ResultDetails<bool>> AliasAsync(string jsonContent);

        /// <summary>
        ///     Create a new warmer 
        /// </summary>
        /// <param name="warmer"> Wamrer with Query or Agg </param>
        /// <param name="index">  index if required </param>
        /// <param name="type">   type if required </param>
        /// <returns> true if created </returns>
        bool WarmerCreate(Warmer warmer, string index = "", string type = "");

        /// <summary>
        ///     Create a new warmer async 
        /// </summary>
        /// <param name="warmer"> Wamrer with Query or Agg </param>
        /// <param name="index">  index if required </param>
        /// <param name="type">   type if required </param>
        /// <returns> true if created </returns>
        Task<ResultDetails<bool>> WarmerCreateAsync(Warmer warmer, string index = "", string type = "");

        /// <summary>
        ///     deletes a warmer 
        /// </summary>
        /// <param name="warmerName"> name of the warmer </param>
        /// <param name="index">      index </param>
        /// <returns> true if ok </returns>
        bool WarmerDelete(string warmerName, string index);

        /// <summary>
        ///     deletes a warmer async 
        /// </summary>
        /// <param name="warmerName"> name of the warmer </param>
        /// <param name="index">      index </param>
        /// <returns> true if ok </returns>
        Task<ResultDetails<bool>> WarmerDeleteAsync(string warmerName, string index = "");

        /// <summary>
        ///     Async Creates any alias command depending on the json content 
        /// </summary>
        /// <param name="aliasParameters"> content for the _aliases, see Elastic documentation </param>
        /// <returns> returns true if the alias command was completed successfully </returns>
        Task<ResultDetails<bool>> AliasAsync(AliasParameters aliasParameters);

        /// <summary>
        ///     Removes a new alias for the index parameter. 
        /// </summary>
        /// <param name="alias"> name of the alias </param>
        /// <param name="index"> index for the alias </param>
        /// <returns> true if the alias was removed </returns>
        bool AliasRemoveForIndex(string alias, string index);

        /// <summary>
        ///     async Removes a new alias for the index parameter. 
        /// </summary>
        /// <param name="alias"> name of the alias </param>
        /// <param name="index"> index for the alias </param>
        /// <returns> true if the alias was removed </returns>
        Task<ResultDetails<bool>> AliasRemoveForIndexAsync(string alias, string index);

        /// <summary>
        ///     Replaces the index for the alias. This can be used when re-indexing live 
        /// </summary>
        /// <param name="alias">    Name of the alias </param>
        /// <param name="indexOld"> Old index which will be removed </param>
        /// <param name="indexNew"> New index which will be mapped to the alias </param>
        /// <returns> Returns true if the index was replaced </returns>
        bool AliasReplaceIndex(string alias, string indexOld, string indexNew);

        /// <summary>
        ///     Async Replaces the index for the alias. This can be used when re-indexing live 
        /// </summary>
        /// <param name="alias">    Name of the alias </param>
        /// <param name="indexOld"> Old index which will be removed </param>
        /// <param name="indexNew"> New index which will be mapped to the alias </param>
        /// <returns> Returns true if the index was replaced </returns>
        Task<ResultDetails<bool>> AliasReplaceIndexAsync(string alias, string indexOld, string indexNew);

        /// <summary>
        ///     Delete the whole index if it exists and Elastic allows delete index. Property
        ///     AllowDeleteForIndex must also be set to true.
        /// </summary>
        /// <typeparam name="T"> Type used to get the index to delete. </typeparam>
        /// <returns> Result details in a task </returns>
        Task<ResultDetails<bool>> DeleteIndexAsync<T>();

        /// <summary>
        ///     Delete the whole index if it exists and Elastic allows delete index. Property
        ///     AllowDeleteForIndex must also be set to true.
        /// </summary>
        /// <typeparam name="T"> Type used to get the index to delete. </typeparam>
        /// <returns> Result details in a , true if ok </returns>
        bool DeleteIndex<T>();

        /// <summary>
        ///     Delete the whole index if it exists and Elastic allows delete index. Property
        ///     AllowDeleteForIndex must also be set to true.
        /// </summary>
        /// <returns> Result details in a , true if ok </returns>
        bool DeleteIndex(string index);
    }
}