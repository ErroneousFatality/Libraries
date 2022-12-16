using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

using AndrejKrizan.Common.Extensions;
using AndrejKrizan.EntityFramework.Common.Internal.Extensions.Expressions;

using Microsoft.EntityFrameworkCore;

namespace AndrejKrizan.EntityFramework.Common.Repositories
{
    public class CompositeKeyRepository<TEntity> : Repository<TEntity>
        where TEntity : class
    {
        // Properties
        private ImmutableArray<PropertyInfo> KeyPropertyInfos { get; }

        // Constructors

        /// <exception cref="ArgumentException"> Expression must point to a member. </exception>
        public CompositeKeyRepository(
            DbContext dbContext,
            Expression<Func<TEntity, object>> keyPropertyExpression,
            params Expression<Func<TEntity, object>>[] additionalKeyPropertyExpressions
        )
            : base(dbContext)
        {
            KeyPropertyInfos = additionalKeyPropertyExpressions
                .Prepend(keyPropertyExpression)
                .Select((keyPropertyExpression) => keyPropertyExpression.GetEntityKeyPropertyInfo())
                .ToImmutableArray(additionalKeyPropertyExpressions.Length + 1);
        }

        // Methods

        /// <exception cref="ArgumentException"> Number of keys does not match number of registered key properties. </exception>
        public async Task<bool> ExistsAsync(object key, params object[] additionalKeys)
            => await DbSet.AnyAsync(KeysEqual(key, additionalKeys));

        /// <exception cref="ArgumentException"> Number of keys does not match number of registered key properties. </exception>

        public async Task<TEntity?> GetAsync(object key, params object[] additionalKeys)
            => await DbSet.FindAsync(CombineKeys(key, additionalKeys));

        /// <exception cref="ArgumentException"> Number of keys does not match number of registered key properties. </exception>
        public void Remove(object key, params object[] additionalKeys)
        {
            ImmutableArray<object> keys = CombineKeys(key, additionalKeys);
            TEntity entity = Mock(keys);
            DbSet.Remove(entity);
        }

        // Helper methods
        /// <exception cref="ArgumentException"> Number of keys does not match number of registered key properties. </exception>
        protected Expression<Func<TEntity, bool>> KeysEqual(object key, params object[] additionalKeys)
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
            ImmutableArray<object> keys = CombineKeys(key, additionalKeys);
            BinaryExpression keysEqualExpression = KeyPropertyInfos.Zip(keys)
                .Select(((PropertyInfo PropertyInfo, object Value) key)
                    => Expression.Equal(
                        Expression.Property(parameterExpression, key.PropertyInfo),
                        Expression.Constant(key.Value)
                    )
                )
                .Aggregate(Expression.AndAlso);
            Expression<Func<TEntity, bool>> keysEqualLambda = Expression.Lambda<Func<TEntity, bool>>(
                keysEqualExpression,
                parameterExpression
            );
            return keysEqualLambda;
        }

        /// <exception cref="ArgumentException"> Number of keys does not match number of registered key properties. </exception>
        protected ImmutableArray<object> CombineKeys(object key, params object[] additionalKeys)
        {
            int keyCount = additionalKeys.Length + 1;
            if (keyCount != KeyPropertyInfos.Length)
            {
                throw new ArgumentException("Number of keys does not match number of registered key properties.");
            }
            ImmutableArray<object> keys = additionalKeys
                .Prepend(key)
                .ToImmutableArray(keyCount);
            return keys;
        }

        // Private methods
        private TEntity Mock(IEnumerable<object> keys)
        {
            TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity), nonPublic: true)!;
            foreach ((PropertyInfo propertyInfo, object _key) in KeyPropertyInfos.Zip(keys))
            {
                propertyInfo.SetValue(entity, _key);
            }
            return entity;
        }
    }
}
