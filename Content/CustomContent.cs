using static WECCL.Utils.NumberFormatUtils;
using WECCL.Saves;

namespace WECCL.Content;

public static class CustomContent
{
    internal static readonly List<AudioClip> CustomClips = new();

    internal static readonly Dictionary<string, CostumeData> CustomCostumes = new()
    {
        { "legs_material", new CostumeData("legs_material", "legs", typeof(Texture2D)) },
        { "legs_flesh", new CostumeData("legs_flesh", typeof(Texture2D)) },
        { "legs_shape", new CostumeData("legs_shape", "shape4;shape6", typeof(Mesh)) },
        { "body_material", new CostumeData("body_material", "body", typeof(Texture2D)) },
        { "body_flesh_male", new CostumeData("body_flesh_male", "body_flesh", typeof(Texture2D)) },
        { "body_flesh_female", new CostumeData("body_flesh_female", "body_female", typeof(Texture2D)) },
        { "body_shape", new CostumeData("body_shape", "shape2", typeof(Mesh)) },
        { "face_female", new CostumeData("face_female", typeof(Texture2D)) },
        { "face_male", new CostumeData("face_male", "face", typeof(Texture2D)) },
        { "face_shape", new CostumeData("face_shape", "shape3", typeof(Mesh)) },
        { "arms_material", new CostumeData("arms_material", "arm", typeof(Texture2D)) },
        { "arms_flesh", new CostumeData("arms_flesh", "arm_flesh", typeof(Texture2D)) },
        { "arms_shape", new CostumeData("arms_shape", "shape9;shape12", typeof(Mesh)) },
        { "arms_glove", new CostumeData("arms_glove", "glove", typeof(Texture2D)) },
        { "legs_footwear_special", new CostumeData("legs_footwear_special", "custom", typeof(Texture2D)) },
        { "legs_footwear", new CostumeData("legs_footwear", "shoes", typeof(Texture2D)) },
        { "body_collar", new CostumeData("body_collar", "collar", typeof(Texture2D)) },
        {
            "hair_texture_transparent", new CostumeData("hair_texture_transparent", "hair_alpha", typeof(Texture2D))
        },
        { "hair_texture_solid", new CostumeData("hair_texture_solid", "hair", typeof(Texture2D)) },
        { "hair_hairstyle_solid", new CostumeData("hair_hairstyle_solid", "shape17", typeof(Mesh)) },
        { "hair_hairstyle_transparent", new CostumeData("hair_hairstyle_transparent", "shape-17", typeof(Mesh)) },
        { "hair_extension", new CostumeData("hair_extension", "shape18", typeof(Mesh)) },
        { "hair_shave", new CostumeData("hair_shave", "shave", typeof(Texture2D)) },
        { "face_beard", new CostumeData("face_beard", "beard", typeof(Texture2D)) },
        { "face_mask", new CostumeData("face_mask", "mask", typeof(Texture2D)) },
        { "body_pattern", new CostumeData("body_pattern", typeof(Texture2D)) },
        { "legs_kneepad", new CostumeData("legs_kneepad", "kneepads", typeof(Texture2D)) },
        { "legs_pattern", new CostumeData("legs_pattern", typeof(Texture2D)) },
        { "legs_laces", new CostumeData("legs_laces", "lace", typeof(Texture2D)) },
        { "face_headwear", new CostumeData("face_headwear", "shape28;shape31", typeof(Mesh)) },
        { "arms_elbow_pad", new CostumeData("arms_elbow_pad", "pad", typeof(Texture2D)) },
        { "arms_wristband", new CostumeData("arms_wristband", "wristband", typeof(Texture2D)) }
    };

    internal static readonly Dictionary<string, Dictionary<string,Texture2D>> ResourceOverridesTextures = new();
    internal static readonly Dictionary<string, Dictionary<string,AudioClip>> ResourceOverridesAudio = new();
    internal static readonly Dictionary<string, Dictionary<string,Mesh>> ResourceOverridesMeshes = new();
    
    public static bool HasConflictingOverrides = false;

    internal static List<string> Prefixes = new();
    
    public static void AddResourceOverride(string texName, string name, Texture2D texture)
    {
        var split = name.Split('/');
        var prefix = split.Length > 1 ? split[0] : "manual";
        if (!ResourceOverridesTextures.ContainsKey(texName))
        {
            ResourceOverridesTextures.Add(texName, new Dictionary<string, Texture2D>{ { name, texture } });
        }
        else if (!ResourceOverridesTextures[texName].ContainsKey(name))
        {
            ResourceOverridesTextures[texName].Add(name, texture);
            HasConflictingOverrides = true;
        }
        else
        {
            Plugin.Log.LogWarning($"Duplicate texture override for {name}!");
        }
        if (!Prefixes.Contains(prefix))
        {
            Prefixes.Add(prefix);
        }
    }
    
    public static void AddResourceOverride(string texName, string name, AudioClip audioClip)
    {
        var split = name.Split('/');
        var prefix = split.Length > 1 ? split[0] : "manual";
        if (!ResourceOverridesAudio.ContainsKey(texName))
        {
            ResourceOverridesAudio.Add(texName, new Dictionary<string, AudioClip>{ { name, audioClip } });
        }
        else if (!ResourceOverridesAudio[texName].ContainsKey(name))
        {
            ResourceOverridesAudio[texName].Add(name, audioClip);
            HasConflictingOverrides = true;
        }
        else
        {
            Plugin.Log.LogWarning($"Duplicate audio override for {name}!");
        }
        if (!Prefixes.Contains(prefix))
        {
            Prefixes.Add(prefix);
        }
    }
    
    public static void AddResourceOverride(string texName, string name, Mesh mesh)
    {
        var split = name.Split('/');
        var prefix = split.Length > 1 ? split[0] : "manual";
        if (!ResourceOverridesMeshes.ContainsKey(texName))
        {
            ResourceOverridesMeshes.Add(texName, new Dictionary<string, Mesh>{ { name, mesh } });
        }
        else if (!ResourceOverridesMeshes[texName].ContainsKey(name))
        {
            ResourceOverridesMeshes[texName].Add(name, mesh);
            HasConflictingOverrides = true;
        }
        else
        {
            Plugin.Log.LogWarning($"Duplicate mesh override for {name}!");
        }
        if (!Prefixes.Contains(prefix))
        {
            Prefixes.Add(prefix);
        }
    }

    internal static List<BetterCharacterDataFile> ImportedCharacters = new();
    internal static List<string> FilesToDeleteOnSave = new();
    
    public static AudioClip GetHighestPriorityAudioOverride(string name)
    {
        if (!ResourceOverridesAudio.ContainsKey(name))
        {
            return null;
        }
        
        var audioClips = ResourceOverridesAudio[name];
        var highestPriority = 0;
        AudioClip highestPriorityClip = null;
        foreach (var audioClip in audioClips)
        {
            var split = audioClip.Key.Split('/');
            var prefix = split.Length > 1 ? split[0] : "manual";
            var priority = 0;
            priority = -Prefixes.IndexOf(prefix) + Prefixes.Count;
            if (priority > highestPriority)
            {
                highestPriority = priority;
                highestPriorityClip = audioClip.Value;
            }
        }
        return highestPriorityClip;
    }
    
    public static Texture2D GetHighestPriorityTextureOverride(string name)
    {
        if (!ResourceOverridesTextures.ContainsKey(name))
        {
            return null;
        }
        
        var textures = ResourceOverridesTextures[name];
        var highestPriority = 0;
        Texture2D highestPriorityTexture = null;
        foreach (var texture in textures)
        {
            var split = texture.Key.Split('/');
            var prefix = split.Length > 1 ? split[0] : "manual";
            var priority = 0;
            priority = -Prefixes.IndexOf(prefix) + Prefixes.Count;
            if (priority > highestPriority)
            {
                highestPriority = priority;
                highestPriorityTexture = texture.Value;
            }
        }
        return highestPriorityTexture;
    }

    public static void SetHighestPriorityTextureOverride(string name, Texture2D texture)
    {
        if (!ResourceOverridesTextures.ContainsKey(name))
        {
            ResourceOverridesTextures.Add(name, new Dictionary<string, Texture2D>());
        }
        
        var textures = ResourceOverridesTextures[name];
        var highestPriority = 0;
        string highestPriorityKey = null;
        foreach (var textureKey in textures)
        {
            var split = textureKey.Key.Split('/');
            var prefix = split.Length > 1 ? split[0] : "manual";
            var priority = 0;
            priority = -Prefixes.IndexOf(prefix) + Prefixes.Count;
            if (priority > highestPriority)
            {
                highestPriority = priority;
                highestPriorityKey = textureKey.Key;
            }
        }
        ResourceOverridesTextures[name][highestPriorityKey ?? name] = texture;
    }

    internal static void LoadPrefixes()
    {
        var orderedP = MetaFile.Data.PrefixPriorityOrder;
        if (orderedP.Count > 0)
        {
            var newPrefixes = new List<string>();
            foreach (var prefix in orderedP)
            {
                if (!newPrefixes.Contains(prefix) && Prefixes.Contains(prefix))
                {
                    newPrefixes.Add(prefix);
                }
            }
            foreach (var prefix in Prefixes)
            {
                if (!newPrefixes.Contains(prefix))
                {
                    newPrefixes.Add(prefix);
                    MetaFile.Data.HidePriorityScreenNextTime = false;
                }
            }
            Prefixes = newPrefixes;
        }
    }
    
    internal static void SavePrefixes()
    {
        MetaFile.Data.PrefixPriorityOrder = Prefixes;
        MetaFile.Data.Save();
    }

    public static Color GetSkinColor(int index)
    {
        Dictionary<string, string> dict;
        if (index > 0)
        {
            index -= VanillaCounts.MaterialCounts[3] + 1;
            dict = CustomCostumes["face_male"].CustomObjects[index].Item3;
        }
        else
        {
            index = -index - (VanillaCounts.FaceFemaleCount + 1);
            dict = CustomCostumes["face_female"].CustomObjects[index].Item3;
        }

        if (dict.Keys.Any(t => t == "skin_color" || t == "skin_colour" || t == "skin_tone"))
        {
            var color = dict.First(t => t.Key == "skin_color" || t.Key == "skin_colour" || t.Key == "skin_tone").Value;
            var split = color.Split(',');
            return new Color(float.Parse(split[0], Nfi), float.Parse(split[1], Nfi), float.Parse(split[2], Nfi));
        }
        return Color.white;
    }
}