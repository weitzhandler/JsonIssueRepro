using Microsoft.OData.ModelBuilder;

namespace JsonIssueRepro
{
    public class EdmModelBuilder : ODataConventionModelBuilder
    {
        public EdmModelBuilder()
        {
            ComplexType<LanguageTitle>();
            EntitySet<ColumnMetadata>();
        }

        EntitySetConfiguration<TEntity> EntitySet<TEntity, TIdentifier>()
          where TEntity : class
        {
            var identifier = $"{typeof(TIdentifier).Name}s";
            return EntitySet<TEntity>(identifier);
        }

        EntitySetConfiguration<TEntity> EntitySet<TEntity>()
          where TEntity : class =>
          EntitySet<TEntity, TEntity>();
    }
}