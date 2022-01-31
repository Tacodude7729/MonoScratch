using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace ScratchSharp.Project {
    public abstract class ScratchAsset {
        public string AssetID;
        public string Name;
        public string MD5Ext;
        public string DataFormat;

        public ScratchAsset(JObject jsonObj) {
            dynamic json = jsonObj;
            AssetID = json.assetId;
            Name = json.name;
            MD5Ext = json.md5ext;
            DataFormat = json.dataFormat;
        }

        public ScratchAsset(string assetID, string name, string md5ext, string dataformat) {
            AssetID = assetID;
            Name = name;
            MD5Ext = md5ext;
            DataFormat = dataformat;
        }

        public virtual dynamic Serialize() {
            dynamic obj = new JObject();
            obj.assetId = AssetID;
            obj.name = Name;
            obj.md5ext = MD5Ext;
            obj.dataFormat = DataFormat;
            return obj;
        }
    }

    public class ScratchCostume : ScratchAsset {

        public int RotationCenterX, RotationCenterY;
        public int? BitmapResolution;

        public ScratchCostume(dynamic json) : base((JObject)json) {
            RotationCenterX = json.rotationCenterX;
            RotationCenterY = json.rotationCenterY;
            if (json.ContainsKey("bitmapResolution"))
                BitmapResolution = json.bitmapResolution;
        }

        public ScratchCostume(string assetID, string name, string md5ext, string dataformat, int rotCenterX, int rotCenterY, int? bitmapResolution) : base(assetID, name, md5ext, dataformat) {
            RotationCenterX = rotCenterX;
            RotationCenterY = rotCenterY;
            BitmapResolution = bitmapResolution;
        }

        public override dynamic Serialize() {
            dynamic obj = base.Serialize();
            obj.rotationCenterX = RotationCenterX;
            obj.rotationCenterY = RotationCenterY;
            if (BitmapResolution != null)
                obj.bitmapResolution = BitmapResolution;
            return obj;
        }

    }

    public class ScratchSound : ScratchAsset {
        public int Rate;
        public int SampleCount;

        public ScratchSound(dynamic json) : base((JObject)json) {
            Rate = json.rate;
            SampleCount = json.sampleCount;
        }

        public ScratchSound(string assetID, string name, string md5ext, string dataformat, int rate, int samplecount) : base(assetID, name, md5ext, dataformat) {
            this.Rate = rate;
            this.SampleCount = samplecount;
        }

        public override dynamic Serialize() {
            dynamic obj = base.Serialize();
            obj.rate = Rate;
            obj.sampleCount = SampleCount;
            return obj;
        }

    }
}