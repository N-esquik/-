using TMPro;
using UnityEngine;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private TextMeshProUGUI _textCountResourceRock;

    private void OnEnable()
    {
        if (_resourceStorage != null)
        {
            _resourceStorage.OnResourceCountChanged += HandleResourceCountChanged;
        }
    }

    private void OnDisable()
    {
        if (_resourceStorage != null)
        {
            _resourceStorage.OnResourceCountChanged -= HandleResourceCountChanged;
        }
    }

    private void HandleResourceCountChanged(int newCount)
    {
        if (_textCountResourceRock == null)
            return;

        _textCountResourceRock.text = newCount.ToString();
    }
}
