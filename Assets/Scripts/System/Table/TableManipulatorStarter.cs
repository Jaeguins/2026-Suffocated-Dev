using UnityEngine;

public class TableManipulatorStarter : MonoBehaviour
{
    [Header("tsv 테이블 경로")]
    public string TablePath;
    private void Awake()
    {
        var holder = SystemHolder.Get();

        if (holder.TableManipulator == null)
        {
           holder.TableManipulator = new TableManipulator();
           holder.TableManipulator.Initialize(TablePath);

           // 구체화된 ModelManipulator<T> 컴포넌트는 프리팹에 포함되어 있어야 함
           // 비어있는 오브젝트로 생성 시 직접 컴포넌트를 추가해야 함
        }
    }
}