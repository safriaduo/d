using FronkonGames.TinyTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject arrowBodyParent;
    [SerializeField] private MeshRenderer arrowBody;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float bodyOffsetZ;

    private Vector2 defaultMaterialScale;

    private void Start()
    {
        defaultMaterialScale = arrowBody.material.mainTextureScale;
    }

    void Update()
    {
        if (target != null && origin != null)
        {
            transform.position = target.transform.position;
            transform.LookAt(origin.transform.position);

            float arrowBodyLenght = Vector3.Distance(origin.transform.position, target.transform.position) + bodyOffsetZ;

            var currScale = arrowBodyParent.transform.localScale;
            currScale.z = arrowBodyLenght;
            arrowBodyParent.transform.localScale = currScale;

            arrowBody.material.mainTextureOffset += new Vector2(0f, Time.deltaTime * animationSpeed);
            arrowBody.material.mainTextureScale = new Vector2(defaultMaterialScale.x, defaultMaterialScale.y * arrowBodyLenght);
        }
    }
}
