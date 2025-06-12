using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections;

public class ShaderPropertyAnimator : MonoBehaviour
{
    [Header("Shader Property Settings")]
    [SerializeField] private Material targetMaterial;
    [SerializeField] private string shaderPropertyName = "_MyShaderProperty";

    [Header("ForceField Options")]
    [SerializeField] private bool useForceFieldController;
    [SerializeField] private ForceFieldController forceFieldController;

    [Header("Animation Settings")]
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 1f;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool loop = true;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool reverseAnimation = false;

    private Coroutine animationCoroutine;
    private int shaderPropertyID;
    private bool isPaused = false;
    private float elapsedTime = 0f;

    private void Start()
    {
        // Cache the property ID for better performance
        shaderPropertyID = Shader.PropertyToID(shaderPropertyName);
        
        if (autoStart)
        {
            StartAnimation(reverseAnimation);
        }
    }

    public void SetMaterial(Material material, string propertyName)
    {
        targetMaterial = material;
        shaderPropertyName = propertyName;
    }

    /// <summary>
    /// Starts the shader property animation.
    /// </summary>
    /// <param name="reverse">If true, the animation plays in reverse.</param>
    public void StartAnimation(bool reverse = false)
    {
        reverseAnimation = reverse;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            elapsedTime = 0f;
        }
        animationCoroutine = StartCoroutine(AnimateShaderProperty());
    }

    /// <summary>
    /// Stops the shader property animation.
    /// </summary>
    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    /// <summary>
    /// Pauses the shader property animation.
    /// </summary>
    public void PauseAnimation()
    {
        isPaused = true;
    }

    /// <summary>
    /// Resumes the shader property animation if paused.
    /// </summary>
    public void ResumeAnimation()
    {
        isPaused = false;
    }

    /// <summary>
    /// Resets the shader property animation to its initial state.
    /// </summary>
    public void ResetAnimation()
    {
        elapsedTime = 0f;
        SetShaderPropertyValue(minValue);
    }

    /// <summary>
    /// Sets a new animation duration.
    /// </summary>
    /// <param name="newDuration">The new duration for the animation.</param>
    public void SetAnimationSpeed(float newDuration)
    {
        animationDuration = newDuration;
    }

    private IEnumerator AnimateShaderProperty()
    {
        yield return new WaitForSeconds(.1f);
        if(useForceFieldController)
            SetMaterial(forceFieldController.materialLayers[0], shaderPropertyName);
        while (loop || elapsedTime < animationDuration)
        {
            if (!isPaused)
            {
                
                
                float t = (elapsedTime % animationDuration) / animationDuration;

                if (reverseAnimation)
                    t = 1f - t;

                // Apply the animation curve for ease-in and ease-out
                float curveValue = animationCurve.Evaluate(t);

                // Interpolate between minValue and maxValue
                float value = Mathf.Lerp(minValue, maxValue, curveValue);

                // Set the shader property
                SetShaderPropertyValue(value);

                elapsedTime += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void SetShaderPropertyValue(float value)
    {
        if (targetMaterial.HasProperty(shaderPropertyID))
        {
            targetMaterial.SetFloat(shaderPropertyID, value);
        }
        else
        {
            Debug.LogWarning($"Shader does not have a float property with the name {shaderPropertyName}");
        }
    }
}

