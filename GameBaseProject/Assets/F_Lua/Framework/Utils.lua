require("Framework/XArray")
require("Framework/Model")

Utils = {}

_assert = nil
_traceProtobuf = nil

local hex2Binary = {
	["0"] = "0000",
	["1"] = "0001",
	["2"] = "0010",
	["3"] = "0011",
	["4"] = "0100",
	["5"] = "0101",
	["6"] = "0110",
	["7"] = "0111",
	["8"] = "1000",
	["9"] = "1001",
	["A"] = "1010",
	["B"] = "1011",
	["C"] = "1100",
	["D"] = "1101",
	["E"] = "1110",
	["F"] = "1111",
}

--[[断言]]
function _assert(condition , msg )
	if(not Model.assertEnable)then
		return
	end

	if(not condition)then
		Fatal("assert fail description:")
		Fatal(msg)
	end
end

--[[警告]]
function _warm(condition, msg)
	if(not Model.assertEnable)then
		return
	end
	if(not condition)then
		_trace("assert fail description:")
		_trace(msg)
	end
end

--[[抛出异常]]
function _throw(msg )
	if(not Model.assertEnable)then
		return
	end
	error(msg)
end

--[[
	清空数组的所有元素
	@param t 要清空的顺序表
--]]
Utils.clear = function(t)
	local n = #t
	while(n > 0) do
		table.remove(t , n)	--从后往前删可以提高性能
		n = n - 1
	end
end


--[[清空protobuf的repeated 字段table]]
Utils.clearProtobufTable = function(t)
	local n = #t
	while(n > 0) do
		t:remove(n)
		n = n - 1
	end

end

--[[得到任意一个table的长度]]
Utils.get_length_from_any_table = function(tableValue)
	local tableLength = 0
	
	for k, v in pairs(tableValue) do
		tableLength = tableLength + 1
	end
	
	return tableLength
end

--[[
	clone对象，针对protobuf的地方使用
	@param objLeft	左值
	@param objRight 右值
--]]
Utils.tclone = function(objLeft , objRight)
	local strData = objRight:SerializeToString()
	objLeft:ParseFromString(strData)
end

--[[
	对table里面的每一项做值拷贝，只有一层有效
	@param objLeft	左值
	@param objRight 右值
--]]
Utils.copy = function(objLeft , objRight)
	for k,v in pairs(objRight)do
		objLeft[k] = v
	end
end

--[[
	根据分隔符分割字符串，返回分割后的table
--]]
Utils.split = function(s, delim)
  assert (type (delim) == "string" and string.len (delim) > 0, "bad delimiter")
  local start = 1
  local t = {}  -- results table

  -- find each instance of a string followed by the delimiter
  while true do
    local pos = string.find (s, delim, start, true) -- plain find
    if not pos then
      break
    end

    table.insert (t, string.sub (s, start, pos - 1))
    start = pos + string.len (delim)
  end -- while

  -- insert final one (after last delimiter)
  table.insert (t, string.sub (s, start))
  return t
end


Utils.int2ip = function(intIP)
	local retIP = ""
	local leftValue = intIP
	for i=0,3 do
		local temp = math.pow(256,3-i)
		local sectionValue = math.floor(leftValue/temp)
		leftValue = leftValue % temp
		retIP = sectionValue..retIP
		if(i~=3) then
			retIP = "."..retIP
		end
	end
	return retIP
end

--[[
	打印调试信息
--]]
_trace = function(msg)
	if(not Model.traceEnable or msg == nil)then
		return
	end

	Trace(msg)
end

--[[字符长度]]
Utils.length = function(str)
	return #(str:gsub('[\128-\255][\128-\255]',' '))
end

--[[子字符串]]
Utils.sub = function(str,s,e)
	str = str:gsub('([\001-\127])','\000%1')
	str = str:sub(s*2+1,e*2)
	str = str:gsub('\000','')
	return str
end
--[[
	将文本碎片
	@param str 原文本
	@return XArray<string>
]]
function Utils.splitWord(str)
	local ret = XArray.create()
	local n = string.len(str)
	local str2 = ""
    local i = 1 
    while i <= n do
   		local flag = string.byte(str , i)
   		if(flag < 128)then--1byte
   			str2 = string.sub(str , i , i)
   			ret:add(str2)
   			i = i + 1
   		elseif(flag >= 128 + 64 and flag < 128 + 64 + 32)then--2byte
   			str2 = string.sub(str, i , i + 1)
   			ret:add(str2)
   			i = i + 2
   		elseif(flag < 128 + 64 + 32 + 16)then--3byte
   			str2 = string.sub(str, i , i + 2)
   			ret:add(str2)
   			i = i + 3
   		elseif(flag < 128 + 64 + 32 + 16 + 8)then--4byte
   			str2 = string.sub(str, i , i + 3)
   			ret:add(str2)
   			i = i + 4
   		elseif(flag < 128 + 64 + 32 + 16 + 8 + 4)then--5byte
   			str2 = string.sub(str, i , i + 4)
   			ret:add(str2)
   			i = i + 5
   		else--6byte
   			str2 = string.sub(str, i , i + 5)
   			ret:add(str2)
   			i = i + 6
   		end
    end
 	return ret
 end 

 function Utils.makeWorld(xarray,n)
 	local str1, str2 = "",""
 	local i = 1
 	while i <= xarray.size do
 		if(i <= n)then
 			str1 = str1 .. xarray:at(i)
 		else
 			str2 = str2 .. xarray:at(i)
 		end
 		i = i + 1
 	end
 	return str1, str2
 end


Utils.trans = function(str)
   local n = string.len(str)
   local str2 = ""
   local i = 1 
   while i <= n do
   		local flag = string.byte(str , i)
   		if(flag < 128)then--1byte
   			str2 = str2 .. string.sub(str , i , i)
   			str2 = str2 .. "\n"
   			i = i + 1
   		elseif(flag >= 128 + 64 and flag < 128 + 64 + 32)then--2byte
   			str2 = str2 .. string.sub(str, i , i + 1)
   			str2 = str2 .. "\n"
   			i = i + 2
   		elseif(flag < 128 + 64 + 32 + 16)then--3byte
   			str2 = str2 .. string.sub(str, i , i + 2)
   			str2 = str2 .. "\n"
   			i = i + 3
   		elseif(flag < 128 + 64 + 32 + 16 + 8)then--4byte
   			str2 = str2 .. string.sub(str, i , i + 3)
   			str2 = str2 .. "\n"
   			i = i + 4
   		elseif(flag < 128 + 64 + 32 + 16 + 8 + 4)then--5byte
   			str2 = str2 .. string.sub(str, i , i + 4)
   			str2 = str2 .. "\n"
   			i = i + 5
   		else--6byte
   			str2 = str2 .. string.sub(str, i , i + 5)
   			str2 = str2 .. "\n"
   			i = i + 6
   		end
   end
   return str2
end

--[[--
 * @Description: 得到字符串（utf8编码）的字节长度
 ]]
Utils.utf8CharsLen = function(str)
   local n = string.len(str)
   local i = 1 
   local count = 0
   while i <= n do
   		local flag = string.byte(str , i)
   		if(flag < 128)then--1byte
   			count = count + 1
   			i = i + 1
   		elseif(flag >= 192 and flag < 224)then--2byte
   			count = count + 1
   			i = i + 2
   		elseif(flag < 240)then--3byte
   			count = count + 1
   			i = i + 3
   		elseif(flag < 248)then--4byte
   			count = count + 1
   			i = i + 4
   		elseif(flag < 252)then--5byte
   			count = count + 1
   			i = i + 5
   		else--6byte
   			count = count + 1
   			i = i + 6
   		end
   end
   return count
end

Utils.transSubString = function(str, num)
	local arr = Utils.splitWord(str)
	str = Utils.makeWorld(arr,num) .. "..."
	return str
end


Utils.CHINESE_MAP = {[0]="零", [1]="一", [2]="二", [3]="三", [4]="四", [5]="五", [6]="六", [7]="七", [8]="八", [9]="九"}
--[[数字转中文]]
Utils.toChinese = function(num, isIncludeTen)
	--assert(num<99)
	if(num > 99) then num = 99 end
	if(num < 10) then
		return Utils.CHINESE_MAP[num]
	else
		local a = math.floor(num/10)
		local b = num % 10
		if(b==0) then
			if (isIncludeTen and a == 1) then
				return "十"
			end
			return Utils.CHINESE_MAP[a].."十"
		else
			return Utils.CHINESE_MAP[a].."十"..Utils.CHINESE_MAP[b]
		end
	end
	return ""
end

--[[
	排序的算法是选择排序，并非高效的排序方法，但是是稳定的
	对数组排序，lua中提供了table.sort方法，但是该方法有时候会出错，而且是莫名其妙的错误
	
	如果需要对数组乱序，则可以这样用
	Utils.sort(t , function () return math.random(1000) > 500 end)
--]]
function Utils.sort(t , sort_reg)
    local len = #t
    for k = 1, len do
        local pos = k
        for i = k + 1, len do
            if sort_reg(t[pos] , t[i]) then
                pos = i
            end
        end
        local tmp = t[pos]
        t[pos] = t[k]
        t[k] = tmp
    end
end

-----------下面的是类型检测函数，在有的地方可以作为排除的处理或者断言使用----------------------

function Utils.isBoolean(value )
	local t = type(value)
	return t == "boolean"
end

function Utils.isNumber(value )
	local t = type(value)
	return t == "number"
end

function Utils.isString(value)
	local t = type(value)
	return t == "string"
end

function Utils.isTable(value )
	local t = type(value)
	return t == "table"
end

function Utils.isFunction(value )
	local t = type(value)
	return t == "function"
end

function _check_fn(fn,where )
--	if(Utils.isFunction(fn))then
--		return
--	end

--	local msg = where.." type error,function value required"
--	error(msg)
end

function _check_n( n, where )
	if(Utils.isNumber(n))then
		return
	end
	local msg = where.." type error,number value required"
	error(msg)
end

function _check_str(str,where )
	if(Utils.isString(str))then
		return
	end
	local msg = where.." type error,string value required"
	error(msg)
end

function _check_table(t , where )
	if(Utils.isTable(t))then
		return
	end
	local msg = where.." type error,table value required"
	error(msg)
end

function Utils.checkHasBattleInfo( ... )
	local playerInfo = LuaGlobal.myPlayer
	local ret = false
	if (playerInfo) then
		
		ret = playerInfo:HasField("battle_info")
		ret = ret and playerInfo.battle_info:HasField("is_empty")
		ret = ret and (not playerInfo.battle_info.is_empty)
	end
	return ret
end

local __inner_weakTable_key = {__mode = "k"}
local __inner_weakTable_value = {__mode = "v"}
local __inner_weakTable_keyvalue = {__mode = "kv"}


--[[
	将数据表的Key设置为弱引用
]]
function useWeakKey(t)
	setmetatable(t,__inner_weakTable_key)
end
--[[
	将数据表的value设置为弱引用
]]
function useWeakValue(t )
	setmetatable(t,__inner_weakTable_value)
end
--[[
	将数据表的key和value设置为弱引用
]]
function useWeakKeyValue(t )
	setmetatable(t,__inner_weakTable_keyvalue)
end

--[[
	交换protocol 数组
]]
function Utils.swapProtoTable(t1,t2 )
	if(not XArray)then require("XArray") end
	local xa1 = XArray.create()
	xa1:build(t1)
	local xa2 = XArray.create()
	xa1:build(t2)

	local sa1 = XArray.create()
	local sa2 = XArray.create()

	local ser = function (xa,sa )
		xa:forEach(nil,function (it )
			sa:add(it:SerializeToString())
		end)
	end

	ser(xa1,sa1)
	ser(xa2,sa2)

	Utils.clear(t1)
	Utils.clear(t2)

	local fn = function (a,t )
		a:forEach(nil,
		function(str )
			local newItem = t:add()
			newItem:ParseFromString(str)
		end)
	end

	fn(sa1,t2)
	fn(sa2,t1)
end
--[[
	安全地删除ccnode
]]
function Utils.safe_deleteCCNode(ccnode)
	if(ccnode and CCObject:safe_check(ccnode))then
		local parentNode = ccnode:getParent()
		if(parentNode and CCObject:safe_check(parentNode))then
			ccnode:removeFromParentAndCleanup(true)
		end
	end
end

--[[

]]
function Utils.getLegalValue(v1,v2)
	if(not v1 or v1 == "")then
		return v2
	end
	return v1
end
--[[
	由于某些用户名字比较奇葩会破坏我们的格式，所以决定先截断名字
	*originName 字符串
	*curLen 截取长度
	*needStrFix 是否需要省略号补正
]]
function Utils.getLegalName(originName,cutLen,needStrFix)
	local newName = nil

	local len = Utils.getUtf8Len(originName)----string.len
	_trace("lance test 原有字符串长度："..len)
	if(len >cutLen)then
		newName = Utils.subUtf8(originName,cutLen)
		_trace("lance test 截取字符："..cutLen..":"..newName)

		if(needStrFix)then
			newName = newName.."..."
		end
	else
		newName = originName
	end

	return newName
end
--[[
	获取字符串长度
]]
function Utils.getUtf8Len(str)
	local len = #str;
	local left = len;
	local cnt = 0;
	local arr={0,0xc0,0xe0,0xf0,0xf8,0xfc};
	while left ~= 0 do
	local tmp=string.byte(str,-left);
	local i=#arr;
	while arr[i] do
	if tmp>=arr[i] then left=left-i;break;end
	i=i-1;
	end
	cnt=cnt+1;
	end
	return cnt
end
--[[
	截取字符串
]]
function Utils.subUtf8(str,len)
	local newStr = ""
	local xName = Utils.splitWord(str)
	if(xName.size >= len)then
		for i = 1,len do
			newStr = newStr..xName:at(i)
		end
	else
		newStr = str
	end
	return newStr
end

function table.find_if(t, cond)
	for k,v in pairs(t) do
		if cond(v) then
			return k, v
		end
	end
	return nil
end

Utils.table_find_if = table.find_if

function table.remove_if(t, cond)
	for i = #t, 1, -1 do
		if cond(t[i]) then
			return table.remove(t, i), i
		end
	end
	return nil
end

Utils.table_remove_if = table.remove_if

function table.removeAll_if(t, cond)
	local ret = {}
	for i = #t, 1, -1 do
		if cond(t[i]) then
			local o = table.remove(t, i)
			table.insert(ret, o)
		end
	end
	return ret
end

Utils.table_removeAll_if = table.removeAll_if

--[[
	将table序列化成string
]]
function Utils.tableToString(t)
	local mark={}
	local assign={}
	
	local function ser_table(tbl,parent)
		mark[tbl]=parent
		local tmp={}
		for k,v in pairs(tbl) do
			local key= type(k)=="number" and "["..k.."]" or k
			if type(v)=="table" then
				local dotkey= parent..(type(k)=="number" and key or "."..key)
				if mark[v] then
					table.insert(assign,dotkey.."="..mark[v])
				else
					table.insert(tmp, key.."="..ser_table(v,dotkey))
				end
			elseif (type(v) == "string") then
				table.insert(tmp, key.."=".."\""..v.."\"")
			else
				table.insert(tmp, key.."="..v)
			end
		end
		return "{"..table.concat(tmp,",").."}"
	end
 
	return "do local ret="..ser_table(t,"ret")..table.concat(assign," ").." return ret end"
end

--[[--
 * @Description: 将一个十进制数转换成二进制字符串  
 * @param:       number 十进制数
 * @return:      二进制字符串
 ]]
function Utils.GetBinaryFromNumber(number)
	local ret = ""
	local hexString = string.format("%x", number)
	for k = 1, string.len(hexString) do
		local charValue = string.sub(hexString, k, k)
		Trace("charValue: "..charValue)
		local binStr = hex2Binary[tostring(string.upper(charValue))]
		if (binStr ~= nil) then
			ret = ret..binStr
		else
			Trace("binStr is nil")
		end
	end

	local start, endPos = string.find(ret, "1")
	ret = string.sub(ret, start)
	return ret
end