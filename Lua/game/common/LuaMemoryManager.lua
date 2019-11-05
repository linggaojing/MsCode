LuaMemoryManager = LuaMemoryManager or BaseClass()

function LuaMemoryManager:__init(  )
	collectgarbage("setpause", 110)
    collectgarbage("setstepmul", 200)
    if logMgr then
    	logMgr.EnableLog = true
    end
	self:OverideRequire()
	_G.print = function ( ... )
		if Application.platform == RuntimePlatform.Android or Application.platform == RuntimePlatform.IPhonePlayer then
			return
		end
		log(...)
	end
end

function LuaMemoryManager:OverideRequire()
	local function newRequire(file_path)
		oriRequire(file_path)
		-- PrintCallStack()
	end
	_G.oriRequire = _G.require
	_G.require = newRequire
end