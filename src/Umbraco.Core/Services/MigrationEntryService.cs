using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Umbraco.Core.Services
{
    /// <summary>
    /// Manages migration entries in the database
    /// </summary>
    public sealed class MigrationEntryService : RepositoryService, IMigrationEntryService
    {
        public MigrationEntryService(IDatabaseUnitOfWorkProvider provider, RepositoryFactory repositoryFactory, ILogger logger)
            : base(provider, repositoryFactory, logger)
        {
        }

        /// <summary>
        /// Creates a migration entry, will throw an exception if it already exists
        /// </summary>
        /// <param name="migrationName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IMigrationEntry CreateEntry(string migrationName, Version version)
        {
            var entry = new MigrationEntry
            {
                MigrationName = migrationName,
                Version = version
            };

            var uow = UowProvider.GetUnitOfWork();
            using (var repo = RepositoryFactory.CreateMigrationEntryRepository(uow))
            {
                repo.AddOrUpdate(entry);
                uow.Commit();
            }

            return entry;
        }

        /// <summary>
        /// Finds a migration by name and version, returns null if not found
        /// </summary>
        /// <param name="migrationName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IMigrationEntry FindEntry(string migrationName, Version version)
        {
            var uow = UowProvider.GetUnitOfWork();
            using (var repo = RepositoryFactory.CreateMigrationEntryRepository(uow))
            {
                var query = Query<IMigrationEntry>.Builder
                    .Where(x => x.MigrationName.ToUpper() == migrationName.ToUpper() && x.Version == version);
                return repo.GetByQuery(query).FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets all entries for a given migration name
        /// </summary>
        /// <param name="migrationName"></param>
        /// <returns></returns>
        public IEnumerable<IMigrationEntry> GetAll(string migrationName)
        {
            var uow = UowProvider.GetUnitOfWork();
            using (var repo = RepositoryFactory.CreateMigrationEntryRepository(uow))
            {
                var query = Query<IMigrationEntry>.Builder
                    .Where(x => x.MigrationName.ToUpper() == migrationName.ToUpper());
                return repo.GetByQuery(query);
            }
        }

    }
}