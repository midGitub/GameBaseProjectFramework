//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: dataconfig_tablemodeone.proto
namespace dataconfig
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TableModeOne")]
  public partial class TableModeOne : global::ProtoBuf.IExtensible
  {
    public TableModeOne() {}
    
    private uint _Position;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"Position", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint Position
    {
      get { return _Position; }
      set { _Position = value; }
    }
    private uint _StrengthenLevel;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"StrengthenLevel", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint StrengthenLevel
    {
      get { return _StrengthenLevel; }
      set { _StrengthenLevel = value; }
    }
    private uint _StrengthenRate = (uint)0;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"StrengthenRate", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((uint)0)]
    public uint StrengthenRate
    {
      get { return _StrengthenRate; }
      set { _StrengthenRate = value; }
    }
    private uint _MaterialID = (uint)0;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"MaterialID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((uint)0)]
    public uint MaterialID
    {
      get { return _MaterialID; }
      set { _MaterialID = value; }
    }
    private uint _MaterialNum;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"MaterialNum", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint MaterialNum
    {
      get { return _MaterialNum; }
      set { _MaterialNum = value; }
    }
    private uint _Cost;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"Cost", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public uint Cost
    {
      get { return _Cost; }
      set { _Cost = value; }
    }
    private uint _FailAdd = (uint)0;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"FailAdd", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((uint)0)]
    public uint FailAdd
    {
      get { return _FailAdd; }
      set { _FailAdd = value; }
    }
    private readonly global::System.Collections.Generic.List<dataconfig.TableModeOne.InternalType_AddAttrInfo> _AddAttrInfo = new global::System.Collections.Generic.List<dataconfig.TableModeOne.InternalType_AddAttrInfo>();
    [global::ProtoBuf.ProtoMember(8, Name=@"AddAttrInfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<dataconfig.TableModeOne.InternalType_AddAttrInfo> AddAttrInfo
    {
      get { return _AddAttrInfo; }
    }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"InternalType_AddAttrInfo")]
  public partial class InternalType_AddAttrInfo : global::ProtoBuf.IExtensible
  {
    public InternalType_AddAttrInfo() {}
    
    private uint _key = (uint)0;
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"key", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((uint)0)]
    public uint key
    {
      get { return _key; }
      set { _key = value; }
    }
    private uint _value = (uint)0;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"value", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((uint)0)]
    public uint value
    {
      get { return _value; }
      set { _value = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TableModeOneArray")]
  public partial class TableModeOneArray : global::ProtoBuf.IExtensible
  {
    public TableModeOneArray() {}
    
    private readonly global::System.Collections.Generic.List<dataconfig.TableModeOne> _items = new global::System.Collections.Generic.List<dataconfig.TableModeOne>();
    [global::ProtoBuf.ProtoMember(1, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<dataconfig.TableModeOne> items
    {
      get { return _items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}