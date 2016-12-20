--[[
用来开启调试标记位，目前包括Trace，asset等
]]
Model = {}
local  this = Model

this.assertEnable = true -- 是否打开assert

this.traceEnable = true    -- 是否打开Trace

local objDebugUI = nil
function this.ShowDebugDlg()
	if IsNil(objDebugUI) then
		objDebugUI = newNormalUI("Prefabs/UI/debugui")
	end
end
