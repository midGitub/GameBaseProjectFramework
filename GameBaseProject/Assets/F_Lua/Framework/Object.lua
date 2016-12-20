LuaObject = {}


local function Pototype(obj,fnName)
	local key = "pototype_"..fnName
	local function __innerCall(t,...)
		local v = t[key]
		if(v)then
			for i = 1, #v do
				local fn = v[i]
				fn(t,unpack(arg))
			end  
		end
	end
	if(not obj[key])then
		obj[key] = {}
	end
	if(obj[fnName])then
		table.insert(obj[key], obj[fnName])
	end
	return __innerCall
end

function LuaObject.create()
	local this = {}
	this.Class = LuaObject
	this.name = "LuaObject"
	this.__index = this
	this.Pototype = Pototype

	function this.instanceOf(Class)
		return this.Class == Class
	end

	return this
end

function LuaObject.interfaceImpl(obj , interface )
	for k,v in pairs(interface) do
		obj[k] = v
	end
end

function LuaObject.singletonMake(Class , fnInit)
	function Class.getInstance(...)
		if(not Class.instance)then
			if(arg)then
				Class.instance = Class.create(unpack(arg))
			else
				Class.instance = Class.create()
			end
			if(fnInit)then
				fnInit(Class.instance)
			end
		end
		return Class.instance
	end
end