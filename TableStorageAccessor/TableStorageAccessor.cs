using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.SportsCloud.TableStorage
{
    /// <summary>
    /// Class representing a connection to a single table in Azure Table Storage
    /// </summary>
    public class TableStorageAccessor
    {
        private const string PartitionKeyKey = "PartitionKey";
        private const string RowKeyKey = "RowKey";

        // https://msdn.microsoft.com/en-us/library/azure/dd894038.aspx
        private const int MaxBatchSize = 100;

        public CloudStorageAccount StorageAccount { get; private set; }

        public CloudTable Table { get; private set; }

        public TableStorageAccessor(string connectionString, string tableName)
        {
            StorageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = StorageAccount.CreateCloudTableClient();
            Table = tableClient.GetTableReference(tableName);
            Table.CreateIfNotExists();
        }

        #region Retrieve Operations

        /// <summary>
        /// Retrieve all entites for a specified partition key
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="partitionKey">partition key</param>
        /// <returns>list of entities</returns>
        public async Task<List<T>> RetrieveEntitiesAsync<T>(string partitionKey) where T : TableEntity, new()
        {
            TableContinuationToken continuationToken = null;
            var tableEntities = new List<T>();
            string queryString = TableQuery.GenerateFilterCondition(PartitionKeyKey, QueryComparisons.Equal, partitionKey);
            
            TableQuery<T> query = new TableQuery<T>().Where(queryString);

            do
            {
                var queryResult = await Table.ExecuteQuerySegmentedAsync(query, continuationToken).ConfigureAwait(false);

                continuationToken = queryResult.ContinuationToken;

                tableEntities.AddRange(queryResult.Results);

            } while (continuationToken != null);

            return tableEntities;
        }

        public async Task<List<T>> RetrieveEntitiesByRowKeyAsync<T>(string rowKey) where T : TableEntity, new()
        {
            TableContinuationToken continuationToken = null;
            var tableEntities = new List<T>();
            string queryString = TableQuery.GenerateFilterCondition(RowKeyKey, QueryComparisons.Equal, rowKey);

            TableQuery<T> query = new TableQuery<T>().Where(queryString);

            do
            {
                var queryResult = await Table.ExecuteQuerySegmentedAsync(query, continuationToken).ConfigureAwait(false);

                continuationToken = queryResult.ContinuationToken;

                tableEntities.AddRange(queryResult.Results);

            } while (continuationToken != null);

            return tableEntities;
        }

        /// <summary>
        /// Retrieves a list of entities with a specified partition key and row keys
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="partitionKey">partition key</param>
        /// <param name="rowKeys">row keys</param>
        /// <param name="useQuery">whether to build an 'OR' query with the row keys, or get all for the partition key and filter based on row key</param>
        /// <returns>list of entities</returns>
        public async Task<List<T>> RetrieveEntitiesAsync<T>(string partitionKey, IEnumerable<string> rowKeys, bool useQuery=false) where T : TableEntity, new()
        {
            // doing a point query for each row key is the recommended method for getting multiple row keys
            // https://social.msdn.microsoft.com/forums/azure/en-US/5160b393-6f40-4354-86a0-8357cb6cf5d6/best-way-to-select-different-unrelated-records-using-rowkey
            if (useQuery)
            {
                var results = new ConcurrentBag<T>();
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = 10
                };
                Parallel.ForEach(rowKeys, parallelOptions, 

                    async rowKey =>
                    {
                        try
                        {
                            T entity = await RetrieveEntityAsync<T>(partitionKey, rowKey);
                            results.Add(entity);
                        }
                        catch (EntityNotFoundException)
                        {
                            // do nothing                            
                        }
                    }

                    );
                
                return results.ToList();
            }

            List<string> rowKeyList = rowKeys.ToList();

            // optimize for just a single entity request
            if (rowKeyList.Count() == 1)
            {
                var entityResults = new List<T>();
                try
                {
                    T entity = await RetrieveEntityAsync<T>(partitionKey, rowKeyList.First()).ConfigureAwait(false);

                    entityResults.Add(entity);
                    
                }
                catch (EntityNotFoundException)
                {
                    // do nothing because the list will be empty
                }

                return entityResults;
            }

            var entities = await RetrieveEntitiesAsync<T>(partitionKey).ConfigureAwait(false);

            var subsetEntities = entities.Where(e => rowKeyList.Contains(e.RowKey)).ToList();

            return subsetEntities;
        }

        /// <summary>
        /// Retrieve a single entity by row and partition key
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="partitionKey">partition key</param>
        /// <param name="rowKey">row key</param>
        /// <returns>entity</returns>
        public async Task<T> RetrieveEntityAsync<T>(string partitionKey, string rowKey) where T : TableEntity, new()
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult tableResult = await Table.ExecuteAsync(retrieveOperation).ConfigureAwait(false);

            if (tableResult.HttpStatusCode.Equals((int) HttpStatusCode.OK))
            {
                var entity = tableResult.Result as T;

                return entity;
            }
            
            if (tableResult.HttpStatusCode.Equals((int) HttpStatusCode.NotFound))
            {
                string errorMessage = String.Format("Entity not found at parititionKey: {0}, rowKey: {1}", partitionKey, rowKey);

                throw new EntityNotFoundException(errorMessage);
            }

            return null;
        }

        #endregion


        #region InsertOrReplace operations

        /// <summary>
        /// Batch Insert or replace input entities asynchronously
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntities">list of entities</param>
        /// <returns>list of table results</returns>
        public async Task<IList<TableResult>> InsertOrReplaceEntitiesBatchAsync<T>(IList<T> tableEntities) where T : TableEntity, new()
        {
            return await BatchEntitiesAsync(tableEntities, (operation, entity) => operation.InsertOrReplace(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert or replace input entities asynchronously one at a time
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntities">input entities</param>
        /// <returns>array of table results</returns>
        public async Task<IList<TableResult>> InsertOrReplaceEntitiesAsync<T>(IEnumerable<T> tableEntities) where T : TableEntity, new()
        {
            return await OperateOnEntitiesAsync(tableEntities, TableOperation.InsertOrReplace).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert or Replace a singular table entity
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntity">table entity</param>
        /// <returns>table result</returns>
        public async Task<TableResult> InsertOrReplaceEntityAsync<T>(T tableEntity) where T : TableEntity, new()
        {
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(tableEntity);

            var result = await Table.ExecuteAsync(insertOrReplaceOperation).ConfigureAwait(false);
            return result;
        }

        #endregion


        #region Replace Operations

        /// <summary>
        /// Batch Replace entities
        /// </summary>
        /// <typeparam name="T">type of entities</typeparam>
        /// <param name="tableEntities">input entities</param>
        /// <returns>list of table results</returns>
        public async Task<IList<TableResult>> ReplaceEntitiesBatchAsync<T>(IList<T> tableEntities)
            where T : TableEntity, new()
        {
            return await BatchEntitiesAsync(tableEntities, (operation, entity) => operation.Replace(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Replace input entities async one at a time
        /// </summary>
        /// <typeparam name="T">table type</typeparam>
        /// <param name="tableEntities">input entites</param>
        /// <returns>array of table results</returns>
        public async Task<TableResult[]> ReplaceEntitiesAsync<T>(IEnumerable<T> tableEntities) where T : TableEntity, new()
        {
            return await OperateOnEntitiesAsync(tableEntities, TableOperation.Replace).ConfigureAwait(false);
        }

        /// <summary>
        /// Replace a single entity async
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntity">input entity</param>
        /// <returns>table result</returns>
        public async Task<TableResult> ReplaceEntityAsync<T>(T tableEntity) where T : TableEntity, new()
        {
            var replaceOperation = TableOperation.Replace(tableEntity);

            var result = await Table.ExecuteAsync(replaceOperation).ConfigureAwait(false);
            return result;
        }

        #endregion


        #region Delete Operation

        /// <summary>
        /// Batch delete all entities with partition key
        /// </summary>
        /// <typeparam name="T">type of entities to delete</typeparam>
        /// <param name="partitionKey">partition key</param>
        /// <returns>list of table results</returns>
        public async Task<IList<TableResult>> DeleteAllEntitiesBatchAsync<T>(string partitionKey) where T : TableEntity, new()
        {
            var tableEntities = await RetrieveEntitiesAsync<T>(partitionKey).ConfigureAwait(false);

            return await DeleteEntitiesBatchAsync(tableEntities).ConfigureAwait(false);
        }

        /// <summary>
        /// Batch delete a list of entities
        /// </summary>
        /// <typeparam name="T">type of entity to delete</typeparam>
        /// <param name="tableEntities">list of entities to delete</param>
        /// <returns>list of table results</returns>
        public async Task<IList<TableResult>> DeleteEntitiesBatchAsync<T>(IList<T> tableEntities) where T : TableEntity, new()
        {
            return await BatchEntitiesAsync(tableEntities, (operation, entity) => operation.Delete(entity)).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete input table entities
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntities">table entities to delete</param>
        /// <returns>array of table results</returns>
        public async Task<TableResult[]> DeleteEntitiesAsync<T>(IEnumerable<T> tableEntities) where T : TableEntity, new()
        {
            var tasks = tableEntities.Select(TableOperation.Delete).Select(operation => Table.ExecuteAsync(operation)).ToList();

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a single entity
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntity">the entity</param>
        /// <returns>table result of operation</returns>
        public async Task<TableResult> DeleteEntityAsync<T>(T tableEntity) where T : TableEntity, new()
        {
            var deleteOperation = TableOperation.Delete(tableEntity);
            var task = Table.ExecuteAsync(deleteOperation);

            return await task.ConfigureAwait(false);
        }

        #endregion


        #region Private Common Operations

        /// <summary>
        /// Run a batch operation on a list of entities
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntities">table entities to do operation on</param>
        /// <param name="batchEntityOperation">batch operation to perform</param>
        /// <returns>list of table results</returns>
        private async Task<IList<TableResult>> BatchEntitiesAsync<T>(IList<T> tableEntities, Action<TableBatchOperation, ITableEntity> batchEntityOperation) where T : TableEntity, new()
        {
            var results = new List<TableResult>();

            // trying to run a batch operation with 0 entities throws an error, so do a quick check first to avoid the error
            if (tableEntities.Count == 0)
            {
                return results;
            }

            int index = 0;
            while (true)
            {
                // take as many entities as can fit in a batch
                var batchEntities = tableEntities.Skip(index * MaxBatchSize).Take(MaxBatchSize);

                var batchOperation = new TableBatchOperation();

                // track the batch count so we don't have to re-enumerate the linq query
                int batchSize = 0;
                foreach (var batchEntity in batchEntities)
                {
                    batchEntityOperation(batchOperation, batchEntity);
                    batchSize++;
                }

                results.AddRange(await Table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false));

                // if we didn't get the maximum number of allowed batch operations, then we've reached the end
                if (batchSize < MaxBatchSize)
                {
                    break;
                }

                index++;
            }

            return results;
        }

        /// <summary>
        /// Operate on Entities Async one at a time
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="tableEntities">input entities</param>
        /// <param name="operation">operation to operate with</param>
        /// <returns>array of table results</returns>
        private async Task<TableResult[]> OperateOnEntitiesAsync<T>(IEnumerable<T> tableEntities, Func<ITableEntity, TableOperation> operation) where T : TableEntity, new()
        {
            var tasks = new List<Task<TableResult>>();

            foreach (var tableEntity in tableEntities)
            {
                var op = operation(tableEntity);
                var insertTask = Table.ExecuteAsync(op);
                tasks.Add(insertTask);
            }

            return await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        #endregion
    }
}
