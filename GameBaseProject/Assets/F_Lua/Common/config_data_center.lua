--[[--
 * @Description:	lua层配置文件管理
 * @Author:			farley
 * @Path:			config_data_center
 * @DateTime:		2016-12-10 10:06:25
]]

config_data_center = {}
local this = config_data_center

local configProtobufDict = {}
local configProtobufItemsDict = {}
local pre = "protobuf_conf_parser/dataconfig_"
local dp = "dataconfig_"
local pdt = configProtobufDict

local configInitFuncDict = {
	[dp.."tablemodeone"] = function () require (pre.."tablemodeone") pdt[dp.."tablemodeone"] = tablemodeone_x end,
	[dp.."tablemodetwo"] = function () require (pre.."tablemodetwo") pdt[dp.."tablemodetwo"] = tablemodetwo_x end,
}

--[[--
 * @Description: 载入配置文件
]]
function loadConfig(configName)
	local luaByteBuffer =  ProtobufDataConfigMgr.ReadConfigDataToLua(configName)
	Trace("luaByteBuffer:"..tostring(luaByteBuffer))
end

function this.test()
	loadConfig("dataconfig_tablemodeone")
end