﻿using Domain.Entities;

namespace Application.Abstractions.Data;

public interface IPhaseRepository
{
    // isExistById
    Task<bool> IsExistById(Guid id);
    Task<bool> IsAllPhaseExistByIdAsync(List<Guid> phaseIds);
    void AddPhase(Phase phase);
    void UpdatePhase(Phase phase);
    Task<Phase> GetPhaseById(Guid id);
    Task<List<Phase>> GetPhases();
    Task<Phase> GetPhaseByName(string name);
    Task<bool> IsAllPhase1(List<Guid> phaseIds);
    Task<bool> IsPhase2(Guid phaseId);
}
