using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 目前在scene memory里，当角色全部卸载后，会留下1.整合的mesh，2.整合的texture，3.整合的material。4.部件的material
/// 以上需要修正
/// </summary>
public class sCombineSkinnedMesh 
{

    /// <summary>
    /// 组合SkinnedMeshRenderers并且共享同一个骨骼
    /// 并且整合材质减少drawcall，但会增加贴图大小和内存大小）
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="meshes"></param>
    /// <param name="combine"></param>
    public static void CombineObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
    {
        // Fetch all bones of the skeleton
        List<Transform> transforms = new List<Transform>();
        transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

        List<Material> materials = new List<Material>();//the list of materials
        List<CombineInstance> combineInstances = new List<CombineInstance>();//the list of meshes
        List<Transform> bones = new List<Transform>();//the list of bones

        // Below informations only are used for merge materilas(bool combine = true)
        List<Vector2[]> oldUV = null;
        Material newMaterial = null;
        Texture2D newDiffuseTex = null;

        // Collect information from meshes
        for (int i = 0; i < meshes.Length; i++)
        {
            SkinnedMeshRenderer smr = meshes[i];
            materials.AddRange(smr.materials); // Collect materials
            // Collect meshes
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);
            }
            // Collect bones
            for (int j = 0; j < smr.bones.Length; j++)
            {
                int tBase = 0;
                for (tBase = 0; tBase < transforms.Count; tBase++)
                {
                    if (smr.bones[j].name.Equals(transforms[tBase].name))
                    {
                        bones.Add(transforms[tBase]);
                        break;
                    }
                }
            }

            Object.Destroy(smr.gameObject);
        }
       
        // merge materials
        if (combine)
        {
            newMaterial = new Material(materials[0].shader);
            oldUV = new List<Vector2[]>();
            // merge the texture
            List<Texture2D> Textures = new List<Texture2D>();
            for (int i = 0; i < materials.Count; i++)
            {
                Textures.Add(materials[i].GetTexture(sConst.combineDiffuseTexture) as Texture2D);
            }

            newDiffuseTex = new Texture2D(sConst.combineTextureMax, sConst.combineTextureMax, TextureFormat.ETC_RGB4, false);
            Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
            for( int i = 0; i < Textures.Count; ++i )
            {
                Resources.UnloadAsset(Textures[i]);
                GameObject.Destroy(materials[i]);
            }
            newMaterial.mainTexture = newDiffuseTex;

            // reset uv
            Vector2[] uva, uvb;
            for (int j = 0; j < combineInstances.Count; j++)
            {
                uva = (Vector2[])(combineInstances[j].mesh.uv);
                uvb = new Vector2[uva.Length];
                for (int k = 0; k < uva.Length; k++)
                {
                    uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
                }
                oldUV.Add(combineInstances[j].mesh.uv);
                combineInstances[j].mesh.uv = uvb;
            }
        }

        // Create a new SkinnedMeshRenderer
        SkinnedMeshRenderer r = skeleton.GetComponent<SkinnedMeshRenderer>();
        r.sharedMesh = new Mesh();
        r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);// Combine meshes
        r.bones = bones.ToArray();// Use new bones

        r.receiveShadows = false;
        r.useLightProbes = false;
        r.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        if (combine)
        {
            r.material = newMaterial;
            for (int i = 0; i < combineInstances.Count; i++)
            {
                combineInstances[i].mesh.uv = oldUV[i];
            }
        }
        else
        {
            r.materials = materials.ToArray();
        }
      
    }
}
