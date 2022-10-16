solution_prefix="candidatewatch"
table_affix="Table"
keyvault_name="SecVaultPrimary"
fec_access_key="FECAPISecret"
process_queues=["committeeprocess","financetotalsprocess","schedulebcandidateprocess","schedulebpageprocess"]
functions=["CW-BuildCandidatebyYearPartition","CW-GetCandidateData","CW-GetFinanceData","CW-GetCommitteeData","CW-GetScheduleBDisbursement","CW-PurgeAllSolutionTables","CW-SendUpdates","CW-Tests","CW-ValidateScheduleB"]
state_twoletter=["AL","KY","OH","AK","LA","OK","AZ","ME","OR","AR","MD","PA","AS","MA","PR","CA","MI","RI","CO","MN","SC","CT","MS","SD","DE","MO","TN","DC","MT","TX","FL","NE","TT","GA","NV","UT","GU","NH","VT","HI","NJ","VA","ID","NM","VI","IL","NY","WA","IN","NC","WV","IA","ND","WI","KS","CM","WY"]
keyvault_id="/subscriptions/782918b5-c24a-4ef2-8042-1b90bf912ae3/resourceGroups/BatchRendering/providers/Microsoft.KeyVault/vaults/SecVaultPrimary"
