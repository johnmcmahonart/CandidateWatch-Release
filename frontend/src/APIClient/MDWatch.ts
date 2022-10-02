import { emptySplitApi as api } from "./EmptyAPI";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    getApiCandidateByKey: build.query<
      GetApiCandidateByKeyApiResponse,
      GetApiCandidateByKeyApiArg
    >({
      query: (queryArg) => ({ url: `/api/Candidate/${queryArg.key}` }),
    }),
    getApiCandidateYears: build.query<
      GetApiCandidateYearsApiResponse,
      GetApiCandidateYearsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/Candidate/years`,
        params: { years: queryArg.years },
      }),
    }),
    getApiFinanceTotalsByKey: build.query<
      GetApiFinanceTotalsByKeyApiResponse,
      GetApiFinanceTotalsByKeyApiArg
    >({
      query: (queryArg) => ({ url: `/api/FinanceTotals/${queryArg.key}` }),
    }),
    getApiFinanceTotalsByKeyYears: build.query<
      GetApiFinanceTotalsByKeyYearsApiResponse,
      GetApiFinanceTotalsByKeyYearsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/FinanceTotals/${queryArg.key}/years`,
        params: { years: queryArg.years },
      }),
    }),
    getApiFinanceTotalsYears: build.query<
      GetApiFinanceTotalsYearsApiResponse,
      GetApiFinanceTotalsYearsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/FinanceTotals/years`,
        params: { years: queryArg.years },
      }),
    }),
    getApiScheduleBDetailByKey: build.query<
      GetApiScheduleBDetailByKeyApiResponse,
      GetApiScheduleBDetailByKeyApiArg
    >({
      query: (queryArg) => ({ url: `/api/ScheduleBDetail/${queryArg.key}` }),
    }),
    getApiScheduleBDetailByKeyYears: build.query<
      GetApiScheduleBDetailByKeyYearsApiResponse,
      GetApiScheduleBDetailByKeyYearsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/ScheduleBDetail/${queryArg.key}/years`,
        params: { years: queryArg.years },
      }),
    }),
    getApiScheduleBDetailYears: build.query<
      GetApiScheduleBDetailYearsApiResponse,
      GetApiScheduleBDetailYearsApiArg
    >({
      query: (queryArg) => ({
        url: `/api/ScheduleBDetail/years`,
        params: { years: queryArg.years },
      }),
    }),
    getApiScheduleBOverviewByKey: build.query<
      GetApiScheduleBOverviewByKeyApiResponse,
      GetApiScheduleBOverviewByKeyApiArg
    >({
      query: (queryArg) => ({ url: `/api/ScheduleBOverview/${queryArg.key}` }),
    }),
    getApiUiCandidatesbyYearByYear: build.query<
      GetApiUiCandidatesbyYearByYearApiResponse,
      GetApiUiCandidatesbyYearByYearApiArg
    >({
      query: (queryArg) => ({
        url: `/api/UI/CandidatesbyYear/${queryArg.year}`,
        params: { wasElected: queryArg.wasElected },
      }),
    }),
    getApiUiElectionYears: build.query<
      GetApiUiElectionYearsApiResponse,
      GetApiUiElectionYearsApiArg
    >({
      query: () => ({ url: `/api/UI/ElectionYears` }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as MDWatchAPI };
export type GetApiCandidateByKeyApiResponse =
  /** status 200 Success */ CandidateDto[];
export type GetApiCandidateByKeyApiArg = {
  key: string;
};
export type GetApiCandidateYearsApiResponse =
  /** status 200 Success */ CandidateDto[];
export type GetApiCandidateYearsApiArg = {
  years?: number[];
};
export type GetApiFinanceTotalsByKeyApiResponse =
  /** status 200 Success */ FinanceTotalsDto[];
export type GetApiFinanceTotalsByKeyApiArg = {
  key: string;
};
export type GetApiFinanceTotalsByKeyYearsApiResponse =
  /** status 200 Success */ FinanceTotalsDto[];
export type GetApiFinanceTotalsByKeyYearsApiArg = {
  years?: number[];
  key: string;
};
export type GetApiFinanceTotalsYearsApiResponse =
  /** status 200 Success */ FinanceTotalsDto[];
export type GetApiFinanceTotalsYearsApiArg = {
  years?: number[];
};
export type GetApiScheduleBDetailByKeyApiResponse =
  /** status 200 Success */ ScheduleBDetailDto[];
export type GetApiScheduleBDetailByKeyApiArg = {
  key: string;
};
export type GetApiScheduleBDetailByKeyYearsApiResponse =
  /** status 200 Success */ ScheduleBDetailDto[];
export type GetApiScheduleBDetailByKeyYearsApiArg = {
  years?: number[];
  key: string;
};
export type GetApiScheduleBDetailYearsApiResponse =
  /** status 200 Success */ ScheduleBDetailDto[];
export type GetApiScheduleBDetailYearsApiArg = {
  years?: number[];
};
export type GetApiScheduleBOverviewByKeyApiResponse =
  /** status 200 Success */ ScheduleBCandidateOverview[];
export type GetApiScheduleBOverviewByKeyApiArg = {
  key: string;
};
export type GetApiUiCandidatesbyYearByYearApiResponse =
  /** status 200 Success */ CandidateUidto[];
export type GetApiUiCandidatesbyYearByYearApiArg = {
  wasElected?: boolean;
  year: number;
};
export type GetApiUiElectionYearsApiResponse =
  /** status 200 Success */ number[];
export type GetApiUiElectionYearsApiArg = void;
export type CandidatePrincipalCommittees = {
  affiliatedCommitteeName?: string | null;
  candidateIds?: string[] | null;
  committeeId?: string | null;
  committeeType?: string | null;
  committeeTypeFull?: string | null;
  cycles?: number[] | null;
  designation?: string | null;
  designationFull?: string | null;
  filingFrequency?: string | null;
  firstF1Date?: string | null;
  firstFileDate?: string | null;
  lastF1Date?: string | null;
  lastFileDate?: string | null;
  name?: string | null;
  organizationType?: string | null;
  organizationTypeFull?: string | null;
  party?: string | null;
  partyFull?: string | null;
  state?: string | null;
  treasurerName?: string | null;
};
export type CandidateDto = {
  candidateId?: string | null;
  candidateStatus?: string | null;
  cycles?: number[] | null;
  district?: string | null;
  electionYears?: number[] | null;
  hasRaisedFunds?: boolean;
  inactiveElectionYears?: number[] | null;
  lastF2Date?: string | null;
  lastFileDate?: string | null;
  name?: string | null;
  office?: string | null;
  officeFull?: string | null;
  party?: string | null;
  partyFull?: string | null;
  principalCommittees?: CandidatePrincipalCommittees[] | null;
  state?: string | null;
};
export type FinanceTotalsDto = {
  activeThrough?: number | null;
  addressState?: string | null;
  candidateElectionYear?: number | null;
  candidateId?: string | null;
  candidateStatus?: string | null;
  cashOnHandEndPeriod?: number | null;
  cycle?: number;
  cycles?: number[] | null;
  debtsOwedByCommittee?: number | null;
  disbursements?: number | null;
  electionYear?: number;
  electionYears?: number[] | null;
  federalFundsFlag?: boolean | null;
  hasRaisedFunds?: boolean | null;
  individualItemizedContributions?: number | null;
  lastF2Date?: string | null;
  name?: string | null;
  office?: string | null;
  officeFull?: string | null;
  otherPoliticalCommitteeContributions?: number | null;
  party?: string | null;
  partyFull?: string | null;
  state?: string | null;
};
export type ScheduleBDetailDto = {
  committeeId?: string | null;
  committeeName?: string | null;
  cycle?: number;
  recipientId?: string | null;
  recipientName?: string | null;
  total?: number;
};
export type ScheduleBCandidateOverview = {
  totalDisbursements?: number;
  totalResultPages?: number;
  candidateId?: string | null;
  principalCommitteeId?: string | null;
};
export type CandidateUidto = {
  candidateId?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  party?: string | null;
  wasElected?: boolean;
  district?: string | null;
};
export const {
  useGetApiCandidateByKeyQuery,
  useGetApiCandidateYearsQuery,
  useGetApiFinanceTotalsByKeyQuery,
  useGetApiFinanceTotalsByKeyYearsQuery,
  useGetApiFinanceTotalsYearsQuery,
  useGetApiScheduleBDetailByKeyQuery,
  useGetApiScheduleBDetailByKeyYearsQuery,
  useGetApiScheduleBDetailYearsQuery,
  useGetApiScheduleBOverviewByKeyQuery,
  useGetApiUiCandidatesbyYearByYearQuery,
  useGetApiUiElectionYearsQuery,
} = injectedRtkApi;
