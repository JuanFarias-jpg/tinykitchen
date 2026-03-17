using UnityEngine;

[CreateAssetMenu(fileName = "NewFloatVariable", menuName = "TinyKitchen/Float Variable")]
public class FloatVariable : ScriptableObject
{
    [Tooltip("Valor inicial que se asigna al comenzar el juego.")]
    [SerializeField] private float initialValue;

    [Tooltip("Valor actual en runtime.")]
    [SerializeField] private float runtimeValue;

    public float Value
    {
        get => runtimeValue;
        set => runtimeValue = value;
    }

    public float InitialValue => initialValue;
    public void ResetToInitial()
    {
        runtimeValue = initialValue;
    }
    public void ApplyChange(float amount)
    {
        runtimeValue += amount;
    }
}