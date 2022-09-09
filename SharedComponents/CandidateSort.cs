using System.Collections.Generic;
using MDWatch.Model;
namespace MDWatch.Utilities

{
    public static class CandidateSort
    {
        public static IEnumerable<CandidatebyYear> Year(this IEnumerable<Candidate> candidates)
        {
            List<CandidatebyYear> sortedCandidates = new();

            foreach (var candidate in candidates)
            {
                foreach (var item in candidate.Cycles)
                {
                    try  //check if year exists in list, if not create new item in collection. If it does exist add candidate to the correct year
                    {
                        var i = sortedCandidates.FindIndex(x => x.Year.Equals(item));
                        sortedCandidates[i].Candidates.Add(candidate.CandidateId);
                    }
                    catch
                    {
                        sortedCandidates.Add(new CandidatebyYear()
                        {
                            Year = item,
                            Candidates = new List<string>() { candidate.CandidateId }
                        });
                    }
                }
            }
            return sortedCandidates;
        }
    }
}