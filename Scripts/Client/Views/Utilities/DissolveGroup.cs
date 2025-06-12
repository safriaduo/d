using INab.Dissolve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dawnshard.Views
{
    /// <summary>
    /// Dissolves a group of renderers
    /// </summary>
    [RequireComponent(typeof(Dissolver))]
    public class DissolveGroup : MonoBehaviour
    {
        [SerializeField] private Material baseMaterial;
        [SerializeField] private List<Renderer> rendererGroup;

        private Dissolver dissolver;
        
        void Start()
        {
            dissolver = GetComponent<Dissolver>();

            // Crea una nuova istanza del materiale
            Material newMaterial = new(baseMaterial);

            // Applica il nuovo materiale alle sprites
            foreach (Renderer renderer in rendererGroup)
            {
                renderer.material = newMaterial;
            }

            dissolver.materials = new List<Material> { newMaterial };
        }
    }
}
