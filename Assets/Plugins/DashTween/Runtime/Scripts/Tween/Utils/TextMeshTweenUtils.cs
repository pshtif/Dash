/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dash
{
    public class TextMeshTweenUtils
    {
        static private Dictionary<TMP_Text, TMP_MeshInfo[]> _meshCache = new Dictionary<TMP_Text, TMP_MeshInfo[]>();

        static public void Cache(TMP_Text p_text)
        {
            p_text.ForceMeshUpdate();
            _meshCache.Add(p_text, p_text.textInfo.CopyMeshInfoVertexData());
        }
        
        static public void Scale(TMP_Text p_text, int p_index, float p_scale)
        {
            TMP_TextInfo textInfo = p_text.textInfo;
            if (textInfo.characterCount <= p_index)
                return;

            p_text.renderMode = TextRenderFlags.DontRender;

            Matrix4x4 matrix;
            
            if (!_meshCache.ContainsKey(p_text))
                Cache(p_text);
            
            TMP_MeshInfo[] cachedMeshInfoVertexData = _meshCache[p_text];
            
            TMP_CharacterInfo charInfo = textInfo.characterInfo[p_index];

            if (!charInfo.isVisible)
                return;
                
            int materialIndex = textInfo.characterInfo[p_index].materialReferenceIndex;
                
            int vertexIndex = textInfo.characterInfo[p_index].vertexIndex;
                
            Vector3[] charVertices = cachedMeshInfoVertexData[materialIndex].vertices;

                // Determine the center point of each character at the baseline.
                //Vector2 charMidBasline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
                
            Vector2 center = (charVertices[vertexIndex + 0] + charVertices[vertexIndex + 2]) / 2;
            Vector3 offset = center;

            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = charVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = charVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = charVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = charVertices[vertexIndex + 3] - offset;

            matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * p_scale);

            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;

            // Restore Source UVS which have been modified by the sorting
            // Vector2[] sourceUVs0 = cachedMeshInfoVertexData[materialIndex].uvs0;
            // Vector2[] destinationUVs0 = textInfo.meshInfo[materialIndex].uvs0;
            //
            // destinationUVs0[vertexIndex + 0] = sourceUVs0[vertexIndex + 0];
            // destinationUVs0[vertexIndex + 1] = sourceUVs0[vertexIndex + 1];
            // destinationUVs0[vertexIndex + 2] = sourceUVs0[vertexIndex + 2];
            // destinationUVs0[vertexIndex + 3] = sourceUVs0[vertexIndex + 3];

            // Restore Source Vertex Colors
            // Color32[] sourceColors32 = cachedMeshInfoVertexData[materialIndex].colors32;
            // Color32[] destinationColors32 = textInfo.meshInfo[materialIndex].colors32;
            //
            // destinationColors32[vertexIndex + 0] = sourceColors32[vertexIndex + 0];
            // destinationColors32[vertexIndex + 1] = sourceColors32[vertexIndex + 1];
            // destinationColors32[vertexIndex + 2] = sourceColors32[vertexIndex + 2];
            // destinationColors32[vertexIndex + 3] = sourceColors32[vertexIndex + 3];
            
            
            textInfo.meshInfo[materialIndex].mesh.vertices = textInfo.meshInfo[materialIndex].vertices;
            //textInfo.meshInfo[materialIndex].mesh.uv = textInfo.meshInfo[p_index].uvs0;
            //textInfo.meshInfo[materialIndex].mesh.colors32 = textInfo.meshInfo[materialIndex].colors32;

            //p_text.UpdateGeometry(textInfo.meshInfo[materialIndex].mesh, materialIndex);
            p_text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}