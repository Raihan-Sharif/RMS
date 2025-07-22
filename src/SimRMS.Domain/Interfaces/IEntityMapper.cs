using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LB.DAL.Core.Common;

namespace SimRMS.Domain.Interfaces
{
    /// <summary>
    /// Pure domain interface for entity mapping operations
    /// No infrastructure dependencies
    /// </summary>
    public interface IEntityMapper
    {
        /// <summary>
        /// Gets the primary key property name for an entity type
        /// </summary>
        string GetPrimaryKeyName<T>() where T : class;

        /// <summary>
        /// Gets the table name for an entity type
        /// </summary>
        string GetTableName<T>() where T : class;

        /// <summary>
        /// Gets the primary key value from an entity
        /// </summary>
        TKey? GetEntityId<TKey, TEntity>(TEntity entity) where TEntity : class;
    }
}
