KeyInptManager = KeyInptManager or BaseClass()
local Input = UnityEngine.Input
local KeyCode = UnityEngine.KeyCode
function KeyInptManager:__init(  )
	UpdateBeat:Add(self.Update,self)
end

function KeyInptManager:Update(  )
	if Input.GetKeyDown(KeyCode.Escape) then
		Application.Quit()
	end
end

