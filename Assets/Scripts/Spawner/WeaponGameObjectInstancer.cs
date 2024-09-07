using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGameObjectInstancer : MonoBehaviour
{
    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    GameObject toInstance;

    public void Instance()
    {
        GameObject instance = Instantiate(toInstance, spawnPoint.position, Quaternion.identity);
        IProjectile projectile = instance.GetComponent<IProjectile>();
        if(projectile != null)
        {
            projectile.SpawnPoint = spawnPoint;
        }
    }

}
