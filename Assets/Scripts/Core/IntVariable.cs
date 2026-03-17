using UnityEngine;

[CreateAssetMenu(fileName = "NewIntVariable", menuName = "TinyKitchen/Int Variable")]
public class IntVariable : ScriptableObject
{
    [Tooltip("Valor inicial que se asigna al comenzar el juego.")]
    [SerializeField] private int initialValue;

    [Tooltip("Valor actual en runtime.")]
    [SerializeField] private int runtimeValue;
    public int Value
    {
        get => runtimeValue;
        set => runtimeValue = value;
    }
    public int InitialValue => initialValue;
    public void ResetToInitial()
    {
        runtimeValue = initialValue;
    }
    public void ApplyChange(int amount)
    {
        runtimeValue += amount;
    }
}