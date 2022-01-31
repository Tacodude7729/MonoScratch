using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ScratchSharp.Project {

    public class ScratchVariable {
        public string ID;
        public string Name;
        public string Value;

        public ScratchVariable(string id, string name, string value) {
            ID = id;
            Name = name;
            Value = value;
        }

        public dynamic Serialize() {
            JArray arr = new JArray();
            arr.Add(Name);
            arr.Add(ScratchBlockParser.OptomizeData(Value));
            return arr;
        }
    }

    public class ScratchList {
        public string ID;
        public string Name;
        public List<string> Values;

        public ScratchList(string id, string name, List<string> values) {
            ID = id;
            Name = name;
            Values = values;
        }

        public dynamic Serialize() {
            JArray arr = new JArray();
            arr.Add(Name);
            JArray content = new JArray();
            for (int i = 0; i < Values.Count; i++)
                content.Add(ScratchBlockParser.OptomizeData(Values[i]));
            arr.Add(content);
            return arr;
        }
    }
}