using UnityEngine;

public class ModelManipulatorStarter : MonoBehaviour
{
    [Header("ModelManipulator 프리팹 (비어있으면 빈 오브젝트로 생성)")]
    [SerializeField] private GameObject _modelManipulatorPrefab;

    private void Awake()
    {
        var holder = SystemHolder.Get();

        if (holder.ModelManipulator == null)
        {
            GameObject go = _modelManipulatorPrefab != null
                ? Instantiate(_modelManipulatorPrefab)
                : new GameObject("ModelManipulator");

            // 구체화된 ModelManipulator<T> 컴포넌트는 프리팹에 포함되어 있어야 함
            // 비어있는 오브젝트로 생성 시 직접 컴포넌트를 추가해야 함
        }
    }
}
