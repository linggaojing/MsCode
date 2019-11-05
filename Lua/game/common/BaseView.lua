BaseView = BaseView or BaseClass()

function BaseView:__definedVar()
    return {
        _class_type = self,
        base_file = "",
        panel_name = "",
        layer_name = "Main",
        use_click_bg = false,
        is_set_zdepth = false,
        gameObjcet = false,
        transform = false,
        load_callback = false,
        destroy_callback = false,
        use_local_view = false,
    }
end

function BaseView:__init()

end

function BaseView:Open()
    BaseView.DoCreateView(self)
end

function BaseView:DoCreateView()
    function load_view_finish(objs)
        if objs then
        	if self._use_delete_method then
        		destroy(objs)
        		return
        	end
            self.gameObjcet = objs
            self.transform = objs.transform
            if self.load_callback then
                self:load_callback()
            end
        end
    end
    luaViewMgr:LoadView(self,self.base_file,self.panel_name, self.layer_name, load_view_finish)
end

function BaseView:AddClickEvent(go, call_func)
    if go == nil then
        return
    end
    if not self.luaBehaviour and self.gameObjcet then
        self.luaBehaviour = self.gameObjcet:GetComponent("LuaBehaviour")
    end
    if not self.luaBehaviour then
        print("找不到LuaBehaviour")
        return
    end
    self.luaBehaviour:AddClick(go, call_func)
end

function BaseView:Close(  )
	self:Destroy()
end

function BaseView:Destroy(  )
	self:DeleteMe()
	if self.gameObjcet then
		destroy(self.gameObjcet)
	end
	if self.destroy_callback then
		self.destroy_callback()
	end
end
