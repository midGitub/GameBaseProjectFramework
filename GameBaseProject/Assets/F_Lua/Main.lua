--[[--
 * @Description:	通过LuaClient进入Main，作为Lua层的入口
 * @Author:			farley
 * @Path:			class_name
 * @DateTime:		2016-11-16 17:21:52
]]
--主入口函数。从这里开始lua逻辑
require "Common/GameMain"

function Main()					
	GameMain.StartGame()
end