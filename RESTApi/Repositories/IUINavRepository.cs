﻿using RESTApi.DTOs;

namespace RESTApi.Repositories
{
    public interface IUINavRepository
    {
        public Task <IEnumerable<int>> GetElectionYears();
        public Task <IEnumerable<CandidateUIDTO>> GetCandidates(int year, bool wasElected = default);
    }
}
