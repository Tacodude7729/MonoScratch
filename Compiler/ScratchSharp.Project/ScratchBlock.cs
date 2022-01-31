using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ScratchSharp.Project {

    // https://en.scratch-wiki.info/wiki/Scratch_File_Format#Blocks
    public class ScratchBlock {
        public string ID;
        public string Opcode;

        public string? NextID;
        public string? ParentID;

        public Dictionary<string, BlockInput> Inputs;
        public Dictionary<string, BlockField> Fields;

        public bool Shadow;
        public bool TopLevel;

        public BlockMutation? Mutation;
        public dynamic? CommentJson; // TODO Comments
        public double? X, Y;

        public ScratchBlock(string id, dynamic json) {
            ID = id;
            Opcode = json.opcode;
            NextID = json.next;
            ParentID = json.parent;
            Shadow = json.shadow;
            TopLevel = json.topLevel;

            if (json.ContainsKey("mutation")) {
                Mutation = new BlockMutation(json.mutation);
            }
            if (json.ContainsKey("comment"))
                CommentJson = json.comment;
            if (json.ContainsKey("x")) {
                X = json.x;
                Y = json.y;
            }

            Inputs = new Dictionary<string, BlockInput>();
            foreach (JProperty inputEntry in json.inputs) {
                Inputs.Add(inputEntry.Name, ScratchBlockParser.ParseBlockInput(inputEntry.Value));
            }

            Fields = new Dictionary<string, BlockField>();
            foreach (JProperty fieldEntry in json.fields) {
                Fields.Add(fieldEntry.Name, ScratchBlockParser.ParseBlockField(fieldEntry.Value));
            }
        }

        public ScratchBlock(string id, string opcode, bool topLevel, bool shadow = false) {
            ID = id;
            Opcode = opcode;
            TopLevel = topLevel;
            Shadow = shadow;

            Inputs = new Dictionary<string, BlockInput>();
            Fields = new Dictionary<string, BlockField>();
        }

        public dynamic Serialize() {
            dynamic obj = new JObject();

            obj.opcode = Opcode;
            obj.next = NextID;
            obj.parent = ParentID;
            obj.shadow = Shadow;
            obj.topLevel = TopLevel;

            if (Mutation != null)
                obj.mutation = Mutation.Serialize();
            if (CommentJson != null)
                obj.comment = CommentJson;
            if (X != null) {
                obj.x = X;
                obj.y = Y;
            }

            dynamic inputsObj = new JObject();
            foreach (KeyValuePair<string, BlockInput> input in Inputs) {
                inputsObj[input.Key] = input.Value.Serialize();
            }
            obj.inputs = inputsObj;

            dynamic fieldsObj = new JObject();
            foreach (KeyValuePair<string, BlockField> field in Fields) {
                fieldsObj[field.Key] = field.Value.Serialize();
            }
            obj.fields = fieldsObj;

            return obj;
        }
    }

    public static class ScratchBlockParser {
        public static BlockInputPrimitive ParseBlockInputPrimitive(dynamic primitive) {
            if (primitive is JArray list) {
                BlockInputPrimitiveType type = (BlockInputPrimitiveType)(int)(primitive[0]);
                switch (type) {
                    case BlockInputPrimitiveType.Number:
                    case BlockInputPrimitiveType.PositiveNumber:
                    case BlockInputPrimitiveType.WholeNumber:
                    case BlockInputPrimitiveType.IntegerNumber:
                    case BlockInputPrimitiveType.AngleNumber:
                    case BlockInputPrimitiveType.ColourPicker:
                    case BlockInputPrimitiveType.Text:
                        return new BlockInputPrimitiveRaw(type, primitive[1].ToString());
                    case BlockInputPrimitiveType.Broadcast:
                        return new BlockInputPrimitiveBroadcast((string)primitive[1], (string)primitive[2]);
                    case BlockInputPrimitiveType.Variable:
                    case BlockInputPrimitiveType.List:
                        if (list.Count == 5)
                            return new BlockInputPrimitiveVariableList(type, (string)primitive[1], (string)primitive[2], (double)primitive[3], (double)primitive[4]);
                        else
                            return new BlockInputPrimitiveVariableList(type, (string)primitive[1], (string)primitive[2], null, null);
                    default: throw new SystemException();
                }
            } else {
                return new BlockInputPrimitiveID((string)primitive);
            }
        }

        public static BlockInput ParseBlockInput(dynamic blockInput) {
            BlockInputType type = (BlockInputType)(int)blockInput[0];

            switch (type) {
                case BlockInputType.SameShadow:
                    BlockInputPrimitive primitive = ParseBlockInputPrimitive(blockInput[1]);
                    return new BlockInput(primitive, primitive);
                case BlockInputType.NoShadow:
                    return new BlockInput(ParseBlockInputPrimitive(blockInput[1]), null);
                case BlockInputType.DifferentShadow:
                    return new BlockInput(ParseBlockInputPrimitive(blockInput[1]), ParseBlockInputPrimitive(blockInput[2]));
                default: throw new SystemException();
            }
        }

        public static BlockField ParseBlockField(dynamic blockField) {
            if (blockField.Count == 2)
                return new BlockField((string)blockField[0], (string?)blockField[1]);
            else
                return new BlockField((string)blockField[0], null);
        }

        public static ScratchList ParseList(string id, dynamic obj) {
            List<string> contents = new List<string>();
            foreach (dynamic item in obj[1])
                contents.Add(item.ToString());
            return new ScratchList(id, (string)obj[0], contents);
        }

        public static ScratchVariable ParseVariable(string id, dynamic obj) {
            return new ScratchVariable(id, (string)obj[0], obj[1].ToString());
        }

        public static JToken OptomizeData(string str) {
            if (str == "0") return new JValue(0);
            if (str.StartsWith("0") || str.StartsWith("-0") || str.Length > 15) return str;
            if (long.TryParse(str, out long valLong)) {
                return new JValue(valLong);
            } else if (double.TryParse(str, out double valDouble)) {
                return new JValue(valDouble);
            }
            return new JValue(str);
        }
    }

    public class BlockMutation {

        public string ProcCode;
        public bool? WithoutScreenRefresh;
        public bool? HasNext;
        public List<BlockMutationArgument>? Arguments;
        public bool hasNamesAndDefaults = false;

        public BlockMutation(dynamic obj) {
            ProcCode = obj.proccode;
            if (obj.ContainsKey("warp")) if (obj.warp != "null") WithoutScreenRefresh = obj.warp;
            if (obj.ContainsKey("hasNext")) if (obj.hasNext != "null") HasNext = obj.hasNext;

            if (obj.ContainsKey("argumentids")) {
                Arguments = new List<BlockMutationArgument>();
                // JSON-ception. Scratch why..?
                JArray argumentsIds = (JArray)JsonConvert.DeserializeObject((string)obj.argumentids)!;
                JArray? argumentNames = null, argumentDefaults = null;
                if (obj.ContainsKey("argumentnames"))
                    argumentNames = (JArray)JsonConvert.DeserializeObject((string)obj.argumentnames)!;
                if (obj.ContainsKey("argumentdefaults"))
                    argumentDefaults = (JArray)JsonConvert.DeserializeObject((string)obj.argumentdefaults)!;

                for (int i = 0; i < argumentsIds.Count; i++) {
                    string id = (string)argumentsIds[i]!;
                    string? name = (string?)argumentNames?[i];
                    dynamic? @default = argumentDefaults?[i];
                    Arguments.Add(new BlockMutationArgument(id, name, @default));
                }
            }
        }

        public BlockMutation(string proccode) {
            this.ProcCode = proccode;
        }

        public dynamic Serialize() {
            dynamic obj = new JObject();
            obj.tagName = "mutation";
            obj.children = new JArray();
            obj.proccode = ProcCode;
            if (WithoutScreenRefresh != null)
                obj.warp = WithoutScreenRefresh;
            if (HasNext != null)
                obj.hasNext = ((bool)HasNext ? "true" : "false"); // why?

            if (Arguments != null) {
                JArray argumentIds = new JArray();
                JArray argumentNames = new JArray();
                JArray argumentDefaults = new JArray();

                foreach (BlockMutationArgument argument in Arguments) {
                    argumentIds.Add(argument.ID);
                    if (argument.Name != null)
                        argumentNames.Add(argument.Name);
                    if (argument.Default != null)
                        argumentDefaults.Add(argument.Default);
                }

                obj.argumentids = JsonConvert.SerializeObject(argumentIds);
                if (hasNamesAndDefaults) {
                    obj.argumentnames = JsonConvert.SerializeObject(argumentNames);
                    obj.argumentdefaults = JsonConvert.SerializeObject(argumentDefaults);
                }
            }
            return obj;
        }
    }

    public record BlockMutationArgument(string ID, string? Name, dynamic? Default);

    public record BlockField(string Name, string? ID) {
        public dynamic Serialize() {
            JArray obj = new JArray();
            obj.Add(Name);
            obj.Add(ID);
            return obj;
        }
    }

    public record BlockInput(BlockInputPrimitive Block, BlockInputPrimitive? Shadow) {
        public dynamic Serialize() {
            JArray obj = new JArray();
            if (Shadow == null) {
                obj.Add(BlockInputType.NoShadow);
                obj.Add(Block.Serialize());
            } else {
                if (Block == Shadow) {
                    obj.Add(BlockInputType.SameShadow);
                    obj.Add(Block.Serialize());
                } else {
                    obj.Add(BlockInputType.DifferentShadow);
                    obj.Add(Block.Serialize());
                    obj.Add(Shadow.Serialize());
                }
            }
            return obj;
        }
    }

    public interface BlockInputPrimitive {
        public dynamic Serialize();
    }

    public class BlockInputPrimitiveID : BlockInputPrimitive {
        public string ID;

        public BlockInputPrimitiveID(string id) {
            ID = id;
        }

        public dynamic Serialize() {
            return ID;
        }
    }

    // For all the primitives who only have one element.
    public class BlockInputPrimitiveRaw : BlockInputPrimitive {
        public readonly BlockInputPrimitiveType Type;

        public string Value;

        public BlockInputPrimitiveRaw(BlockInputPrimitiveType type, string value) {
            Type = type;
            Value = value;
        }

        public dynamic Serialize() {
            JArray array = new JArray();
            array.Add(Type);
            array.Add(Value); // TODO Optomize here?
            return array;
        }
    }

    // Used for variables and lists
    public class BlockInputPrimitiveVariableList : BlockInputPrimitive {
        public readonly BlockInputPrimitiveType Type;

        public string Name;
        public string ID;
        public double? X, Y;

        public BlockInputPrimitiveVariableList(BlockInputPrimitiveType type, string name, string id, double? x = null, double? y = null) {
            Type = type;
            Name = name;
            ID = id;
            X = x;
            Y = y;
        }

        public dynamic Serialize() {
            JArray array = new JArray();
            array.Add(Type);
            array.Add(Name);
            array.Add(ID);
            if (X != null & Y != null) {
                array.Add(X);
                array.Add(Y);
            }
            return array;
        }
    }

    public class BlockInputPrimitiveBroadcast : BlockInputPrimitive {
        public string Name;
        public string ID;

        public BlockInputPrimitiveBroadcast(string name, string id) {
            Name = name;
            ID = id;
        }

        public dynamic Serialize() {
            JArray array = new JArray();
            array.Add(BlockInputPrimitiveType.Broadcast);
            array.Add(Name); // TODO Remove names here?
            array.Add(ID);
            return array;
        }
    }

    // https://github.com/LLK/scratch-vm/blob/develop/src/serialization/sb3.js#L39-L41
    public enum BlockInputType : int {
        SameShadow = 1,
        NoShadow = 2,
        DifferentShadow = 3
    }

    // https://github.com/LLK/scratch-vm/blob/develop/src/serialization/sb3.js#L60-L81
    public enum BlockInputPrimitiveType : int {
        Number = 4,
        PositiveNumber = 5,
        WholeNumber = 6,
        IntegerNumber = 7,
        AngleNumber = 8,
        ColourPicker = 9,
        Text = 10,
        Broadcast = 11,
        Variable = 12,
        List = 13
    }
}