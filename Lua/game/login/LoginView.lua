LoginView = LoginView or BaseClass(BaseView)
local table_insert = table.insert
local table_sort = table.sort

function LoginView:__init()
    self.base_file = "login"
    self.panel_name = "LoginView"
    self.layer_name = "Main"
    -- self.use_local_view = true
    self.background_alpha = 0.5
    self.is_set_zdepth = false
    self.use_background = true
    self.destroy_imm = true
    self.change_scene_close = true
    self.click_bg_toClose = false
    self.load_callback = function()
        self:LoadSuccess()
        self:InitEvent()
    end

    self.open_callback = function()
        self:SetData()
    end

    self.close_callback = function()
    end

    self.destroy_callback = function()
        self:Remove()
    end
end

function LoginView:Remove()
    self.model = nil
end

function LoginView:LoadSuccess()
    -- self.btn =
    --     GetChildGameObjects(
    --     self.transform,
    --     {
    --         "BG/Panel/welcome"
    --     }
    -- )
    -- local function click_func( target,x,y )
    --     print("可以了啊",target,x,y)
    -- end
    -- AddUpEvent(self.btn,click_func)
end

function LoginView:Open()
    BaseView.Open(self)
end

function LoginView:InitEvent()
    local function clickFunc(target, x, y)
        print("=====131321312313")
    end
    -- self:AddClickEvent(self.btn, clickFunc)
end

function LoginView:SetData()
end
