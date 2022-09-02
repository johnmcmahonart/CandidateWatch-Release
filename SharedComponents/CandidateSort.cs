using System.Collections.Generic;
using MDWatch.Model;
namespace MDWatch.Utilities

{
    public static class CandidateSort
    {
        public static CandidatebyYear Year(this IEnumerable<Candidate> candidates)
            {
            CandidatebyYear sortedCandidates = new();
            sortedCandidates.year = new();
            foreach (var candidate in candidates)
            {
                foreach (var item in candidate.ElectionYears)
                {
                    if (sortedCandidates.year.ContainsKey(item)) //check if the key already exists in the list, if not initialize
                    {
                        sortedCandidates.year[item].Add(candidate.CandidateId);
                    }
                    else 
                    {
                        sortedCandidates.year.Add(item, new List<string> { candidate.CandidateId });
                    }


                }
            }
            return sortedCandidates;



        }

    }
}
