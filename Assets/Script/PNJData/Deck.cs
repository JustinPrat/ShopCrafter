using System.Collections.Generic;

public class Deck<T>
{
    private List<T> valuesRemaining;
    private List<T> sourceValues;

    public void Initialize(List<T> values)
    {
        sourceValues = values;
        valuesRemaining = new List<T>(values);
    }

    public T Pick()
    {
        int index = UnityEngine.Random.Range(0, valuesRemaining.Count);
        T pickedItem = valuesRemaining[index];
        valuesRemaining.RemoveAt(index);

        if (valuesRemaining.Count <= 0)
            valuesRemaining = new List<T>(sourceValues);

        return pickedItem;
    }
}