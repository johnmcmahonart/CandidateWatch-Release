using Azure;
using Azure.Data.Tables;
using MDWatch.Model;
using MDWatch.Utilities;
using RESTApi.DTOs;

namespace RESTApi.Repositories
{
    public class UINavRepository : IUINavRepository
    //minimal repository for displaying parts of the UI
    {
        private string _electionYearsPartition = "CandidatebyYear";
        private string _candidatePartition = "Candidate";
        
        private bool CurrentlyElected(Candidate candidate)
        {
            //candidatestatus c = current candidate, candidatestatus f = future candidate
            return candidate.IncumbentChallenge == "I" && (candidate.CandidateStatus == "C" | candidate.CandidateStatus == "F") ? true : false;
        }
        public async Task<IEnumerable<CandidateUIDTO>> GetCandidates(int year, bool wasElected)
        {
            
            {
                List<CandidateUIDTO> outDTO = new();
                List<Candidate> candidates = new();
                List<CandidatebyYear> sortedCandidates = new();

                //get candidates grouped by year
                AsyncPageable<TableEntity> candidatebyYear = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_electionYearsPartition}'");
                await foreach (var item in candidatebyYear)
                {
                    sortedCandidates.Add(item.TableEntityToModel<CandidatebyYear>());
                    
                }
                //get candidate records from table

                var i = sortedCandidates.FindIndex(x => x.Year.Equals(year));
                foreach (var candidateforYear in sortedCandidates[i].Candidates)
                {
                    AsyncPageable<TableEntity> candidate = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_candidatePartition}' and {General.GetMemberName((Candidate c) => c.CandidateId)}  eq '{candidateforYear}'");

                    await foreach (var record in candidate)
                    {
                        var candidateModel = record.TableEntityToModel<Candidate>();
                        if (wasElected) //filters only elected candidates
                        {
                            if (CurrentlyElected(candidateModel))
                            {
                                candidates.Add(candidateModel);
                            }
                        }
                        else
                        {
                            candidates.Add(candidateModel);
                        }
                    }
                }
                //build DTO
                foreach (var candidate in candidates)
                {
                    List<string> names = candidate.Name.FixCandidateName().ToList();
                    if (names.Count > 1)
                    {
                        outDTO.Add(new CandidateUIDTO
                        {
                            CandidateId = candidate.CandidateId,
                            FirstName = names[0],
                            LastName = names[1],
                            Party = candidate.Party,
                            District = candidate.District,
                            WasElected = CurrentlyElected(candidate)
                        });
                    }
                    else
                    {
                        outDTO.Add(new CandidateUIDTO
                        {
                            CandidateId = candidate.CandidateId,
                            FirstName = names[0],
                            LastName = "n/a",
                            Party = candidate.Party,
                            District= candidate.District,
                            WasElected = CurrentlyElected(candidate)
                        });
                    }
                }

                return outDTO.AsReadOnly();
            }
        }

        

        public async Task<IEnumerable<int>> GetElectionYears()
        {
            List<CandidatebyYear> yearsFromTable = new();
            var yearsOut = new List<int>();
            AsyncPageable<TableEntity> candidatebyYear = IStateAzureTableClient._tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq '{_electionYearsPartition}'");
            await foreach (var item in candidatebyYear)
            {
                yearsFromTable.Add(item.TableEntityToModel<CandidatebyYear>());
            }
            //get just years

            foreach (var year in yearsFromTable)
            {
                yearsOut.Add(year.Year);
            }
            return yearsOut.AsReadOnly();
        }
    }
}