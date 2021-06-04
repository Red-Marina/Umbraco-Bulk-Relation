# Bulk relations for Umbraco

A service that compliments the existing Umbraco relations service but allows bulk upsert/merge of relations
into the Umbraco database table.

## Why?

Because the Umbraco relations service doesn't do this, and it is slow to add and remove relations across multiple service calls.

## How?

Here is the definition of the service.

```
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
```

If you install the NUGET package into your project and solution a User Composer will register the IBulkRelationService for injection into your own services.

Alternatively you can new up the concrete BulkRelationService
