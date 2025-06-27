using System.Text;
using UnityEngine;

public static class Utilities 
{
    public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask)
    {
        int objLayerMask = 1 << obj.layer;
        return (layerMask.value & objLayerMask) > 0;
    }

    public static byte[] Encode(this object request)
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
        return Encoding.UTF8.GetBytes(json);
    }
}
