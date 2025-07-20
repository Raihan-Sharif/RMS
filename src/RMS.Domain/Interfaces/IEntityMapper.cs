using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Domain.Interfaces
{
    public interface IEntityMapper
    {
        Type GetDbEntityType(Type domainEntityType);
        Type GetDomainEntityType(Type dbEntityType);
        string GetTableName(Type domainEntityType);
        string GetPrimaryKeyName(Type domainEntityType);
        Type GetPrimaryKeyType(Type domainEntityType);
        bool IsRegistered(Type domainEntityType);
    }
}
