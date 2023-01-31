using UnityEditor;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    private GameObject _particlePrefab;
    private void Awake()
    {
        UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/ParticleDimondEffect.prefab", typeof(GameObject));
        _particlePrefab = (GameObject)prefab;
    }

    private void OnTriggerEnter(Collider other)
    {
        var config = StatsRepository.Load();
        config._diamonds++;
        StatsRepository.Save(config);
        StatsView.Instance.UpdateDiamonds(config._diamonds);

        gameObject.SetActive(false);
        Instantiate(_particlePrefab, transform.position, Quaternion.identity);

        enabled = false;
    }
}
