namespace RedMarina.Umbraco.BulkRelation.Interfaces
{
    public interface IBulkRelationService
    {
        void BulkRelate(int parent, int[] child, string alias);

        // Takes UDIs
        void BulkRelate(int parent, string[] child, string alias);

        void Delete(int parent, string alias);
    }
}
