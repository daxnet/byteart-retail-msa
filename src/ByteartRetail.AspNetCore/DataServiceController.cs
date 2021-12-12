using ByteartRetail.Common;
using ByteartRetail.Common.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace ByteartRetail.AspNetCore
{
    public abstract class DataServiceController<TEntity> : ControllerBase
        where TEntity : class, IEntity
    {
        #region Private Fields

        private readonly IDataAccessObject _dao;
        private readonly ILogger _logger;

        #endregion Private Fields

        #region Protected Constructors

        protected DataServiceController(IDataAccessObject dao, ILogger logger)
            => (_dao, _logger) = (dao, logger);

        #endregion Protected Constructors

        #region Protected Properties

        protected IDataAccessObject Dao => _dao;
        protected ILogger Logger => _logger;

        #endregion Protected Properties

        #region Public Methods

        /// <summary>
        /// Creates an new entity, if the id of the entity is not specified, or updates an existing entity,
        /// if the id of the entity is specified.
        /// </summary>
        /// <param name="entity">The entity to be created.</param>
        /// <returns>The resource location of the created entity.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAsync([FromBody] TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _dao.AddAsync(entity);
            // ReSharper disable once Mvc.ActionNotResolved
            return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity);
        }

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to be deleted.</param>
        /// <returns>The delete result.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _dao.DeleteByIdAsync<TEntity>(id);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <param name="size">The page size.</param>
        /// <param name="page">The page number.</param>
        /// <param name="projectionFields">The fields to be returned by the query. Names are comma-spearated.</param>
        /// <returns>A list of entities, with the pagination information.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] string? projectionFields = null,
            [FromQuery] int size = 25,
            [FromQuery] int page = 1)
        {
            var pagedResult = await _dao.GetAsync<TEntity>(
                _ => true,
                x => x.Id,
                pageSize: size,
                pageNumber: page,
                projectionFields: projectionFields);

            return Ok(pagedResult);
        }

        /// <summary>
        /// Gets the entity by its identifier.
        /// </summary>
        /// <param name="id">The id of the entity.</param>
        /// <returns>The found entity.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var entity = await _dao.GetByIdAsync<TEntity>(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="id">The id of the entity to be updated.</param>
        /// <param name="entity">The entity that contains the updating values.</param>
        /// <returns>The update result.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEntity = await _dao.GetByIdAsync<TEntity>(id);
            if (existingEntity != null)
            {
                entity.Id = id; // Ensure that the Id of the entity is not null.
                await _dao.UpdateByIdAsync(id, entity);
                return NoContent();
            }

            await _dao.AddAsync(entity);
            // ReSharper disable once Mvc.ActionNotResolved
            return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, entity.Id);
        }

        #endregion Public Methods
    }
}