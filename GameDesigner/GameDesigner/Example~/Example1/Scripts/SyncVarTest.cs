using Net.Share;
using Net.UnityComponent;
using UnityEngine;

public class SyncVarTest : NetworkBehaviour
{
    [SyncVar]
    public SyncVarClass test;
    [SyncVar(authorize = false)]
    public SyncVarClass test1;
    [SyncVar(authorize = false, hook = nameof(OnTest2Value))]
    public SyncVarClass test2;
    [SyncVar]
    public SyncVarClass test3;

    public void OnTest2Value(SyncVarClass old, SyncVarClass value) 
    {
        Debug.Log(value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            test = new SyncVarClass();
            test1 = new SyncVarClass();
            test2 = new SyncVarClass();
        }
    }
}

[System.Serializable]
public class SyncVarClass 
{
    public int value;

    public override bool Equals(object obj)
    {
        if (!(obj is SyncVarClass obj1))
            return false;
        return obj1.value == value;
    }

    public override string ToString()
    {
        return $"value={value}";
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}