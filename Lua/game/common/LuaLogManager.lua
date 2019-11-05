LuaLogManager = LuaLogManager or BaseClass()
function LuaLogManager:__init()
	LuaLogManager.Instance = self

    self:SetLogEnable(true)
	local old_print_func = _G.print
	_G.print = function ( ... )
		if RuntimePlatform and (Application.platform == RuntimePlatform.Android or Application.platform == RuntimePlatform.IPhonePlayer) then
			return
		end
    	self:Log(...)
	end
end

function  LuaLogManager:getInstance()
	if LuaLogManager.Instance == nil then
		LuaLogManager.New()
	end
	return LuaLogManager.Instance
end

function LuaLogManager:__delete()
end

function LuaLogManager:PackageContent(...)
	local arg = {...}
	local printResult = ""
    for i,v in pairs(arg) do
       printResult = printResult .. tostring(v) .. "\t"
    end
    return printResult
end

function LuaLogManager:SetLogEnable(value)
	if logMgr then
		logMgr.EnableLog = value
	end
end


function LuaLogManager:Log( ... )
    if logMgr then
		if RuntimePlatform and (Application.platform == RuntimePlatform.Android or Application.platform == RuntimePlatform.IPhonePlayer) then
			return
		end
		local printResult = self:PackageContent(...)
		logMgr.Log(printResult)
    end
end

--警告日志--
function LuaLogManager:LogWarn( ... ) 
	if logMgr then
		if RuntimePlatform and (Application.platform == RuntimePlatform.Android or Application.platform == RuntimePlatform.IPhonePlayer) then
			return
		end
		local printResult = self:PackageContent(...)
    	logMgr.LogWarning(printResult)
    end
end

--错误日志--
function LuaLogManager:LogError( ... ) 
	if logMgr then
		local printResult = self:PackageContent(...)
    	logMgr.LogError(printResult)

    	GameError.Instance:SendErrorToPHP(printResult)
    end
end
