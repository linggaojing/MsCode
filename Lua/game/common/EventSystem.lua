EventSystem = EventSystem or BaseClass()

function EventSystem:__init(  )
	self.event_count = 1
	self.evnt_list = {}
	self.evnt_id_2_name_list = {}
end


function EventSystem:Bind(event,callBack )
	if event == nil then
		print("没有事件名")
		return
	end
	if callBack == nil then
		print("事件没有回调")
		return
	end
	local list = self:GetEventList(event)
	self.event_count = self.event_count + 1
	list[self.event_count] = callBack
	self.evnt_id_2_name_list[self.event_count] = event
	return self.event_count
end

function EventSystem:UnBind( event_id )
	if event_id then
		local event_name = self.evnt_id_2_name_list[event_id]
		if event_name then
			local list = self:GetEventList(event_name)
			if list then
				list[event_id] = nil
			end
		end
		self.evnt_id_2_name_list[event_id] = nil
	end
end

function EventSystem:GetEventList( event )
	local list = self.evnt_list[event]
	if list == nil then
		list = {}
		self.evnt_list[event] = list
	end
	return list
end

function EventSystem:Fire( event,... )
	if event == nil then
		print("这是个空的event")
		return
	end
	local list = self:GetEventList(event)
	for _,callBack in pairs(list) do
		callBack(...)
	end
end