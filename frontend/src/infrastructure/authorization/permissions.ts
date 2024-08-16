export const policies = {
	getTimesheetEntriesEndpointPolicy: 'timespace:timesheet:view',
	addTimesheetEntryEndpointPolicy: 'timespace:timesheet:add',
	approveAllTimesheetEntriesEndpointPolicy: 'timespace:timesheet:approval:approve-all',
	approveTimesheetEntryEndpointPolicy: 'timespace:timesheet:approval:approve',
	denyTimesheetEntryEndpointPolicy: 'timespace:timesheet:approval:deny',
	getApprovableTimesheetEntriesEndpointPolicy: 'timespace:timesheet:approval:view',
	getMembersEndpointPolicy: 'timespace:tenant:members:view',
	getInvitesEndpointPolicy: 'timespace:tenant:invites:view',
	updateMemberEndpointPolicy: 'timespace:tenant:members:update',
	disableMemberEndpointPolicy: 'timespace:tenant:members:disable',
	getAggregatedTimesheetEntriesEndpointPolicy: 'timespace:timesheet:aggregated:view'
}
