using DutyDock.Domain.Common.Models.Entities;

namespace DutyDock.Application.Common.Database.Common;

public interface IDataObject<out T> where T : Entity
{
    
}