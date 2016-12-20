--[[--
 * @Description:	游戏管理器
 * @Author:			farley
 * @Path:			Common/GameMain
 * @DateTime:		2016-11-18 11:24:21
]]
require "Common/Global"

GameMain = {}
this = GameMain

function this.StartGame()
	require "Logic/ANormal/ui_normal"
	ui_normal.ShowUI()
end