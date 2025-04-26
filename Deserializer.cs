namespace BinarySerde;

public class Deserializer<T>
{
    private readonly Type _schema;
    private SerializationResult _serialized;
    private uint _offset;
    private int _blobIndex;

    public Deserializer(Type schema)
    {
        _schema = schema;
    }

    public T Deserialize(SerializationResult serialized)
    {
        _serialized = serialized;
        _offset = 0;
        _blobIndex = 0;
        
        return (T)DeserializeValue(_schema)!;
    }

    private object DeserializeValue(Type typeInfo)
    {
        if (IsA(typeInfo, "Instance"))
            return _serialized.Blobs[_blobIndex++];

        if (typeInfo.IsClass || typeInfo.IsInterface)
            return DeserializeStruct(typeInfo);

        var currentOffset = _offset;
        switch (typeInfo.Name)
        {
            case "Byte":
            {
                _offset += sizeof(byte);
                return buffer.readu8(_serialized.Buf, currentOffset);
            }
            case "SByte":
            {
                _offset += sizeof(sbyte);
                return buffer.readi8(_serialized.Buf, currentOffset);
            }
            case "Int16":
            {
                _offset += sizeof(short);
                return buffer.readi16(_serialized.Buf, currentOffset);
            }
            case "UInt16":
            {
                _offset += sizeof(ushort);
                return buffer.readu16(_serialized.Buf, currentOffset);
            }
            case "Int32":
            {
                _offset += sizeof(int);
                return buffer.readi32(_serialized.Buf, currentOffset);
            }
            case "UInt32":
            {
                _offset += sizeof(uint);
                return buffer.readu32(_serialized.Buf, currentOffset);
            }
            case "Single":
            {
                _offset += sizeof(float);
                return buffer.readf32(_serialized.Buf, currentOffset);
            }
            case "Double":
            {
                _offset += sizeof(double);
                return buffer.readf64(_serialized.Buf, currentOffset);
            }
            
            default:
                error($"Unsupported data type '{typeInfo.Name}' was used");
                return null;
        }
    }

    private Dictionary<string, object?> DeserializeStruct(Type typeInfo)
    {
        Dictionary<string, object?> finalStruct = [];
        var properties = typeInfo.GetProperties();
        foreach (var property in properties)
            finalStruct[property.Name] = DeserializeValue(property.PropertyType);

        return finalStruct;
    }
    
    private static bool IsA(Type typeInfo, string name) => typeInfo.Name == name || typeInfo.BaseType != null && IsA(typeInfo.BaseType, name);
}