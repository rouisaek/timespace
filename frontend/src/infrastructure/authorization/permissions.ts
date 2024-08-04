export const permissions = {
	user: {
		time: {
			view: 'timespace:time:view',
			add: 'timespace:time:add'
		}
	},
	employee: {
		time: {
			view: 'timespace:employee:time:view',
			clockHours: 'timespace:employee:time:clock-hours'
		}
	},
	manager: {
		time: {
			view: 'timespace:manager:time:view',
			approve: 'timespace:manager:time:approve'
		}
	}
}
