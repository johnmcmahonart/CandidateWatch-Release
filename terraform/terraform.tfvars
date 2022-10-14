solution_prefix="candidatewatch"
table_affix="Table"
keyvault="SecVaultPrimary"
fec_access_key="FECAPISecret"
process_queues=["committeeprocess","financetotalsprocess","schedulebcandidateprocess","schedulebpageprocess"]
functions=["BuildCandidatebyYearPartition","GetCandidateData","GetFinanceData","GetCommitteeData","GetScheduleBDisbursement","PurgeAllSolutionTables","SendUpdates","Tests","ValidateScheduleB"]
state_twoletter=["AL","KY","OH","AK","LA","OK","AZ","ME","OR","AR","MD","PA","AS","MA","PR","CA","MI","RI","CO","MN","SC","CT","MS","SD","DE","MO","TN","DC","MT","TX","FL","NE","TT","GA","NV","UT","GU","NH","VT","HI","NJ","VA","ID","NM","VI","IL","NY","WA","IN","NC","WV","IA","ND","WI","KS","CM","WY"]

