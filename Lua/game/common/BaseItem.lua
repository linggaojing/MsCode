BaseItem = BaseItem or BaseClass()

function BaseItem:__definedVar( )
	return {
        _class_type = self,
        base_file = "",
        panel_name = "",
        gameObjcet = false,
        transform = false,
        LoadCallBack = false,
        is_loaded = false,
    }
end

function BaseItem:__init( parent,prefab )
	self.parent = parent
	self.prefab = prefab
end

function BaseItem:StarLoad(  )
	if self.prefab then
		self.gameObjcet = newObject(prefab)
	else
		function load_item_finish(objs)
			if objs then
				if self._use_delete_method then
					destroy(objs)
					return
				end
				self:ResLoadCallBack(objs)
			end
		end
		luaViewMgr:LoadItem(self,self.base_file,self.panel_name, load_item_finish)
	end
end

function BaseItem:ResLoadCallBack( objs )
	self.gameObjcet = objs
	self.transform = objs.transform
	if self.LoadCallBack then
		self:LoadCallBack()
	end
	if self.position_cache then
		self:SetPosition(self.position_cache.x,self.position_cache.y,self.position_cache.z)
		self.position_cache = nil
	end
end

function BaseItem:SetPosition( x,y,z )
	x = x or 0
	y = y or 0
	z = z or 0
	if self.is_loaded then
		self.transform.localPosition = Vector3(x,y,z)
	else
		self.position_cache = luaTabel.XYZ(x,y,z)
	end
end