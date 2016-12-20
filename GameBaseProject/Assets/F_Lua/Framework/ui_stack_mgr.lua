--[[--
 * @Description: 负责管理lua层进入的UI
 * @Author:      armyshu
 * @FileName:    ui_stack_mgr
 * @DateTime:    2016-05-23 15:19:33
 ]]


ui_stack_mgr = {}
local this = ui_stack_mgr

this.ui_stack = {}
this.registFlag = false

local allValidHud = {}

--[[--
 * @Description: 
 ]]
function this.PushToStack(uiObj)
	if (not this.registFlag) then
		this.registFlag = true
		Notifier.regist(cmdName.SHOW_SCENE, function ( ... )
			this.ui_stack = {}
		end)
	end

	if (uiObj.needToHideOther) then
		for k, v in ipairs(this.ui_stack) do
			if (not IsNil(v.gameObject) and v.canNotHide == nil) then
				if (v.gameObject.activeSelf) then
					if (not (uiObj.isDialogue and v.isHud)) then
						v.gameObject:SetActive(false)
						v.controlVisibleBySelf = false
						v.hideByStack = true
					end
				else
					if (not v.hideByStack) then
						v.controlVisibleBySelf = true
					else
						if (uiObj.isDialogue and v.isHud) then
							v.gameObject:SetActive(true)
							v.controlVisibleBySelf = false
							v.hideByStack = false
						end
					end
				end
			end
		end

		--hall_system.SetSceneObjVisible(false)
	end

	table.insert(this.ui_stack, uiObj)
end

function this.PopFromStack(uiObj)
	if (uiObj.needToHideOther and this.ShouldActivePreviousUI(uiObj)) then
		local objIdx = this.FindUIObjIdx(uiObj)
		if (objIdx ~= -1) then
			for k = 1, objIdx do
				local uiObjElement = this.ui_stack[k]
				if (not uiObjElement.controlVisibleBySelf and not IsNil(uiObjElement.gameObject)) then
					uiObjElement.gameObject:SetActive(true)
					if (uiObjElement.OnEnable ~= nil) then
						uiObjElement:OnEnable()
					end

					if (uiObjElement.hideByStack) then
						uiObjElement.hideByStack = false
					end
				end
			end
		end
	end

	local objIdx = this.FindUIObjIdx(uiObj)
	if (objIdx ~= -1) then
		table.remove(this.ui_stack, objIdx)
	end

	if (this.IsFullScreenUIExistInStack()) then
		Trace("PopFromStack, hall_system.SetSceneObjVisible(false)")
		hall_system.SetSceneObjVisible(false)
	else
		hall_system.SetSceneObjVisible(true)
	end
end

function this.ClearStack()
	this.ui_stack = {}
end

function this.FindUIObjIdx(uiObj)
	local ret = -1
	for k,v in ipairs(this.ui_stack) do
		if (v == uiObj ) then
			ret = k
			break
		end
	end

	return ret
end

function this.DestroyAllExceptMainScene()
	local idxToRemove = {}
	for k, v in ipairs(this.ui_stack) do
		if not this.ShouldPreserve(v) then
			if (not IsNil(v.gameObject)) then
				local targetIdx = this.FindUIObjIdx(v)
				table.insert(idxToRemove, targetIdx)
				destroy(v.gameObject)
			end
		else
			if (not IsNil(v.gameObject) and (v.controlVisibleBySelf == false)) then
				v.gameObject:SetActive(true)
			end
		end
	end

	for k, v in ipairs(idxToRemove) do
		table.remove(this.ui_stack, v)
	end
end