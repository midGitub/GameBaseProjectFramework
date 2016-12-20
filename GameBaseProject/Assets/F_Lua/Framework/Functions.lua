--//////////////////////////日志控制相关 start ////////////////////////

TIME_NAME = 
{
	SECOND = 1,
	MINUTE = 2,
	HOUR   = 3,
	DAY    = 4,
}

LOG = 
{
    task = 1,          -- 任务
}

-- 模块的前缀
local logFilterDict = 
{
	[LOG.task]          = "[task]",
}

BestLog = {}
BestLog.filterCode = nil --过滤码默认为空

local resMgr = nil

local prefixLog = ""
local function MakePrefixStr(filterCode)
	if (filterCode ~= nil and logFilterDict[filterCode] ~= nil) then
		prefixLog = logFilterDict[filterCode]			
	else
		prefixLog = ""
	end
end

--跟踪日志--
function Trace(str, filterCode)
	if (BestLog.filterCode == filterCode or BestLog.filterCode == nil) then
		MakePrefixStr(filterCode)
		local strValue = str
		if (type(str) ~= "string") then
			strValue = tostring(str)
		end
		print(prefixLog..strValue);  	
    end
end

--警告日志--
function warning(str, filterCode) 
	if (BestLog.filterCode == filterCode or BestLog.filterCode == nil) then
		MakePrefixStr(filterCode)
		local strValue = str
		if (type(str) ~= "string") then
			strValue = tostring(str)
		end
		Debugger.LogWarning(prefixLog..strValue)
	end
end

--错误日志--
function Fatal(str) 
	Debugger.LogError(str)
end

--//////////////////////////日志控制相关 end ////////////////////////

--查找对象--
function find(str)
	return GameObject.Find(str);
end

function destroy(obj)
	if (not IsNil(obj)) then
		GameObject.Destroy(obj);
	end
end


function newobject(prefab)
	if prefab ~= nil then
		return GameObject.Instantiate(prefab);
	else
		return nil
	end
end

--同步加载资源对象 ----Start----------->
function newNormalObjSync(path, type)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	local abp = AssetBundleParams.New(path, type)
	local retGo = resMgr:LoadNormalObjSync(abp)

	return retGo
end

function newSceneResidentMemoryObjSync(path, type)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	local abp = AssetBundleParams.New(path, type)
	local retGo = resMgr:LoadSceneResidentMemoryObjSync(abp)

	return retGo
end

function newResidentMemoryObjSync(path, type)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	local abp = AssetBundleParams.New(path, type)
	local retGo = resMgr:LoadResidentMemoryObjSync(abp)

	return retGo
end
--同步加载资源对象 ----End----------->

--异步加载资源对象 ----Start----------->
function newNormalObjAsync(path, type, loadCallback, isSort)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	local abp = AssetBundleParams.New(path, type)
	if isSort ~= nil then
		abp.IsSort = isSort
	end
	resMgr:LoadNormalObjAsync(abp, Best.AssetBundleInfo.LoadAssetCompleteHandler(loadCallback))
end

function newSceneResidentMemoryObjAsync(path, type, loadCallback, isSort)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	local abp = AssetBundleParams.New(path, type)
	if isSort ~= nil then
		abp.IsSort = isSort
	end
	resMgr:LoadSceneResidentMemoryObjAsync(abp, Best.AssetBundleInfo.LoadAssetCompleteHandler(loadCallback))
end

function newResidentMemoryObjAsync(path, type, loadCallback, isSort)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	local abp = AssetBundleParams.New(path, type)
	if isSort ~= nil then
		abp.IsSort = isSort
	end
	resMgr:LoadResidentMemoryObjAsync(abp, Best.AssetBundleInfo.LoadAssetCompleteHandler(loadCallback))
end
--异步加载资源对象 ----End----------->

--同步加载UI预设 ----Start----------->
function newNormalUI(path, parent)
    local prefab = newNormalObjSync(path, typeof(GameObject))
	local obj = newNormalUIprefab(prefab, parent)	
	if obj ~= nil then
		local ctrl = obj:AddComponent(typeof(Best.LuaDestroyBundle))
		ctrl.BundleName = path
		ctrl.ResType = typeof(GameObject)
	end
	
	return obj
end

function newSceneResidentMemoryUI(path, parent)
    local prefab = newSceneResidentMemoryObjSync(path, typeof(GameObject))
	local obj = newNormalUIprefab(prefab, parent)
	if obj ~= nil then
		local ctrl = obj:AddComponent(typeof(Best.LuaDestroyBundle))
		ctrl.BundleName = path
		ctrl.ResType = typeof(GameObject)
	end
	
	return obj
end

function newResidentMemoryUI(path, parent)
    local prefab = newResidentMemoryObjSync(path, typeof(GameObject))
	local obj = newNormalUIprefab(prefab, parent)
	if obj ~= nil then
		local ctrl = obj:AddComponent(typeof(Best.LuaDestroyBundle))
		ctrl.BundleName = path
		ctrl.ResType = typeof(GameObject)
	end
	
	return obj
end
--同步加载UI预设 ----End----------->

--异步加载UI预设 ----Start----------->
function newNormalUIAsync(path, func, isSort)
	UISys.Instance:DisableUICamera()
	newNormalObjAsync(path, typeof(GameObject), func, isSort)
end

function newSceneResidentMemoryUIAsync(path, func, isSort)
	UISys.Instance:DisableUICamera()
	newSceneResidentMemoryObjAsync(path, typeof(GameObject), func, isSort)
end

function newResidentMemoryUIAsync(path, func, isSort)
	UISys.Instance:DisableUICamera()
	newResidentMemoryObjAsync(path, typeof(GameObject), func, isSort)
end

function newUIAsyncCallback(abi)
	local go = newNormalUIprefab(abi.mainObject)
	local ctrl = go:AddComponent(typeof(Best.LuaDestroyBundle))
	ctrl.BundleName = abi.GoPath
	ctrl.ResType = abi.GoType
	UISys.Instance:EnableUICamera()

	return go
end
--异步加载UI预设 ----End----------->

function  newNormalUIprefab(prefab, parent)
	local obj = newobject(prefab)	
	
	--parent, default ui root
	local ui_root_trans = parent
	if ui_root_trans == nil then
		ui_root_trans = UISys.Instance.transform
	end
	
	if obj ~= nil and obj.transform.parent ~= ui_root_trans then
		obj.transform.parent = ui_root_trans
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero
	end
	
	return obj
end

function unloadobj(path, type)
	if resMgr == nil then
		resMgr = GameKernel.GetResourceMgr()
	end
	resMgr:UnloadResource(path, type)
end

--[[--
 * @Description: 通过名字获取子控件  
 * @param:       父控件的Transform,子控件名字 
 * @return:      返回子控件Transform
 ]]
function child(go ,str)
	if go == nil then
		Trace("go == nil")
		return nil
	end
	return go:FindChild(str);
end

--[[--
 * @Description: 通过名字获取子控件（深度优先搜索，拿到即止，不需要指定路径，但需要细心规划控件名字，不要重名）  
 * @param:       trans      父控件的Transform
                 childName  子控件名字   
 * @return:      子控件Transform
 ]]
function child_ext(trans, childName)
	local ret = nil
	local childNum = trans.childCount

	for k = 0, childNum-1 do
		local childTrans = trans:GetChild(k)
		if (childTrans ~= nil) then
			if (childTrans.gameObject.name == childName) then
				ret = childTrans
				break
			else
				ret = child_ext(childTrans, childName)
				if (ret ~= nil) then
					break
				end
			end
		end
	end

	return ret
end

--[[--
 * @Description: 获取子控件组件
 * @param:       控件transfrom，组件名
 * @return:      返回子控件Transform
 ]]
function componentGet(trans , typeName)		
	if trans == nil then
		Trace("componentGet trans is nil")
		return nil
	end
	--Trace(typeName)
	return trans.gameObject:GetComponent(typeName);
end

--[[--
 * @Description: 获取子控件组件
 * @param:       控件transform，子控件名称 , 组件名 
 * @return:      返回子控件Transform
 ]]
function subComponentGet(trans , childCompName, typeName)		
	if trans == nil then
		Trace("componentGet trans is nil")
		return nil
	end
	local transChild = child(trans, childCompName)
	if transChild == nil then
		return nil
	end
	return transChild.gameObject:GetComponent(typeName)
end

--[[--
 * @Description: 获取子控件组件
 * @param:       控件transform
 		         子控件名称 (非路径方式)
 		         组件名 
 * @return:      返回子控件Transform
 ]]
function subComponentGet_ext(trans , childCompName, typeName)		
	if trans == nil then
		Trace("componentGet trans is nil")
		return nil
	end
	local transChild = child_ext(trans, childCompName)
	if transChild == nil then
		return nil
	end
	return transChild.gameObject:GetComponent(typeName)
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallback(trans, para1, para2, para3)
	if (type(para1) == "string") then
		local child_trans = trans:Find(para1)
		if (child_trans ~= nil) then
			local btnObj = child_trans.gameObject

			if (para3 ~= nil) then
				UIEventListener.Get(btnObj).onClick = function (...)
					para2(para3, ...)
				end
			else
				UIEventListener.Get(btnObj).onClick = para2
			end
		else
			Fatal("can not find the control, its name is: "..para1)
		end
	elseif (type(para1) == "function") then
		if (para2 == nil) then
			UIEventListener.Get(trans.gameObject).onClick = para1
		else
			UIEventListener.Get(trans.gameObject).onClick = function (...)
					para1(para2, ...)
				end
		end
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallback_ext(trans, para1, para2)
	if (type(para1) == "string") then
		local child_trans = child_ext(trans, para1)
		if (child_trans ~= nil) then
			local btnObj = child_trans.gameObject
			UIEventListener.Get(btnObj).onClick = para2
		else
			Fatal("can not find the control, its name is: "..para1)
		end
	elseif (type(para1) == "function") then
		UIEventListener.Get(trans.gameObject).onClick = para1
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addDBClickCallbackSelf(go, callback)
	if (go ~= nil) then
		UIEventListener.Get(go).onDoubleClick = callback
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addClickCallbackSelf(go, callback, self)
	if (go ~= nil) then
		if self ~= nil then
		    UIEventListener.Get(go).onClick = function(obj) callback(self, obj) end
		else
		    UIEventListener.Get(go).onClick = callback
		end
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressedCallback(parentTrans, controlName, callback)
	local trans = parentTrans:Find(controlName)
	if (trans ~= nil) then
		local btnObj = trans.gameObject
		UIEventListenerEx.GetEx(btnObj).onPressed = callback
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressedCallbackSelf(parentTrans, controlName, callback, self)
	local trans = parentTrans:Find(controlName)
	if (trans ~= nil) then
		local btnObj = trans.gameObject
		if self ~= nil then 
			 UIEventListenerEx.GetEx(btnObj).onPressed = function(obj) callback(self, obj) end
		else
			UIEventListenerEx.GetEx(btnObj).onPressed = callback
		end
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addDropCallbackSelf(go, callback, self)
	if (go ~= nil) then
		if (self == nil) then
			UIEventListenerEx.GetEx(go).onDrop = callback
		else
			UIEventListenerEx.GetEx(go).onDrop = function(...) callback(self, ...) end
		end
	else
		Fatal("can not drop the nil control")
	end
end

--[[--
 * @Description: 按钮点击事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addDragCallbackSelf(go, callback)
	if (go ~= nil) then
		UIEventListenerEx.GetEx(go).onDrag = callback
	else
		Fatal("can not drop the nil control")
	end
end


function addDragStartCallbackSelf(go, callback, self)
	if (go ~= nil) then
		if (self == nil) then
			UIEventListenerEx.GetEx(go).onDragStart = callback
		else
			UIEventListenerEx.GetEx(go).onDragStart = function(...) callback(self, ...) end
		end
	else
		Fatal("can not drag the nil control")
	end
end

function addDragEndCallbackSelf(go, callback, self)
	if (go ~= nil) then
		if (self == nil) then
			UIEventListenerEx.GetEx(go).onDragEnd = callback
		else
			UIEventListenerEx.GetEx(go).onDragEnd = function(...) callback(self, ...) end
		end
	else
		Fatal("can not drag the nil control")
	end
end

function addSelectCallbackSelf(go, callback, self)
	if (go ~= nil) then
		if self ~= nil then
			UIEventListenerEx.GetEx(go).onSelect = function(...) callback(self, ...) end
		else
			UIEventListenerEx.GetEx(go).onSelect = callback
		end
	else
		Fatal("can not drag the nil control")
	end
end

--[[--
 * @Description: press事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressBoolCallback(parentTrans, controlName,callback, self)
	local trans = parentTrans:Find(controlName)
	if (trans ~= nil) then
		local btnObj = trans.gameObject
		if self ~= nil then
			UIEventListenerEx.GetEx(btnObj).onPress = function(...) callback(self, ...) end
		else
			UIEventListenerEx.GetEx(btnObj).onPress = callback
		end
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: press事件注册  
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addPressBoolCallbackSelf(go, callback, self)
	if (go ~= nil) then
		if self ~= nil then
			UIEventListenerEx.GetEx(go).onPress = function(...) callback(self, ...) end
		else
			UIEventListenerEx.GetEx(go).onPress = callback
		end
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: 添加Tween动画结束回调
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function addTweenFinishedCallback(parentTrans, controlName, callback)
	local tween = subComponentGet(parentTrans, controlName , "UITweener")

	if (trans ~= nil) then
		tween:AddOnFinished(EventDelegate.Callback(this.OnComboTweenFinish))
	else
		Fatal("can not find the control, its name is: "..controlName)
	end
end

--[[--
 * @Description: 添加uitoggle结束回调
 * @param:       控件transform，子控件名称 , 回调函数
 ]]
function initToggleObj(parentTrans, controlName, callback, self)
	local toggleObj = nil
	if controlName ~= nil then
		toggleObj = subComponentGet(parentTrans, controlName, 'UIToggle')
	else
		toggleObj = componentGet(parentTrans, 'UIToggle')
	end
	
	if (toggleObj ~= nil) then
		if self ~= nil then
			EventDelegate.Add(toggleObj.onChange, EventDelegate.Callback(function() callback(self) end))
		else
			EventDelegate.Add(toggleObj.onChange, EventDelegate.Callback(callback))
		end
	end

	return toggleObj
end

--[[--
 * @Description: 将Uint64转换成字符串  
 * @param:       myUint64 server下发的uint64结构体 
 * @return:      字符串
 ]]
function toUint64String(myUint64)
	local highStr = tostring(myUint64.High)
	local lowStr = tostring(myUint64.Low)
	return highStr..lowStr
end

--[[--
 * @Description: 将两个uint32转换成protobuf的MyUint64  
 ]]
function toMyUint64(high, low)
	require("protocol_generated/cs_comm_pb")
	local myUint64 = MyUint64()
	myUint64.High = high
	myUint64.Low = low
	return myUint64
end

--[[--
 * @Description: MyUint64 赋值
 ]]
function setMyUint64(myUint64, high, low)
	myUint64.High = high
	myUint64.Low = low
end

function CopyMyUint64(myUint64_out, myUint64_in)
	myUint64_out.High = myUint64_in.High
	myUint64_out.Low = myUint64_in.Low
end

--[[--
 * @Description: 判断两个MyUint64是否相等
 ]]
function MyUint64Equals(myUint64_1, myUint64_2)
    return myUint64_1.High == myUint64_2.High and myUint64_1.Low == myUint64_2.Low
end

--[[--
 * @Description: 将unit64Helper转换成protobu的MyUint64
 ]]
function uint64helperToMyUint64(n64)	
	local myUint64 = MyUint64()
	local l,h  = int64.tonum2(n64)
	myUint64.High = h
	myUint64.Low = l
	return myUint64
end

--[[--
 * @Description: 将protobuf的MyUint64转换成Uint64Helper  
 * @param:       myUint64 protobuf的MyUint64 
 * @return:      Uint64Helper
 ]]
function myUint64ToLuaInt64(myUint64)
	return int64.new(myUint64.Low, myUint64.High)		
end

function destroyAllChild(trans)
	local childNum = trans.childCount
	for k = 0, childNum-1 do
		local childTrans = trans:GetChild(k)
		if (childTrans ~= nil) then 
			destroy(childTrans.gameObject)
		end
	end
end


--[[--
 * @Description: 一系列帮助函数  
 ]]
function Vector3ToTriple(vec3, trip)
	trip.x = math.ceil(vec3.x * 100)
	trip.y = math.ceil(vec3.y * 100)
	trip.z = math.ceil(vec3.z * 100)
end

function TripleToVector3(trip)
	local vec3 = Vector3.zero
	if trip ~= nil then
		vec3.x = (trip.x) / 100
		vec3.y = (trip.y) / 100
		vec3.z = (trip.z) / 100
	end
	return vec3
end

function CopyTriple(outDat, inData)
	outDat.x = inData.x
	outDat.y = inData.y
	outDat.z = inData.z
end

function PosEquals(v1, v2, precision)
	if precision == nil then
	    precision = 0.01
	end
	
	if math.abs(v1.x - v2.x) < precision and math.abs(v1.z - v2.z) < precision then
		return true
	else
		return false
	end
end

function DirEquals(v1, v2)
    if math.abs(v1.x - v2.x) < 0.01 and math.abs(v1.z - v2.z) < 0.01 then
		return true
	else
		return false
	end
end

function DirCalc(v1, v2)
	local Dir = v1 - v2
	Dir.y = 0
	Dir:SetNormalize()
	return Dir
end

function TimeSecToString(sec)
	if (sec ~= nil) then
		local intTime = math.floor(sec)
		return string.format("%02d:%02d", math.floor(intTime / 60), math.floor(intTime % 60))
	else
		return ""
	end
end

--[[--
 * @Description: 将毫秒转化为 天时分秒的格式  不足1秒大于0 默认返回1秒  大于1秒后面尾数舍去
 * @param:       msec (毫秒)
 * @return:      day(天) hour(时) minute(分) second(秒)
 ]]
function TimeMillisecondToParams(msec)
	local timesprit = {1000, 1000*60, 1000*60*60, 1000*60*60*24}
	local time_array = {0, 0, 0, 0}
	if msec~=nil and msec>0 then
	 	local len = table.getn(timesprit)

		local isLessthenSec = true
		local timeMod = msec
		for i=len,1,-1 do
			local intsTime = timeMod/timesprit[i]
			time_array[i] = math.floor(intsTime)
			if time_array[i] ~= 0 and isLessthenSec == true then
				isLessthenSec = false
			end
			timeMod = timeMod%timesprit[i]
			if timeMod == 0 then
				break
			end
		end

		if timeMod ~= 0 and isLessthenSec == true then
			time_array[TIME_NAME.SECOND] = time_array[TIME_NAME.SECOND] + 1
		end
	end
	
	return time_array[TIME_NAME.DAY],time_array[TIME_NAME.HOUR],time_array[TIME_NAME.MINUTE],time_array[TIME_NAME.SECOND]
end

function Vector3.DistanceXZ(va, vb)
	return math.sqrt((va.x - vb.x)^2 + (va.z - vb.z)^2)
end

function GetCurrSceneType()
	return game_scene.getCurSceneType()
end

function GetCurrSceneID()
	return game_scene.GetCurSceneID
end

--[[--
 * @Description: Restart ParticleSystem In Children
 ]]
function RestartParticleSystem(go)
    if (go == nil) then
        return
    end
    local childrenParticleSystems = go:GetComponentsInChildren(typeof(UnityEngine.ParticleSystem))
    local len = childrenParticleSystems.Length -1 
	if len >= 0 then
		for i=0,len do
			childrenParticleSystems[i]:Simulate(0, true, true)
			childrenParticleSystems[i]:Play(true)
		end
	end
end

function SetHeroCameraFollow(gameObject)
	if (gameObject ~= nil) then
		local cameraObj = GameObject.FindGameObjectWithTag("MainCamera")
		if (cameraObj ~= nil) then
			local tpCamera = cameraObj:GetComponent("ThirdPersonCameraHall")
			if (tpCamera ~= nil) then
				tpCamera:SetFollowObject(gameObject)
			end
		end
	end
end

local trieFilter = nil

--[[--
 * @Description: 替换掉str中的脏字和敏感词，变温*号，返回替换后的字符串；如果没有脏字
                 和敏感词，返回原字符串
 ]]
function CheckAndReplaceForBadWords(str)
	if (trieFilter == nil) then
		trieFilter = TrieFilter.GetInstance()
	end
	return trieFilter:Replace(str)
end

function AddNavMeshComponent(go)
	local navMeshAgent = go:GetComponent(typeof(UnityEngine.NavMeshAgent))
	if (navMeshAgent == nil) then
		go:AddComponent(typeof(UnityEngine.NavMeshAgent))
		navMeshAgent.enabled = false
	end
end

--unity 对象判断为空, 如果你有些对象是在c#删掉了，lua 不知道
--判断这种对象为空时可以用下面这个函数。
function IsNil(uobj)
	local ret = false
	if (uobj == nil) then
		ret = true
	end

	if (uobj ~= nil ) then
		if (uobj:Equals(nil)) then
			ret = true
		end
	end

	return ret
end