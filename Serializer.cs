namespace BinarySerde;

public class Serializer<T>
{
    private readonly Type _schema;
    private readonly List<Instance> _blobs = [];
    private uint _currentSize = (uint)math.pow(2, 8);
    private uint _offset;
    private Roblox.Buffer _buf;
    
    public Serializer(Type schema)
    {
        _schema = schema;
        _buf = buffer.create(_currentSize);
    }
    
    public SerializationResult Serialize(T data)
    {
        _offset = 0;
        _blobs.Clear();
        SerializeValue(data!, _schema);
        
        var trimmed = buffer.create(_offset);
        buffer.copy(trimmed, 0, _buf, 0, _offset);
        
        return new SerializationResult { Buf = _buf, Blobs = _blobs };
    }
    
    private void SerializeValue(object value, Type typeInfo)
    {
        if (IsA(typeInfo, "Instance"))
        {
            _blobs.Add((Instance)value);
            return;
        }
    
        if (typeInfo.IsClass || typeInfo.IsInterface)
        {
            SerializeStruct(value, typeInfo);
            return;
        }
    
        var currentOffset = _offset;
        switch (typeInfo.Name)
        {
            case "Byte":
            {
                Allocate(sizeof(byte));
                buffer.writeu8(_buf, currentOffset, (byte)value);
                break;
            }
            case "SByte":
            {
                Allocate(sizeof(sbyte));
                buffer.writei8(_buf, currentOffset, (sbyte)value);
                break;
            }
            case "Int16":
            {
                Allocate(sizeof(short));
                buffer.writei16(_buf, currentOffset, (short)value);
                break;
            }
            case "UInt16":
            {
                Allocate(sizeof(ushort));
                buffer.writeu16(_buf, currentOffset, (ushort)value);
                break;
            }
            case "Int32":
            {
                Allocate(sizeof(int));
                buffer.writei32(_buf, currentOffset, (int)value);
                break;
            }
            case "UInt32":
            {
                Allocate(sizeof(uint));
                buffer.writeu32(_buf, currentOffset, (uint)value);
                break;
            }
            case "Single":
            {
                Allocate(sizeof(float));
                buffer.writef32(_buf, currentOffset, (float)value);
                break;
            }
            case "Double":
            {
                Allocate(sizeof(double));
                buffer.writef64(_buf, currentOffset, (double)value);
                break;
            }
    
            default:
                error($"Unsupported data type '{typeInfo.Name}' was used");
                break;
        }
    }
    
    private void SerializeStruct(object value, Type typeInfo)
    {
        var properties = typeInfo.GetProperties();
        foreach (var property in properties)
        {
            var propertyValue = ((Dictionary<string, object>)value)[property.Name];
            SerializeValue(propertyValue, property.PropertyType);
        }
    }
    
    private void Allocate(uint size)
    {
        _offset += size;
        if (_offset <= _currentSize) return;
    
        var newSize = (uint)math.pow(2, math.ceil(math.log(_offset) / math.log(2)));
        var oldBuf = _buf;
    
        _currentSize = newSize;
        _buf = buffer.create(newSize);
        buffer.copy(_buf, 0, oldBuf);
    }
    
    private static bool IsA(Type typeInfo, string name) => typeInfo.Name == name || typeInfo.BaseType != null && IsA(typeInfo.BaseType, name);
}