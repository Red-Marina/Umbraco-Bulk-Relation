using RedMarina.Umbraco.BulkRelation.Application.Helper;
using RedMarina.Umbraco.BulkRelation.Application.Services;
using RedMarina.Umbraco.BulkRelation.Interfaces;
using Umbraco.Core.Composing;

namespace RedMarina.Umbraco.BulkRelation.Startup
{
    class BulkRelationUserComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterFor<IBulkRelationService, BulkRelationService>(Lifetime.Singleton);
            composition.RegisterFor<IConnectionHelper, SqlConnectionHelper>(Lifetime.Singleton);
            composition.RegisterFor<IEmbeddedResourceProvider, EmbeddedResourceProvider>(Lifetime.Singleton);
        }
    }
}
