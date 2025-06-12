using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightGroup : MonoBehaviour
{
    [SerializeField] private Material baseMaterial;
    [SerializeField] private List<Renderer> rendererGroup;
    private const string MATERIAL_ALPHA_PROPERTY = "_Alpha";

    public void Highlight()
    {
        StartCoroutine(HighlightActionCard());
    }

    private IEnumerator HighlightActionCard()
    {
        List<GameObject> newRenderers = new();
        foreach (Renderer renderer in rendererGroup)
        {
            GameObject highlightRendererGO = Instantiate(renderer.gameObject, renderer.transform.parent);
            highlightRendererGO.GetComponent<Renderer>().material = baseMaterial;
            newRenderers.Add(highlightRendererGO);
        }
        for (float curve = 0f, increment=0f; curve <= 1f; increment*=curve/10f, curve += 0.05f+increment)
        {
            baseMaterial.SetFloat(MATERIAL_ALPHA_PROPERTY, curve);
            yield return new WaitForEndOfFrame();
        }
        baseMaterial.SetFloat(MATERIAL_ALPHA_PROPERTY, 1f);
        yield return new WaitForSeconds(.5f);

        for (float curve = 1f, increment = 0f; curve >= 0f; increment *= curve / 10f, curve -= 0.05f + increment)
        {
            baseMaterial.SetFloat(MATERIAL_ALPHA_PROPERTY, curve);
            yield return new WaitForEndOfFrame();
        }
        foreach (GameObject go in newRenderers)
        {
            Destroy(go);
        }
    }
}
