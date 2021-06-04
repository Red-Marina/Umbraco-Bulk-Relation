using RedMarina.Umbraco.BulkRelation.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace RedMarina.Umbraco.BulkRelation.Application.Services
{
    public class BulkRelationService : IBulkRelationService
    {
        private readonly IDictionary<string, int> _relationTypes;
        private readonly IRelationService _relationService;
        private readonly IConnectionHelper _connectionHelper;
        private readonly IEmbeddedResourceProvider _embeddedResourceProvider;
        private readonly IEntityService _entityService;

        public BulkRelationService(IRelationService relationService,
            IConnectionHelper connectionHelper,
            IEmbeddedResourceProvider embeddedResourceProvider,
            IEntityService entityService)
        {
            _relationService = relationService;
            _connectionHelper = connectionHelper;
            _relationTypes = new Dictionary<string, int>();
            _embeddedResourceProvider = embeddedResourceProvider;
            _entityService = entityService;
        }

        private void EnsureRelationTypeAlias(string alias)
        {
            if (_relationTypes.ContainsKey(alias))
            {
                return;
            }

            IRelationType relationType = _relationService.GetRelationTypeByAlias(alias);

            if (relationType != null)
            {
                _relationTypes.Add(alias, relationType.Id);
            }
            else
            {
                throw new ArgumentException($"No relation type with alias {alias}");
            }
        }

        public void BulkRelate(int parent, int[] children, string alias)
        {
            if (children.Length > 1)
            {
                children = children.Distinct().ToArray();
            }

            EnsureRelationTypeAlias(alias);
            int relationType = _relationTypes[alias];

            DataTable dt = new DataTable();
            dt.Columns.Add("parentId", typeof(int));
            dt.Columns.Add("childId", typeof(int));
            dt.Columns.Add("relType", typeof(int));
            dt.Columns.Add("datetime", typeof(DateTime));
            dt.Columns.Add("comment", typeof(string));

            foreach (int child in children)
            {
                dt.Rows.Add(parent, child, relationType, DateTime.UtcNow, string.Empty);
            }

            using (SqlConnection connection = _connectionHelper.GetConnection())
            {
                if (dt.Rows.Count < 1)
                {
                    // No rows, remove old relations
                    using (SqlCommand command = new SqlCommand(@"DELETE FROM UmbracoRelation WHERE
                                parentId = @ParentId 
                                AND reltype = @RelType", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@ParentId", parent));
                        command.Parameters.Add(new SqlParameter("@RelType", relationType));
                        command.ExecuteNonQuery();
                    }

                    return;
                }

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    string sql = _embeddedResourceProvider.ReadEmbeddedResource("RedMarina.Umbraco.BulkRelation.Resources.Sql.CreateRelationTempTable.sql");
                    string mergeSql = _embeddedResourceProvider.ReadEmbeddedResource("RedMarina.Umbraco.BulkRelation.Resources.Sql.MergeRelations.sql");
                    mergeSql = mergeSql.Replace("@childIds", string.Join(",", children));

                    // Create a temp table
                    new SqlCommand(sql, connection, transaction).ExecuteNonQuery();
                    using (var sqlBulk = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, transaction))
                    {
                        // Bulk load the Temp table
                        sqlBulk.DestinationTableName = "#TempUmbracoRelation";
                        sqlBulk.WriteToServer(dt);
                    }

                    using (SqlCommand command = new SqlCommand(mergeSql, connection, transaction))
                    {
                        command.Parameters.Add(new SqlParameter("@ParentId", parent));
                        command.Parameters.Add(new SqlParameter("@RelType", relationType));
                        // Merge the tables.
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public void BulkRelate(int parent, string[] child, string alias)
        {
            BulkRelate(parent, UdisToInts(child), alias);
        }

        private int[] UdisToInts(IEnumerable<string> udis)
        {
            // difficult to see which is quickest, but turning these relatedCharacteristics into int ids, will make the lookups for relations easier/quicker?
            IList<int> ints = new List<int>();
            foreach (string udiString in udis)
            {
                if (Udi.TryParse(udiString, out Udi udi))
                {
                    var attempt = _entityService.GetId(udi);
                    if (attempt.Success)
                    {
                        ints.Add(attempt.Result);
                    }
                }
            }
            return ints.ToArray();
        }

        public void Delete(int parent, string alias)
        {
            EnsureRelationTypeAlias(alias);
            int relationType = _relationTypes[alias];

            using (SqlConnection connection = _connectionHelper.GetConnection())
            {
                // No rows, remove old relations
                using (SqlCommand command = new SqlCommand(@"DELETE FROM UmbracoRelation WHERE
                                parentId = @ParentId 
                                AND reltype = @RelType", connection))
                {
                    command.Parameters.Add(new SqlParameter("@ParentId", parent));
                    command.Parameters.Add(new SqlParameter("@RelType", relationType));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}