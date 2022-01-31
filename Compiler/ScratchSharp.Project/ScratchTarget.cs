using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ScratchSharp.Project {

    public abstract class ScratchTarget {

        public static ScratchTarget Parse(dynamic json, ScratchProject project) {
            if ((bool)json.isStage) return new ScratchStage(json, project);
            else return new ScratchSprite(json, project);
        }

        public readonly string Name;

        public ScratchProject Project;

        public int LayerOrder;
        public int Volume;
        public int CurrentCostume;

        public abstract bool IsStage { get; }

        public readonly List<ScratchCostume> Costumes;
        public readonly List<ScratchSound> Sounds;

        public readonly Dictionary<string, ScratchBlock> Blocks;
        public readonly Dictionary<string, BlockInputPrimitiveVariableList> LoneVarListBlocks;

        public readonly Dictionary<string, ScratchVariable> Variables;
        public readonly Dictionary<string, ScratchList> Lists;

        public readonly dynamic BroadcastsJson; // TODO Parse broadcasts
        public readonly dynamic CommentsJson; // TODO Parse comments

        public ScratchTarget(string name, ScratchProject project) {
            Name = name;
            Project = project;
            LayerOrder = 0;
            Volume = 100;
            CurrentCostume = 0;
            BroadcastsJson = new JObject();
            CommentsJson = new JObject();
            Costumes = new List<ScratchCostume>();
            Sounds = new List<ScratchSound>();
            Variables = new Dictionary<string, ScratchVariable>();
            Lists = new Dictionary<string, ScratchList>();
            LoneVarListBlocks = new Dictionary<string, BlockInputPrimitiveVariableList>();
            Blocks = new Dictionary<string, ScratchBlock>();
        }

        public ScratchTarget(JObject jsonObject, ScratchProject project) {
            dynamic json = jsonObject;

            Name = json.name;
            Project = project;

            LayerOrder = json.layerOrder;
            Volume = json.volume;
            CurrentCostume = json.currentCostume;

            BroadcastsJson = json.broadcasts;
            CommentsJson = json.comments;

            Costumes = new List<ScratchCostume>();
            foreach (dynamic costume in json.costumes)
                Costumes.Add(new ScratchCostume(costume));
            Sounds = new List<ScratchSound>();
            foreach (dynamic sound in json.sounds)
                Sounds.Add(new ScratchSound(sound));

            Variables = new Dictionary<string, ScratchVariable>();
            foreach (JProperty variable in json.variables)
                Variables.Add(variable.Name, ScratchBlockParser.ParseVariable(variable.Name, variable.Value));
            Lists = new Dictionary<string, ScratchList>();
            foreach (JProperty list in json.lists)
                Lists.Add(list.Name, ScratchBlockParser.ParseList(list.Name, list.Value));

            Blocks = new Dictionary<string, ScratchBlock>();
            LoneVarListBlocks = new Dictionary<string, BlockInputPrimitiveVariableList>();
            foreach (JProperty blockEntry in json.blocks) {
                if (blockEntry.Value is JObject)
                    Blocks.Add(blockEntry.Name, new ScratchBlock(blockEntry.Name, blockEntry.Value));
                else
                    LoneVarListBlocks.Add(blockEntry.Name, (BlockInputPrimitiveVariableList)ScratchBlockParser.ParseBlockInputPrimitive(blockEntry.Value));

            }
        }

        public virtual dynamic Serialize() {
            dynamic obj = new JObject();

            obj.name = Name;
            obj.layerOrder = LayerOrder;
            obj.volume = Volume;
            obj.currentCostume = CurrentCostume;
            obj.broadcasts = BroadcastsJson;
            obj.comments = CommentsJson;

            obj.isStage = IsStage;

            // TODO Add serialize method to these
            dynamic costumesArr = new JArray();
            foreach (ScratchCostume costume in Costumes)
                costumesArr.Add(costume.Serialize());
            obj.costumes = costumesArr;
            dynamic soundsArr = new JArray();
            foreach (ScratchSound sound in Sounds)
                soundsArr.Add(sound.Serialize());
            obj.sounds = soundsArr;

            dynamic variablesObj = new JObject();
            foreach (ScratchVariable variable in Variables.Values)
                variablesObj[variable.ID] = variable.Serialize();
            obj.variables = variablesObj;
            dynamic listsObj = new JObject();
            foreach (ScratchList list in Lists.Values)
                listsObj[list.ID] = list.Serialize();
            obj.lists = listsObj;

            dynamic blocksObj = new JObject();
            foreach (ScratchBlock block in Blocks.Values)
                blocksObj[block.ID] = block.Serialize();
            foreach (KeyValuePair<string, BlockInputPrimitiveVariableList> loneBlock in LoneVarListBlocks)
                blocksObj[loneBlock.Key] = loneBlock.Value.Serialize();
            obj.blocks = blocksObj;

            return obj;
        }

        public ScratchVariable? GetVariableByID(string id) {
            if (Project.Stage.Variables.TryGetValue(id, out ScratchVariable? variable))
                return variable;
            Variables.TryGetValue(id, out variable);
            return variable;
        }

        public ScratchVariable? GetVariableByName(string name) {
            ScratchVariable? var = Project.Stage.GetOwnVariableByName(name);
            if (var != null) return var;
            return GetOwnVariableByName(name);
        }

        private ScratchVariable? GetOwnVariableByName(string name) {
            foreach (KeyValuePair<string, ScratchVariable> var in Variables)
                if (var.Value.Name == name) return var.Value;
            return null;
        }

        public ScratchList? GetListByID(string id) {
            if (Project.Stage.Lists.TryGetValue(id, out ScratchList? variable))
                return variable;
            Lists.TryGetValue(id, out variable);
            return variable;
        }

        public ScratchList? GetListByName(string name) {
            ScratchList? var = Project.Stage.GetOwnListByName(name);
            if (var != null) return var;
            return GetOwnListByName(name);
        }

        private ScratchList? GetOwnListByName(string name) {
            foreach (KeyValuePair<string, ScratchList> var in Lists)
                if (var.Value.Name == name) return var.Value;
            return null;
        }
    }

    public class ScratchStage : ScratchTarget {
        public override bool IsStage => true;

        public int Tempo;
        public int VideoTransparency;
        public string VideoState;
        public string? TextToSpeechLanguage;

        public ScratchStage(ScratchProject project) : base("Stage", project) {
            Tempo = 60;
            VideoTransparency = 50;
            VideoState = "on";
        }

        public ScratchStage(dynamic json, ScratchProject project) : base((JObject)json, project) {
            Tempo = json.tempo;
            VideoTransparency = json.videoTransparency;
            VideoState = json.videoState;
            TextToSpeechLanguage = json.textToSpeechLanguage;
        }

        public override dynamic Serialize() {
            dynamic obj = base.Serialize();
            obj.tempo = Tempo;
            obj.videoTransparency = VideoTransparency;
            obj.videoState = VideoState;
            obj.textToSpeechLanguage = TextToSpeechLanguage;
            return obj;
        }
    }

    public class ScratchSprite : ScratchTarget {
        public override bool IsStage => false;

        public bool Visible;
        public int X, Y;
        public int Size;
        public int Direction;
        public bool Draggable;
        public string RotationStyle;

        public ScratchSprite(string name, ScratchProject project) : base(name, project) {
            Visible = true;
            X = 0;
            Y = 0;
            Size = 100;
            Direction = 90;
            Draggable = false;
            RotationStyle = "all around";
        }

        public ScratchSprite(dynamic json, ScratchProject project) : base((JObject)json, project) {
            Visible = json.visible;
            X = json.x;
            Y = json.y;
            Size = json.size;
            Direction = json.direction;
            Draggable = json.draggable;
            RotationStyle = json.rotationStyle;
        }

        public override dynamic Serialize() {
            dynamic obj = base.Serialize();
            obj.visible = Visible;
            obj.x = X;
            obj.y = Y;
            obj.size = Size;
            obj.direction = Direction;
            obj.draggable = Draggable;
            obj.rotationStyle = RotationStyle;
            return obj;
        }

    }
}