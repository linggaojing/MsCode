LuaViewManager = LuaViewManager or BaseClass()

function LuaViewManager:__init(  )
	self.Instance = self
end

function LuaViewManager:getInstance()
	if self.Instance == nil then
		self.Instance = LuaViewManager.New()
	end
	return self.Instance
end

function LuaViewManager:LoadView(ref_tar,base_file,panel_name,layer_name, load_finish )
	--todo 先在乱判断一波 符合再调用接口
	if ref_tar.use_local_view then
		local go_path = "GameRoot/Canvas/"..layer_name.."/"..panel_name
		local go = GameObject.Find(go_path)
		if go then
			go.transform.localScale = Vector3.one
	        go.transform.localPosition = Vector3.zero
	        go.gameObject:SetActive(true)
			load_finish(go)
		else
			print("can not find "..go_path)
			return
		end
	else
		panelMgr:CreatePanel(base_file, panel_name, layer_name, load_finish)
	end
end


function LuaViewManager:LoadItem(ref_tar,base_file,panel_name, load_finish )
	--todo 先在乱判断一波 符合再调用接口
	panelMgr:CreateItem(base_file, panel_name, load_finish)
end