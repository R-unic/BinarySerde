using BinarySerde;

var serializer = new Serializer<MySchema>(typeof(MySchema));
var result = serializer.Serialize(new MySchema { MyField = 69, MyOtherField = 69.420, Skibidi = script });
var deserializer = new Deserializer<MySchema>(typeof(MySchema));
var myStruct = deserializer.Deserialize(result);
print(myStruct);

class MySchema
{
    public required byte MyField { get; init; }
    public required double MyOtherField { get; init; }
    public required Instance Skibidi { get; init; }
}