function PrintCallStack( )
	-- body
	local level = 1
	while true do
		local info = debug.getinfo(level, "Sl")
		if not info then break end
		if info.what == "C" then
			print(level, "C function")
		else
			print(string.format("[%s]:%d",
				info.short_src, info.currentline))
		end
		level = level + 1
	end
end

function PrintTable( tbl , level)
	-- if RuntimePlatform and (ApplicationPlatform == RuntimePlatform.Android or ApplicationPlatform == RuntimePlatform.IPhonePlayer) then
	-- 	return
	-- end

	if tbl == nil or type(tbl) ~= "table" then
		return
	end

	level = level or 1

	local indent_str = ""
	for i = 1, level do
		indent_str = indent_str.."	"
	end
	print(indent_str .. "{")
	for k,v in pairs(tbl) do

		local item_str = string.format("%s%s = %s", indent_str .. "	",tostring(k), tostring(v))
		print(item_str)
		if type(v) == "table" then
			PrintTable(v, level + 1)
		end
	end
	print(indent_str .. "}")
end

--获取文件名
local function getFileName(str, index)
    if not index then
        index = 1
    end
    local si = string.find(str, "%.", index)

    if si then
        return getFileName(str, si + 1)
    else
        return string.sub(str, index)
    end
end

function GetChildGameObjects( transform,name_list )
	if transform then
		local list = {}
		for i,name in ipairs(name_list) do
			list[i] = transform:Find(name).gameObject
		end
		return unpack(list)
	end
end

function destroy( go )
	Util.DestroyGo(go)
end

--输出日志--
function log(...)
	local str = packageContent(...)
    Util.Log(str);
end

function packageContent( ... )
	local str = ""
	local args = {...}
	for i,v in ipairs(args) do
		str = str..tostring(v).."\t"
	end
	return str
end

--错误日志--
function logError(str) 
	Util.LogError(str);
end

--警告日志--
function logWarn(str) 
	Util.LogWarning(str);
end

function newObject(prefab)
	return GameObject.Instantiate(prefab);
end

function AddUpEvent( go,call_back )
	LuaEventListener.AddUpEvent(go,call_back)
end

function newObject( prefab )
	return Util.NewObject(prefab)
end

function AddClickEvent( go,func )
	LuaClickListener.Get(go).onClick = func
end



-- local function moduleRequire(name)
--     if not package.loaded[name] then
--         local loader = loadfile
--         if loader == nil then
--             error("unable to load module " .. name)
--         end
--         package.loaded[name] = true
--         local loadRes = loader(name .. ".lua")
--         local env = {_G = _G} --引入全局表的环境 通过_G.xxxx形式访问全局变量

--         --local loadResWithEnv =  --设置环境，让加载的文件模块化

--         local res = setfenv(loadRes, env)

--         _G[name] = env --把模块变成只读
--         if res ~= nil then
--             package.loaded[name] = res
--         end
--     end
--     return package.loaded[name]
-- end
-- _G.originalRequire = _G.require
-- _G.require = moduleRequire
