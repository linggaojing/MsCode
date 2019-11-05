--主入口函数。从这里开始lua逻辑
-- require('LuaDebuggee').StartDebug("127.0.0.1", 9826)
local require = require
require "systool.RequireEngine"
require "systool.RequireSystool"
require "game.common.LuaMemoryManager"

require "Common.protocal"

require("game.login.LoginView")
LuaMemoryManager.New()
GameMain = GameMain or {}
function Main()
    require("game.common.RequireCommon")
    luaViewMgr = LuaViewManager:getInstance()
    luaLogMgr = LuaLogManager:getInstance()
    GlobalEventSystem = EventSystem.New()
    GameControllerFiles = GameControllerFiles or {}
    GameControllerNameFiles = GameControllerNameFiles or {}
    require("game.common.ModulesController")
    ModulesController.New()
    UpdateBeat:Add(GameMain.Update)
    LateUpdateBeat:Add(GameMain.LateUpdate)
end

function GameMain.Update(  )
    if not GameMain.require_controller_done and GameControllerFiles then
        GameMain.StarRequireGameController()
    end

    if GameMain.require_controller_done and GameControllerNameFiles then
        GameMain.StarInstanceGameController()
    end
end

function GameMain.LateUpdate(  )

end

--分帧加载controller
function GameMain.StarRequireGameController(  )
    for i=1,3 do
        RequireControllerIndex = RequireControllerIndex or 1
        require(GameControllerFiles[RequireControllerIndex])
        if RequireControllerIndex == #GameControllerFiles then
            GameMain.require_controller_done = true
            GameControllerFiles = nil
            return
        end
        RequireControllerIndex = RequireControllerIndex + 1
    end
end

--分帧实例化controller
function GameMain.StarInstanceGameController(  )
    for i=1,3 do
        InstanceControllerIndex = InstanceControllerIndex or 1
        local name = GameControllerNameFiles[InstanceControllerIndex]
        if name then
            _G[name].New()
        end
        if InstanceControllerIndex == #GameControllerNameFiles then
            GameMain.instance_controller_done = true
            GameControllerNameFiles = nil
            return
        end
        InstanceControllerIndex = InstanceControllerIndex + 1
    end
end