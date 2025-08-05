using UnityEngine;

[ExecuteInEditMode]
public class ForceEnableGPUInstancing : MonoBehaviour
{
    void Start()
    {
        ForceGPUInstancingOnChildren();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        ForceGPUInstancingOnChildren();
    }
#endif

    public void ForceGPUInstancingOnChildren()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null) continue;

            Material[] materials = renderer.sharedMaterials;
            bool materialsChanged = false;

            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == null) continue;

                // Принудительно включаем GPU Instancing, даже если Unity его блокирует
                if (!materials[i].enableInstancing)
                {
                    // Создаем новый материал, чтобы обойти возможные блокировки
                    Material newMaterial = new Material(materials[i])
                    {
                        name = materials[i].name + " (GPU Instanced)",
                        enableInstancing = true
                    };

                    materials[i] = newMaterial;
                    materialsChanged = true;
                }
            }

            if (materialsChanged)
            {
                renderer.sharedMaterials = materials;
            }
        }
    }
}