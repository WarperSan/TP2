using BossModule;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Gravestone : MonoBehaviour, IInteractable
{
    private Collider _collider;
    private Fiddlesticks fiddlesticks;
    
    [SerializeField]
    private Light _light;

    [SerializeField]
    private ParticleSystem[] particles;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        fiddlesticks = FindObjectOfType<Fiddlesticks>(true);
    }

    public void OnClick()
    {
        _collider.enabled = false;
        fiddlesticks.Damage();
        _light.color = Color.green;

        foreach (var item in particles)
            item.Play();
    }
}
