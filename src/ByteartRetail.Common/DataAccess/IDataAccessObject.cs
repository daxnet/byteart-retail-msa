using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ByteartRetail.Common.DataAccess
{
    /// <summary>
    /// Represents that the implemented classes are data access objects that perform
    /// CRUD operations on the given entity type.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IDataAccessObject : IDisposable
    {

        #region Public Methods

        /// <summary>
        /// Adds the given entity asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be added.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The task which performs the adding operation.</returns>
        Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : IEntity;

        /// <summary>
        /// Deletes the entity by specified identifier asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The identifier which represents the entity that is going to be deleted.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The task which performs the deletion operation.</returns>
        Task DeleteByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : IEntity;

        /// <summary>
        /// Finds the entities which match the specified criteria that is defined by the given specification asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="expr">The expression which defines the matching criteria.</param>
        /// <param name="sortExpression">The sorting expression.</param>
        /// <param name="sortOrder">The order of the sorting.</param>
        /// <param name="pageSize">The number of objects returned in a single page.</param>
        /// <param name="pageNumber">The requested page number.</param>
        /// <param name="projectionFields">A list of the field names that will be returned by the query. Field names are comma-separated.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The task which performs the data retrieval operation, and after the operation
        /// has completed, would return a list of entities that match the specified criteria.</returns>
        Task<PagedResult<TEntity>> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>> expr,
            Expression<Func<TEntity, object>> sortExpression,
            SortOrder sortOrder = SortOrder.Ascending,
            int pageSize = 25,
            int pageNumber = 1,
            string? projectionFields = null,
            CancellationToken cancellationToken = default) where TEntity : IEntity;

        /// <summary>
        /// Gets the entity by specified identifier asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The task which performs the data retrieval operation.</returns>
        Task<TEntity> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken = default) where TEntity : IEntity;

        /// <summary>
        /// Updates the entity by the specified identifier asynchronously.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The identifier of the entity to be updated.</param>
        /// <param name="entity">The entity which contains the updated value.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The task which performs the updating operation.</returns>
        Task UpdateByIdAsync<TEntity>(Guid id, TEntity entity, CancellationToken cancellationToken = default) where TEntity : IEntity;

        /// <summary>
        /// Bulk updates the entities that match the specified criteria.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to be updated.</typeparam>
        /// <param name="filterExpr">The criteria on the entities that needs to be updated.</param>
        /// <param name="updateExpr">The property expression of the updating entity.</param>
        /// <param name="updatedValue">The value to be updated to.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>The task that executes the bulk update.</returns>
        Task UpdateMultipleAsync<TEntity>(
            Expression<Func<TEntity, bool>> filterExpr,
            Expression<Func<TEntity, object>> updateExpr,
            object updatedValue,
            CancellationToken cancellationToken = default)
            where TEntity : IEntity;

        #endregion Public Methods

    }
}
