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
	[dp.."tablemodeone"] = function () require (pre.."tablemodeone_pb") pdt[dp.."tablemodeone"] = tablemodeone_x end,
	[dp.."tablemodetwo"] = function () require (pre.."tablemodetwo_pb") pdt[dp.."tablemodetwo"] = tablemodetwo_x end,
}

--[[--
 * @Description: 载入配置文件
]]
function loadConfig(configName)
	local luaByteBuffer =  ProtobufDataConfigMgr.ReadConfigDataToLua(configName)
	if configProtobufDict[configName] == nil then
		local func = configInitFuncDict[configName]
		if func ~= nil then
			func()
		else
			Fatal("Func read file error:"..configName)
		end

		if configProtobufDict[configName] == nil then
			Fatal("read file error:"..configName)
		end
	end

	local protobufConfig = configProtobufDict[configName]:GetProtobuf()
	protobufConfig:ParseFromString(luaByteBuffer)

	local protobufItems = configProtobufDict[configName].New()
	protobufItems:ParseData(protobufConfig)
	configProtobufItemsDict[configName] = protobufItems.items

	protobufConfig:Clear()
	protobufConfig = nil
end

--[[--
 * @Description: 读取配置文件列表
]]
function this.loadConfigByName(configName)
	if configProtobufItemsDict[configName] ~= nil then
		return configProtobufItemsDict[configName]
	end
	return nil
end

function this.test()
	loadConfig("dataconfig_tablemodeone")

	local tableConfig = this.loadConfigByName("dataconfig_tablemodeone")
	Trace("length:"..#tableConfig)
	for i = 1,#tableConfig do
		Trace("position:"..tableConfig.ParaOne)
	end
end