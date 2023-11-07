using FishNet.Object;
using UnityEngine;

public class SpawnAsChild : NetworkBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] NetworkObject prefabWithNOB;
    [SerializeField] NetworkObject prefabWithNB;
    [SerializeField] NetworkObject prefabWithNT;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Invoke(nameof(ParentAndSpawnObject), 1);
        Invoke(nameof(SpawnAndParentObjWithNB), 2);
        Invoke(nameof(SpawnAndParentObjWithNT), 3);
    }

    private void ParentAndSpawnObject()
    {
        var obj = Instantiate(prefabWithNOB);
        obj.transform.SetParent(hand.transform);
        Spawn(obj);
    }

    private void SpawnAndParentObjWithNB()
    {
        var obj = Instantiate(prefabWithNB);
        Spawn(obj);
        obj.SetParent(hand.GetComponent<NetworkObject>());
    }

    private void SpawnAndParentObjWithNT()
    {
        var obj = Instantiate(prefabWithNT);
        Spawn(obj);
        obj.GetComponent<NetworkObject>().SetParent(hand.GetComponent<NetworkObject>());
    }
}
