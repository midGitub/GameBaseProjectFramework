--[[--
 * @Description:	lua层UI模板
 * @Author:			farley
 * @Path:			Logic/ANormal/ui_normal_click
 * @DateTime:		2016-09-03 10:25:08
]]
ui_normal_click = ui_base.New()
local this = ui_normal_click

local CloseUI = nil

function this.Awake()
	this:RegistUSRelation() --调用ui_base中的US关系注册，US简写表示UI-Scene
	this.Init()
end

function this.Start()
	this.RegistEvent()
end

function this.OnDestroy()
	this.UnregistEvent()
	this.Uninit()
	this:UnRegistUSRelation() -- 注销UI关系
	this.gameObject = nil
end

--初始化
function this.Init()
	
end

--注册事件
function this.RegistEvent()
	addClickCallback(this.transform,"background/btn_close",CloseUI)
end

--反初始化
function this.Uninit()
	
end

--注销事件
function this.UnregistEvent()
	
end

--[[--
 * @Description: 显示UI
]]
function this.ShowUI()
	if IsNil(this.gameObject) then
		newNormalUI("Prefab/LogicUI/ANormal/ui_normal_click")
	end

	this.gameObject:SetActive(true)
end

--[[--
 * @Description: 关闭UI
]]
function CloseUI()
	this.gameObject:SetActive(false)
end