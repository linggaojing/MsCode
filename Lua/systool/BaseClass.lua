--所有类的基类

local function CreateFunc(class_type, obj, ...)
    if class_type.base then
        CreateFunc(class_type.base, obj, ...)
    end
    if class_type.__init then
        class_type.__init(obj, ...)
    end
end

local function DeleteFunc(self)
    if self._use_delete_method then
        return
    end
    self._use_delete_method = true
    local now_class = self._class_type
    while now_class ~= nil do
        if now_class.__delete then
            now_class.__delete(self)
        end
        now_class = now_class.base
    end
end

function BaseClass(base)
    local class_type = {
        New = nil,
        DeleteMe = nil,
        base = nil
    }
    class_type.base = base

    class_type.New = function(...)
    	if class_type._is_instance then
    		return
    	end
        local obj = nil --实例化中最上层的表

        --该类型是否有定义值的__definedVar方法
        if class_type.__definedVar then
            obj = class_type:__definedVar()
        else
            obj = {
                _class_type = class_type,
                _use_delete_method = false,
                DeleteMe = nil
            }
        end

        --将该对象的数据层设置为其自身
        setmetatable(
            obj,
            {
                __index = function(t, k)
                    --底层数据移动到上层 实现继承 重写
                    local value = class_type[k]
                    obj[k] = value
                    return value
                end
            }
        )

        CreateFunc(class_type, obj, ...)
        obj.DeleteMe = DeleteFunc
        if class_type.getInstance then
        	class_type._is_instance = true
        end
        return obj
    end

    --如果该类是子类，如果某些键值不存在，则在父类中查找
    --由此可以实现继承
    if base then
        setmetatable(
            class_type,
            {
                __index = function(t, k)
                    --底层数据移动到上层 实现继承 重写
                    local value = base[k]
                    class_type[k] = value
                    return value
                end
            }
        )
    end

    return class_type
end
