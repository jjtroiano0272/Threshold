using UnityEngine;
using UnityEngine.UI;
using TMPro; // Only if using TMP


public class ZDepthDebugger : MonoBehaviour
{
    [System.Serializable]
    public class LayerObject
    {
        public Transform transform;
        public float originalZ;
    }

    public Slider depthSlider;
    public LayerObject[] layerObjects;

    // public Text valueText; // <- Use this if using UnityEngine.UI.Text
    public TextMeshProUGUI valueText; // <- Use this instead if using TMP

    void Start()
    {
        foreach (var layer in layerObjects)
        {
            if (layer.transform != null)
                layer.originalZ = layer.transform.position.z;
        }

        // Update immediately at start
        UpdateZDepths(depthSlider.value);

        // Listen for changes
        depthSlider.onValueChanged.AddListener(UpdateZDepths);
    }

    void UpdateZDepths(float scale)
    {
        foreach (var layer in layerObjects)
        {
            if (layer.transform != null)
            {
                Vector3 pos = layer.transform.position;
                pos.z = layer.originalZ * scale;
                layer.transform.position = pos;
            }
        }

        if (valueText != null)
            valueText.text = $"Z Scale: {scale:F2}";
    }
}
