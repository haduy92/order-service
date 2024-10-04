using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishClass.Application.Models;
using EnglishClass.Common.Dependencies;

namespace EnglishClass.Application.Interfaces;

public interface ICardService : ITransientDependency
{
    Task<IEnumerable<CreateCardRequest>> GetByUserId(Guid id);
    Task<CreateCardRequest> GetById(Guid id);
    Task<Guid> Create(CreateCardRequest card);
    Task Update(string text, string description);
    Task Delete(Guid id);
}
